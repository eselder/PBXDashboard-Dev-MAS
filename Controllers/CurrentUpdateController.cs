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
    public class CurrentUpdateController : ControllerBase
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
        private Boolean isStart = false;
        private const double updateSeconds = 2.0;

        public List<CurrentCall> currentCalls = new List<CurrentCall>();
        public List<ExtensionStatus> extensionStatuses = new List<ExtensionStatus>();

        public CurrentUpdateController(IMemoryCache cache)
        {
            _cache = cache;

            _cache.Set("ExtensionList", this.extensionList);
        }

        string _result = "";
        // GET: api/CurrentUpdate
        [HttpGet]
        public IEnumerable<string> Get()
        {
            _result = updateCurrentCalls(_cache);
            return new string[] { "Update Current Calls", $"{_result}" };
        }
        public string updateCurrentCalls(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            DateTime now = DateTime.Now;
            DateTime oldTick;
            isStart = !_cache.TryGetValue("Now", out oldTick);
            if (isStart)
            {
                Console.WriteLine("Null");
            }
            _cache.Set("Now", now, DateTime.Now.AddDays(1));
            DateTime now0;
            _cache.TryGetValue("Now", out now0);
            double tickDiff = (now - oldTick).TotalSeconds;

            Console.WriteLine("Updating Current Calls");
            string apiResult;
            IgnoreBadCertificates();
            var client = new RestClient("https://192.168.1.80/json");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Basic T0NfQWRtaW46V0BlZlJ0cGMyMDIw");
            request.AddParameter("application/json", "{\"request\": {\"method\": \"switchvox.currentCalls.getList\", \"parameters\": {}}}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            apiResult = response.Content;
            JObject o = JObject.Parse(apiResult);
            o = JObject.Parse(o["response"]["result"]["current_calls"].ToString());
            if (Int32.Parse(o["total_items"].ToString()) == 0)
            {
                this.currentCalls = new List<CurrentCall>();
                Console.WriteLine("0 current Calls");
                var ccjson = JsonSerializer.Serialize(currentCalls);
                _cache.Set("CurrentCalls", ccjson, DateTime.Now.AddDays(1));
                Console.WriteLine("Cleared Old Calls");

                foreach (string ext in extensionList)
                {
                    ExtensionStatus extS = new ExtensionStatus();
                    extS.ID = ext;
                    double iTime;
                    _cache.Set(ext + "-IsCCall", false, DateTime.Now.AddDays(1));
                    string presence;
                    _cache.TryGetValue(ext + "-Presence", out presence);
                    string subPresence;
                    _cache.TryGetValue(ext + "-SubPresence", out subPresence);
                    string oldStatus;
                    _cache.TryGetValue(ext + "-Status", out oldStatus);
                    if (now.Minute % 66 == 0 && false)
                    {
                        updateLoggedOutStatus(ext, presence, subPresence, oldStatus, tickDiff);
                        continue;
                    }
                    else if (presence != "available")
                    {
                        updateLoggedOutStatus(ext, presence, subPresence, oldStatus, tickDiff);
                        continue;
                    }
                    else
                    {
                        if (oldStatus == "away" || oldStatus == "xa" || oldStatus == "dnd")
                        {
                            string lastActivity1;
                            lastActivity1 = DateTime.Now.ToShortTimeString() + " Logged In";
                            _cache.Set(ext + "-LastActivity", lastActivity1, DateTime.Now.AddDays(1));
                        }
                        string oStatus;
                        if (!_cache.TryGetValue(ext + "-Status", out oStatus))
                        {
                            _cache.Set(ext + "-Status", "idle", DateTime.Now.AddDays(1));
                            _cache.Set(ext + "-SubStatus", "", DateTime.Now.AddDays(1));
                            List<double> emptyList = new List<double>();
                            _cache.Set(ext + "-idleTimeList", emptyList, DateTime.Now.AddDays(1));
                        }
                        string oldStatus0;
                        _cache.TryGetValue(ext + "-Status", out oldStatus0);
                        _cache.Set(ext + "-Status", "idle", DateTime.Now.AddDays(1));
                        _cache.Set(ext + "-SubStatus", "", DateTime.Now.AddDays(1));
                        _cache.Set(ext + "-OldPBXID", "0", DateTime.Now.AddDays(1));
                        double oldLoggedInTime;
                        if (!_cache.TryGetValue(ext + "-LoggedInTime", out oldLoggedInTime))
                        {
                            _cache.Set(ext + "-LoggedInTime", updateSeconds, DateTime.Now.AddDays(1));
                        }
                        else
                        {
                            _cache.TryGetValue(ext + "-LoggedInTime", out oldLoggedInTime);
                            _cache.Set(ext + "-LoggedInTime", tickDiff + oldLoggedInTime, DateTime.Now.AddDays(1));
                        }
                        if (!_cache.TryGetValue(ext + "-ITime", out iTime))
                        {
                            _cache.Set(ext + "-ITime", updateSeconds, DateTime.Now.AddDays(1));
                            _cache.Set(ext + "-ATime", updateSeconds, DateTime.Now.AddDays(1));
                        }
                        else
                        {
                            double oldITime;
                            if (oldStatus0 == "idle")
                            {
                                double oldATime;
                                _cache.TryGetValue(ext + "-ATime", out oldATime);
                                _cache.Set(ext + "-ATime", tickDiff + oldATime, DateTime.Now.AddDays(1));
                            }
                            else
                            {
                                _cache.Set(ext + "-ATime", tickDiff, DateTime.Now.AddDays(1));
                                _cache.TryGetValue(ext + "-IdleTimeList", out List<double> iTimeList);
                                iTimeList.Add(0);

                            }
                            _cache.TryGetValue(ext + "-ITime", out oldITime);
                            _cache.Set(ext + "-ITime", tickDiff + oldITime, DateTime.Now.AddDays(1));
                            if (!_cache.TryGetValue(ext + "-IdleTimeList", out List<double> iTimeListx))
                            {
                                _cache.Set(ext + "-IdleTimeList", new List<double>() { 0 }, DateTime.Now.AddDays(1));
                            }
                            _cache.TryGetValue(ext + "-IdleTimeList", out List<double> iTimeList0);
                            iTimeList0[iTimeList0.Count - 1] += tickDiff;
                            _cache.Set(ext + "-IdleTimeList", iTimeList0, DateTime.Now.AddDays(1));
                            extS.ITime = tickDiff + oldITime;
                            double str;
                            _cache.TryGetValue(ext + "-ITime", out str);
                            double atime;
                            _cache.TryGetValue(ext + "-ATime", out atime);

                        }
                    }

                    string status;
                    _cache.TryGetValue(ext + "-Status", out status);
                    extS.Status = status;
                    string subStatus;
                    if (_cache.TryGetValue(ext + "-SubStatus", out subStatus))
                    {
                        extS.SubStatus = subStatus;
                    }
                    string lastActivity;
                    _cache.TryGetValue(ext + "-LastActivity", out lastActivity);
                    extS.LastActivity = lastActivity;
                    List<string> sList0;
                    _cache.TryGetValue(ext + "-StatusList", out sList0);
                    extS.StatusList = sList0;
                    List<string> subList0;
                    _cache.TryGetValue(ext + "-SubStatusList", out subList0);
                    extS.SubStatusList = subList0;
                    List<double> dList0;
                    _cache.TryGetValue(ext + "-DurationList", out dList0);
                    extS.DurationList = dList0;
                    List<DateTime> loTList0;
                    _cache.TryGetValue(ext + "-LogoutTimesList", out loTList0);
                    extS.LogoutTimesList = loTList0;
                    string lastActivityIdle;
                    _cache.TryGetValue(ext + "-lastActivityIdle", out lastActivityIdle);
                    extS.LastActivityIdle = lastActivityIdle;
                    _cache.TryGetValue(ext + "-ITime", out iTime);
                    extS.ITime = iTime;
                    double aTime;
                    _cache.TryGetValue(ext + "-ATime", out aTime);
                    extS.ATime = aTime;
                    double cTime;
                    _cache.TryGetValue(ext + "-CTime", out cTime);
                    extS.CTime = cTime;
                    double loggedInTime;
                    _cache.TryGetValue(ext + "-LoggedInTime", out loggedInTime);
                    extS.LoggedInTime = loggedInTime;
                    int totalCalls;
                    int tICalls;
                    _cache.TryGetValue(ext + "-TotalICalls", out tICalls);
                    extS.TotalICalls = tICalls;
                    _cache.TryGetValue(ext + "-TotalCalls", out totalCalls);
                    extS.TotalCalls = totalCalls;
                    int tOCalls;
                    _cache.TryGetValue(ext + "-TotalOCalls", out tOCalls);
                    extS.TotalOCalls = tOCalls;
                    List<double> iTList;
                    _cache.TryGetValue(ext + "-IdleTimeList", out iTList);
                    extS.IdleTimeList = iTList;
                    this.extensionStatuses.Add(extS);
                }
            }
            else if (Int32.Parse(o["total_items"].ToString()) == 1)
            {
                this.currentCalls = new List<CurrentCall>();
                Console.WriteLine("1 current Call.");
                _cache.Set("CurrentCalls", this.currentCalls, DateTime.Now.AddDays(1));

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
                this.currentCalls.Add(ccall);
                var ccjson = JsonSerializer.Serialize(this.currentCalls);
                _cache.Set("CurrentCalls", ccjson, DateTime.Now.AddDays(1));

                foreach (string ext in extensionList)
                {
                    ExtensionStatus extS = new ExtensionStatus();
                    extS.ID = ext;
                    double iTime;
                    _cache.Set(ext + "-IsCCall", false, DateTime.Now.AddDays(1));
                    string presence;
                    _cache.TryGetValue(ext + "-Presence", out presence);
                    string subPresence;
                    _cache.TryGetValue(ext + "-SubPresence", out subPresence);
                    string oldStatus;
                    _cache.TryGetValue(ext + "-Status", out oldStatus);
                    if (now.Minute % 66 == 0 && false)
                    {
                        updateLoggedOutStatus(ext, presence, subPresence, oldStatus, tickDiff);
                        continue;
                    }
                    else if (presence != "available")
                    {
                        updateLoggedOutStatus(ext, presence, subPresence, oldStatus, tickDiff);
                        continue;
                    }
                    else
                    {
                        if (oldStatus == "away" || oldStatus == "xa" || oldStatus == "dnd")
                        {
                            string lastActivity1;
                            lastActivity1 = DateTime.Now.ToShortTimeString() + " Logged In";
                            _cache.Set(ext + "-LastActivity", lastActivity1, DateTime.Now.AddDays(1));
                        }
                        if (ccall.ToCallerIdNumber == ext || ccall.FromCallerIdNumber == ext)
                        {
                            _cache.Set(ext + "-IsCCall", true, DateTime.Now.AddDays(1));
                            string oStatus;
                            if (!_cache.TryGetValue(ext + "-Status", out oStatus))
                            {
                                _cache.Set(ext + "-Status", ccall.State, DateTime.Now.AddDays(1));
                                Console.WriteLine("Updating initial Call Counts");
                                List<double> emptyList = new List<double>();
                                _cache.Set(ext + "-idleTimeList", emptyList, DateTime.Now.AddDays(1));
                            }
                            _cache.TryGetValue(ext + "-Status", out oldStatus);
                            _cache.TryGetValue(ext + "-Presence", out presence);
                            _cache.TryGetValue(ext + "-SubPresence", out subPresence);
                            if (presence != "available")
                            {
                                _cache.Set(ext + "-Status", presence, DateTime.Now.AddDays(1));
                                _cache.Set(ext + "-SubStatus", subPresence, DateTime.Now.AddDays(1));
                                if (oldStatus != ccall.State)
                                {
                                    if (ccall.ToCallerIdNumber.Length == 3 && ccall.FromCallerIdNumber.Length == 3)
                                    {
                                        string lastActivity1;
                                        lastActivity1 = "Internal: " + ccall.FromCallerIdName + " to " + ccall.ToCallerIdName;
                                        _cache.Set(ext + "-LastActivity", lastActivity1, DateTime.Now.AddDays(1));
                                        string lastActivityIdle1 = DateTime.Now.ToString("HH:mm") + " " + lastActivity1;
                                        _cache.Set(ext + "-LastActivityIdle", lastActivityIdle1, DateTime.Now.AddDays(1));
                                        _cache.Set(ext + "-LastActiveTime", DateTime.Now, DateTime.Now.AddDays(1));
                                    }
                                    else if (ccall.ToCallerIdNumber == ext)
                                    {
                                        string lastActivity1;
                                        if (ccall.FromCallerIdName == "Unknown")
                                        {
                                            lastActivity1 = "Incoming: " + ccall.FromCallerIdNumber;
                                        }
                                        else
                                        {
                                            lastActivity1 = "Incoming: " + ccall.FromCallerIdName + " " + ccall.FromCallerIdNumber;
                                        }
                                        _cache.Set(ext + "-LastActivity", lastActivity1, DateTime.Now.AddDays(1));
                                        string lastActivityIdle1 = DateTime.Now.ToString("HH:mm") + " " + lastActivity1;
                                        _cache.Set(ext + "-LastActivityIdle", lastActivityIdle1, DateTime.Now.AddDays(1));
                                        _cache.Set(ext + "-LastActiveTime", DateTime.Now, DateTime.Now.AddDays(1));
                                    }
                                    else
                                    {
                                        string lastActivity1;
                                        if (ccall.ToCallerIdName == "Unknown")
                                        {
                                            lastActivity1 = "Outgoing: " + ccall.ToCallerIdNumber;
                                        }
                                        else
                                        {
                                            lastActivity1 = "Outgoing: " + ccall.ToCallerIdName + " " + ccall.ToCallerIdNumber;
                                        }
                                        _cache.Set(ext + "-LastActivity", lastActivity1, DateTime.Now.AddDays(1));
                                        string lastActivityIdle1 = DateTime.Now.ToString("HH:mm") + " " + lastActivity1;
                                        _cache.Set(ext + "-LastActivityIdle", lastActivityIdle1, DateTime.Now.AddDays(1));
                                        _cache.Set(ext + "-LastActiveTime", DateTime.Now, DateTime.Now.AddDays(1));
                                    }
                                }

                                _cache.Set(ext + "-Status", ccall.State);
                                double cTime0;
                                if (!_cache.TryGetValue(ext + "-CTime", out cTime0))
                                {
                                    _cache.Set(ext + "-CTime", updateSeconds, DateTime.Now.AddDays(1));
                                    _cache.Set(ext + "-ATime", updateSeconds, DateTime.Now.AddDays(1));
                                    List<double> emptyList = new List<double>();
                                    _cache.Set(ext + "-idleTimeList", emptyList, DateTime.Now.AddDays(1));
                                }
                                else
                                {
                                    double oldCTime;
                                    if (oldStatus == ccall.State)
                                    {
                                        double oldATime;
                                        _cache.TryGetValue(ext + "-ATime", out oldATime);
                                        _cache.Set(ext + "-ATime", tickDiff + oldATime, DateTime.Now.AddDays(1));
                                    }
                                    else
                                    {
                                        _cache.Set(ext + "-ATime", tickDiff, DateTime.Now.AddDays(1));
                                    }
                                    double oldLoggedInTime;
                                    _cache.TryGetValue(ext + "-LoggedInTime", out oldLoggedInTime);
                                    _cache.Set(ext + "-LoggedInTime", tickDiff + oldLoggedInTime, DateTime.Now.AddDays(1));
                                    _cache.TryGetValue(ext + "-CTime", out oldCTime);
                                    _cache.Set(ext + "-CTime", tickDiff + oldCTime, DateTime.Now.AddDays(1));
                                    double str;
                                    _cache.TryGetValue(ext + "-CTime", out str);
                                }
                            }
                            else
                            {
                                _cache.Set(ext + "-Status", ccall.State, DateTime.Now.AddDays(1));
                                _cache.Set(ext + "-SubStatus", subPresence, DateTime.Now.AddDays(1));
                                if (oldStatus != ccall.State)
                                {
                                    Console.WriteLine("New Call");
                                    if (ccall.ToCallerIdNumber.Length == 3 && ccall.FromCallerIdNumber.Length == 3)
                                    {
                                        string lastActivity1;
                                        lastActivity1 = "Internal: " + ccall.FromCallerIdName + " to " + ccall.ToCallerIdName;
                                        _cache.Set(ext + "-LastActivity", lastActivity1, DateTime.Now.AddDays(1));
                                        string lastActivityIdle1 = DateTime.Now.ToString("HH:mm") + " " + lastActivity1;
                                        _cache.Set(ext + "-LastActivityIdle", lastActivityIdle1, DateTime.Now.AddDays(1));
                                        _cache.Set(ext + "-LastActiveTime", DateTime.Now, DateTime.Now.AddDays(1));
                                    }
                                    else if (ccall.ToCallerIdNumber == ext)
                                    {
                                        string lastActivity1;
                                        if (ccall.FromCallerIdName == "Unknown")
                                        {
                                            lastActivity1 = "Incoming: " + ccall.FromCallerIdNumber;
                                        }
                                        else
                                        {
                                            lastActivity1 = "Incoming: " + ccall.FromCallerIdName + " " + ccall.FromCallerIdNumber;
                                        }
                                        _cache.Set(ext + "-LastActivity", lastActivity1, DateTime.Now.AddDays(1));
                                        string lastActivityIdle1 = DateTime.Now.ToString("HH:mm") + " " + lastActivity1;
                                        _cache.Set(ext + "-LastActivityIdle", lastActivityIdle1, DateTime.Now.AddDays(1));
                                        _cache.Set(ext + "-LastActiveTime", DateTime.Now, DateTime.Now.AddDays(1));
                                    }
                                    else
                                    {
                                        string lastActivity1;
                                        if (ccall.ToCallerIdName == "Unknown")
                                        {
                                            lastActivity1 = "Outgoing: " + ccall.ToCallerIdNumber;
                                        }
                                        else
                                        {
                                            lastActivity1 = "Outgoing: " + ccall.ToCallerIdName + " " + ccall.ToCallerIdNumber;
                                        }
                                        _cache.Set(ext + "-LastActivity", lastActivity1, DateTime.Now.AddDays(1));
                                        string lastActivityIdle1 = DateTime.Now.ToString("HH:mm") + " " + lastActivity1;
                                        _cache.Set(ext + "-LastActivityIdle", lastActivityIdle1, DateTime.Now.AddDays(1));
                                        _cache.Set(ext + "-LastActiveTime", DateTime.Now, DateTime.Now.AddDays(1));
                                    }
                                }

                                _cache.Set(ext + "-Status", ccall.State);
                                double cTime0;
                                if (!_cache.TryGetValue(ext + "-CTime", out cTime0))
                                {
                                    _cache.Set(ext + "-CTime", updateSeconds, DateTime.Now.AddDays(1));
                                    _cache.Set(ext + "-ATime", updateSeconds, DateTime.Now.AddDays(1));
                                }
                                else
                                {
                                    double oldCTime;
                                    if (oldStatus == ccall.State)
                                    {
                                        double oldATime;
                                        _cache.TryGetValue(ext + "-ATime", out oldATime);
                                        _cache.Set(ext + "-ATime", tickDiff + oldATime, DateTime.Now.AddDays(1));
                                    }
                                    else
                                    {
                                        _cache.Set(ext + "-ATime", tickDiff, DateTime.Now.AddDays(1));
                                    }
                                    double oldLoggedInTime;
                                    _cache.TryGetValue(ext + "-LoggedInTime", out oldLoggedInTime);
                                    _cache.Set(ext + "-LoggedInTime", tickDiff + oldLoggedInTime, DateTime.Now.AddDays(1));
                                    _cache.TryGetValue(ext + "-CTime", out oldCTime);
                                    _cache.Set(ext + "-CTime", tickDiff + oldCTime, DateTime.Now.AddDays(1));
                                    double str;
                                    _cache.TryGetValue(ext + "-CTime", out str);
                                }
                            }
                        }
                        else
                        {
                            extS.ID = ext;
                            double iTime0;
                            _cache.Set(ext + "-IsCCall", false, DateTime.Now.AddDays(1));
                            _cache.TryGetValue(ext + "-Presence", out presence);
                            _cache.TryGetValue(ext + "-SubPresence", out subPresence);
                            _cache.TryGetValue(ext + "-Status", out oldStatus);
                            if (presence != "available")
                            {
                                _cache.Set(ext + "-Status", presence, DateTime.Now.AddDays(1));
                                _cache.Set(ext + "-SubStatus", subPresence, DateTime.Now.AddDays(1));
                                double atime;
                                _cache.TryGetValue(ext + "-ATime", out atime);
                                if (atime > 1000000000)
                                {
                                    _cache.Set(ext + "-ITime", 0, DateTime.Now.AddDays(1));
                                    _cache.Set(ext + "-ATime", updateSeconds, DateTime.Now.AddDays(1));
                                    _cache.Set(ext + "-LastActivity", "N/A", DateTime.Now.AddDays(1));
                                    List<double> emptyList = new List<double>();
                                    _cache.Set(ext + "-idleTimeList", emptyList, DateTime.Now.AddDays(1));
                                }
                                else
                                {
                                    double oldATime;
                                    if (oldStatus == presence)
                                    {
                                        _cache.TryGetValue(ext + "-ATime", out oldATime);
                                        _cache.Set(ext + "-ATime", tickDiff + oldATime, DateTime.Now.AddDays(1));
                                    }
                                    else
                                    {
                                        _cache.Set(ext + "-ATime", tickDiff, DateTime.Now.AddDays(1));
                                    }
                                }
                            }
                            else
                            {
                                string oStatus;
                                if (!_cache.TryGetValue(ext + "-Status", out oStatus))
                                {
                                    _cache.Set(ext + "-Status", "idle", DateTime.Now.AddDays(1));
                                    _cache.Set(ext + "-SubStatus", "", DateTime.Now.AddDays(1));
                                }
                                string oldStatus0;
                                _cache.TryGetValue(ext + "-Status", out oldStatus0);
                                _cache.Set(ext + "-Status", "idle", DateTime.Now.AddDays(1));
                                _cache.Set(ext + "-SubStatus", "", DateTime.Now.AddDays(1));
                                _cache.Set(ext + "-OldPBXID", "0", DateTime.Now.AddDays(1));
                                double oldLoggedInTime;
                                if (!_cache.TryGetValue(ext + "-LoggedInTime", out oldLoggedInTime))
                                {
                                    _cache.Set(ext + "-LoggedInTime", updateSeconds, DateTime.Now.AddDays(1));
                                }
                                else
                                {
                                    _cache.TryGetValue(ext + "-LoggedInTime", out oldLoggedInTime);
                                    _cache.Set(ext + "-LoggedInTime", tickDiff + oldLoggedInTime, DateTime.Now.AddDays(1));
                                }
                                if (!_cache.TryGetValue(ext + "-ITime", out iTime0))
                                {
                                    _cache.Set(ext + "-ITime", updateSeconds, DateTime.Now.AddDays(1));
                                    _cache.Set(ext + "-ATime", updateSeconds, DateTime.Now.AddDays(1));
                                }
                                else
                                {
                                    double oldITime;
                                    if (oldStatus0 == "idle")
                                    {
                                        double oldATime;
                                        _cache.TryGetValue(ext + "-ATime", out oldATime);
                                        _cache.Set(ext + "-ATime", tickDiff + oldATime, DateTime.Now.AddDays(1));
                                    }
                                    else
                                    {
                                        _cache.Set(ext + "-ATime", tickDiff, DateTime.Now.AddDays(1));
                                        _cache.TryGetValue(ext + "-IdleTimeList", out List<double> iTimeList);
                                        iTimeList.Add(0);

                                    }
                                    _cache.TryGetValue(ext + "-ITime", out oldITime);
                                    _cache.Set(ext + "-ITime", tickDiff + oldITime, DateTime.Now.AddDays(1));
                                    if (!_cache.TryGetValue(ext + "-IdleTimeList", out List<double> iTimeListx))
                                    {
                                        _cache.Set(ext + "-IdleTimeList", new List<double>() { 0 }, DateTime.Now.AddDays(1));
                                    }
                                    _cache.TryGetValue(ext + "-IdleTimeList", out List<double> iTimeList0);
                                    iTimeList0[iTimeList0.Count - 1] += tickDiff;
                                    _cache.Set(ext + "-IdleTimeList", iTimeList0, DateTime.Now.AddDays(1));
                                    extS.ITime = tickDiff + oldITime;
                                    double str;
                                    _cache.TryGetValue(ext + "-ITime", out str);
                                    double atime;
                                    _cache.TryGetValue(ext + "-ATime", out atime);

                                }
                            }
                        }
                    }
                    string status;
                    _cache.TryGetValue(ext + "-Status", out status);
                    extS.Status = status;
                    string subStatus;
                    if (_cache.TryGetValue(ext + "-SubStatus", out subStatus))
                    {
                        extS.SubStatus = subStatus;
                    }
                    string lastActivity;
                    _cache.TryGetValue(ext + "-LastActivity", out lastActivity);
                    extS.LastActivity = lastActivity;
                    List<string> sList0;
                    _cache.TryGetValue(ext + "-StatusList", out sList0);
                    extS.StatusList = sList0;
                    List<string> subList0;
                    _cache.TryGetValue(ext + "-SubStatusList", out subList0);
                    extS.SubStatusList = subList0;
                    List<double> dList0;
                    _cache.TryGetValue(ext + "-DurationList", out dList0);
                    extS.DurationList = dList0;
                    List<DateTime> loTList0;
                    _cache.TryGetValue(ext + "-LogoutTimesList", out loTList0);
                    extS.LogoutTimesList = loTList0;
                    string lastActivityIdle;
                    _cache.TryGetValue(ext + "-lastActivityIdle", out lastActivityIdle);
                    extS.LastActivityIdle = lastActivityIdle;
                    _cache.TryGetValue(ext + "-ITime", out iTime);
                    extS.ITime = iTime;
                    double aTime;
                    _cache.TryGetValue(ext + "-ATime", out aTime);
                    extS.ATime = aTime;
                    double cTime;
                    _cache.TryGetValue(ext + "-CTime", out cTime);
                    extS.CTime = cTime;
                    int tOCalls;
                    _cache.TryGetValue(ext + "-TotalOCalls", out tOCalls);
                    extS.TotalOCalls = tOCalls;
                    int tICalls;
                    _cache.TryGetValue(ext + "-TotalICalls", out tICalls);
                    extS.TotalICalls = tICalls;
                    double loggedInTime;
                    _cache.TryGetValue(ext + "-LoggedInTime", out loggedInTime);
                    extS.LoggedInTime = loggedInTime;
                    int totalCalls;
                    _cache.TryGetValue(ext + "-TotalCalls", out totalCalls);
                    extS.TotalCalls = totalCalls;
                    List<double> iTList;
                    _cache.TryGetValue(ext + "-IdleTimeList", out iTList);
                    extS.IdleTimeList = iTList;
                    this.extensionStatuses.Add(extS);
                }
            }
            ///////////////////////////////////  MULTI CALL /////////////////////////////////////////////
            else
            {
                List<string> eList = new List<string>(extensionList);
                this.currentCalls = new List<CurrentCall>();

                JArray a = JArray.Parse(o["current_call"].ToString());
                foreach (var joC in a)
                {
                    CurrentCall ccall = new CurrentCall();
                    try
                    {
                        ccall.StartTime = DateTime.Parse(joC["start_time"].ToString());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    ccall.StartTime = DateTime.Parse(joC["start_time"].ToString());
                    ccall.Duration = Int32.Parse(joC["duration"].ToString());
                    ccall.ToCallerIdName = joC["to_caller_id_name"].ToString();
                    ccall.ToCallerIdNumber = joC["to_caller_id_number"].ToString();
                    ccall.FromCallerIdName = joC["from_caller_id_name"].ToString();
                    ccall.FromCallerIdNumber = joC["from_caller_id_number"].ToString();
                    ccall.Format = joC["format"].ToString();
                    ccall.State = joC["state"].ToString();
                    ccall.PBXID = joC["id"].ToString();

                    this.currentCalls.Add(ccall);

                    foreach (string ext in extensionList)
                    {
                        ExtensionStatus extS = new ExtensionStatus();
                        extS.ID = ext;
                        _cache.Set(ext + "-IsCCall", false, DateTime.Now.AddDays(1));
                        string presence;
                        _cache.TryGetValue(ext + "-Presence", out presence);
                        string subPresence;
                        _cache.TryGetValue(ext + "-SubPresence", out subPresence);
                        string oldStatus;
                        _cache.TryGetValue(ext + "-Status", out oldStatus);
                        if (now.Minute % 66 == 0 && eList.Contains(ext) && false)
                    {
                        eList.RemoveAll(e => e == ext);
                        updateLoggedOutStatus(ext, presence, subPresence, oldStatus, tickDiff);
                        continue;
                    }
                        else if (presence != "available" && eList.Contains(ext))
                        {
                            eList.RemoveAll(e => e == ext);
                            updateLoggedOutStatus(ext, presence, subPresence, oldStatus, tickDiff);
                            continue;
                        }
                        else if(!eList.Contains(ext))
                        {
                            continue;
                        }
                        else
                        {
                            if (oldStatus == "away" || oldStatus == "xa" || oldStatus == "dnd")
                            {
                                string lastActivity1;
                                lastActivity1 = DateTime.Now.ToShortTimeString() + " Logged In";
                                _cache.Set(ext + "-LastActivity", lastActivity1, DateTime.Now.AddDays(1));
                            }
                            _cache.Set(ext + "-IsCCall", false, DateTime.Now.AddDays(1));
                            if (ccall.ToCallerIdNumber == ext || ccall.FromCallerIdNumber == ext)
                            {
                                eList.RemoveAll(e => e == ext);
                                _cache.Set(ext + "-IsCCall", true, DateTime.Now.AddDays(1));
                                string oStatus;
                                if (!_cache.TryGetValue(ext + "-Status", out oStatus))
                                {
                                    _cache.Set(ext + "-Status", ccall.State, DateTime.Now.AddDays(1));
                                    List<double> emptyList = new List<double>();
                                    _cache.Set(ext + "-idleTimeList", emptyList, DateTime.Now.AddDays(1));
                                }
                                _cache.TryGetValue(ext + "-Status", out oldStatus);
                                _cache.Set(ext + "-Status", ccall.State, DateTime.Now.AddDays(1));
                                Console.WriteLine("Old Status: " + oldStatus);
                                if (oldStatus != ccall.State)
                                {
                                    if (oldStatus != ccall.State)
                                    {
                                        if (ccall.ToCallerIdNumber.Length == 3 && ccall.FromCallerIdNumber.Length == 3)
                                        {
                                            string lastActivity1;
                                            lastActivity1 = "Internal: " + ccall.FromCallerIdName + " to " + ccall.ToCallerIdName;
                                            _cache.Set(ext + "-LastActivity", lastActivity1, DateTime.Now.AddDays(1));
                                            string lastActivityIdle1 = DateTime.Now.ToString("HH:mm") + " " + lastActivity1;
                                            _cache.Set(ext + "-LastActivityIdle", lastActivityIdle1, DateTime.Now.AddDays(1));
                                            _cache.Set(ext + "-LastActiveTime", DateTime.Now, DateTime.Now.AddDays(1));
                                        }
                                        else if (ccall.ToCallerIdNumber == ext)
                                        {
                                            string lastActivity1;
                                            if (ccall.FromCallerIdName == "Unknown")
                                            {
                                                lastActivity1 = "Incoming: " + ccall.FromCallerIdNumber;
                                            }
                                            else
                                            {
                                                lastActivity1 = "Incoming: " + ccall.FromCallerIdName + " " + ccall.FromCallerIdNumber;
                                            }
                                            _cache.Set(ext + "-LastActivity", lastActivity1, DateTime.Now.AddDays(1));
                                            string lastActivityIdle1 = DateTime.Now.ToString("HH:mm") + " " + lastActivity1;
                                            _cache.Set(ext + "-LastActivityIdle", lastActivityIdle1, DateTime.Now.AddDays(1));
                                            _cache.Set(ext + "-LastActiveTime", DateTime.Now, DateTime.Now.AddDays(1));
                                        }
                                        else
                                        {
                                            string lastActivity1;
                                            if (ccall.ToCallerIdName == "Unknown")
                                            {
                                                lastActivity1 = "Outgoing: " + ccall.ToCallerIdNumber;
                                            }
                                            else
                                            {
                                                lastActivity1 = "Outgoing: " + ccall.ToCallerIdName + " " + ccall.ToCallerIdNumber;
                                            }
                                            _cache.Set(ext + "-LastActivity", lastActivity1, DateTime.Now.AddDays(1));
                                            string lastActivityIdle1 = DateTime.Now.ToString("HH:mm") + " " + lastActivity1;
                                            _cache.Set(ext + "-LastActivityIdle", lastActivityIdle1, DateTime.Now.AddDays(1));
                                            _cache.Set(ext + "-LastActiveTime", DateTime.Now, DateTime.Now.AddDays(1));
                                        }
                                    }
                                }

                                _cache.Set(ext + "-Status", ccall.State);
                                double cTime0;
                                if (!_cache.TryGetValue(ext + "-CTime", out cTime0))
                                {
                                    _cache.Set(ext + "-CTime", updateSeconds, DateTime.Now.AddDays(1));
                                    _cache.Set(ext + "-ATime", updateSeconds, DateTime.Now.AddDays(1));
                                }
                                else
                                {
                                    double oldCTime;
                                    if (oldStatus == ccall.State)
                                    {
                                        double oldATime;
                                        _cache.TryGetValue(ext + "-ATime", out oldATime);
                                        _cache.Set(ext + "-ATime", tickDiff + oldATime, DateTime.Now.AddDays(1));
                                    }
                                    else
                                    {
                                        _cache.Set(ext + "-ATime", tickDiff, DateTime.Now.AddDays(1));
                                    }
                                    double oldLoggedInTime;
                                    _cache.TryGetValue(ext + "-LoggedInTime", out oldLoggedInTime);
                                    _cache.Set(ext + "-LoggedInTime", tickDiff + oldLoggedInTime, DateTime.Now.AddDays(1));
                                    _cache.TryGetValue(ext + "-CTime", out oldCTime);
                                    _cache.Set(ext + "-CTime", tickDiff + oldCTime, DateTime.Now.AddDays(1));
                                    double str;
                                    _cache.TryGetValue(ext + "-CTime", out str);
                                }
                            }
                            string status;
                            _cache.TryGetValue(ext + "-Status", out status);
                            extS.Status = status;
                            string subStatus;
                            if (_cache.TryGetValue(ext + "-SubStatus", out subStatus))
                            {
                                extS.SubStatus = subStatus;
                            }
                        }
                        string lastActivity;
                        _cache.TryGetValue(ext + "-LastActivity", out lastActivity);
                        extS.LastActivity = lastActivity;
                        List<string> sList0;
                        _cache.TryGetValue(ext + "-StatusList", out sList0);
                        extS.StatusList = sList0;
                        List<string> subList0;
                        _cache.TryGetValue(ext + "-SubStatusList", out subList0);
                        extS.SubStatusList = subList0;
                        List<double> dList0;
                        _cache.TryGetValue(ext + "-DurationList", out dList0);
                        extS.DurationList = dList0;
                        List<DateTime> loTList0;
                        _cache.TryGetValue(ext + "-LogoutTimesList", out loTList0);
                        extS.LogoutTimesList = loTList0;
                        string lastActivityIdle;
                        _cache.TryGetValue(ext + "-lastActivityIdle", out lastActivityIdle);
                        extS.LastActivityIdle = lastActivityIdle;
                        double iTime;
                        _cache.TryGetValue(ext + "-ITime", out iTime);
                        extS.ITime = iTime;
                        double aTime;
                        _cache.TryGetValue(ext + "-ATime", out aTime);
                        extS.ATime = aTime;
                        double cTime;
                        _cache.TryGetValue(ext + "-CTime", out cTime);
                        extS.CTime = cTime;
                        int tOCalls;
                        _cache.TryGetValue(ext + "-TotalOCalls", out tOCalls);
                        extS.TotalOCalls = tOCalls;
                        int tICalls;
                        _cache.TryGetValue(ext + "-TotalICalls", out tICalls);
                        extS.TotalICalls = tICalls;
                        double loggedInTime;
                        _cache.TryGetValue(ext + "-LoggedInTime", out loggedInTime);
                        extS.LoggedInTime = loggedInTime;
                        int totalCalls;
                        _cache.TryGetValue(ext + "-TotalCalls", out totalCalls);
                        extS.TotalCalls = totalCalls;
                        List<double> iTList;
                        _cache.TryGetValue(ext + "-IdleTimeList", out iTList);
                        extS.IdleTimeList = iTList;
                        this.extensionStatuses.Add(extS);
                    }
                }
                foreach (string ext in eList)
                {
                    _cache.TryGetValue(ext + "-IsCCall", out Boolean isActive);
                        ExtensionStatus extS0 = new ExtensionStatus();
                        extS0.ID = ext;
                        double iTime0;
                        _cache.Set(ext + "-IsCCall", false, DateTime.Now.AddDays(1));
                        string presence;
                        _cache.TryGetValue(ext + "-Presence", out presence);
                        string subPresence;
                        _cache.TryGetValue(ext + "-SubPresence", out subPresence);
                        string oldStatus;
                        _cache.TryGetValue(ext + "-Status", out oldStatus);
                        if (presence != "available")
                        {
                            updateLoggedOutStatus(ext, presence, subPresence, oldStatus, tickDiff);
                            continue;
                        }
                        else
                        {
                            if (oldStatus == "away" || oldStatus == "xa" || oldStatus == "dnd")
                            {
                                string lastActivity1;
                                lastActivity1 = DateTime.Now.ToShortTimeString() + " Logged In";
                                _cache.Set(ext + "-LastActivity", lastActivity1, DateTime.Now.AddDays(1));
                            }
                            string oStatus;
                            if (!_cache.TryGetValue(ext + "-Status", out oStatus))
                            {
                                _cache.Set(ext + "-Status", "idle", DateTime.Now.AddDays(1));
                                _cache.Set(ext + "-SubStatus", "", DateTime.Now.AddDays(1));
                                List<double> emptyList = new List<double>();
                                _cache.Set(ext + "-idleTimeList", emptyList, DateTime.Now.AddDays(1));
                            }
                            string oldStatus0;
                            _cache.TryGetValue(ext + "-Status", out oldStatus0);
                            _cache.Set(ext + "-Status", "idle", DateTime.Now.AddDays(1));
                            _cache.Set(ext + "-SubStatus", "", DateTime.Now.AddDays(1));
                            _cache.Set(ext + "-OldPBXID", "0", DateTime.Now.AddDays(1));
                            double oldLoggedInTime;
                            if (!_cache.TryGetValue(ext + "-LoggedInTime", out oldLoggedInTime))
                            {
                                _cache.Set(ext + "-LoggedInTime", updateSeconds, DateTime.Now.AddDays(1));
                            }
                            else
                            {
                                _cache.TryGetValue(ext + "-LoggedInTime", out oldLoggedInTime);
                                _cache.Set(ext + "-LoggedInTime", tickDiff + oldLoggedInTime, DateTime.Now.AddDays(1));
                            }
                            if (!_cache.TryGetValue(ext + "-ITime", out iTime0))
                            {
                                _cache.Set(ext + "-ITime", updateSeconds, DateTime.Now.AddDays(1));
                                _cache.Set(ext + "-ATime", updateSeconds, DateTime.Now.AddDays(1));
                            }
                            else
                            {
                                double oldITime;
                                if (oldStatus0 == "idle")
                                {
                                    double oldATime;
                                    _cache.TryGetValue(ext + "-ATime", out oldATime);
                                    _cache.Set(ext + "-ATime", tickDiff + oldATime, DateTime.Now.AddDays(1));
                                }
                                else
                                {
                                    _cache.Set(ext + "-ATime", tickDiff, DateTime.Now.AddDays(1));
                                    if (!_cache.TryGetValue(ext + "-IdleTimeList", out List<double> iTimeListx0))
                                    {
                                        _cache.Set(ext + "-IdleTimeList", new List<double>() { 0 }, DateTime.Now.AddDays(1));
                                    }
                                    _cache.TryGetValue(ext + "-IdleTimeList", out List<double> iTimeList);
                                    iTimeList.Add(0);

                                }
                                _cache.TryGetValue(ext + "-ITime", out oldITime);
                                _cache.Set(ext + "-ITime", tickDiff + oldITime, DateTime.Now.AddDays(1));
                                if (!_cache.TryGetValue(ext + "-IdleTimeList", out List<double> iTimeListx))
                                {
                                    _cache.Set(ext + "-IdleTimeList", new List<double>() { 0 }, DateTime.Now.AddDays(1));
                                }
                                _cache.TryGetValue(ext + "-IdleTimeList", out List<double> iTimeList0);
                                iTimeList0[iTimeList0.Count - 1] += tickDiff;
                                _cache.Set(ext + "-IdleTimeList", iTimeList0, DateTime.Now.AddDays(1));
                                extS0.ITime = tickDiff + oldITime;
                                double str;
                                _cache.TryGetValue(ext + "-ITime", out str);
                                double atime;
                                _cache.TryGetValue(ext + "-ATime", out atime);

                            }
                        }
                    string status;
                    ExtensionStatus extS = new ExtensionStatus();
                    extS.ID = ext;
                    _cache.TryGetValue(ext + "-Status", out status);
                    extS.Status = status;
                    string subStatus;
                    if (_cache.TryGetValue(ext + "-SubStatus", out subStatus))
                    {
                        extS.SubStatus = subStatus;
                    }
                    string lastActivity;
                    _cache.TryGetValue(ext + "-LastActivity", out lastActivity);
                    extS.LastActivity = lastActivity;
                    List<string> sList0;
                    _cache.TryGetValue(ext + "-StatusList", out sList0);
                    extS.StatusList = sList0;
                    List<string> subList0;
                    _cache.TryGetValue(ext + "-SubStatusList", out subList0);
                    extS.SubStatusList = subList0;
                    List<double> dList0;
                    _cache.TryGetValue(ext + "-DurationList", out dList0);
                    extS.DurationList = dList0;
                    List<DateTime> loTList0;
                    _cache.TryGetValue(ext + "-LogoutTimesList", out loTList0);
                    extS.LogoutTimesList = loTList0;
                    string lastActivityIdle;
                    _cache.TryGetValue(ext + "-lastActivityIdle", out lastActivityIdle);
                    extS.LastActivityIdle = lastActivityIdle;
                    double iTime;
                    _cache.TryGetValue(ext + "-ITime", out iTime);
                    extS.ITime = iTime;
                    double aTime;
                    _cache.TryGetValue(ext + "-ATime", out aTime);
                    extS.ATime = aTime;
                    double cTime;
                    _cache.TryGetValue(ext + "-CTime", out cTime);
                    extS.CTime = cTime;
                    int tOCalls;
                    _cache.TryGetValue(ext + "-TotalOCalls", out tOCalls);
                    extS.TotalOCalls = tOCalls;
                    int tICalls;
                    _cache.TryGetValue(ext + "-TotalICalls", out tICalls);
                    extS.TotalICalls = tICalls;
                    double loggedInTime;
                    _cache.TryGetValue(ext + "-LoggedInTime", out loggedInTime);
                    extS.LoggedInTime = loggedInTime;
                    int totalCalls;
                    _cache.TryGetValue(ext + "-TotalCalls", out totalCalls);
                    extS.TotalCalls = totalCalls;
                    List<double> iTList;
                    _cache.TryGetValue(ext + "-IdleTimeList", out iTList);
                    extS.IdleTimeList = iTList;
                    this.extensionStatuses.Add(extS);
                }
                var ccjson = JsonSerializer.Serialize(this.currentCalls);
                _cache.Set("CurrentCalls", ccjson, DateTime.Now.AddDays(1));
            }

            var esjson = JsonSerializer.Serialize(this.extensionStatuses);
            _cache.Set("ExtensionStatuses", esjson, DateTime.Now.AddDays(1));

            Console.WriteLine("Current Calls Saved");
            return "complete";
        }

        public ExtensionStatus updateLoggedOutStatus(string ext, string presence, string subPresence, string oldStatus, double tickDiff)
        {
            ExtensionStatus extS = new ExtensionStatus();
            bool startNewStatus;
            double statusDuration;
            double iTime;
            _cache.Set(ext + "-Status", presence, DateTime.Now.AddDays(1));
            _cache.Set(ext + "-SubStatus", subPresence, DateTime.Now.AddDays(1));
            double atime;
            _cache.TryGetValue(ext + "-ATime", out atime);
            if (atime > 1000000000)
            {
                _cache.Set(ext + "-ITime", 0, DateTime.Now.AddDays(1));
                _cache.Set(ext + "-ATime", updateSeconds, DateTime.Now.AddDays(1));
                _cache.Set(ext + "-LastActivity", "N/A", DateTime.Now.AddDays(1));
                List<double> emptyList = new List<double>();
                _cache.Set(ext + "-idleTimeList", emptyList, DateTime.Now.AddDays(1));
                startNewStatus = true;
                statusDuration = 0;
            }
            else
            {
                double oldATime;
                if (oldStatus == presence)
                {
                    _cache.TryGetValue(ext + "-ATime", out oldATime);
                    _cache.Set(ext + "-ATime", tickDiff + oldATime, DateTime.Now.AddDays(1));
                    startNewStatus = false;
                    statusDuration = tickDiff + oldATime;
                }
                else
                {
                    _cache.Set(ext + "-ATime", tickDiff, DateTime.Now.AddDays(1));
                    string lActivity = DateTime.Now.ToShortTimeString() + " Logged Out";
                    _cache.Set(ext + "-LastActivity", lActivity, DateTime.Now.AddDays(1));
                    startNewStatus = true;
                    statusDuration = tickDiff;
                }
            }
            extS.ID = ext;
            string status;
            _cache.TryGetValue(ext + "-Status", out status);
            extS.Status = status;
            string subStatus;
            if (_cache.TryGetValue(ext + "-SubStatus", out subStatus))
            {
                extS.SubStatus = subStatus;
            }
            if (!_cache.TryGetValue(ext + "-StatusList", out List<string> iTimeListx))
            {
                _cache.Set(ext + "-StatusList", new List<string>(), DateTime.Now.AddDays(1));
                _cache.Set(ext + "-DurationList", new List<double>(), DateTime.Now.AddDays(1));
                _cache.Set(ext + "-SubStatusList", new List<string>(), DateTime.Now.AddDays(1));
                _cache.Set(ext + "-LogoutTimesList", new List<DateTime>(), DateTime.Now.AddDays(1));
                startNewStatus = true;
            }
            if (startNewStatus)
            {
                _cache.TryGetValue(ext + "-StatusList", out List<string> sList);
                sList.Add(status);
                _cache.Set(ext + "-StatusList", sList, DateTime.Now.AddDays(1));
                _cache.TryGetValue(ext + "-SubStatusList", out List<string> subList);
                subList.Add(subStatus);
                _cache.Set(ext + "-SubStatusList", subList, DateTime.Now.AddDays(1));
                _cache.TryGetValue(ext + "-DurationList", out List<double> dList);
                dList.Add(statusDuration);
                _cache.Set(ext + "-DurationList", dList, DateTime.Now.AddDays(1));
                _cache.TryGetValue(ext + "-LogoutTimesList", out List<DateTime> loTList);
                loTList.Add(DateTime.Now);
                _cache.Set(ext + "-LogoutTimesList", loTList, DateTime.Now.AddDays(1));
            }
            else
            {
                _cache.TryGetValue(ext + "-StatusList", out List<string> sList);
                sList[sList.Count - 1] = status;
                _cache.Set(ext + "-StatusList", sList, DateTime.Now.AddDays(1));
                _cache.TryGetValue(ext + "-SubStatusList", out List<string> subList);
                subList[sList.Count - 1] = subStatus;
                _cache.Set(ext + "-SubStatusList", subList, DateTime.Now.AddDays(1));
                _cache.TryGetValue(ext + "-DurationList", out List<double> dList);
                dList[dList.Count - 1] = statusDuration;
                _cache.Set(ext + "-DurationList", dList, DateTime.Now.AddDays(1));
            }
            string lastActivity;
            _cache.TryGetValue(ext + "-LastActivity", out lastActivity);
            extS.LastActivity = lastActivity;
            List<string> sList0;
            _cache.TryGetValue(ext + "-StatusList", out sList0);
            extS.StatusList = sList0;
            List<string> subList0;
            _cache.TryGetValue(ext + "-SubStatusList", out subList0);
            extS.SubStatusList = subList0;
            List<double> dList0;
            _cache.TryGetValue(ext + "-DurationList", out dList0);
            extS.DurationList = dList0;
            List<DateTime> loTList0;
            _cache.TryGetValue(ext + "-LogoutTimesList", out loTList0);
            extS.LogoutTimesList = loTList0;
            string lastActivityIdle;
            _cache.TryGetValue(ext + "-lastActivityIdle", out lastActivityIdle);
            extS.LastActivityIdle = lastActivityIdle;
            _cache.TryGetValue(ext + "-ITime", out iTime);
            extS.ITime = iTime;
            double aTime;
            _cache.TryGetValue(ext + "-ATime", out aTime);
            extS.ATime = aTime;
            double cTime;
            _cache.TryGetValue(ext + "-CTime", out cTime);
            extS.CTime = cTime;
            double loggedInTime;
            _cache.TryGetValue(ext + "-LoggedInTime", out loggedInTime);
            extS.LoggedInTime = loggedInTime;
            int totalCalls;
            int tICalls;
            _cache.TryGetValue(ext + "-TotalICalls", out tICalls);
            extS.TotalICalls = tICalls;
            _cache.TryGetValue(ext + "-TotalCalls", out totalCalls);
            extS.TotalCalls = totalCalls;
            int tOCalls;
            _cache.TryGetValue(ext + "-TotalOCalls", out tOCalls);
            extS.TotalOCalls = tOCalls;
            this.extensionStatuses.Add(extS);
            List<double> iTList;
            _cache.TryGetValue(ext + "-IdleTimeList", out iTList);
            extS.IdleTimeList = iTList;
            return extS;
        }

        public string updateExtensionState(IMemoryCache cache)
        {
            return "updateExtensionState";
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

