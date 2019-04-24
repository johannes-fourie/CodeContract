using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;

namespace CodeContract
{
    public class Contract
    {
        private string _eventSource;

        public Contract()
        {
            CreateEventSource(System.AppDomain.CurrentDomain.FriendlyName);
        }

        private enum check
        {
            pre,
            post,
            invariante
        }

        public List<(string name, object preValue, Func<object> getPostValue)> Invariantes { get; } = new List<(string, object, Func<object>)>();

        public Dictionary<string, object> OldValue { get; } = new Dictionary<string, object>();

        public void CheckInvarianets(
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = -1,
            [CallerMemberName] string callerMemberName = "")
        {
            foreach (var invariante in Invariantes)
            {
                var postValue = invariante.getPostValue();
                if (invariante.preValue != postValue)
                {
                    LogContractViolationEvent(
                        $"Invarient '{invariante.name}' changed from [{invariante.preValue}] to [{postValue}]",
                        callerFilePath,
                        callerLineNumber,
                        callerMemberName,
                        check.invariante);
                }
            }
        }

        public void PostCondition(
            bool check,
            string message = null,
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = -1,
            [CallerMemberName] string callerMemberName = "")
        {
            if (!check)
            {
                LogContractViolationEvent(message, callerFilePath, callerLineNumber, callerMemberName, Contract.check.post);
            }
        }

        public void PreCondition(
            bool check,
            string message = null,
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = -1,
            [CallerMemberName] string callerMemberName = "")
        {
            if (!check)
            {
                LogContractViolationEvent(message, callerFilePath, callerLineNumber, callerMemberName, Contract.check.pre);
            }
        }

        private void CreateEventSource(string currentAppName)
        {
            _eventSource = currentAppName;
            try
            {
                if (!EventLog.SourceExists(_eventSource))
                {
                    EventLog.CreateEventSource(_eventSource, "Application");
                }
            }
            catch (SecurityException)
            {
                _eventSource = "Application";
            }
        }

        private void LogContractViolationEvent(
            string message,
            string callerFilePath,
            int callerLineNumber,
            string callerMemberName,
            check check)
        {
            var logMessage = $"Contract violation: [{check}], [{callerFilePath}], [{callerLineNumber}], [{callerMemberName}], [{message}]";

            new EventLog("Application") { Source = _eventSource }
            .WriteEntry(message: logMessage, type: EventLogEntryType.Warning);

            Debug.WriteLine(logMessage);
        }
    }
}