using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PBXDashboard_Dev.Models;
using RestSharp;

namespace PBXDashboard_Dev.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateController : ControllerBase
    {
        List<string> collectionsExts = new List<string>{"1103",
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
        private readonly DataContext _context;

        public UpdateController(DataContext context)
        {
            _context = context;
        }

        string _result = "";
        // GET: api/Update
        [HttpGet]
        public IEnumerable<string> Get()
        {
            _result = updateCalls();
            return new string[] { "Update", $"{_result}" };
        }

        public string updateCalls()
        {
            string answer = "";

            Console.WriteLine("Updating extensions");
            string apiResult;
            IgnoreBadCertificates();
            var client = new RestClient("https://192.168.1.80/json");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Basic YWRtaW46V0BlZlJ0cGMyMDIw");
            request.AddParameter("application/json", "{\"request\": {\"method\": \"switchvox.extensions.search\", \"parameters\": {\"min_extension\": \"1000\", \"max_extension\": \"10000\"}}}",             ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            apiResult = response.Content;
            JObject o = JObject.Parse(apiResult);
            JArray a = JArray.Parse((o["response"]["result"]["extensions"]["extension"]).ToString());
            apiResult = "";
            int i = 0;
            foreach (var jo in a)
            {
                Report report = new Report();
                i++;

                Extension ext = new Extension();

                var extsList = _context.Extensions.ToList();

                foreach (var ext0 in extsList)
                    {
                        if (ext0.Number == jo["number"].ToString())
                        {
                            ext = ext0;
                        }
                    }

                ext.Number = jo["number"].ToString();
                ext.Status = jo["status"].ToString();
                if (jo["last_name"] != null)
                {
                    ext.LastName = jo["last_name"].ToString();
                }
                ext.TypeDisplay = jo["type_display"].ToString();
                ext.Display = jo["display"].ToString();
                ext.AccountID = jo["account_id"].ToString();
                if (!collectionsExts.Contains(ext.AccountID))
                {
                  continue;
                }
                if (jo["first_name"] != null)
                {
                    ext.FirstName = jo["first_name"].ToString();
                }
                if (jo["member_count"] != null)
                {
                    string member_count = jo["member_count"].ToString();
                    ext.MemberCount = Int32.Parse(member_count);
                }
                if (jo["strategy"] != null)
                {
                    ext.Strategy = jo["strategy"].ToString();
                }
                if (jo["call_queue_name"] != null)
                {
                    ext.CallQueueName = jo["call_queue_name"].ToString();
                }
                ext.Type = jo["type"].ToString();
                apiResult += "Ext: " + ext.Number + ", " + ext.Display + "\n";

                _context.Update(ext);

                report.TimeStamp = DateTime.Now;
                report.ReportString = $"Added new ext: {i}";
                _context.Update(report);
                _context.SaveChanges();
            }
            updateTalkTimeRecords(_context);

            Console.WriteLine("Updating Calls");
            var client0 = new RestClient("https://192.168.1.80/json");
            client0.Timeout = -1;
            var request0 = new RestRequest(Method.POST);
            request0.AddHeader("Content-Type", "application/json");
            request0.AddHeader("Authorization", "Basic T0NfQWRtaW46V0BlZlJ0cGMyMDIw");
            DateTime now = DateTime.Now;
            TimeSpan lookbackperiod = new TimeSpan(0, 5, 0);
            DateTime lookbackpoint = now - lookbackperiod;
            string nowString = now.ToString("yyyy-MM-dd HH:mm:ss");
            string lookbackpointString = lookbackpoint.ToString("yyyy-MM-dd HH:mm:ss");
            string parameterString0 = "{\"request\": {\"method\":\"switchvox.callLogs.search\", \"parameters\":{\"start_date\": \"";
            parameterString0 += lookbackpointString;
            parameterString0 += "\", \"end_date\": \"";
            parameterString0 += nowString;
            parameterString0 += "\", \"items_per_page\": \"1000\"}}}";
            request0.AddParameter("application/json", parameterString0, ParameterType.RequestBody);
            response = client0.Execute(request0);
            request0.AddParameter("application/json", parameterString0, ParameterType.RequestBody);
            IRestResponse response0 = client.Execute(request0);
            apiResult = "";
            apiResult = response0.Content;
            o = JObject.Parse(apiResult);

            if(o["response"]["result"]["calls"]["call"] != null)
            {
                Console.WriteLine("Calls not null");

                if((o["response"]["result"]["calls"]["total_items"]).ToString() == "1")
                {
                    var callsList1 = _context.Calls.ToList();
                    Call call = new Call();
                    foreach (var call0 in callsList1)
                    {
                        if (call0.PBXID == Int32.Parse((o["response"]["result"] ["calls"]["call"]["id"]).ToString()))
                        {
                            call = call0;
                            Console.WriteLine("Updating Existing Call");
                        }
                    }
                    string pbxId = o["response"]["result"]["calls"]["call"]["id"].ToString();
                    call.PBXID = Int64.Parse(pbxId);
                    call.UniqueID = o["response"]["result"]["calls"]["call"]["uniqueid"].ToString();
                    call.Origination = o["response"]["result"]["calls"]["call"]["origination"].ToString();
                    string startTime = o["response"]["result"]["calls"]["call"]["start_time"].ToString();
                    call.StartTime = DateTime.Parse(startTime);
                    call.From = o["response"]["result"]["calls"]["call"]["from_number"].ToString();
                    if (o["response"]["result"]["calls"]["call"]["from_account_id"] != null)
                    {
                        string fromAccountId = o["response"]["result"]["calls"]["call"]["from_account_id"].ToString();
                        call.FromAccountID = Int32.Parse(fromAccountId);
                    }
                    call.FromName = o["response"]["result"]["calls"]["call"]["from_name"].ToString();
                    call.FromNumber = o["response"]["result"]["calls"]["call"]["from_number"].ToString();
                    call.To = o["response"]["result"]["calls"]["call"]["to"].ToString();
                    call.ToNumber = o["response"]["result"]["calls"]["call"]["to_number"].ToString();
                    string totalDuration = o["response"]["result"]["calls"]["call"]["total_duration"].ToString();
                    call.TotalDuration = Int32.Parse(totalDuration);
                    string talkDuration = o["response"]["result"]["calls"]["call"]["talk_duration"].ToString();
                    call.TalkDuration = Int32.Parse(talkDuration);
                    string cdrCallId = o["response"]["result"]["calls"]["call"]["cdr_call_id"].ToString();
                    call.CdrCallID = Int32.Parse(cdrCallId);
                    Console.WriteLine($"Updated Call: {call.PBXID} - {DateTime.Now}");

                    JArray ea = JArray.Parse((o["response"]["result"]["calls"]["call"]["events"]["event"]).ToString());
                    int j = 0;
                    var eventList = _context.Events.ToList();
                        foreach(Event e in eventList)
                        {
                            if(e.Call == call)
                            {
                                _context.Remove(e);
                                Console.WriteLine("Reset Event");
                            }
                        }
                    call.Events = null;
                    call.Events = new List<Event>();

                    foreach(var joe in ea)
                    {
                        foreach(var eventU in call.Events)
                        {
                            if (eventU.StartTime == DateTime.Parse(joe["start_time"].ToString()) && eventU.Display == joe["display"].ToString())
                            {
                                Console.WriteLine("Already in database");
                                continue;
                            }
                        }
                      Console.WriteLine("Adding Event " + j);
                      call.Events.Add(new Event());
                      call.Events[j].StartTime = DateTime.Parse(joe["start_time"].ToString());
                      call.Events[j].Type = joe["type"].ToString();
                      call.Events[j].Display = joe["display"].ToString();
                      j++;
                    }

                    _context.Calls.Update(call);
                }
                else
                {
                    a = JArray.Parse((o["response"]["result"]["calls"]["call"]).ToString());
                    answer += " Call(s) updated";
                    var callsList = _context.Calls.ToList();
                    foreach (var joC in a)
                    {
                        Call call = new Call();
                        foreach (var call0 in callsList)
                        {
                            if (call0.PBXID == Int32.Parse((joC["id"]).ToString()))
                            {
                                call = call0;
                            }
                        }
                        string pbxId = joC["id"].ToString();
                        call.PBXID = Int64.Parse(pbxId);
                        call.UniqueID = joC["uniqueid"].ToString();
                        call.Origination = joC["origination"].ToString();
                        string startTime = joC["start_time"].ToString();
                        call.StartTime = DateTime.Parse(startTime);
                        call.From = joC["from_number"].ToString();
                        if (joC["from_account_id"] != null)
                        {
                            string fromAccountId = joC["from_account_id"].ToString();
                            call.FromAccountID = Int32.Parse(fromAccountId);
                        }
                        call.FromName = joC["from_name"].ToString();
                        call.FromNumber = joC["from_number"].ToString();
                        call.To = joC["to"].ToString();
                        call.ToNumber = joC["to_number"].ToString();
                        string totalDuration = joC["total_duration"].ToString();
                        call.TotalDuration = Int32.Parse(totalDuration);
                        string talkDuration = joC["talk_duration"].ToString();
                        call.TalkDuration = Int32.Parse(talkDuration);
                        string cdrCallId = joC["cdr_call_id"].ToString();
                        call.CdrCallID = Int32.Parse(cdrCallId);
                        JArray ea = JArray.Parse(joC["events"]["event"].ToString());
                        int j = 0;
                        var eventList = _context.Events.ToList();
                        foreach(Event e in eventList)
                        {
                            if(e.Call == call)
                            {
                                _context.Remove(e);
                                Console.WriteLine("Reset Event");
                            }
                        }
                        call.Events = null;
                        call.Events = new List<Event>();
                        foreach(var joe in ea)
                        {
                            Console.WriteLine("Adding Event " + j);
                          call.Events.Add(new Event());
                          call.Events[j].StartTime = DateTime.Parse(joe["start_time"].ToString());
                          call.Events[j].Type = joe["type"].ToString();
                          call.Events[j].Display = joe["display"].ToString();
                          j++;
                        }

                        _context.Update(call);
                        Console.WriteLine($"Updated Call: {call.PBXID} - {DateTime.Now}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Calls Null");
                answer = "No new calls";
                Console.WriteLine(DateTime.Now);
                return answer;
            }
            Console.WriteLine(DateTime.Now);
            _context.SaveChanges();
            return answer;
        }

        public static void updateQueues(DataContext dbcontext)
        {
            DataContext _context = dbcontext;
            Console.WriteLine("Updating Queues");
            var clientQ = new RestClient("https://192.168.1.80/json");
            clientQ.Timeout = -1;
            var requestQ = new RestRequest(Method.POST);
            requestQ.AddHeader("Content-Type", "application/json");
            requestQ.AddHeader("Authorization", "Basic T0NfQWRtaW46V0BlZlJ0cGMyMDIw");
            requestQ.AddParameter("application/json", "{\"request\": {\"method\": \"switchvox.callQueues.getCurrentStatus\", \"parameters\": {\"account_id\": \"1145\"}}}",           ParameterType.RequestBody);
            IRestResponse responseQ = clientQ.Execute(requestQ);
            string apiResult = "";
            apiResult = responseQ.Content;
            JObject o = JObject.Parse(apiResult);

            if(o["response"]["result"]["call_queue"] != null)
            {
                Console.WriteLine("Queues not null");

                var queueList = _context.Queues.ToList();
                Queue queue = new Queue();
                foreach (var queue0 in queueList)
                {
                    if (queue0.AccountID == o["response"]["result"]["call_queue"]["account_id"].ToString())
                    queue = queue0;
                }
                queue.Extension = o["response"]["result"]["call_queue"]["extension"].ToString();
                queue.Strategy = o["response"]["result"]["call_queue"]["strategy"].ToString();
                queue.AccountID = o["response"]["result"]["call_queue"]["account_id"].ToString();
                JArray qa = JArray.Parse((o["response"]["result"]["call_queue"]["queue_members"]["queue_member"]).ToString());
                int k = 0;
                queue.Members = new List<Extension>();

                foreach(var joq in qa)
                {
                  try
                  {
                    var extq = _context.Extensions.Where(id => id.AccountID == joq["account_id"].ToString()).FirstOrDefault();
                    queue.Members.Add(extq);
                  }
                  catch(Exception e)
                  {
                      continue;
                  }
                  
                  k++;
                }

                _context.Update(queue);
                _context.SaveChanges();
            }
            var requestQ1 = new RestRequest(Method.POST);
            requestQ1.AddHeader("Content-Type", "application/json");
            requestQ1.AddHeader("Authorization", "Basic T0NfQWRtaW46V0BlZlJ0cGMyMDIw");
            requestQ1.AddParameter("application/json", "{\"request\": {\"method\": \"switchvox.callQueues.getCurrentStatus\", \"parameters\": {\"account_id\": \"1124\"}}}", ParameterType.RequestBody);
            IRestResponse responseQ1 = clientQ.Execute(requestQ);
            string apiResult1 = "";
            apiResult1 = responseQ.Content;
            JObject o1 = JObject.Parse(apiResult);

            if (o1["response"]["result"]["call_queue"] != null)
            {
                Console.WriteLine("Queues not null");

                var queueList = _context.Queues.ToList();
                Queue queue = new Queue();
                foreach (var queue0 in queueList)
                {
                    if (queue0.AccountID == o1["response"]["result"]["call_queue"]["account_id"].ToString())
                        queue = queue0;
                }
                queue.Extension = o1["response"]["result"]["call_queue"]["extension"].ToString();
                queue.Strategy = o1["response"]["result"]["call_queue"]["strategy"].ToString();
                queue.AccountID = o1["response"]["result"]["call_queue"]["account_id"].ToString();
                JArray qa = JArray.Parse((o1["response"]["result"]["call_queue"]["queue_members"]["queue_member"]).ToString());
                int k = 0;
                queue.Members = new List<Extension>();

                foreach (var joq in qa)
                {
                    try
                    {
                        var extq = _context.Extensions.Where(id => id.AccountID == joq["account_id"].ToString()).FirstOrDefault();
                        queue.Members.Add(extq);
                    }
                    catch (Exception e)
                    {
                        continue;
                    }

                    k++;
                }

                _context.Update(queue);
                _context.SaveChanges();
            }
            var requestQ2 = new RestRequest(Method.POST);
            requestQ2.AddHeader("Content-Type", "application/json");
            requestQ2.AddHeader("Authorization", "Basic T0NfQWRtaW46V0BlZlJ0cGMyMDIw");
            requestQ2.AddParameter("application/json", "{\"request\": {\"method\": \"switchvox.callQueues.getCurrentStatus\", \"parameters\": {\"account_id\": \"1145\"}}}", ParameterType.RequestBody);
            IRestResponse responseQ2 = clientQ.Execute(requestQ2);
            string apiResult2 = "";
            apiResult2 = responseQ.Content;
            JObject o2 = JObject.Parse(apiResult);

            if (o2["response"]["result"]["call_queue"] != null)
            {
                Console.WriteLine("Queues not null");

                var queueList = _context.Queues.ToList();
                Queue queue = new Queue();
                foreach (var queue0 in queueList)
                {
                    if (queue0.AccountID == o2["response"]["result"]["call_queue"]["account_id"].ToString())
                        queue = queue0;
                }
                queue.Extension = o2["response"]["result"]["call_queue"]["extension"].ToString();
                queue.Strategy = o2["response"]["result"]["call_queue"]["strategy"].ToString();
                queue.AccountID = o2["response"]["result"]["call_queue"]["account_id"].ToString();
                JArray qa = JArray.Parse((o2["response"]["result"]["call_queue"]["queue_members"]["queue_member"]).ToString());
                int k = 0;
                queue.Members = new List<Extension>();

                foreach (var joq in qa)
                {
                    try
                    {
                        var extq = _context.Extensions.Where(id => id.AccountID == joq["account_id"].ToString()).FirstOrDefault();
                        queue.Members.Add(extq);
                    }
                    catch (Exception e)
                    {
                        continue;
                    }

                    k++;
                }

                _context.Update(queue);
                _context.SaveChanges();
            }
        }
        public static void UpdateCurrentCalls(DataContext context)
        {
            DataContext _context = context;
            Console.WriteLine("Updating Current Calls");
            string apiResult;
            IgnoreBadCertificates();
            var client = new RestClient("https://192.168.1.80/json");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Basic T0NfQWRtaW46V0BlZlJ0cGMyMDIw");
            request.AddParameter("application/json", "{\"request\": {\"method\": \"switchvox.currentCalls.getList\", \"parameters\": {}}}",  ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            apiResult = response.Content;
            JObject o = JObject.Parse(apiResult);
            o = JObject.Parse(o["response"]["result"]["current_calls"].ToString());
            if (Int32.Parse(o["total_items"].ToString()) == 0)
            {
                Console.WriteLine("0 current Calls");
                var currentCallsList = _context.CurrentCalls.ToList();
                foreach (var call in currentCallsList)
                {
                    _context.Remove(call);
                }
                Console.WriteLine("Cleared Old Calls");
            }
            else if (Int32.Parse(o["total_items"].ToString()) == 1)
            {
                Console.WriteLine("1 current Call.");
                var currentCallsList = _context.CurrentCalls.ToList();
                foreach (var call in currentCallsList)
                {
                    _context.Remove(call);
                }
                Console.WriteLine("Cleared Old Calls");
                CurrentCall ccall = new CurrentCall();
                ccall.StartTime = DateTime.Parse(o["current_call"]["start_time"].ToString());
                ccall.Duration = Int32.Parse(o["current_call"]["duration"].ToString());
                ccall.ToCallerIdName = o["current_call"]["to_caller_id_name"].ToString();
                ccall.ToCallerIdNumber = o["current_call"]["to_caller_id_number"].ToString();
                ccall.FromCallerIdName = o["current_call"]["from_caller_id_name"].ToString();
                ccall.FromCallerIdNumber = o["current_call"]["from_caller_id_number"].ToString();
                ccall.Format = o["current_call"]["format"].ToString();
                ccall.State = o["current_call"]["state"].ToString();
                ccall.PBXID = o["current_call"]["id"].ToString();
                _context.Update(ccall);
            }
            else
            {
                Console.WriteLine($"{(o["total_items"].ToString())} current Calls.");
                 var currentCallsList = _context.CurrentCalls.ToList();
                foreach (var call in currentCallsList)
                {
                    _context.Remove(call);
                }
                JArray a = JArray.Parse(o["current_call"].ToString());
                foreach (var joC in a)
                {
                    CurrentCall ccall = new CurrentCall();
                    ccall.StartTime = DateTime.Parse(joC["start_time"].ToString());
                    ccall.Duration = Int32.Parse(joC["duration"].ToString());
                    ccall.ToCallerIdName = joC["to_caller_id_name"].ToString();
                    ccall.ToCallerIdNumber = joC["to_caller_id_number"].ToString();
                    ccall.FromCallerIdName = joC["from_caller_id_name"].ToString();
                    ccall.FromCallerIdNumber = joC["from_caller_id_number"].ToString();
                    ccall.Format = joC["format"].ToString();
                    ccall.State = joC["state"].ToString();
                    ccall.PBXID = joC["id"].ToString();
                    _context.Update(ccall);
                }
            }
            Console.WriteLine("Current Calls Saved"); 
            _context.SaveChanges();
        }
        public static void updateTalkTimeRecords(DataContext context)
        {
            DataContext _context = context;
            List<string> extensionAccountIds = new List<string>();
            foreach(var ext in _context.Extensions.Where(e => e.Type == "sip").ToList())
            {
                extensionAccountIds.Add(ext.AccountID);
            }
            Console.WriteLine("updateTalkTimeRecords" + extensionAccountIds);

            DateTime today = DateTime.Now;

            string todayString = today.ToString("yyyy-MM-dd 00:00:00");
            string endOfTodayString = today.ToString("yyyy-MM-dd 23:59:59");
            string apiResult;
            IgnoreBadCertificates();
            var client = new RestClient("https://192.168.1.80/json");
            client.Timeout = -1;
            foreach(string ext in extensionAccountIds)
            {
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Basic T0NfQWRtaW46V0BlZlJ0cGMyMDIw");
                string parameterString1 = "{\"request\": {\"method\": \"switchvox.callReports.search\", \"parameters\": {\"start_date\": \"";
                parameterString1 += todayString;
                parameterString1 += "\", \"end_date\": \"";
                parameterString1 += endOfTodayString;
                parameterString1 += "\", \"account_ids\": [\"";
                parameterString1 += ext;
                parameterString1 += "\"], \"ignore_weekends\": \"0\", \"breakdown\": \"by_day\", \"report_fields\": [\"total_calls\", \"total_incoming_calls\", \"total_outgoing_calls\", \"talking_duration\", \"call_duration\"]}}}";
                request.AddParameter("application/json", parameterString1, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                apiResult = response.Content;
                JObject o = JObject.Parse(apiResult);
                o = JObject.Parse(o["response"]["result"]["rows"]["row"].ToString());
                TalkTimeRecord record = new TalkTimeRecord();
                var recordList = _context.TalkTimeRecords.ToList();
                record.AccountID = ext;
                record.Date = DateTime.Parse(o["date"].ToString());
                record.TalkingDuration = Int32.Parse(o["talking_duration"].ToString());
                record.TotalOutgoingCalls = Int32.Parse(o["total_outgoing_calls"].ToString());
                record.TotalIncomingCalls = Int32.Parse(o["total_incoming_calls"].ToString());
                record.TotalCalls = Int32.Parse(o["total_calls"].ToString());
                record.CallDuration = Int32.Parse(o["call_duration"].ToString());
                foreach (var record_ in recordList)
                {
                    if (record_.Date.ToString("MM/dd/yyyy") == today.ToString("MM/dd/yyyy") && record_.AccountID == record.AccountID)
                    {
                        _context.TalkTimeRecords.Remove(record_);
                        Console.WriteLine("Talk Time Record Removed");
                    }
                }
                _context.Update(record);
                _context.SaveChanges();
                Console.WriteLine("TalkRecordUpdated: " + ext);
            }
            Console.WriteLine("TalkRecordes Updated.");
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

