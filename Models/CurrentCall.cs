using System;

namespace PBXDashboard_Dev.Models
{
  public class CurrentCall
  {
    public long ID { get; set;}
    public DateTime StartTime {get; set;}
    public int Duration {get; set;}
    public string State {get; set;}
    public string Format {get; set;}
    public string FromCallerIdName {get; set;}
    public string ToCallerIdName {get; set;}
    public string FromCallerIdNumber {get; set;}
    public string ToCallerIdNumber {get; set;}
    public string PBXID {get; set;}
  }
}