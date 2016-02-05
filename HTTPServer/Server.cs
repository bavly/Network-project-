using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint hostEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket.Bind(hostEndPoint);
        }
        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(1000);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = this.serverSocket.Accept();
                //create thread

                Thread newThread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newThread.Start(clientSocket);

            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket
            Socket clientSocket = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientSocket.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.

            string request = "";
            //string response = "";

            ///////////////size is enough
            byte[] requestMsg = new byte[1024];
            byte[] receivedMsg = new byte[1024];


            while (true)
            {
                try
                {
                    // TODO: Receive request
                    int receivedLength = clientSocket.Receive(requestMsg);
                    // TODO: break the while loop if receivedLen==0
                    if (receivedLength == 0)
                    {
                        break;
                    }
                    // TODO: Create a Request object using received request string
                    request = Encoding.ASCII.GetString(requestMsg, 0, receivedLength);

                    Request req = new Request(request);
                    // TODO: Call HandleRequest Method that returns the response
                    Response resp = HandleRequest(req);


                    // TODO: Send Response back to client
                    receivedMsg = Encoding.ASCII.GetBytes(resp.ResponseString);
                    clientSocket.Send(receivedMsg);

                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            clientSocket.Close();
        }


        Response HandleRequest(Request request)
        {
            string content = "";
            StatusCode code;
            try
            {
                //TODO: check for bad request 
                bool badRequest = request.ParseRequest();
                if (badRequest == false)
                {
                    code = StatusCode.BadRequest;
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                }

                //TODO: map the relativeURI in request to get the physical path of the resource.
                string physicalPath = Configuration.RootPath + "\\" + request.relativeURI.Substring(1, request.relativeURI.Length - 1);
                //TODO: check for redirect
                string filename = request.relativeURI.Substring(1, request.relativeURI.Length - 1);
                //string filename = Path.GetFileName(request.relativeURI);
                string redirection = this.GetRedirectionPagePathIFExist(filename);

                // redirect exist
                if (redirection != "")
                {
                    content = LoadDefaultPage(redirection);
                    if (content != "")
                    {
                        content = LoadDefaultPage(redirection);
                        code = StatusCode.OK;
                    }
                    else
                    {
                        code = StatusCode.Redirect;
                        content = LoadDefaultPage(Configuration.RedirectionDefaultPageName);
                    }
                }
                else
                {
                    //TODO: check file exists
                    // physical file not found
                    //TODO: read the physical file
                    content = LoadDefaultPage(physicalPath);
                    if (content == "")
                    {
                        code = StatusCode.NotFound;
                        content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    }
                    // Create OK response
                    else
                    {
                        code = StatusCode.OK;
                    }
                }
                Response resp = new Response(code, "text/html", content, redirection);
                return resp;
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                //code = StatusCode.InternalServerError;
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                Response resp = new Response(StatusCode.InternalServerError, "text/html", content, null);
                return resp;
            }
        }

  

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            if (Configuration.RedirectionRules.ContainsKey(relativePath))
            {
                return Configuration.RedirectionRules[relativePath];
            }
            return string.Empty;
        }
        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            if (!File.Exists(filePath))
            {
                Logger.LogException(new Exception("Not exist " + defaultPageName));
                return string.Empty;
            }
            // else read file and return its content
            StreamReader read = new StreamReader(filePath);
            string file = read.ReadToEnd();
            read.Close();
            return file;
        }
        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                StreamReader read = new StreamReader(filePath);

                // then fill Configuration.RedirectionRules dictionary
                //???
                Configuration.RedirectionRules = new Dictionary<string, string>();
                while (!read.EndOfStream)
                {
                    string line = read.ReadLine();
                    string[] splitedLine = line.Split(',');
                    Configuration.RedirectionRules.Add(splitedLine[0], splitedLine[1]);
                }

            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}