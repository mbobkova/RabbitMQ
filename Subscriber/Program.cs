using System;
using EasyNetQ;
using Messages;
using System.Text;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;

namespace Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var bus = RabbitHutch.CreateBus("host=localhost"))
            {
                bus.Subscribe<Message>("myqueue", HandleTextMessage);

                Console.WriteLine("Listening for messages.");
                Console.ReadLine();
            }
        }

        static void HandleTextMessage(Message textMessage)
        {
            string res = ValidatedEmail(textMessage.Text);

            Console.WriteLine("Got message: {0}", textMessage.Text + " is " + res);

        }

        static string ValidatedEmail(string message)
        {

            string url = "https://stringfunction.azurewebsites.net/api/HttpTriggerJS1?code=04TpixwX4BvXI3/11HTUsj9ny6kPrfpuFQqqZ2gxJCngtatWfilVpQ==";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.ContentType = "application/json; charset=UTF-8";
            request.Method = "POST";


            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(new
                {
                    email = message
                });

                streamWriter.Write(json);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var encoding = Encoding.GetEncoding(response.CharacterSet);
            string resp = "";
            using (var streamReader = new StreamReader(response.GetResponseStream(), encoding))
            {
                var result = streamReader.ReadToEnd();
                resp = result;
            }
            response.Close();

            string res = resp.Equals("true") ? "correct" : "not correct";

            return res;
        }
    }
}