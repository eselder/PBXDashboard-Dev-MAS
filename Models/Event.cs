using System.Collections.Generic;
using System;

namespace PBXDashboard_Dev.Models
{
  public class Event
  {
    public long EventID { get; set; }

    public DateTime StartTime {get; set;}
    public string Type {get; set;}
    public string Display {get; set;}

    public Call Call {get; set;}
  }
}