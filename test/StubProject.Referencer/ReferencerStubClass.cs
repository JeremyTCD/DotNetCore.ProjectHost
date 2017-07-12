using StubProject.Referencee;

namespace StubProject.Referencer
{
    public class ReferencerStubClass
    {
        public ReferencerStubClass()
        {
            // Force StubProject.Referencee.dll to load
            ReferenceeStubClass stub = new ReferenceeStubClass();
        }
    }
}
