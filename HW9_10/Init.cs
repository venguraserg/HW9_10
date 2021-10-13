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
            string repoDir = Path.Combine(Directory.GetCurrentDirectory(), "Repository");
            if (!Directory.Exists(repoDir)) { Directory.CreateDirectory(repoDir); }

            return new TelegramBotClient(token);
        }
    }
}
