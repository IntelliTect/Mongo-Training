using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntelliTect.Training.Mongo._1_ClientManagement
{
    [TestClass]
    public class ClientTests
    {
        [TestMethod]
        public void WhenClientIsRequestedTwiceItGivesTheSameClient()
        {
            var result = Mongo.Client;
            var result2 = Mongo.Client;

            Assert.AreSame( result, result2 );

        }
    }
}
