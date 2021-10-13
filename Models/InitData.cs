using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using PBXDashboard_Dev.Models;
using RestSharp;
using Microsoft.AspNetCore.Identity;

namespace PBXDashboard_Dev
{
  public class InitData
  {
        public static void InitDatabase(DataContext context)
        {
            string result;

            DataContext _context = context;

            IgnoreBadCertificates();

            Console.WriteLine("InitData");

            updateExtsQs(_context);

            Console.WriteLine("Importing first calls");

            //DateTime today = DateTime.Now;
            DateTime today = new DateTime(2021, 8, 12);
            TimeSpan reportPeriod = new TimeSpan(1,0,0,0);
            //DateTime beginDate = today - reportPeriod;
            DateTime beginDate = new DateTime(2021, 8, 4);
            string todayString = today.ToString("yyyy-MM-dd HH:mm:ss");
            string beginDateString = beginDate.ToString("yyyy-MM-dd HH:mm:ss");
            var client = new RestClient("https://192.168.1.80/json");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Basic T0NfQWRtaW46V0BlZlJ0cGMyMDIw");
            string parameterString = "{\"request\": {\"method\": \"switchvox.callLogs.search\", \"parameters\": {\"start_date\": \"";
            parameterString += beginDateString;
            parameterString += "\", \"end_date\": \"";
            parameterString += todayString;

            parameterString += "\"}}}";
            Console.WriteLine(parameterString);
            request.AddParameter("application/json", parameterString, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            result = response.Content;
            JObject o = JObject.Parse(result);
            string pageNumber = o["response"]["result"]["calls"]["page_number"].ToString();
            string itemsPerPage = o["response"]["result"]["calls"]["items_per_page"].ToString();
            string totalPages = o["response"]["result"]["calls"]["total_pages"].ToString();
            JArray a = JArray.Parse((o["response"]["result"]["calls"]["call"]).ToString());
            result = "";

            int i = 0;
            foreach (var jo in a)
            {
                i++;

                Call call = new Call();
                call.Events = new List<Event>();
                string pbxId = jo["id"].ToString();
                call.PBXID = Int64.Parse(pbxId);
                call.UniqueID = jo["uniqueid"].ToString();
                call.Origination = jo["origination"].ToString();
                string startTime = jo["start_time"].ToString();
                call.StartTime = DateTime.Parse(startTime);
                call.From = jo["from_number"].ToString();
                if (jo["from_account_id"] != null)
                {
                    string fromAccountId = jo["from_account_id"].ToString();
                    call.FromAccountID = Int32.Parse(fromAccountId);
                }
                if (jo["to_account_id"] != null)
                {
                  string toAccountId = jo["to_account_id"].ToString();
                  call.ToAccountID = Int32.Parse(toAccountId);
                }
                call.FromName = jo["from_name"].ToString();
                call.FromNumber = jo["from_number"].ToString();
                call.To = jo["to"].ToString();
                call.ToNumber = jo["to_number"].ToString();
                string totalDuration = jo["total_duration"].ToString();
                call.TotalDuration = Int32.Parse(totalDuration);
                string talkDuration = jo["talk_duration"].ToString();
                call.TalkDuration = Int32.Parse(talkDuration);
                string cdrCallId = jo["cdr_call_id"].ToString();
                call.CdrCallID = Int32.Parse(cdrCallId);
                result += "Call: " + call.ToNumber + ", " + call.Origination + "\n";

                try
                {
                    JArray ea = JArray.Parse((jo["events"]["event"]).ToString());
                    int j = 0;
                    foreach (var joe in ea)
                    {
                        call.Events.Add(new Event());
                        call.Events[j].StartTime = DateTime.Parse(joe["start_time"].ToString());
                        call.Events[j].Type = joe["type"].ToString();
                        call.Events[j].Display = joe["display"].ToString();
                        j++;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                _context.Update(call);


                _context.SaveChanges();
            }
            for (int k = 2; k <= Int32.Parse(totalPages); k++)
            {
                Console.WriteLine($"Call Page {k} of {totalPages} - {DateTime.Now}");
                client = new RestClient("https://192.168.1.80/json");
                client.Timeout = -1;
                request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Basic T0NfQWRtaW46V0BlZlJ0cGMyMDIw");
                parameterString = "{\"request\": {\"method\": \"switchvox.callLogs.search\", \"parameters\": {\"start_date\": \"";
                parameterString += beginDateString;
                parameterString += "\", \"end_date\": \"";
                parameterString += todayString;
                parameterString += "\", \"items_per_page\": \"120\",";
                parameterString += " \"page_number\": \"";
                parameterString += k.ToString();
                parameterString += "\"}}}";
                request.AddParameter("application/json", parameterString, ParameterType.RequestBody);
                response = client.Execute(request);
                result = response.Content;
                o = JObject.Parse(result);
                pageNumber = o["response"]["result"]["calls"]["page_number"].ToString();
                itemsPerPage = o["response"]["result"]["calls"]["items_per_page"].ToString();
                totalPages = o["response"]["result"]["calls"]["total_pages"].ToString();
                try
                {
                    a = JArray.Parse((o["response"]["result"]["calls"]["call"]).ToString());
                    result = "";
                    i = 0;
                    foreach (var jo in a)
                    {
                        i++;

                        Call call = new Call();
                        call.Events = new List<Event>();
                        string pbxId = jo["id"].ToString();
                        call.PBXID = Int64.Parse(pbxId);
                        call.UniqueID = jo["uniqueid"].ToString();
                        call.Origination = jo["origination"].ToString();
                        string startTime = jo["start_time"].ToString();
                        call.StartTime = DateTime.Parse(startTime);
                        call.From = jo["from_number"].ToString();
                        if (jo["from_account_id"] != null)
                        {
                            string fromAccountId = jo["from_account_id"].ToString();
                            call.FromAccountID = Int32.Parse(fromAccountId);
                        }
                        if (jo["to_account_id"] != null)
                        {
                            string toAccountId = jo["to_account_id"].ToString();
                            call.ToAccountID = Int32.Parse(toAccountId);
                        }
                        call.FromName = jo["from_name"].ToString();
                        call.FromNumber = jo["from_number"].ToString();
                        call.To = jo["to"].ToString();
                        call.ToNumber = jo["to_number"].ToString();
                        string totalDuration = jo["total_duration"].ToString();
                        call.TotalDuration = Int32.Parse(totalDuration);
                        string talkDuration = jo["talk_duration"].ToString();
                        call.TalkDuration = Int32.Parse(talkDuration);
                        string cdrCallId = jo["cdr_call_id"].ToString();
                        call.CdrCallID = Int32.Parse(cdrCallId);
                        result += "Call: " + call.ToNumber + ", " + call.Origination + "\n";

                        try
                        {
                            JArray ea = JArray.Parse((jo["events"]["event"]).ToString());
                            int j = 0;
                            foreach (var joe in ea)
                            {
                                call.Events.Add(new Event());
                                call.Events[j].StartTime = DateTime.Parse(joe["start_time"].ToString());
                                call.Events[j].Type = joe["type"].ToString();
                                call.Events[j].Display = joe["display"].ToString();
                                j++;
                            }
                        }
                        catch (Exception e)
                        {
                            JObject eo = JObject.Parse((jo["events"]["event"]).ToString());
                            int j = 0;
                            call.Events.Add(new Event());
                            call.Events[j].StartTime = DateTime.Parse(eo["start_time"].ToString());
                            call.Events[j].Type = eo["type"].ToString();
                            call.Events[j].Display = eo["display"].ToString();
                            j++;
                        }

                        _context.Update(call);

                        _context.SaveChanges();
                    }
                }
                catch (Exception e)
                {

                }
            }

            // Add the Extension and Queue data for each Queue
            List<Extension> callQueues = new List<Extension>();
            callQueues = _context.Extensions.Where(
              e => e.Type == "call_queue"
            ).ToList();
            foreach (Extension callqueue in callQueues)
            {
              Console.WriteLine($"CallQueues: {callqueue.Display}");
            }
            foreach(Extension callQueue in callQueues)
            {
              Console.WriteLine($"Call Queue AccountID: {callQueue.AccountID} <{callQueue.Display}>");
                          // Add the info from the queue report to the call
            Console.WriteLine("Adding queue report data.");
            client = new RestClient("https://192.168.1.80/json");
            client.Timeout = -1;
            request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Basic T0NfQWRtaW46V0BlZlJ0cGMyMDIw");
            parameterString = "{\"request\": {\"method\": \"switchvox.callQueueLogs.search\", \"parameters\": {\"start_date\": \"";
            parameterString += beginDateString;
            parameterString += "\", \"end_date\": \"";
            parameterString += todayString;
            parameterString += "\", \"queue_account_ids\": [\"";
            parameterString += callQueue.AccountID;
            parameterString += "\"], \"call_types\": [\"completed_calls\", \"abandoned_calls\", \"redirected_calls";
            parameterString += "\"]}}}";
            request.AddParameter("application/json", parameterString, ParameterType.RequestBody);
            response = client.Execute(request);
            result = response.Content;
            o = JObject.Parse(result);
            pageNumber = o["response"]["result"]["calls"]["page_number"].ToString();
            itemsPerPage = o["response"]["result"]["calls"]["items_per_page"].ToString();
            totalPages = o["response"]["result"]["calls"]["total_pages"].ToString();
            Console.WriteLine($"Total QueueReport Pages: {totalPages}");
            if (o["response"]["result"]["calls"]["call"] != null)
            {
              a = JArray.Parse((o["response"]["result"]["calls"]["call"]).ToString());
            }
            else
            {
              continue;
            }
            result = "";

            i = 0;
            foreach (var jo in a)
            {
              if (jo["uniqueid"] != null)
              {
                Call call = new Call();
                call = _context.Calls.Where(c => c.UniqueID ==
                  jo["uniqueid"].ToString()).FirstOrDefault();
                if (call == null)
                {
                            continue;
                }
                if (call.QueueLogs == null)
                {
                  call.QueueLogs = new List<QueueLog>();
                }
                QueueLog queueLog = new QueueLog();
                queueLog.QueueAccountID = jo["queue_account_id"].ToString();
                queueLog.QueueExtension = jo["queue_extension"].ToString();
                queueLog.EnterPosition = Int32.Parse(jo["enter_position"].ToString());
                queueLog.QueueName = jo["queue_name"].ToString();
                if (jo["member_account_id"] != null)
                {
                  queueLog.MemberAccountID = jo["member_account_id"].ToString();
                }
                queueLog.WaitTime = Int32.Parse(jo["wait_time"].ToString());
                if (jo["member_extension"] != null)
                {
                  queueLog.MemberExtension = jo["member_extension"].ToString();
                }
                queueLog.UniqueID = jo["uniqueid"].ToString();
                if (jo["member_name"] != null)
                {
                  queueLog.MemberName = jo["member_name"].ToString();
                }
                if (jo["talk_time"] != null)
                {
                  queueLog.TalkTime = jo["talk_time"].ToString();
                }
                if (jo["start_time"] != null)
                {
                  queueLog.StartTime = DateTime.Parse(jo["start_time"].ToString());
                }
                queueLog.Type = jo["type"].ToString();
                queueLog.MemberMisses = Int32.Parse(jo["member_misses"].ToString());
                call.QueueLogs.Add(queueLog);
                i++;
                _context.Update(call);
                _context.SaveChanges();
              }
            }
            if (Int32.Parse(totalPages) >= 2)
            {
              for(int k = 2; k <= Int32.Parse(totalPages); k++)
            {
            Console.WriteLine($"Init Page QueueReports {k} of {totalPages} - {DateTime.Now}");
            client = new RestClient("https://192.168.1.80/json");
            client.Timeout = -1;
            request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Basic T0NfQWRtaW46V0BlZlJ0cGMyMDIw");
            parameterString = "{\"request\": {\"method\": \"switchvox.callQueueLogs.search\", \"parameters\": {\"start_date\": \"";
            parameterString += beginDateString;
            parameterString += "\", \"end_date\": \"";
            parameterString += todayString;
            parameterString += "\", \"queue_account_ids\": [\"1145\"], \"call_types\": [\"completed_calls\", \"abandoned_calls\", \"redirected_calls";
            parameterString += "\"], \"page_number\": \"";
            parameterString += k.ToString();
            parameterString += "\"}}}";
            request.AddParameter("application/json", parameterString, ParameterType.RequestBody);
            response = client.Execute(request);
            Console.WriteLine($"Queue report received page {k}");
            result = response.Content;
            o = JObject.Parse(result);
            pageNumber = o["response"]["result"]["calls"]["page_number"].ToString();
            itemsPerPage = o["response"]["result"]["calls"]["items_per_page"].ToString();
            totalPages = o["response"]["result"]["calls"]["total_pages"].ToString();
            try 
            {
              a = JArray.Parse((o["response"]["result"]["calls"]["call"]).ToString());
            result = "";
            i = 0;
            foreach (var jo in a)
            {
                if (jo["uniqueid"] != null)
              {
                Call call = new Call();
                call = _context.Calls.Where(c => c.UniqueID ==
                  jo["uniqueid"].ToString()).FirstOrDefault();
                if (call.QueueLogs == null)
                {
                  call.QueueLogs = new List<QueueLog>();
                }
                QueueLog queueLog = new QueueLog();
                queueLog.QueueAccountID = jo["queue_account_id"].ToString();
                queueLog.QueueExtension = jo["queue_extension"].ToString();
                queueLog.EnterPosition = Int32.Parse(jo["enter_position"].ToString());
                queueLog.QueueName = jo["queue_name"].ToString();
                if (jo["member_account_id"] != null)
                {
                  queueLog.MemberAccountID = jo["member_account_id"].ToString();
                }
                queueLog.WaitTime = Int32.Parse(jo["wait_time"].ToString());
                if (jo["member_extension"] != null)
                {
                  queueLog.MemberExtension = jo["member_extension"].ToString();
                }
                if (jo["member_name"] != null)
                {
                  queueLog.MemberName = jo["member_name"].ToString();
                }
                queueLog.UniqueID = jo["uniqueid"].ToString();
                if (jo["start_time"] != null)
                {
                  queueLog.StartTime = DateTime.Parse(jo["start_time"].ToString());
                }
                if (jo["talk_time"] != null)
                {
                  queueLog.TalkTime = jo["talk_time"].ToString();
                }
                queueLog.Type = jo["type"].ToString();
                queueLog.MemberMisses = Int32.Parse(jo["member_misses"].ToString());
                call.QueueLogs.Add(queueLog);
                i++;
                _context.Update(call);
                _context.SaveChanges();
              }
            }
            }
            catch(Exception e)
            {
              try{
              o =  JObject.Parse((o["response"]["result"]["calls"]["call"]).ToString());
              if (o["uniqueid"] != null)
              {
                Call call = new Call();
                call = _context.Calls.Where(c => c.UniqueID ==
                  o["uniqueid"].ToString()).FirstOrDefault();
                if (call.QueueLogs == null)
                {
                  call.QueueLogs = new List<QueueLog>();
                }
                QueueLog queueLog = new QueueLog();
                queueLog.QueueAccountID = o["queue_account_id"].ToString();
                queueLog.QueueExtension = o["queue_extension"].ToString();
                queueLog.EnterPosition = Int32.Parse(o["enter_position"].ToString());
                queueLog.QueueName = o["queue_name"].ToString();
                if (o["member_account_id"] != null)
                {
                  queueLog.MemberAccountID = o["member_account_id"].ToString();
                }
                queueLog.WaitTime = Int32.Parse(o["wait_time"].ToString());
                if (o["member_extension"] != null)
                {
                  queueLog.MemberExtension = o["member_extension"].ToString();
                }
                if (o["member_name"] != null)
                {
                  queueLog.MemberName = o["member_name"].ToString();
                }
                queueLog.UniqueID = o["uniqueid"].ToString();
                if (o["start_time"] != null)
                {
                  queueLog.StartTime = DateTime.Parse(o["start_time"].ToString());
                }
                if (o["talk_time"] != null)
                {
                  queueLog.TalkTime = o["talk_time"].ToString();
                }
                queueLog.Type = o["type"].ToString();
                queueLog.MemberMisses = Int32.Parse(o["member_misses"].ToString());
                call.QueueLogs.Add(queueLog);
                i++;
                _context.Update(call);
                _context.SaveChanges();
              }

            }
            catch(Exception e0){Console.WriteLine(e0);}
            }
          }
            }
            }
            //initTalkTimeRecords(_context);
            Console.WriteLine($"***INIT COMPLETE*** {DateTime.Now}");
        }

        public static string updateExtsQs(DataContext context)
        {
          DataContext _context = context;
            string answer = "";

            Console.WriteLine("Importing extensions");
            string apiResult;
            IgnoreBadCertificates();
            var client = new RestClient("https://192.168.1.80/json");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Basic T0NfQWRtaW46V0BlZlJ0cGMyMDIw");
            request.AddParameter("application/json", "{\"request\": {\"method\": \"switchvox.extensions.search\", \"parameters\": {\"min_extension\": \"100\", \"max_extension\": \"10000\", \"items_per_page\": \"1000\"}}}",             ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            apiResult = response.Content;
            JObject o = JObject.Parse(apiResult);
            JArray a = JArray.Parse((o["response"]["result"]["extensions"]["extension"]).ToString());
            apiResult = "";
            int i = 0;
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
                string dateCreated = jo["date_created"].ToString();
                ext.DateCreated = DateTime.Parse(dateCreated);
                if (jo["last_name"] != null)
                {
                    ext.LastName = jo["last_name"].ToString();
                }
                ext.TypeDisplay = jo["type_display"].ToString();
                ext.Display = jo["display"].ToString();
                Console.WriteLine("Display: " + ext.Display);
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

                Console.WriteLine($"Updating Ext: {ext.Number} - <{ext.Display}> - {ext.Type}");

                report.TimeStamp = DateTime.Now;
                report.ReportString = $"Added new ext: {i}";
                _context.Update(report);
                _context.SaveChanges();
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
            requestQ.AddParameter("application/json", "{\"request\": {\"method\": \"switchvox.callQueues.getCurrentStatus\", \"parameters\": {\"account_id\": \"1124\"}}}",           ParameterType.RequestBody);
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
                  Console.WriteLine("Updating Member " + k);
                  var extq = _context.Extensions.Where(id => id.AccountID == joq["account_id"].ToString()).FirstOrDefault();
                  queue.Members.Add(extq);
                  k++;
                }

                _context.Update(queue);
                _context.SaveChanges();

                
            }
            var requestQ1 = new RestRequest(Method.POST);
            requestQ1.AddHeader("Content-Type", "application/json");
            requestQ1.AddHeader("Authorization", "Basic T0NfQWRtaW46V0BlZlJ0cGMyMDIw");
            requestQ1.AddParameter("application/json", "{\"request\": {\"method\": \"switchvox.callQueues.getCurrentStatus\", \"parameters\": {\"account_id\": \"1124\"}}}",           ParameterType.RequestBody);
            var responseQ1 = clientQ.Execute(requestQ);
            apiResult = "";
            apiResult = responseQ.Content;
            o = JObject.Parse(apiResult);
            if(o["response"]["result"]["call_queue"] != null)
            {
                Console.WriteLine("Queues not null");

                var queueList = _context.Queues.ToList();
                var queue = new Queue();
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
                  Console.WriteLine("Updating Member " + k);
                  var extq = _context.Extensions.Where(id => id.AccountID == joq["account_id"].ToString()).FirstOrDefault();
                  queue.Members.Add(extq);
                  k++;
                }

                _context.Update(queue);
                _context.SaveChanges();
            }
            // Updating New Collections Queue
            var requestQ2 = new RestRequest(Method.POST);
            requestQ1.AddHeader("Content-Type", "application/json");
            requestQ1.AddHeader("Authorization", "Basic T0NfQWRtaW46V0BlZlJ0cGMyMDIw");
            requestQ1.AddParameter("application/json", "{\"request\": {\"method\": \"switchvox.callQueues.getCurrentStatus\", \"parameters\": {\"account_id\": \"1145\"}}}", ParameterType.RequestBody);
            var responseQ2 = clientQ.Execute(requestQ);
            apiResult = "";
            apiResult = responseQ.Content;
            o = JObject.Parse(apiResult);
            if (o["response"]["result"]["call_queue"] != null)
            {
                Console.WriteLine("Queues not null");

                var queueList = _context.Queues.ToList();
                var queue = new Queue();
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

                foreach (var joq in qa)
                {
                    Console.WriteLine("Updating Member " + k);
                    var extq = _context.Extensions.Where(id => id.AccountID == joq["account_id"].ToString()).FirstOrDefault();
                    queue.Members.Add(extq);
                    k++;
                }

                _context.Update(queue);
                _context.SaveChanges();
            }
        }
        public static void initTalkTimeRecords(DataContext context)
        {
          DataContext _context = context;
          List<string> extensionAccountIds = new List<string>();
          foreach(var ext in _context.Extensions.Where(e => e.Type == "sip").ToList())
          {
              extensionAccountIds.Add(ext.AccountID);
          }
          Console.WriteLine("initTalkTimeRecords" + extensionAccountIds);
          DateTime today = DateTime.Now;
          TimeSpan reportPeriod = new TimeSpan(1,0,0,0);
          DateTime beginDate = today - reportPeriod;
          string todayString = today.ToString("yyyy-MM-dd HH:mm:ss");
          string beginDateString = beginDate.ToString("yyyy-MM-dd HH:mm:ss");
          foreach(string ext in extensionAccountIds)
          {
              string apiResult;
              IgnoreBadCertificates();
              var client = new RestClient("https://192.168.1.80/json");
              client.Timeout = -1;
              var request = new RestRequest(Method.POST);
              request.AddHeader("Content-Type", "application/json");
              request.AddHeader("Authorization", "Basic T0NfQWRtaW46V0BlZlJ0cGMyMDIw");
              string parameterString1 = "{\"request\": {\"method\":\"switchvox.callReports.search\", \"parameters\":{\"start_date\": \"";
              parameterString1 += beginDateString;
              parameterString1 += "\", \"end_date\": \"";
              parameterString1 += todayString;
              parameterString1 += "\", \"account_ids\": [\"";
              parameterString1 += ext;
              parameterString1 += "\"], \"ignore_weekends\": \"0\",\"breakdown\": \"by_day\", \"report_fields\": [\"total_calls\",\"total_incoming_calls\", \"total_outgoing_calls\",\"talking_duration\", \"call_duration\"], \"items_per_page\": \"1000\"}}}";
              request.AddParameter("application/json", parameterString1,ParameterType.RequestBody);
              IRestResponse response = client.Execute(request);
              apiResult = response.Content;
              JObject o = JObject.Parse(apiResult);
              JArray a = JArray.Parse(o["response"]["result"]["rows"]["row"].ToString());
              int k = 0;
              foreach (var jo in a)
              {
                TalkTimeRecord record = new TalkTimeRecord();
                var recordList = _context.TalkTimeRecords.ToList();
                foreach (var record0 in recordList)
                {
                    if (record0.Date.ToString("M/dd/yyyy") == jo["date"].ToString()  && ext == record0.AccountID)
                    {
                        record = record0;
                    }
                }
                record.AccountID = ext;
                record.Date = DateTime.Parse(jo["date"].ToString());
                record.TalkingDuration = Int32.Parse(jo["talking_duration"].ToString ());
                record.TotalOutgoingCalls = Int32.Parse(jo["total_outgoing_calls"].ToString());
                record.TotalIncomingCalls = Int32.Parse(jo["total_incoming_calls"].ToString());
                record.TotalCalls = Int32.Parse(jo["total_calls"].ToString());
                record.CallDuration = Int32.Parse(jo["call_duration"].ToString());
                _context.Update(record);
                _context.SaveChanges();
                k++;
                Console.WriteLine("TalkRecordUpdated: " + ext + " no: " + k);
              }
              Console.WriteLine(ext + " Talk Records Init Completed.");
          }
          Console.WriteLine("initRecordes Updated.");
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