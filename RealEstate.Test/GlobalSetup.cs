using NUnit.Framework;

namespace RealEstate.Test
{
    [SetUpFixture]
    public class GlobalSetup
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Global setup for all tests
            TestContext.WriteLine("Starting RealEstate API Tests");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // Global cleanup for all tests
            TestContext.WriteLine("RealEstate API Tests Completed");
        }
    }
}
