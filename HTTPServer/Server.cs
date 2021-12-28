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
            IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse("127.0.0.1"), portNumber);
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.serverSocket.Bind(ipEnd);
            
           
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

                Console.WriteLine("New client accepted: {0}", clientSocket.RemoteEndPoint + "connected = " +clientSocket.Connected);

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
                    byte[] dataReceived =new byte[100000] ;
                    int len = clientSocket.Receive(dataReceived);
                    // TODO: break the while loop if receivedLen==0
                    if (len == 0)
                        break;
                    // TODO: Create a Request object using received request string

                 //   Console.WriteLine(Encoding.ASCII.GetString(dataReceived));
                    Request request = new Request(Encoding.ASCII.GetString( dataReceived ) );
                    // TODO: Call HandleRequest Method that returns the response
                    Response response= HandleRequest(request);
                    // TODO: Send Response back to client
                   // Console.WriteLine(response.ResponseString);
                   
                    
                    clientSocket.Send(Encoding.ASCII.GetBytes(response.ResponseString) );
 
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            // TODO: close client socket
        }
 

        Response HandleRequest(Request request)
        {
           // throw new NotImplementedException();
            string content ="";
            bool isGoodRequest = false;
            string physicalPath = "";
            string redirectedUri = "";
            int statusCode = (int)StatusCode.OK;
            StreamReader reader;
            try
            {
                //TODO: check for bad request 
                isGoodRequest = request.ParseRequest();

                if (!isGoodRequest)
                {
                    physicalPath = Configuration.RootPath + "\\" + Configuration.BadRequestDefaultPageName;
                    //TODO: check file exists
                    reader = new StreamReader(physicalPath);
                    //TODO: read the physical file
                    content = reader.ReadToEnd();

                    return new Response(StatusCode.BadRequest, "text/html", "Bad request", redirectedUri);
                }

                if (request.relativeURI == "/")
                
                {
                    return new Response(StatusCode.OK, "text/html", LoadDefaultPage("main.html"), redirectedUri); 
                }
                //TODO: map the relativeURI in request to get the physical path of the resource.

                LoadRedirectionRules("redirectionRules.txt");

                //TODO: check for redirect
                redirectedUri = GetRedirectionPagePathIFExist(request.relativeURI);

                if(redirectedUri == "")
                {
                    physicalPath += Configuration.RootPath+"\\"+request.relativeURI;
                }
                else
                {
                    physicalPath = Configuration.RootPath + "\\" +Configuration.RedirectionDefaultPageName;

                    //Load Redirect Page To make him request again
                    reader = new StreamReader(physicalPath);
                    //TODO: read the physical file
                    content = reader.ReadToEnd();

                    return  new Response(StatusCode.Redirect, "text/html", content, redirectedUri);
                     
                }
                //


                //TODO: check file exists
               reader = new StreamReader(physicalPath);
                //TODO: read the physical file
                content = reader.ReadToEnd();
                // Create OK response
                Response response = new Response(StatusCode.OK, "text/html", content, redirectedUri);

                return response;
            }

            catch(FileNotFoundException ex)
            {
                //statusCode = (int)

                physicalPath = Configuration.RootPath + "\\" + Configuration.NotFoundDefaultPageName;
                //TODO: check file exists
                reader = new StreamReader(physicalPath);
                //TODO: read the physical file
                content = reader.ReadToEnd();

                Response response = new Response(StatusCode.NotFound, "text/html", content, redirectedUri);

                return response;
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class

               
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                string redirectionPath = GetRedirectionPagePathIFExist(request.relativeURI);

                physicalPath = Configuration.RootPath + "\\" + Configuration.InternalErrorDefaultPageName;
                //TODO: check file exists
                reader = new StreamReader(physicalPath);
                //TODO: read the physical file
                content = reader.ReadToEnd();

                
                return new Response(StatusCode.InternalServerError, "text/html", content, redirectionPath);
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            string redirectedPage;

          relativePath =   relativePath.Replace("/","");

                if (Configuration.RedirectionRules.ContainsKey(relativePath))
                {


                   Configuration.RedirectionRules.TryGetValue(relativePath,out redirectedPage);

                //Append the root path to the new path of the redirected page

                if (redirectedPage != null)
                   return  Configuration.RootPath + "\\" + redirectedPage;

                

                   
                }
            

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

                int r = 0;
                   
                while (!reader.EndOfStream)
                {
                    

                    line =   reader.ReadLine().Trim().Split(seprators);

                    oldAddress = line[0];
                    newAddress = line[1];

                    // then fill Configuration.RedirectionRules dictionary 

                    if(oldAddress != null && newAddress !=null)
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
