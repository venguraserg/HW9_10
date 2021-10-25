using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telegram.Bot;
using Telegram.Bot.Args;
using HW9_10;
using System.IO;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using System.Collections.ObjectModel;

namespace HW9_10_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static TelegramBotClient bot;
        static List<User> users;
        static bool statusBot = false;

        public MainWindow()
        {
            InitializeComponent();
            users = new List<User>();
            var token = GetToken();
            TB_Token.Text = token;
            bot = Init.Initialization(ref users, token);
            bot.OnMessage += Bot_OnMessage;
            bot.OnMessage += Bot_RefreshLV;
            
            bot.StartReceiving();
            

        }

        private void Bot_RefreshLV(object sender, MessageEventArgs e)
        {
            LV_Users.Items.Refresh();
        }

        private static string GetToken()
        {
            string path = "token.txt";
            

            if (!File.Exists(path))
            {
                File.Create(path);
            }
            return File.ReadAllText(path);
        }

        [Obsolete]
        private void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            //инициализируем текущего пользователя
            User user = GetUser(e.Message.Chat.Id);
            //определяем тип сообщения
            if (e.Message.Type == MessageType.Text)
            {   //если с "/" в начале определяем как команду
                if (e.Message.Text.Substring(0, 1) == "/")
                {
                    switch (e.Message.Text)
                    {
                        //Команда ХЕЛП
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
                                user.CurrentFolder = string.Empty;


                            }
                            break;
                        // Команда старт
                        case "/start":
                            {
                                bot.SendTextMessageAsync(e.Message.Chat.Id, "/help - для получения списка команд");
                                user.CurrentFolder = string.Empty;
                            }
                            break;
                        // просмотр хранилища
                        case "/veiw_reposytory":
                            {
                                string folderListForMenu = string.Empty;
                                //формируем перечень папок из папки пользователя, добавляем / в начало чтобы проходило как команда
                                foreach (var item in user.FolderList)
                                {
                                    folderListForMenu = folderListForMenu + "/" + item + "\n";

                                }

                                bot.SendTextMessageAsync(e.Message.Chat.Id, folderListForMenu);
                                user.CurrentFolder = string.Empty;
                            }
                            break;
                    }

                    //если в свич чтото не попало, значит это либо имя папки, либо чтото непонятное...
                    //сдесь мы ловим имя нужнй папки
                    var folderRequest = e.Message.Text.TrimStart('/');
                    var userFolder = user.FolderList.SingleOrDefault(i => i == e.Message.Text.TrimStart('/'));
                    //если есть такая папка у юзера - выведем список файлов этой папки
                    if (folderRequest == userFolder)
                    {
                        user.CurrentFolder = folderRequest;
                        UpdapeListUsers(user);
                        var fileList = Directory.GetFiles(System.IO.Path.Combine(user.RepoPath, e.Message.Text.TrimStart('/')));
                        var fileListForMenu = $"Список файлов в директории {folderRequest}.\nВыберите номер файла\n";
                        for (int i = 0; i < fileList.Length; i++)
                        {
                            fileListForMenu = fileListForMenu + $"{i + 1}." + new FileInfo(fileList[i]).Name + "\n";
                        }

                        bot.SendTextMessageAsync(e.Message.Chat.Id, fileListForMenu);
                    }



                }
                else
                {
                    //выбираем файл для выгрузки 
                    if (user.CurrentFolder != string.Empty)
                    {
                        var correctParse = int.TryParse(e.Message.Text, out int numberFile);
                        var fileList = Directory.GetFiles(System.IO.Path.Combine(user.RepoPath, user.CurrentFolder));
                        if (correctParse && (numberFile > 0 && numberFile <= fileList.Length))
                        {
                            string tempPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Repository", user.CurrentFolder, fileList[numberFile - 1]);
                            string tempFileName = new FileInfo(fileList[numberFile - 1]).Name;
                            Console.WriteLine($"передача файла {tempPath}");
                            _ = UploadFileAsync(e.Message.Chat.Id, tempPath, tempFileName);
                            user.CurrentFolder = string.Empty;
                        }
                        else
                        {
                            Console.WriteLine("not correct input");
                            bot.SendTextMessageAsync(e.Message.Chat.Id, "не верный ввод. . .");
                            user.CurrentFolder = string.Empty;
                        }
                    }

                }
            }
            else
            {
                // если не ТЕКСТ то загружаем в определенную папку
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


                    default:

                        bot.SendTextMessageAsync(e.Message.Chat.Id, $"не верный ввод, тип данных {e.Message.Type.ToString()} не поддерживается, отправьте файл или команду. . .");
                        break;
                }

            }

            

        }

        /// <summary>
        /// Асинхронный медод выгрузки файла из хранилища
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static async Task UploadFileAsync(long chatId, string path, string fileName)
        {
            using (var stream = File.OpenRead(path))
            {
                InputOnlineFile iof = new InputOnlineFile(stream);
                iof.FileName = fileName;
                await bot.SendDocumentAsync(chatId, iof);
            }
        }




        /// <summary>
        /// Обновление листа пользователей
        /// </summary>
        /// <param name="user"></param>
        private static void UpdapeListUsers(User user)
        {
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].UserId == user.UserId)
                {
                    users[i] = user;
                    return;
                }
            }
        }

        /// <summary>
        /// Получение пользователя по Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
                for (int i = 0; i < users.Count; i++)
                {
                    if (users[i].UserId == id) users[i] = new User(id, users[i].CurrentFolder);
                }

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
                if (users[i].UserId == id)
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
        static async void DownLoad(string fileId, string fileName, string filePath)
        {
            var file = await bot.GetFileAsync(fileId);

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            var tempPath = System.IO.Path.Combine(filePath, fileName);
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
            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Repository", userId.ToString(), currentDir);
            return path;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            statusBot = !statusBot;
            if (statusBot)
            {
                BTN_ChangeStatusBot.Content = "STOP BOT";
                TB_Token.IsEnabled = false;
                this.Title = "Хранитель работает";
                bot = new TelegramBotClient(TB_Token.Text);
                bot.StartReceiving();
            }
            else
            {
                BTN_ChangeStatusBot.Content = "START BOT";
                TB_Token.IsEnabled = true;
                this.Title = "Хранитель выключен";
                bot.StopReceiving();
            }
            LV_Users.Items.Refresh();
        }
    }
}
