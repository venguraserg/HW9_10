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
            
            return new TelegramBotClient(token);
        }
    }
}
