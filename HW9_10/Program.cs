using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Serilog;
using System.Linq;
using Telegram.Bot.Args;

namespace HW9_10
{
    class Program
    {
        static TelegramBotClient bot;
        static List<User> users;

        [Obsolete]
        static void Main(string[] args)
        {
           // TestMethod();

            users = new List<User>();
            bot = Init.Initialization(ref users);
            bot.OnMessage += Bot_OnMessage;
            bot.StartReceiving();
            Console.ReadKey();
        }

        private static void TestMethod()
        {
            var temp = Directory.GetDirectories(Path.Combine(Directory.GetCurrentDirectory(), "Repository"));
            foreach (var item in temp)
            {
                Console.WriteLine(new DirectoryInfo(item).Name);
                
            }
            
        }


        /// <summary>
        /// Метод обработки события получения сообщения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete]
        private static void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            User user = GetUser(e.Message.Chat.Id);

            if (e.Message.Type == MessageType.Text)
            {
                if (e.Message.Text.Substring(0, 1) == "/")
                {
                    switch (e.Message.Text)
                    {
                        case "/help":
                            {
                                string text = "Бот ХРАНИТЕЛЬ\n" +
                                              "Этот бот сохраняет ваши файлы\n" +
                                              "в репозиторий и сортирует по папкам\n" +
                                              "которые вы можете скачать обратно\n" +
                                              "/start  - начало работы \n" +
                                              "/help   - помощь \n" +
                                              "/veiw_reposytory - получить список папок хранилища";
                                bot.SendTextMessageAsync(e.Message.Chat.Id, text);
                            }
                            break;

                        case "/start":
                            {
                                bot.SendTextMessageAsync(e.Message.Chat.Id, "/help - для получения списка команд");
                            }
                            break;
                        case "/veiw_reposytory":
                            {
                                string folderListForMenu = string.Empty;
                                foreach (var item in user.FolderList)
                                {
                                    folderListForMenu = folderListForMenu +"/" + item + "\n";
                                    
                                }
                                
                                bot.SendTextMessageAsync(e.Message.Chat.Id, folderListForMenu);
                            }
                            break;
                    }

                    var folderRequest = e.Message.Text.TrimStart('/');
                    var userFolder = user.FolderList.SingleOrDefault(i => i == e.Message.Text.TrimStart('/'));
                    if (e.Message.Text.TrimStart('/') == user.FolderList.SingleOrDefault(i=>i == e.Message.Text.TrimStart('/')))
                    {
                        var fileList = Directory.GetFiles(Path.Combine(user.RepoPath, e.Message.Text.TrimStart('/')));
                        var filename = new FileInfo(fileList[0]).Name;
                        
                        bot.SendTextMessageAsync(e.Message.Chat.Id, "ОК");
                    }

                }
                else
                {
                    bot.SendTextMessageAsync(e.Message.Chat.Id, "не верный ввод. . .");
                }
            }
            else
            {
                switch (e.Message.Type)
                {
                    case MessageType.Document:
                        {
                            Console.WriteLine($"Получен файл {e.Message.Document.FileName} от пользователя {e.Message.Chat.Username}");
                            DownLoad(e.Message.Document.FileId, e.Message.Document.FileName, GetDirByFileType(e.Message.Document.MimeType, e.Message.Chat.Id));
                            bot.SendTextMessageAsync(e.Message.Chat.Id, $"Принято");
                        }
                        break;

                    case MessageType.Audio:
                        {
                            Console.WriteLine($"Получен файл {e.Message.Audio.FileName} от пользователя {e.Message.Chat.Username}");
                            bot.SendTextMessageAsync(e.Message.Chat.Id, $"Принят аудиофайл");
                            DownLoad(e.Message.Audio.FileId, e.Message.Audio.FileName, GetDirByFileType(e.Message.Audio.MimeType, e.Message.Chat.Id));

                        }
                        break;
                    case MessageType.Photo:
                        {
                            Console.WriteLine($"Получено фото от пользователя {e.Message.Chat.Username}");
                            //bot.SendTextMessageAsync(e.Message.Chat.Id, $"Принято фото");
                            //DownLoad(e.Message.Photo.FileId, e.Message.Photo.FileName, GetDirByFileType(e.Message.Photo.MimeType, e.Message.Chat.Id));

                        }
                        break;
                    case MessageType.Video:
                        {
                            Console.WriteLine($"Получено Видео от пользователя {e.Message.Chat.Username}");
                            
                        }
                        break;
                    case MessageType.Voice:
                        {
                            Console.WriteLine($"Получено голосовое от пользователя {e.Message.Chat.Username}");
                        }
                        break;
                    case MessageType.Unknown:
                        {
                            Console.WriteLine($"Получено непонятно что от пользователя {e.Message.Chat.Username}");
                        }
                        break;




                    default:

                        bot.SendTextMessageAsync(e.Message.Chat.Id, $"не верный ввод. . . {e.Message.Type.ToString()}");
                        break;
                }

            }

        }

        private static User GetUser(long id)
        {
            User tempUser = new User();
            if (!FindUser(id))
            {
                tempUser = new User(id);
                users.Add(tempUser);
            }
            else
            {
                tempUser = users.Single(i => i.UserId == id);
            }
            return tempUser;
        }

        /// <summary>
        /// Поиск наличия пользователя по Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>true - если пользователь есть, false - если пользователя нет</returns>
        private static bool FindUser(long id)
        {
            for (int i = 0; i < users.Count; i++)
            {
                if(users[i].UserId == id)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Метод загрузки в репозиторий файлов
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="fileName"></param>
        static async void DownLoad(string fileId,string fileName, string filePath)
        {
            var file = await bot.GetFileAsync(fileId);

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
             }


            //FileStream fs = new FileStream("_" + fileName, FileMode.Create);
            //await bot.DownloadFileAsync(file.FilePath, fs);
            //fs.Close();

            //fs.Dispose();
            var tempPath = Path.Combine(filePath, fileName);
                using (FileStream fss = new FileStream(tempPath, FileMode.Create))
            {
                await bot.DownloadFileAsync(file.FilePath, fss);

            };
        }
        /// <summary>
        /// Метод определения пути сохраняемого файла по типу
        /// </summary>
        /// <param name="mimeType"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetDirByFileType(string mimeType, long userId)
        {
            var fileType = mimeType.Split('/');
            var currentDir = fileType[0];
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Repository", userId.ToString(), currentDir);
            return path;                      

        }
    }
}
