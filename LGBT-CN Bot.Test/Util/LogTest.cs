using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LGBTCN.Bot.Test.Util
{
    [TestClass]
    public class LogTest
    {
        [TestMethod]
        public void LogETest()
        {
            Bot.Util.Log.E("test.log", "Hello world!");
        }

        [TestMethod]
        public void LogITest()
        {
            Bot.Util.Log.I("test.log", "Hello world!");
        }

        [TestMethod]
        public void LogWTest()
        {
            Bot.Util.Log.W("test.log", "Hello world!");
        }
    }
}
