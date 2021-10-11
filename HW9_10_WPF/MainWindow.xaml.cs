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

namespace HW9_10_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TelegramBotClient bot;
        Message msg = new Message { Id = 0, Text = "" };
        public MainWindow()
        {
            InitializeComponent();
            bot = new TelegramBotClient("2010417984:AAFJIIV0O0tRp-i-uOffQD4gxdQxr_HcUcg");
            bot.OnMessage += Bot_OnMessage;
            bot.StartReceiving();
        }

        private void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            msg.Id = e.Message.Chat.Id;
            msg.Text = $"Кузьмич не дрыщи усе будзе добра {e.Message.Chat.Id} {e.Message.Text}";
            //
            //this.Title = e.Message.Text;

        }

        private void Btn_1_Click(object sender, RoutedEventArgs e)
        {
            bot.SendTextMessageAsync(msg.Id, msg.Text);
        }
    }
    public class Message
    {
        public long Id { get; set; }
        public string Text { get; set; }
    }
}
