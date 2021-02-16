﻿using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace SWE1_PPB
{
    class Server
    {
        private TcpListener tcpListener = null;
        private int clientCount = 0;

        public Server(IPAddress iPAddress, int port, NpgsqlConnection conn)
        {
            tcpListener = new TcpListener(iPAddress, port);
            tcpListener.Start();

            Console.Write("Waiting for a connection from client...\n\n");

            ServerListener();
        }

        public void ServerListener()
        {
            try
            {
                // Enter the listening loop.
                while (true)
                {
                    TcpClient client = tcpListener.AcceptTcpClient();

                    Console.WriteLine("Client connected!");

                    // each client gets handled by a thread
                    Thread thread = new Thread(ClientHandler);
                    thread.Start(client);
                }
            }
            catch (SocketException e)
            {
                Console.Write(e.Message);
                tcpListener.Stop();
            }
        }

        public void ClientHandler(Object obj)
        {
            Console.WriteLine(String.Format("{0} active connection/s.\n", ++clientCount));

            TcpClient client = (TcpClient)obj;
            // Buffer for reading data
            Byte[] bytes = new Byte[256];
            String data = null;

            data = null;

            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();

            int i;
            /*try
            {
                // Loop to receive all the data sent by the client.
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                    RequestContext requestContext = new RequestContext(data);

                    string ressource = requestContext.Ressource;
                    string requestAnswer = "";

                    dynamic payloadJson = JObject.Parse(requestContext.Payload);

                    // print Request context
                    requestContext.printRequest();

                    switch (requestContext.HttpVerb)
                    {
                        case "GET":
                            //Console.WriteLine("switch: GET\n");
                            if (Regex.Matches(ressource, "/").Count == 2)
                            {
                                try
                                {
                                    //requestAnswer = messages[Int32.Parse(ressource.Substring(ressource.LastIndexOf("/") + 1)) - 1];
                                }
                                catch (Exception e)
                                {
                                    requestAnswer = "Requested ressource does not exist.\n";
                                }
                            }
                            else
                            {
                                //requestAnswer = generatePrintAllMessages(requestAnswer);
                            }

                            if (requestAnswer.Equals("")) requestAnswer = "No messages found.\n";
                            break;
                        case "POST":
                            //Console.WriteLine("switch: POST\n");

                            //requestAnswer = POSTHandler(requestContext);
                            break;

                        case "PUT":
                            //Console.WriteLine("switch: PUT\n");

                            try
                            {
                                //messages[Int32.Parse(ressource.Substring(ressource.LastIndexOf("/") + 1))] = requestContext.Payload;
                                requestAnswer = "Message updated.\n";

                                //requestAnswer = generatePrintAllMessages(requestAnswer);
                            }
                            catch (Exception e)
                            {
                                requestAnswer = "Requested ressource does not exist.\n";
                            }

                            break;
                        case "DELETE":
                            //Console.WriteLine("switch: DELETE\n");

                            try
                            {
                                //messages.Remove(Int32.Parse(ressource.Substring(ressource.LastIndexOf("/") + 1)));
                                requestAnswer = "Message removed.\n";

                                //requestAnswer = generatePrintAllMessages(requestAnswer);
                            }
                            catch (Exception e)
                            {
                                requestAnswer = "Requested ressource does not exist.\n";
                            }

                            break;
                    }

                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(requestAnswer);

                    // Send back a response.
                    stream.Write(msg, 0, msg.Length);
                    Console.WriteLine("\nAnswered:\n" + requestAnswer + "\n");

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(String.Format("{0} active connection/s.\n", --clientCount));
                client.Close();
            }
            */
        }
    }
}