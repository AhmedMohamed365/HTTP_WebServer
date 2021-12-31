using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        string content = "";
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public string _content
        {
            get { return content; }
        }
        public RequestMethod _method
        {
            get { return method; }
        }
        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;

           // Console.WriteLine(requestString);
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {

            //TODO: parse the receivedRequest using the \r\n delimeter   
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            // Parse Request line
            // Validate blank line exist
            // Load header lines into HeaderLines dictionary
            // parse request line 
           

            if ( ValidateBlankLine() )
            {
                string[] sperators = { "\r\n" };

                requestLines = requestLines[0].Split(sperators, StringSplitOptions.RemoveEmptyEntries);
                // bool 
                if ((requestLines.Length >= 2))
                {
                    // request line is more than 3 
                    if (!ParseRequestLine())
                        return false;
                    if (!LoadHeaderLines())
                        return false;

                   // content = 
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        private bool ParseRequestLine()
        {
            string[] requestLine = requestLines[0].Split(' ');

           
            if (requestLine.Length >= 2 && requestLine.Length < 4)
            {
                // first line contains 
                // method type
                if (requestLine[0].Equals(RequestMethod.HEAD))
                {
                    method = RequestMethod.HEAD;
                }
                if (requestLine[0].Equals("POST"))
                {
                    method = RequestMethod.POST;
                }
                if (requestLine[0].Equals(RequestMethod.GET))
                {
                    method = RequestMethod.GET;
                }

                //check content Line exist
                if ( method == RequestMethod.POST && content == "")
                {
                    //Then it's bad Request

                    return false;
                }
                // URI
                if (ValidateIsURI(requestLine[1]))
                    relativeURI = requestLine[1];

                //Not Valid
                else
                    return false;
               // Console.WriteLine(relativeURI);
                // http virsion
                if (requestLine.Length < 3)
                    httpVersion = HTTPVersion.HTTP09;
                else if (requestLine[2].Equals("HTTP/0.9"))
                    httpVersion = HTTPVersion.HTTP09;
                else if (requestLine[2].Equals("HTTP/1.0"))
                    httpVersion = HTTPVersion.HTTP10;
                else if (requestLine[2].Equals("HTTP/1.1"))
                    httpVersion = HTTPVersion.HTTP11;

                return true;
            }
            else
                return false;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }
        private bool LoadHeaderLines()
        {
            int i = 0;
            headerLines = new Dictionary<string, string> ();
            foreach (string iterator in requestLines) 
            {
                string[] sperators = { ": " };
                string[] headerLine = iterator.Split(sperators,StringSplitOptions.RemoveEmptyEntries);
                if (i == 0)
                {
                    i++;
                    continue;
                }
                if (headerLine.Length == 2) 
                {
                    
                        headerLines.Add(headerLine[0], headerLine[1]);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValidateBlankLine()
        {
            string[] blankLine = { "\r\n\r\n" } ;

            requestLines = requestString.Split(blankLine, StringSplitOptions.RemoveEmptyEntries);

           
            if (requestLines.Length == 0)
            {
                return false;
            }

            if (requestLines.Length == 2 )

                content = requestLines[1];

           // Console.WriteLine(content);

            return true;
        }

    }
}
