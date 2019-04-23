using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CodeContract
{
    public class Contract
    {
        public Dictionary<string, object> OldValue { get; } = new Dictionary<string, object>();

        public List<(string name, object preValue, Func<object> getPostValue)> Invariantes { get; } = new List<(string, object, Func<object>)>();

        public void CheckInvarianets(
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = -1,
            [CallerMemberName] string callerMemberName = "")
        {
            foreach(var invariante in Invariantes)
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

        private enum check
        {
            pre,
            post,
            invariante
        }

        private void LogContractViolationEvent(
            string message,
            string callerFilePath,
            int callerLineNumber,
            string callerMemberName,
            check check)
        {
            Debug.WriteLine($"[{check}], [{callerFilePath}], [{callerLineNumber}], [{callerMemberName}], [{message}]");
        }

        public void PreCondition(
            bool check, 
            string message = null, 
            [CallerFilePath] string callerFilePath = "", 
            [CallerLineNumber] int callerLineNumber = -1,
            [CallerMemberName] string callerMemberName = "")
        {
            if(!check)
            {
                LogContractViolationEvent(message, callerFilePath, callerLineNumber, callerMemberName, Contract.check.pre);                
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
    }
}
