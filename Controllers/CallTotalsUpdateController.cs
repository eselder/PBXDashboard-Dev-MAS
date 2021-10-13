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
    public class CallTotalsUpdateController : ControllerBase
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

        public CallTotalsUpdateController(IMemoryCache cache)
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
            return new string[] { "Update Call Totals", $"{_result}" };
        }
        public string updateUserStatuses(IMemoryCache cache)
        {
            Console.WriteLine("Updating Call Totals");
            for (int i = 0; i < extensionList.Count; i++)
            {
                string extNumber = extensionList[i];
                string apiResult;
                IgnoreBadCertificates();
                var client = new RestClient("https://192.168.1.80/json");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                Console.WriteLine("updateTalkTimeRecords" + extNumber);
                DateTime today = DateTime.Now;
                today = today.AddHours(-7);
                Console.WriteLine("NOW: " + today);
                string todayString = today.ToString("yyyy-MM-dd 00:00:00");
                string endOfTodayString = today.ToString("yyyy-MM-dd 23:59:59");
                client = new RestClient("https://192.168.1.80/json");
                client.Timeout = -1;
                request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Basic T0NfQWRtaW46V0BlZlJ0cGMyMDIw");
                string parameterString1 = "{\"request\": {\"method\": \"switchvox.callReports.search\",  \"parameters\": {\"start_date\": \"";
                parameterString1 += todayString;
                parameterString1 += "\", \"end_date\": \"";
                parameterString1 += endOfTodayString;
                parameterString1 += "\", \"account_ids\": [\"";
                parameterString1 += extensionIdsList[i];
                parameterString1 += "\"], \"ignore_weekends\": \"0\", \"breakdown\": \"by_day\", \"report_fields\": [\"total_calls\", \"total_incoming_calls\", \"total_outgoing_calls\"]}}}";
                request.AddParameter("application/json", parameterString1, ParameterType.RequestBody);
                var response = client.Execute(request);
                apiResult = response.Content;
                var o = JObject.Parse(apiResult);
                o = JObject.Parse(o["response"]["result"]["rows"]["row"].ToString());
                int iCalls = Int32.Parse(o["total_incoming_calls"].ToString());
                _cache.Set(extNumber + "-TotalICalls", iCalls, DateTime.Now.AddDays(1));
                
                
                // __________
                Console.WriteLine($"Call Page of {extensionIdsList[i]} - {DateTime.Now}");
                client = new RestClient("https://192.168.1.80/json");
                client.Timeout = -1;
                request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Basic T0NfQWRtaW46V0BlZlJ0cGMyMDIw");
                string parameterString = "";
                parameterString = "{\"request\": {\"method\": \"switchvox.callLogs.search\", \"parameters\": {\"start_date\": \"";
                parameterString += todayString;
                parameterString += "\", \"end_date\": \"";
                parameterString += endOfTodayString;
                parameterString += "\", \"items_per_page\": \"300\"";
                parameterString += ", \"account_ids\": [\"";
                parameterString += extensionIdsList[i];
                parameterString += "\"]}}}";
                request.AddParameter("application/json", parameterString, ParameterType.RequestBody);
                response = client.Execute(request);
                apiResult = response.Content;
                o = JObject.Parse(apiResult);
                int outTotal = 0;
                JArray a = new JArray();
                try {
                    a = JArray.Parse((o["response"]["result"]["calls"]["call"]).ToString());
                }
                catch (Exception eJArray)
                {
                    Console.WriteLine(eJArray);
                }
                if (a != null) {
                    foreach (var jo in a)
                    {
                        if (jo["to_number"].ToString().Length > 3)
                        {
                            outTotal++;
                        }
                    }
                    _cache.Set(extNumber + "-TotalOCalls", outTotal, DateTime.Now.AddDays(1));
                    int tCalls = iCalls + outTotal;
                    _cache.Set(extNumber + "-TotalCalls", tCalls, DateTime.Now.AddDays(1));
                }
            }
            return "Call Totals Updated";
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

