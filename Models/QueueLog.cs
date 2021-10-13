using System;

namespace PBXDashboard_Dev.Models
{
  public class QueueLog
  {
    public long ID {get; set;}
    public string QueueAccountID {get; set;}
    public string QueueExtension {get; set;}
    public int EnterPosition {get; set;}
    public string MemberAccountID {get; set;}
    public int WaitTime {get; set;}
    public string QueueName {get; set;}
    public string MemberExtension {get; set;}
    public DateTime StartTime {get; set;}
    public string UniqueID {get; set;}
    public string MemberName {get; set;}
    public string TalkTime {get; set;}
    public string Type {get; set;}
    public int MemberMisses {get; set;}
  }
}