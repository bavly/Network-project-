using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
          //  throw new NotImplementedException();
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            int content_length = content.Length;
            string content_length_string = content_length.ToString();
            DateTime date = DateTime.Now;
            string date_string = date.ToString();

            headerLines.Add("Content-Type"+ contentType);
            headerLines.Add("Content-Length"+ content_length_string);
            headerLines.Add("Date" + date_string);
            if (redirectoinPath!="")
            {
                headerLines.Add("Redirection Path" + redirectoinPath);
            }
            // TODO: Create the request string
            string status_line=GetStatusLine(code);
            string header_line = "";
            for (int i = 0; i < headerLines.Count; i++)
            {
                header_line += headerLines[i]+ " ";
            }
            //header_line += "\r\n";
            responseString = status_line + header_line + "\r\n" + content;

        }
        
        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = string.Format("HTTP/1.1 ", ((int)code).ToString(), code.ToString());
            return statusLine;
        }
    }
}
