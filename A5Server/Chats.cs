using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/*
 *  FILE            : Chats.cs
 *  PROJECT         : Assignment 05
 *  PROGRAMMERS     : Satbir Singh
 *  FIRST VERSION	: 2020-11-03
 *  DESCRIPTION     : The functions in this file are used to make a server socket.
*/

namespace A5Server
{
    /*
    *   NAME : Chats
    *   PURPOSE : This class has been created for making a server socket used for recieving and sending messages to other clients.
    */
    public class Chats
    {
        TcpClient chatClient;
        StreamReader reader;
        StreamWriter writer;
        string uName;
        bool userTextEmpty = true;

        /*
	    *	METHOD	    : Chats
	    *	DESCRIPTION	: constructor used for starting the client thread
	    *	PARAMETERS	: none
	    *	RETURNS		: none
	    */
        public Chats(TcpClient Client)
        {
            chatClient = Client;
            Thread clientThread = new Thread(new ThreadStart(Start));
            clientThread.Start();
        }

        /*
	    *	METHOD	    : GetName
	    *	DESCRIPTION	: gets the name of the client
	    *	PARAMETERS	: none
	    *	RETURNS		: none
	    */
        private string GetName()
        {
            writer.WriteLine("Enter your Name:");
            writer.Flush();
            string name = reader.ReadLine();

            return name;
        }

        /*
	    *	METHOD	    : CheckText
	    *	DESCRIPTION	: displayes the messages sent by the clients
	    *	PARAMETERS	: none
	    *	RETURNS		: none
	    */
        private void CheckText()
        {
            try
            {
                string userText = "";
                while (userTextEmpty)
                {
                    userText = reader.ReadLine();
                    ChatServer.DisplayMessages(uName, userText);
                }
            }
            catch
            {
                Console.WriteLine("Client disconnected.");
            }
        }

        /*
	    *	METHOD	    : UserExists
	    *	DESCRIPTION	: gets the user's name until it is different than the ones already connected
	    *	PARAMETERS	: none
	    *	RETURNS		: none
	    */
        private void UserExists()
        {
            while (ChatServer.userName.Contains(uName))
            {
                writer.WriteLine(uName + ": User already exists!");
                writer.Flush();
                uName = GetName();
            }
        }

        /*
	    *	METHOD	    : AddUser
	    *	DESCRIPTION	: adds a user to the chat
	    *	PARAMETERS	: none
	    *	RETURNS		: none
	    */
        private void AddUser()
        {
            ChatServer.userName.Add(uName, chatClient);
            ChatServer.userConnected.Add(chatClient, uName);
            ChatServer.SendMessage(uName + " joined!");
        }

        /*
	    *	METHOD	    : Start
	    *	DESCRIPTION	: starts the client thread
	    *	PARAMETERS	: none
	    *	RETURNS		: none
	    */
        private void Start()
        {
            reader = new StreamReader(chatClient.GetStream());
            writer = new StreamWriter(chatClient.GetStream());
            writer.WriteLine("Welcome to the Chat System!");
            uName = GetName();
            UserExists();
            AddUser();
            writer.Flush();
            Thread clientThread = new Thread(new ThreadStart(CheckText));
            clientThread.Start();
        }
    }
}
