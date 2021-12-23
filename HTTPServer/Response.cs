using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;


namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        static  string CRLF = "\r\n";
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {

            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            
            this.code = code;
            GetStatusLine(code);
            headerLines[0] = " Content-Type: \r\n" + contentType;
            headerLines[1] = "Content-Length:\r\n  " + content.Length.ToString();
            headerLines[2] = "Date : \r\n " + DateTime.Now.ToString();
            if(redirectoinPath!=null)
            {
                headerLines[3] = "\r\n]redirection" + redirectoinPath;
            }
            // TODO: Create the request strindasdg
            foreach(string header in headerLines)
            {
                responseString += header;
            }sdasd
                dsd


        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = string.Format("HTTP/1.1 {0}{1}\r\n", "", ((int)code).ToString(), code.ToString());
            
                        
         
            return statusLine;
        }
    }
}

