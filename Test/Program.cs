using System;
using System.Collections.Generic;
using CodeContract;
using System.Linq;
using System.Reflection;
using FsCheck;
using System.Diagnostics;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            new AClass().SomeMethod(-70);

            Console.WriteLine("---------------------------");
            Console.ReadKey();
        }


        public static class Test
        {
            public static void Check(
                Func<bool> when, 
                Func<bool> then, 
                string message)
            {
                if (when() && !then())
                    Debug.WriteLine(message);
            }
        }

        public class AClass
        {
            public enum Time
            {
                Morning,
                Afternoon,
                Evening
            }

            public static class University
            {
                public static readonly string Harvard = "Harvard University";
                public static readonly string CaliforniaBerkeley = "University of California–Berkeley";
                public static readonly string Cambridge = "University of Cambridge";
            }


            public string Name { get; set; }

            public (string m, double d) SomeCode(bool b, Time t, string u, int l, int i, int j)
            {
                string message;
                double tax;

                // code

                return (m: message, d: tax);
            }

            public int SomeMethod(int aNumber)
            {

                var levels = new List<int>() { -10, -5, 0, 1, 30 };

                var b =
                    from boolean in new bool[] { true, false }
                    select boolean;

                var t =
                    from time in
                        Enum
                        .GetValues(typeof(Time))
                        .Cast<Time>()
                    select time;

                var u =
                    from university in
                        typeof(University)
                        .GetFields(BindingFlags.Static | BindingFlags.Public)
                        .Select(fi => (string)fi.GetValue(null))
                    select university;

                var discreateTestPoints =
                    from boolean in new bool[] { true, false }
                    from time in
                        Enum
                        .GetValues(typeof(Time))
                        .Cast<Time>()
                    from university in
                        typeof(University)
                        .GetFields(BindingFlags.Static | BindingFlags.Public)
                        .Select(fi => (string)fi.GetValue(null))
                    from level in levels
                    select (b: boolean, t: time, u: university, l: level);

                var i =
                    Arb
                    .From<int>()
                    .Filter(integer => integer >= 1 && integer < 100)
                    .Generator;

                var j =
                    Arb
                    .From<int>()
                    .Filter(integer => (integer >= 1 && integer <= 50) || (integer >= 60 && integer <= 100))
                    .Generator;

                var config = FsCheck.Configuration.Quick;
                config.MaxNbOfTest = 5;

                foreach (var discreateTestPoint in discreateTestPoints)
                {
                    Prop
                        .ForAll(
                            Arb.From(i),
                            Arb.From(j),
                            (ii, jj) =>
                            {
                                var result = SomeCode(
                                    discreateTestPoint.b,
                                    discreateTestPoint.t,
                                    discreateTestPoint.u,
                                    discreateTestPoint.l,
                                    ii,
                                    jj);

                                Test.Check(
                                    when: () => discreateTestPoint.b,
                                    then: () => result.d > 10,
                                    message: "if b then d > 10");

                                Test.Check(
                                    when: () => ii > 50,
                                    then: () => result.m == "Tax due",
                                    message: "if b then d > 10");
                            })
                        .Check(config);
                }


                var contract = new Contract();
                contract.Invariantes.Add((
                    name: nameof(Name), 
                    preValue: Name, 
                    getPostValue: () => Name));

                // code that changes aNumber

                contract.CheckInvarianets();
                return aNumber;
            }
            /*
            public int SomeMethod(int aNumber)
            {
#if DEBUG
                var contract = new Contract();
                contract.Invariantes.Add((nameof(Name), Name, () => Name));
                contract.OldValue.Add(nameof(aNumber), aNumber);
                contract.PreCondition(aNumber > 0 && aNumber < 100, $"input: 0..100: {aNumber}");
#endif
                //input = 55;
                Name = "ffff";
#if DEBUG
                contract.PostCondition((int)contract.OldValue[nameof(aNumber)] != aNumber, "The value of input should change");
                contract.PostCondition(aNumber > 50 && aNumber < 60, $"output: 50..60: {aNumber}");
                contract.CheckInvarianets();
#endif
                return aNumber;
            }
            */
        }
    }
}
