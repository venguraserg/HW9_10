using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Serilog;

namespace HW9_10
{
    class Program
    {
        static TelegramBotClient bot;

        [Obsolete]
        static void Main(string[] args)
        {

            TestMethod();
            bot = Init.Initialization();
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

        [Obsolete]
        private static void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
          if(e.Message.Type == MessageType.Text)
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
                                              "которые вы можете скачать обратно\n"+
                                              "/start  - начало работы \n" +
                                              "/help   - помощь \n" +
                                              "/getDir - получить список папок хранилища";
                                bot.SendTextMessageAsync(e.Message.Chat.Id, text);
                            }
                            break;

                        case "/start":
                            {
                                bot.SendTextMessageAsync(e.Message.Chat.Id, "/help - для получения списка команд");
                            }
                            break;
                        case "/getDir":
                            {

                            }
                            break;
                    }

                }
                else
                {
                    Debug.WriteLine(e.Message.Chat.FirstName + " не верный ввод . . .");
                    bot.SendTextMessageAsync(e.Message.Chat.Id, "не верный ввод. . .");
                }
            }
          else
          {
                switch (e.Message.Type)
                {
                    case MessageType.Document:
                        {
                            Console.WriteLine(e.Message.Document.FileId + "\n");
                            Console.WriteLine(e.Message.Document.FileName + "\n");
                            Console.WriteLine(e.Message.Document.FileSize + "\n");
                            Console.WriteLine(e.Message.Document.MimeType + "\n");


                            Console.WriteLine(GetDirByFileType(e.Message.Document.MimeType, e.Message.Chat.Id));

                            DownLoad(e.Message.Document.FileId, e.Message.Document.FileName, GetDirByFileType(e.Message.Document.MimeType, e.Message.Chat.Id));
                            bot.SendTextMessageAsync(e.Message.Chat.Id, $"Принято");
                        }
                        break;

                    case MessageType.Audio:
                        {
                            bot.SendTextMessageAsync(e.Message.Chat.Id, $"Принят аудиофайл");
                            var str = e.Message.Audio.FileName.Split('.')[^1];
                            Console.WriteLine(str);
                            DownLoad(e.Message.Audio.FileId, e.Message.Audio.FileName, GetDirByFileType(e.Message.Audio.MimeType, e.Message.Chat.Id));

                        }
                        break;
                    case MessageType.Photo:
                        {
                            bot.SendTextMessageAsync(e.Message.Chat.Id, $"Принято фото");
                        }
                        break;
                    default:

                        bot.SendTextMessageAsync(e.Message.Chat.Id, $"Не верно");
                        break;
                }


          }
            
            
            
            
            

            
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
