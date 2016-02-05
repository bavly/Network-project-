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
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines = new Dictionary<string, string>();

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
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        /// 
        public string[] splittedRequest;

        public bool ParseRequest()
        {
          //  throw new NotImplementedException();

            //TODO: parse the receivedRequest using the \r\n delimeter   
            string[] delimeter = new string[] { "\r\n" };
            splittedRequest = requestString.Split(delimeter, StringSplitOptions.None);
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            
            // Parse Request line
            bool Request_Line = ParseRequestLine();
           
            // Validate blank line exists
            bool Blank_Line = ValidateBlankLine();

            // Load header lines into HeaderLines dictionary
            bool Host_Header = LoadHeaderLines();

            if (Request_Line == true && Host_Header == true && Blank_Line == true)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        private bool ParseRequestLine()
        {
            string[] RequestLineDel = new string[] {" "};
            string[] splittedRequestLine = splittedRequest[0].Split(RequestLineDel, StringSplitOptions.None);

            string methodCheck = splittedRequestLine[0];
            if (methodCheck == "GET")
            {
                this.method = RequestMethod.GET;
            }

            if (methodCheck == "POST")
            {
                this.method = RequestMethod.POST;
            }


            if (methodCheck == "HEAD")
            {
                this.method = RequestMethod.HEAD;
            }


            string version = splittedRequestLine[2];
            if (version == "HTTP/1.0")
            {
                this.httpVersion = HTTPVersion.HTTP10;
            }

            else if (version == "HTTP/1.1")
            {
                this.httpVersion = HTTPVersion.HTTP11;

            }

            else 
            {
                this.httpVersion = HTTPVersion.HTTP09;
            }

            relativeURI = splittedRequestLine[1];
            bool URI = ValidateIsURI(relativeURI);
            if (URI == true)
            {
                return true;
            }
            else
            {
                return false;
            }

            
         //   throw new NotImplementedException();
        }

        private bool ValidateIsURI(string uri)
        {
           return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            string[] HeaderLineDel = new string[] { ":" };
            string[] splittedHeaderLines = splittedRequest[1].Split(HeaderLineDel, StringSplitOptions.None);
            if (!splittedRequest[1].Contains(":"))
            {
                return false;
            }

            HeaderLines.Add(splittedHeaderLines[0], splittedHeaderLines[1]);
            return true;
            //throw new NotImplementedException();
        }

        private bool ValidateBlankLine()
        {
            if (splittedRequest[2] == "")
            {
                return true;
            }
            return false;
       //     throw new NotImplementedException();
        }

    }
}
