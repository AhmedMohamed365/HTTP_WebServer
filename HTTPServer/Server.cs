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
        bool isFileFounded = false;
        bool dont = false;
        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            IPEndPoint ipEnd = new IPEndPoint(IPAddress.Any, portNumber);
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
                
              //  Console.WriteLine("New client accepted: {0}", clientSocket.RemoteEndPoint + "connected = " +clientSocket.Connected);

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
                    Response response = HandleRequest(request);
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
            string redirectedUri = "";
           
           
            StreamReader reader;
            try
            {
                //TODO: check for bad request 
                isGoodRequest = request.ParseRequest();

                if (!isGoodRequest)
                {
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);

                    
                    return new Response(StatusCode.BadRequest, "text/html", content , redirectedUri);
                }

                //Check if it's Post Method
                if(request._method == RequestMethod.POST)
                {
                    


                    
                    

                        string[] infos;
                        string[] message;
                        infos = request._content.Split('&');

                        redirectedUri = "";
                        content = "";
                        foreach (var info in infos)
                        {
                            message = info.Trim().Split('=');

                            content += string.Format("<p><h1> {0} </h1> {1}<p> <br> ", message[0], message[1]);
                        }



                        redirectedUri = Path.Combine(Configuration.RootPath, "infoConfirmed.html");

                        StreamWriter writer = new StreamWriter(redirectedUri, false);

                        Console.WriteLine("Correct");
                        writer.Write(content);
                        writer.Close();

                    redirectedUri = "";

                    //Now we don't need to have redirtection response
                        //content = LoadDefaultPage(Configuration.RedirectionDefaultPageName);

                        return new Response(StatusCode.OK, "text/html", content, redirectedUri);


                    
                }

                if (request.relativeURI == "/")
                {

                    
                    content = LoadDefaultPage("main.html");

                    if (content == "")
                        isFileFounded = true;

                    else
                        //Succeed to find file content 
                        return new Response(StatusCode.OK, "text/html", content, redirectedUri);
                   
                   
                }
                //TODO: map the relativeURI in request to get the physical path of the resource.

                LoadRedirectionRules("redirectionRules.txt");

                //TODO: check for redirect 
                redirectedUri = GetRedirectionPagePathIFExist(request.relativeURI);

                if(redirectedUri == "")
                {

                   
                        request.relativeURI = request.relativeURI.Replace("/", "");

                    

                     content = LoadDefaultPage(request.relativeURI);

                    if (content == "")
                        isFileFounded = true;

                    else
                    return new Response(StatusCode.OK, "text/html", content, redirectedUri);
                }
                else
                {
                    content = LoadDefaultPage(Configuration.RedirectionDefaultPageName);

                    if (content == "")
                        isFileFounded = true;

                    else

                        return  new Response(StatusCode.Redirect, "text/html", content, redirectedUri);
                     

                }

                if (isFileFounded)
                {
                    content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);

                    Response response = new Response(StatusCode.NotFound, "text/html", content, redirectedUri);

                    return response;
                }


               // Console.WriteLine("Exception");
                return null;
            }

         
          

            catch (Exception ex)
            {
                // TODO: log exception using Logger class


                Console.WriteLine("Exception");
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                //GetRedirectionPagePathIFExist(request.relativeURI);

                string redirectionPath = "";

                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);

                
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
            StreamReader reader = null;

            try
            {
                 reader = new StreamReader(filePath);
            }
            catch(FileNotFoundException ex)
            {
               // reader.Close();

                Logger.LogException(ex);

               

                return string.Empty;
            }
            // else read file and return its content
           fileContent =  reader.ReadToEnd();

            reader.Close();

           // isFileFounded = true;
            return fileContent;
        }

        private void LoadRedirectionRules(string filePath)
        {
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(filePath);

                Configuration.RedirectionRules = new Dictionary<string, string>();
                // TODO: using the filepath paramter read the redirection rules from file 

                 
                string [] line ;

                string oldAddress, newAddress;

                char[] seprators = { ','  };

                   
                while (!reader.EndOfStream)
                {
                    
                    //about.html aboutus.html
                    line =   reader.ReadLine().Trim().Split(seprators);

                    if(line.Length == 2)
                    {

                        oldAddress = line[0];
                        newAddress = line[1];

                        // then fill Configuration.RedirectionRules dictionary 

                        if (oldAddress != null && newAddress != null)
                            Configuration.RedirectionRules.Add(oldAddress, newAddress);
                    }
                    

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
