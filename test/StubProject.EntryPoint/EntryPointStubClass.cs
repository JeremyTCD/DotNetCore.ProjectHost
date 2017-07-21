using System;

namespace StubProject.EntryPoint
{
    public class EntryPointStubClass
    {
        public static int Main(string[] args)
        {
            return Convert.ToInt32(args[0]); // Arbitrary number to verify that Main executed successfully
        }
    }
}
