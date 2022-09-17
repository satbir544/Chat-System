using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

/*
 *  FILE            : MainWindow.xaml.cs
 *  PROJECT         : Assignment 05
 *  PROGRAMMERS     : Satbir Singh
 *  FIRST VERSION	: 2020-11-03
 *  DESCRIPTION     : The functions in this file are used to make a client socket
*/

namespace A5Client
{
    /*
    *   NAME : MainWindow
    *   PURPOSE : This class has been created for making a client socket used for communicating with other sockets
    */
    public partial class MainWindow : Window
    {
        private static TcpClient client = new TcpClient();
        public static volatile bool run = true;

        /*
	    *	METHOD	    : MainWindow
	    *	DESCRIPTION	: constructor for initializing the components of the main window
	    *	PARAMETERS	: none
	    *	RETURNS		: none
	    */
        public MainWindow()
        {
            InitializeComponent();
        }

        /*
	    *	METHOD	    : ReadMsg
	    *	DESCRIPTION	: reads the messages recieved from the server and prints then to the chat box
	    *	PARAMETERS	: object sender: contains a reference to the object that raised the event
	    *	              CanExecuteRoutedEventArgs e: provides data for the CanExecute and PreviewCanExecute routed events
	    *	RETURNS		: none
	    */
        private void ReadMsg(object sender, DoWorkEventArgs e)
        {
            StreamReader reader = new StreamReader(client.GetStream());

            while (run == true)
            {
                try
                {
                    string msg = reader.ReadLine();
                    if (msg != "")
                    {
                        Dispatcher.Invoke(() =>
                        {
                            chatBox.Text += msg + Environment.NewLine;
                        });
                    }
                }
                catch
                {
                    break;
                }
                
                Thread.Sleep(100);
            }
        }

        /*
	    *	METHOD	    : Send_Click
	    *	DESCRIPTION	: sends a message to the other clients
	    *	PARAMETERS	: object sender: contains a reference to the object that raised the event
	    *	              CanExecuteRoutedEventArgs e: provides data for the CanExecute and PreviewCanExecute routed events
	    *	RETURNS		: none
	    */
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(msgBox.Text))
                {
                    StreamWriter writer = new StreamWriter(client.GetStream());
                    writer.WriteLine(msgBox.Text);
                    writer.Flush();
                    msgBox.Text = "";
                }
                else
                {
                    msgBox.Text = "";
                }
            }
            catch
            {
                chatBox.Text = "You are not connected to the server.";
            }
        }

        /*
	    *	METHOD	    : Connect_Click
	    *	DESCRIPTION	: connects the client to the server
	    *	PARAMETERS	: object sender: contains a reference to the object that raised the event
	    *	              CanExecuteRoutedEventArgs e: provides data for the CanExecute and PreviewCanExecute routed events
	    *	RETURNS		: none
	    */
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            string address = addrBox.Text;
            int port = 13000;

            try
            {
                client.Connect(address, port);
                connectButton.IsEnabled = false;
                disconnectButton.IsEnabled = true;

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += ReadMsg;
                worker.RunWorkerAsync();
            }
            catch
            {
                chatBox.Text = "Server not available.";
            }
        }

        /*
	    *	METHOD	    : Disconnect_Click
	    *	DESCRIPTION	: closes the client
	    *	PARAMETERS	: object sender: contains a reference to the object that raised the event
	    *	              CanExecuteRoutedEventArgs e: provides data for the CanExecute and PreviewCanExecute routed events
	    *	RETURNS		: none
	    */
        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            disconnectButton.IsEnabled = false;
            run = false;
            client.GetStream().Close();
            client.Close();
        }
    }
}
