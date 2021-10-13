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
        public static TelegramBotClient Initialization(ref List<User> users)
        {
            string path = "token.txt";
            if (!File.Exists(path)) 
            { 
                File.Create(path); 
            }
            var token = File.ReadAllText(path);
            string repoDir = Path.Combine(Directory.GetCurrentDirectory(), "Repository");
            if (!Directory.Exists(repoDir)) 
            { 
                Directory.CreateDirectory(repoDir); 
            }
            else
            {
                var folder = Directory.GetDirectories(Path.Combine(Directory.GetCurrentDirectory(), "Repository"));
                for (int i = 0; i < folder.Length; i++)
                {
                    var tempId = long.Parse(new DirectoryInfo(folder[i]).Name);
                    users.Add(new User(tempId));
                }
            }

            return new TelegramBotClient(token);
        }
    }
}
