using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBXDashboard_Dev.Models
{
  public class Call
  {
    public long CallID { get; set; }

    public long PBXID { get; set; }
    public string UniqueID {get; set; }
    public string Origination {get; set; }
    public DateTime StartTime {get; set; }
    public string From {get; set;}
    public int FromAccountID {get; set;}
    public int ToAccountID {get; set;}
    public string FromName {get; set;}
    public string FromNumber {get; set;}
    public string To {get; set;}
    public string ToNumber {get; set;}
    public int TotalDuration {get; set;}
    public int TalkDuration {get; set;}
    public int CdrCallID {get; set; }

    public List<Event> Events {get; set;}
    public List<QueueLog> QueueLogs {get; set;}
  }
}