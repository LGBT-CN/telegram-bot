using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

namespace LGBTCN.Bot.Test.Util
{
    [TestClass]
    public class HttpTest
    {
        [TestMethod]
        public async Task GetAsyncTestAsync()
        {
            var result = await LGBTCN.Bot.Util.Http.GetAsync("https://ip.sb");
            System.Console.WriteLine(result);
        }
    }
}
