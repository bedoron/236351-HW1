using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlightSearchServerTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestInitialize]
        public void setup()
        {
            Console.WriteLine("Setting up objects");
        }

        [TestCleanup]
        public void teardown()
        {
            Console.WriteLine("Tearing down states");
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
