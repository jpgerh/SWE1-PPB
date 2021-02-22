using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SWE1_PPB
{
    class Server
    {
        private TcpListener tcpListener = null;
        private int clientCount = 0;
        DBHandler dBHandler = new DBHandler();

        public Server(IPAddress iPAddress, int port)
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

                    //Console.WriteLine("Client connected!");

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

        public async Task<string> DELETEHandler(RequestContext requestContext)
        {
            MMC mmc = new MMC();
            User user = new User();

            string ressource = requestContext.Ressource;
            string subressource = ressource.Substring(ressource.IndexOf('/', ressource.IndexOf('/') + 1) + 1);
            ressource = ressource.Substring(0, ressource.IndexOf('/', ressource.IndexOf('/') + 1));

            switch (ressource)
            {
                case "/lib":
                    {
                        string authorization;
                        try
                        {
                            authorization = requestContext.HeaderValues["Authorization"];
                            if (await user.verifyToken(authorization))
                            {
                                await mmc.DeleteMMC(authorization, subressource);
                                return $"{authorization}: Authorization successful. " + await mmc.DeleteMMC(authorization, subressource);
                            }
                            else
                            {
                                return $"{authorization}: Authorization failed.";
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        break;
                    }
            }

            return "";

        }

        public async Task<string> PUTHandler(RequestContext requestContext)
        {

            User user = new User();
            string ressource = requestContext.Ressource;
            string subressource = "";
            if (ressource.Contains("users"))
            {
                subressource = ressource.Substring(ressource.IndexOf('/', ressource.IndexOf('/') + 1) + 1);
                ressource = ressource.Substring(0, ressource.IndexOf('/', ressource.IndexOf('/') + 1));
            }

            switch (ressource)
            {
                case "/users":
                    {
                        dynamic payloadJson = JObject.Parse(requestContext.Payload);
                        string name = (string)payloadJson.Name ?? "";
                        string bio = (string)payloadJson.Bio ?? "";
                        string image = (string)payloadJson.Image ?? "";
                        

                        string authorization;
                        try
                        {
                            authorization = requestContext.HeaderValues["Authorization"];
                            if (await user.verifyToken(authorization))
                            {
                                return await user.editUser(authorization, subressource, name, bio, image);
                            }
                            else
                            {
                                return $"{authorization}: Authorization failed.";
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        break;
                    }
                case "/playlist":
                    {
                        MMC mmc = new MMC();
                        string authorization;
                        dynamic payloadJson = JObject.Parse(requestContext.Payload);
                        int fromPosition = (int)(payloadJson.FromPosition ?? -1);
                        int toPosition = (int)(payloadJson.ToPosition ?? -1);
                        try
                        {
                            authorization = requestContext.HeaderValues["Authorization"];
                            if (await user.verifyToken(authorization))
                            {
                                if (await user.verifyAdmin(authorization) && fromPosition != -1 && toPosition != -1)
                                {
                                    mmc.reorderPlaylist(fromPosition, toPosition);
                                    return "Reorder successful.\n";
                                        
                                } 
                                else
                                {
                                    return $"{authorization}: Reorder failed. User is not admin.\n";
                                }
                                
                            }
                            else
                            {
                                return $"{authorization}: Authorization failed.\n";
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    }
                case "/actions":
                    {
                        string authorization;
                        dynamic payloadJson = JObject.Parse(requestContext.Payload);
                        string actions = (string)payloadJson.actions ?? "";
                        try
                        {
                            authorization = requestContext.HeaderValues["Authorization"];
                            if (await user.verifyToken(authorization))
                            {
                                return $"{authorization}: Authorization successful. " + await user.addAction(authorization, actions);
                            }
                            else
                            {
                                return $"{authorization}: Authorization failed.";
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    }
            }


            return "";
        }

        public async Task<string> GETHandler(RequestContext requestContext)
        {
            User user = new User();
            string ressource = requestContext.Ressource;
            string subressource = "";
            if (ressource.Contains("users"))
            {
                subressource = ressource.Substring(ressource.IndexOf('/', ressource.IndexOf('/') + 1) + 1);
                ressource = ressource.Substring(0, ressource.IndexOf('/', ressource.IndexOf('/') + 1));
            }

            switch (ressource)
            {
                case "/users":
                    {
                        string authorization;
                        try
                        {
                            authorization = requestContext.HeaderValues["Authorization"];
                            if (await user.verifyToken(authorization))
                            {
                                return await user.getUserData(authorization, subressource);
                            }
                            else
                            {
                                return $"{authorization}: Authorization failed.";
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        break;
                    }
                case "/stats":
                    {
                        //Console.WriteLine("stats");
                        string authorization;
                        try
                        {
                            authorization = requestContext.HeaderValues["Authorization"];
                            if (await user.verifyToken(authorization))
                            {
                                return await user.getUserStats(authorization);
                            }
                            else
                            {
                                return $"{authorization}: Authorization failed.";
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        break;
                    }
                case "/score":
                    {
                        string authorization;
                        try
                        {
                            authorization = requestContext.HeaderValues["Authorization"];
                            if (await user.verifyToken(authorization))
                            {
                                return await user.getScoreboard(authorization);
                            }
                            else
                            {
                                return $"{authorization}: Authorization failed.";
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        break;
                    }
                case "/playlist":
                    {
                        MMC mmc = new MMC();
                        try
                        {

                            return await mmc.getPlaylist();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        break;
                    }
                case "/actions":
                    {
                        string authorization;
                        try
                        {
                            authorization = requestContext.HeaderValues["Authorization"];
                            if (await user.verifyToken(authorization))
                            {
                                return await user.getAction(authorization);
                            }
                            else
                            {
                                return $"{authorization}: Authorization failed.";
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        break;
                    }
                case "/lib":
                    {
                        MMC mmc = new MMC();
                        string authorization;
                        try
                        {
                            authorization = requestContext.HeaderValues["Authorization"];
                            if (await user.verifyToken(authorization))
                            {
                                return await mmc.getLibrary(authorization);
                            }
                            else
                            {
                                return $"{authorization}: Authorization failed.";
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        break;
                    }
            }

            return "";
        }

        public async Task<string> POSTHandler(RequestContext requestContext)
        {
            User user = new User();
            string ressource = requestContext.Ressource;

            switch (ressource)
            {
                case "/users":
                    {
                        dynamic payloadJson = JObject.Parse(requestContext.Payload);
                        string username = (string)payloadJson.Username;
                        string password = (string)payloadJson.Password;

                        bool success = await user.Register(username, password);

                        if (success)
                        {
                            return "User registered.";
                        }
                        else
                        {
                            return "User already registered.";
                        }
                        break;
                    }
                case "/sessions":
                    {
                        dynamic payloadJson = JObject.Parse(requestContext.Payload);
                        string username = (string)payloadJson.Username;
                        string password = (string)payloadJson.Password;

                        string token = await user.Login(username, password);

                        if (token.Equals(""))
                        {
                            return "Login failed.";
                        } else
                        {
                            return $"Token: {token}";
                        }

                        break;
                    }
                case "/lib":
                    {
                        dynamic payloadJson = JObject.Parse(requestContext.Payload);

                        MMC mmc = new MMC((string)payloadJson.Name ?? "", (string)payloadJson.Url ?? "", (string)payloadJson.Filetype ?? "", (string)payloadJson.Title ?? "", (string)payloadJson.Artist ?? "", 
                            (string)payloadJson.Album ?? "", (string)payloadJson.Genre ?? "", (string)payloadJson.Path ?? "", (int)(payloadJson.Filesize ?? 0), (int)(payloadJson.Rating ?? 0), (string)payloadJson.Length ?? "");

                        string authorization;
                        try
                        {
                            authorization = requestContext.HeaderValues["Authorization"];
                            if (await user.verifyToken(authorization))
                            {
                                
                                return $"{authorization}: Authorization successful. " + await mmc.AddMMC(authorization) + "\n";
                            }
                            else
                            {
                                return $"{authorization}: Authorization failed.\n";
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        return "Could not get token.";

                        break;
                    }
                case "/battles":
                    {
                        string authorization;
                        try
                        {
                            authorization = requestContext.HeaderValues["Authorization"];

                            if (await user.verifyToken(authorization))
                            {
                                user.setOnline(authorization);
                                return $"{authorization}: Authorization successful.\n";
                            }
                            else
                            {
                                return $"{authorization}: Authorization failed.\n";
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        return "Could not get token.";

                        break;
                    }
                case "/playlist":
                    {

                        dynamic payloadJson = JObject.Parse(requestContext.Payload);
                        string songname = (string)payloadJson.Name;
                        MMC mmc = new MMC();
                        string authorization;

                        try
                        {
                            authorization = requestContext.HeaderValues["Authorization"];

                            if (await user.verifyToken(authorization))
                            {
                                return $"{authorization}: Authorization successful. " + await mmc.AddSongToPlaylist(authorization, songname);
                            }
                            else
                            {
                                return $"{authorization}: Authorization failed.\n";
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        return "Could not get token.";

                        break;
                    }
            }

            return "";
        }

        public async void ClientHandler(Object obj)
        {
            //Console.WriteLine(String.Format("{0} active connection/s.\n", ++clientCount));

            TcpClient client = (TcpClient)obj;
            // Buffer for reading data
            Byte[] bytes = new Byte[1024];
            String data = null;

            data = null;

            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();

            
            int i;
            try
            {
                // Loop to receive all the data sent by the client.
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                    RequestContext requestContext = new RequestContext(data);

                    string ressource = requestContext.Ressource;
                    string requestAnswer = "";

                    //dynamic payloadJson = JObject.Parse(requestContext.Payload);

                    // print Request context
                    //requestContext.printRequest();

                    switch (requestContext.HttpVerb)
                    {
                        case "GET":
                            requestAnswer = await GETHandler(requestContext);
                            break;
                        case "POST":
                            requestAnswer = await POSTHandler(requestContext);
                            break;
                        case "PUT":
                            requestAnswer = await PUTHandler(requestContext);
                            break;
                        case "DELETE":
                            requestAnswer = await DELETEHandler(requestContext);
                            break;
                    }

                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(requestAnswer);

                    // Send back a response.
                    stream.Write(msg, 0, msg.Length);
                    Thread.CurrentThread.Abort();
                    //Console.WriteLine("\nAnswered:\n" + requestAnswer + "\n");

                }

            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                //Console.WriteLine(String.Format("{0} active connection/s.\n", --clientCount));
                client.Close();
            }
            
            
        }
    }
}