namespace CLI
{
    using System;
    using Kundalini.Extensions;
    using Kundalini.Processor;

    public class Program
    {
        public static void Main(string[] args)
        {
            HelloWorld.LoadScript(Scripts.Functions.BytesToString());
            HelloWorld.Add(15, 24);
            HelloWorld.SayHello();

            Console.ReadKey();
        }
    }
}
