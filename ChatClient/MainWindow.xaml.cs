﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace ChatClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string userName;
        private const string host = "127.0.0.1";
        private const int port = 8888;
        static TcpClient client;
        static NetworkStream stream;

        public MainWindow()
        {
            InitializeComponent();

            userName = username.Text;
            client = new TcpClient();      
            client.Connect(host, port); //подключение клиента
                stream = client.GetStream(); // получаем поток

                string message = userName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);
                log.Text = "Добро пожаловать, " + userName;
            // запускаем новый поток для получения данных

            Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
            receiveThread.Start(); //старт потока
                
        }

        
        // отправка сообщений
        void SendMessage()
        {
          
                string messageTo = message.Text;
                byte[] data = Encoding.Unicode.GetBytes(messageTo);
                stream.Write(data, 0, data.Length);
            
        }
        // получение сообщений
         void ReceiveMessage()
        {
            while (true)
            {               
                    byte[] data = new byte[64]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);
                    
                    string message = builder.ToString();
                    Dispatcher.Invoke(()=> { log.AppendText(message + "\r\n"); });//вывод сообщения
                
            }
        }

        static void Disconnect()
        {
            if (stream != null)
                stream.Close();//отключение потока
            if (client != null)
                client.Close();//отключение клиента
            Environment.Exit(0); //завершение процесса
        }

        private void Send(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }
    }
}
