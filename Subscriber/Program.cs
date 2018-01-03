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
            string res = ValidatedPassword(textMessage.Text);

            Console.WriteLine("Got message: {0}", textMessage.Text + " is " + res);

        }

        static string ValidatedPassword(string message)
        {

            string url = "https://validpassword1.azurewebsites.net/api/HttpTriggerJS1?code=WyH9bKl17xmfDtJOtOqb/ncHUnZmuJbMHNFxy2jfm0dgFY9S0NtAMQ==";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.ContentType = "application/json; charset=UTF-8";
            request.Method = "POST";


            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(new
                {
                    password = message
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

            string res = resp.Equals("true") ? "password is correct" : "password is not correct";

            return res;
        }
    }
}