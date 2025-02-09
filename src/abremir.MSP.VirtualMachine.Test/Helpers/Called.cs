using EventTesting.Verifiers;

namespace abremir.MSP.VirtualMachine.Test.Helpers.EventTestingHelper
{
    public static class Called
    {
        public static IVerifier Never()
        {
            return new ExactVerifier(0);
        }
    }
}
