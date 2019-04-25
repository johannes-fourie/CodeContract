using CodeContract;
using System;

namespace Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            new AClass().SomeMethod(-70);

            Console.WriteLine("---------------------------");
            Console.ReadKey();
        }

        public class AClass
        {
            public string Name { get; set; }

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
        }
    }
}