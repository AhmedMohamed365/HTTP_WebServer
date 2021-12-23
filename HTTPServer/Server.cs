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
            IPEndPoint ipEnd = new IPEndPoint(IPAddress.Any, portNumber);
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.serverSocket.Bind(ipEnd);
            //fawzi moza
           
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            this.serverSocket.Listen(100);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = this.serverSocket.Accept();
                Thread thread = new Thread(new ParameterizedThreadStart(HandleConnection));
                thread.Start(clientSocket);
 
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket clientSocket = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientSocket.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request

                    // TODO: break the while loop if receivedLen==0

                    // TODO: Create a Request object using received request string

                    // TODO: Call HandleRequest Method that returns the response

                    // TODO: Send Response back to client

                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                }
            }

            // TODO: close client socket
        }

        Response HandleRequest(Request request)
        {
            throw new NotImplementedException();
            string content ="";
            try
            {
                //TODO: check for bad request 

                //TODO: map the relativeURI in request to get the physical path of the resource.
                
                //TODO: check for redirect

                //TODO: check file exists

                //TODO: read the physical file

                // Create OK response
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class

                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                string redirectionPath = GetRedirectionPagePathIFExist(request.relativeURI);

                //Third param need to be revised ??
             return new Response(StatusCode.InternalServerError, "text/html", "Internal Server Error", redirectionPath);
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty

           string redirectedPage =  Configuration.RedirectionRules[relativePath];

            //Append the root path to the new path of the redirected page

            if (redirectedPage != null)

                return Configuration.RootPath + redirectedPage;

            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            string fileContent = "";
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            StreamReader reader;
            try
            {
                 reader = new StreamReader(filePath);
            }
            catch(FileNotFoundException ex)
            {
                Logger.LogException(ex);

                return string.Empty;
            }
            // else read file and return its content
           fileContent =  reader.ReadToEnd();
            reader.Close();
            return fileContent;
        }

        private void LoadRedirectionRules(string filePath)
        {
            StreamReader reader = new StreamReader(filePath);
            try
            {
                Configuration.RedirectionRules = new Dictionary<string, string>();
                // TODO: using the filepath paramter read the redirection rules from file 

                 
                string [] line ;
                string oldAddress, newAddress;

                char[] seprators = { ',' };
                while (reader.EndOfStream)
                {
                    line =   reader.ReadLine().Trim().Split(seprators);

                    oldAddress = line[0];
                    newAddress = line[2];

                    // then fill Configuration.RedirectionRules dictionary 

                    Configuration.RedirectionRules.Add(oldAddress, newAddress);

                }

                reader.Close();
               

                //I Think Done ? 


            }
            catch (Exception ex)
            {
                reader.Close();
                // TODO: log exception using Logger class

                Logger.LogException(ex);

                Environment.Exit(1);
            }
        }
    }
}
