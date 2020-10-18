using System;
using System.Web;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.IO;
using System.Threading;

namespace HTTPServer
{
    static class HttpServer
    {
        public static HttpListener listener;
        public static string url = "http://localhost:55005/Server/WRITE";
        public static int requestCount = 0;


        public static async Task HandleIncomingConnections()
        {
            bool runServer = true;
            while (runServer)
            {
                HttpListenerContext ctx = await listener.GetContextAsync();
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                // Print out some info about the request
                Console.WriteLine("Request #: {0}", ++requestCount);
                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                var json = ShowRequestData(req);
                CreateResp(json, resp);
                Console.WriteLine();
                resp.Close();
            }
        }
        public static RequsetObject GetObject(string json)
        {
            return JsonConvert.DeserializeObject<RequsetObject>(json);
        }
        public static string MakeJson(RequsetObject requsetObject) 
        {
            return JsonConvert.SerializeObject(requsetObject);
        }

        public static void CreateResp(string jsonInputString, HttpListenerResponse resp) 
        {
            RequsetObject requsetObject = GetObject(jsonInputString);
            string responseJson = "";
            //TODO delay между отправками а не всего метода Thread.Sleep(3000);
            switch (requsetObject.command) 
            {
                case "ConnectBase":
                    requsetObject.command = "Status = ConnectBase";
                    responseJson = MakeJson(requsetObject);
                    resp.OutputStream.Write(Encoding.UTF8.GetBytes(responseJson), 0, Encoding.UTF8.GetBytes(responseJson).Length);
                    break;
                case "ResetBase":
                    requsetObject.command = "Status = ResetBase";
                    responseJson = MakeJson(requsetObject);
                    resp.OutputStream.Write(Encoding.UTF8.GetBytes(responseJson), 0, Encoding.UTF8.GetBytes(responseJson).Length);
                    
                    requsetObject.command = "Status = ConnectBase";
                    responseJson = MakeJson(requsetObject);
                    resp.OutputStream.Write(Encoding.UTF8.GetBytes(responseJson), 0, Encoding.UTF8.GetBytes(responseJson).Length);
                    break;
                case "ApplyConfig":
                    requsetObject.command = "Status = ApplyConfig start";
                    responseJson = MakeJson(requsetObject);
                    resp.OutputStream.Write(Encoding.UTF8.GetBytes(responseJson), 0, Encoding.UTF8.GetBytes(responseJson).Length);

                    requsetObject.command = "Status = ApplyConfig done";
                    responseJson = MakeJson(requsetObject);
                    resp.OutputStream.Write(Encoding.UTF8.GetBytes(responseJson), 0, Encoding.UTF8.GetBytes(responseJson).Length);
                    break;
                case "SimOn":
                    requsetObject.command = "Status = SimOn start";
                    responseJson = MakeJson(requsetObject);
                    resp.OutputStream.Write(Encoding.UTF8.GetBytes(responseJson), 0, Encoding.UTF8.GetBytes(responseJson).Length);

                    requsetObject.command = "Status = DataChanelEstablished";
                    responseJson = MakeJson(requsetObject);
                    resp.OutputStream.Write(Encoding.UTF8.GetBytes(responseJson), 0, Encoding.UTF8.GetBytes(responseJson).Length);
                    break;
                case "DoMeasurement":
                    requsetObject.command = "Status = MeasurementRuning";
                    responseJson = MakeJson(requsetObject);
                    resp.OutputStream.Write(Encoding.UTF8.GetBytes(responseJson), 0, Encoding.UTF8.GetBytes(responseJson).Length);

                    requsetObject.command = "Status = DataChanelEstablished";
                    responseJson = MakeJson(requsetObject);
                    resp.OutputStream.Write(Encoding.UTF8.GetBytes(responseJson), 0, Encoding.UTF8.GetBytes(responseJson).Length);
                    break;
                case "HangUP":
                    requsetObject.command = "Status = HangUP";
                    responseJson = MakeJson(requsetObject);
                    resp.OutputStream.Write(Encoding.UTF8.GetBytes(responseJson), 0, Encoding.UTF8.GetBytes(responseJson).Length);

                    requsetObject.command = "Status = ApplyConfig done";
                    responseJson = MakeJson(requsetObject);
                    resp.OutputStream.Write(Encoding.UTF8.GetBytes(responseJson), 0, Encoding.UTF8.GetBytes(responseJson).Length);
                    break;
                    break;
            }
        }


        public static string ShowRequestData(HttpListenerRequest request)
        {
            System.IO.Stream body = request.InputStream;
            System.Text.Encoding encoding = request.ContentEncoding;
            System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);

            Console.WriteLine("Start of client data:");
            string jsonString = reader.ReadToEnd();
            Console.WriteLine(jsonString);
            Console.WriteLine("End of client data:");
            body.Close();
            reader.Close();
            return jsonString;
        }



        public static void Main(string[] args)
        {
            // Create a Http server and start listening for incoming connections
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            
            Console.WriteLine("Listening for connections on {0}", url);

            // Handle requests
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            // Close the listener
            listener.Close();
        }
    }
}



