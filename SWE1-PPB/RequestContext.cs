using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Schema;

namespace SWE1_PPB
{
    class RequestContext
    {
        private string verbRessourceVersionPattern = @"([a-zA-Z]{3,6}) (.*) ([a-zA-Z0-9./]*)";
        private string headerRegexPattern = @"([a-zA-Z-]+): (.*)";
        private string httpVerb, ressource, httpVersion;
        private string payload = "";

        private Dictionary<string, string> headerValues = new Dictionary<string, string>();

        public RequestContext(string data)
        {
            string[] lines = data.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            httpVerb = Regex.Match(data, verbRessourceVersionPattern).Groups[1].Value;
            ressource = Regex.Match(data, verbRessourceVersionPattern).Groups[2].Value;
            httpVersion = Regex.Match(data, verbRessourceVersionPattern).Groups[3].Value;

            foreach (string line in lines)
            {
                //Console.WriteLine("Line: " + line + " " + httpVerb);
                if (Regex.Match(line, headerRegexPattern).Groups[1].Value.ToString() != "")
                {
                    headerValues.Add(Regex.Match(line, headerRegexPattern).Groups[1].Value.ToString(), Regex.Match(line, headerRegexPattern).Groups[2].Value.ToString());
                }
            }
            if (httpVerb.Equals("POST") || httpVerb.Equals("PUT"))
            {
                payload = lines[lines.Length - 1];
            }
            //Breakpoint
        }

        public string HttpVerb { get { return httpVerb; } }

        public string Ressource { get { return ressource; } }

        public string Payload { get { return payload; } }

        public Dictionary<string, string> HeaderValues { get { return headerValues; } }

        public void printRequest()
        {
            Console.WriteLine("****Request****\nHTTP-Verb: {0}\nRessource: {1}\nHTTP-Version: {2}\nHeader Values:", httpVerb, ressource, httpVersion);

            foreach (KeyValuePair<string, string> item in headerValues)
            {
                Console.WriteLine("{0} -> {1}", item.Key, item.Value);
            }

            if (payload != "")
            {
                Console.WriteLine("Payload: {0}", payload);
            }
            else
            {
                Console.WriteLine("No payload given.");
            }
        }
    }
}
