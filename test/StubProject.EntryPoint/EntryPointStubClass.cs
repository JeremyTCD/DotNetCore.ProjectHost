using System;

namespace StubProject.EntryPoint
{
    public class EntryPointStubClass
    {
        public static int Main(string[] args)
        {
            Console.Write($"{string.Join(",", args)}");

            return 0;
        }
    }
}
