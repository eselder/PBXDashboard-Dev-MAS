using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using PBXDashboard_Dev.Models;
using RestSharp;

namespace PBXDashboard_Dev.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserStatusUpdateController : ControllerBase
    {
        private IMemoryCache _cache;
        private List<string> extensionList = new List<string>{"102",
        "105",
        "106",
        "113",
        "114",
        "115", 
        "117", 
        "118", 
        "119", 
        "122", 
        "123", 
        "127", 
        "128", 
        "131", 
        "132", 
        "133", 
        "136", 
        "137", 
        "138",
        "139", 
        "141", 
        "142",
        "143", 
        "144",
        "145"
        };
        private List<string> extensionIdsList = new List<string>{"1103",
        "1106",
              "1317",
              "1111",
              "1112",
              "1113",
              "1115",
              "1116",
              "1117",
              "1119",
              "1120",
              "1124",
              "1248",
              "1243",
              "1256",
              "1255",
              "1263",
              "1276",
            "1277",
              "1279",
              "1281",
              "1282",
              "1283",
              "1300",
              "1286"
        };
        private Boolean isStart = false;
        private const double updateSeconds = 2.0;

        public List<CurrentCall> currentCalls = new List<CurrentCall>();
        public List<ExtensionStatus> extensionStatuses = new List<ExtensionStatus>();

        public UserStatusUpdateController(IMemoryCache cache)
        {
            _cache = cache;

            _cache.Set("ExtensionList", this.extensionList);
        }

        string _result = "";
        // GET: api/UserStatusUpdate
        [HttpGet]
        public IEnumerable<string> Get()
        {
            _result = updateUserStatuses(_cache);
            return new string[] { "Update Current Calls", $"{_result}" };
        }
        public string updateUserStatuses(IMemoryCache cache)
        {
            Console.WriteLine("Updating User Statuses");
            for (int i = 0; i < extensionList.Count; i++)
            {
                string extNumber = extensionList[i];
                string apiResult;
                IgnoreBadCertificates();
                var client = new RestClient("https://192.168.1.80/json");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Basic YWRtaW46V0BlZlJ0cGMyMDIw");
                string parameterString = "{\"request\": {\"method\": \"switchvox.users.presence.getInfo\", \"parameters\": {\"account_id\": \"";
                parameterString += extensionIdsList[i];
                parameterString += "\"}}}";
                request.AddParameter("application/json", parameterString, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                apiResult = response.Content;
                JObject o = JObject.Parse(apiResult);
                o = JObject.Parse(o["response"]["result"]["presence"].ToString());
                string presence = o["presence"].ToString();
                _cache.Set(extNumber + "-Presence", presence);
                string subPresence = o["sub_presence"].ToString();
                _cache.Set(extNumber + "-SubPresence", subPresence);
                _cache.TryGetValue(extNumber + "-Presence", out string cachePresence);
            }
            _cache.TryGetValue("ExtensionStatuses", out string statuses);
            string dateString = "./LogReports/" + DateTime.Now.ToString("yyyy-MM-dd") + "_LogReport.json";
            System.IO.File.WriteAllText(@dateString, statuses);
            Console.WriteLine("User Statuses Updated");
            return "UserStatuses Updated";
        }

        public static void IgnoreBadCertificates()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
        }
        private static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}

