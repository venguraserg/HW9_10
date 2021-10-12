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
            bot = Init.Initialization();
            bot.OnMessage += Bot_OnMessage;
            bot.StartReceiving();
            Console.ReadKey();
        }

        [Obsolete]
        private static void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            switch (e.Message.Type)
            {
                case MessageType.Text:
                    {
                        if (e.Message.Text.Substring(0, 1) == "/")
                        {
                            switch (e.Message.Text)
                            {
                                case "/help":
                                    {
                                        string text = "/start - начало работы \n" +
                                                      "/help  - помощь \n" + 
                                                      "/" ;
                                        bot.SendTextMessageAsync(e.Message.Chat.Id, text);
                                    }
                                    break;

                                case "/start":
                                    {
                                        bot.SendTextMessageAsync(e.Message.Chat.Id, "/help - для получения списка команд");
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
                    break;

                case MessageType.Document:
                    {
                        Console.WriteLine(e.Message.Document.FileId + "\n\n\n");
                        Console.WriteLine(e.Message.Document.FileName + "\n\n\n");
                        Console.WriteLine(e.Message.Document.FileSize + "\n\n\n");
                        Console.WriteLine(e.Message.Document.MimeType + "\n\n\n");
                        var str = e.Message.Document.FileName.Split('.')[^1];
                        Console.WriteLine(str);

                        DownLoad(e.Message.Document.FileId, e.Message.Document.FileName);
                        bot.SendTextMessageAsync(e.Message.Chat.Id, $"Принято");
                    }
                    break;

                case MessageType.Audio:
                    {
                        bot.SendTextMessageAsync(e.Message.Chat.Id, $"Принят аудиофайл");
                        var str = e.Message.Audio.FileName.Split('.')[^1];
                        Console.WriteLine(str);
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

        static async void DownLoad(string fileId, string path)
        {
            var file = await bot.GetFileAsync(fileId);
            FileStream fs = new FileStream("_" + path, FileMode.Create);
            await bot.DownloadFileAsync(file.FilePath, fs);
            fs.Close();

            fs.Dispose();
            using (FileStream fss = new FileStream("_+" + path, FileMode.Create))
            {
                await bot.DownloadFileAsync(file.FilePath, fss);
                
            };
        }
    }
}
