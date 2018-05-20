﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FCM_PushNotification.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace FCM_PushNotification.Controllers
{
    public class HomeController : Controller
    {
        private readonly string storagePath = "D:\\UserTokens.txt";

        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        public IActionResult Index()
        {
            var model = new Notification
            {
                Title = "         طاقچه",
                Body = "متن آزمایشی از سرور فایربیس",
                Vibrate = new[] {500, 110, 500, 110, 450, 110, 200, 110, 170, 40, 450, 110, 200, 110, 170, 40, 500},
                Icon = "https://taaghche.ir/assets/images/taaghchebrand.png",
                ClickAction = "https://taaghche.ir",
                Tag = Guid.NewGuid().ToString("N") // unique notify or update last noitfy on client
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(Notification model)
        {
            try
            {
                var serverKey =
                    "AAAApXBVEZE:APA91bH56ib3KIqSrLy7h64y6kXjNU_KNvHGG365u3r-ii8TOjaf-jykU25Gm1o5XvWBRMgQIUm1KVtV4NQqkVBp2usA7NDL16aGp3Qq-5WhzxmPCCBTofXlItoCoFE9G_Ct-XoKjWMq";
                var messagingSenderId = "710554227089";
                var tokens = await GetTokensAsync();
                tokens = tokens.Distinct().ToArray();

                foreach (var userToken in tokens)
                {
                    var pn = new MessageModel
                    {
                        To = userToken,
                        Notification = model
                    };

                    var client = new RestClient("https://fcm.googleapis.com/fcm/send");
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Cache-Control", "no-cache");
                    request.AddHeader("Authorization", $"key={serverKey}");
                    request.AddHeader("Sender", $"id={messagingSenderId}");
                    request.AddHeader("Content-Type", "application/json");
                    request.AddParameter("undefined", JsonConvert.SerializeObject(pn), ParameterType.RequestBody);
                    var response = await client.ExecuteTaskAsync(request);

                    if (response.StatusCode != HttpStatusCode.OK)
                        return StatusCode((int) response.StatusCode);

                    //var tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    //tRequest.Method = "post";
                    //tRequest.ContentType = "application/json";
                    //var json = Newtonsoft.Json.JsonConvert.SerializeObject(pn);
                    //var byteArray = Encoding.UTF8.GetBytes(json);
                    //tRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
                    //tRequest.Headers.Add(string.Format("Sender: id={0}", messagingSenderId));
                    //tRequest.ContentLength = byteArray.Length;

                    //using (var dataStream = tRequest.GetRequestStream())
                    //{
                    //    dataStream.Write(byteArray, 0, byteArray.Length);
                    //    using (var tResponse = tRequest.GetResponse())
                    //    {
                    //        using (var dataStreamResponse = tResponse.GetResponseStream())
                    //        {
                    //            using (var tReader = new StreamReader(dataStreamResponse))
                    //            {
                    //                var sResponseFromServer = tReader.ReadToEnd();
                    //                var str = sResponseFromServer;
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                return Error();
            }

            return Ok();
        }

        public async Task<string[]> GetTokensAsync()
        {
            if (System.IO.File.Exists(storagePath))
            {
                var tokens = await System.IO.File.ReadAllLinesAsync(storagePath, Encoding.UTF8);
                return tokens;
            }

            return null;
        }
    }
}