using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace HW9_10
{
    public static class Init
    {
        public static TelegramBotClient Initialization()
        {
            string path = "token.txt";
            var token = File.ReadAllText(path);
            var sss = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Lossso");
            var ssss = Path.Combine(sss, string.Format("{0}_{1:dd.MM.yyy}.log", AppDomain.CurrentDomain.FriendlyName, DateTime.Now));
            Console.WriteLine(sss);
            Console.WriteLine(ssss);
            return new TelegramBotClient(token);
        }
    }
}
