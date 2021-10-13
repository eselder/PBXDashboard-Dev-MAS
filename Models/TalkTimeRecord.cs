using System;

namespace PBXDashboard_Dev.Models
{
  public class TalkTimeRecord
  {
    public long ID {get; set;}
    public string AccountID {get; set;}
    public DateTime Date {get; set;}
    public int TalkingDuration {get; set;}
    public int TotalOutgoingCalls {get; set;}
    public int TotalIncomingCalls {get; set;}
    public int TotalCalls {get; set;}
    public int CallDuration {get; set;}
  }
}