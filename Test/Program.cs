using System;
using CodeContract;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            new testing().SomeMethod(10);

            Console.ReadKey();
        }

        public class testing
        {
            public string Name { get; set; }

            public int SomeMethod(int input)
            {
#if DEBUG
                var contract = new Contract();
                contract.Invariantes.Add((nameof(Name), Name, () => Name));
                contract.OldValue.Add(nameof(input), input);
                contract.PreCondition(input > 0 && input < 100, $"input: 0..100: {input}");
#endif
                input = 55;
                Name = "ffff";
#if DEBUG
                contract.PostCondition((int)contract.OldValue[nameof(input)] != input, "The value of input should change");
                contract.PostCondition(input > 50 && input < 60, $"output: 50..60: {input}");
                contract.CheckInvarianets();
#endif
                return input;
            }
        }
    }
}
