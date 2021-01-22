using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LGBTCN.Bot.Util
{
    public static class Log
    {
        private static void WriteMsg(string tag, string module, string msg)
            => Console.WriteLine($"{DateTime.UtcNow:dd/MM/yyyy HH:mm:ss} {tag}/{module}: {msg}");

        /// <summary>
        /// Information
        /// </summary>
        /// <param name="module"></param>
        /// <param name="msg"></param>
        public static void I(string module, string msg)
            => WriteMsg("I", module, msg);

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="module"></param>
        /// <param name="msg"></param>
        public static void E(string module, string msg)
            => WriteMsg("E", module, msg);

        /// <summary>
        /// Warning
        /// </summary>
        /// <param name="module"></param>
        /// <param name="msg"></param>
        public static void W(string module, string msg)
            => WriteMsg("W", module, msg);
    }
}
