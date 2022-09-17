using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

/*
 *  FILE            : ChatServer.cs
 *  PROJECT         : Assignment 05
 *  PROGRAMMERS     : Satbir Singh
 *  FIRST VERSION	: 2020-11-03
 *  DESCRIPTION     : The functions in this file are used to make a server socket.
*/

namespace A5Server
{
    /*
    *   NAME : ChatServer
    *   PURPOSE : This class has been created for making a server socket used for recieving and sending messages to other clients.
    */
    public class ChatServer
    {
        public static TcpListener chatServer;
        public static Hashtable userName;
        public static Hashtable userConnected;
        private static bool checkingConnection = true;
        private static string addr;
        private static readonly int port = 13000;

        static void Main(string[] args)
        {
            Console.Write("Enter IP Address: ");
            addr = Console.ReadLine();

            ChatServer chatServer = new ChatServer();
        }

        /*
	    *	METHOD	    : ChatServer
	    *	DESCRIPTION	: constructor used for connecting the clients
	    *	PARAMETERS	: none
	    *	RETURNS		: none
	    */
        public ChatServer()
        {
            IPAddress localAddr = IPAddress.Parse(addr);
            userName = new Hashtable(100);
            userConnected = new Hashtable(100);
            chatServer = new TcpListener(localAddr, port);
            Console.WriteLine("Waiting for a connection...");

            int nUsers = 1;
            while (checkingConnection)
            {
                chatServer.Start();
                if (chatServer.Pending())
                {
                    if (nUsers == 1)
                    {
                        Console.WriteLine(nUsers + " Client Connected.");
                    }
                    else if (nUsers > 1)
                    {
                        Console.WriteLine(nUsers + " Clients Connected.");
                    }

                    TcpClient client = chatServer.AcceptTcpClient();
                    Chats chatProcess = new Chats(client);
                    nUsers++;
                }
            }
        }

        /*
	    *	METHOD	    : SendMessage
	    *	DESCRIPTION	: sends a message to the clients
	    *	PARAMETERS	: string message: the message to send
	    *	RETURNS		: none
	    */
        public static void SendMessage(string message)
        {
            StreamWriter writer;
            TcpClient[] tcpClients = new TcpClient[ChatServer.userName.Count];
            ChatServer.userName.Values.CopyTo(tcpClients, 0);
            int i = 0;
            while (i < tcpClients.Length)
            {
                try
                {
                    if (String.IsNullOrEmpty(message) || tcpClients[i] == null)
                    {
                        continue;
                    }
                    else
                    {
                        writer = new StreamWriter(tcpClients[i].GetStream());
                        writer.WriteLine(message);
                        writer.Flush();
                    }
                }
                catch
                {
                    ChatServer.userName.Remove(ChatServer.userConnected[tcpClients[i]]);
                    ChatServer.userConnected.Remove(tcpClients[i]);
                }
                i++;
            }
        }

        /*
	    *	METHOD	    : DisplayMessages
	    *	DESCRIPTION	: displayes the messages recieved from a client
	    *	PARAMETERS	: string name: the name of the client
	    *	              string clientMessage: message recieved from a client
	    *	RETURNS		: none
	    */
        public static void DisplayMessages(string name, string clientMessage)
        {
            StreamWriter writer;
            TcpClient[] tcpClients = new TcpClient[ChatServer.userName.Count];
            ChatServer.userName.Values.CopyTo(tcpClients, 0);
            int i = 0;
            while (i < tcpClients.Length)
            {
                try
                {
                    if (String.IsNullOrEmpty(clientMessage) || tcpClients[i] == null)
                    {
                        continue;
                    }
                    else
                    {
                        writer = new StreamWriter(tcpClients[i].GetStream());
                        writer.WriteLine(name + ": " + clientMessage);
                        writer.Flush();
                        //Console.WriteLine(name + ": " + clientMessage);
                    }
                }
                catch
                {
                    string userLeft = (string)ChatServer.userConnected[tcpClients[i]];
                    ChatServer.SendMessage(userLeft + " has left :(");
                    ChatServer.userName.Remove(userLeft);
                    ChatServer.userConnected.Remove(tcpClients[i]);
                }
                i++;
            }
        }
    }
}
