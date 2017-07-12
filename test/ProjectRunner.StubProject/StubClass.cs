using System;

namespace JeremyTCD.ProjectRunner.Tests.StubProject
{
    public class StubClass
    {
        public static int Main(string[] args)
        {
            Console.Write($"{string.Join(",", args)}");

            return 0;
        }
    }
}
