using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBXDashboard_Dev.Models
{
  public class Report
  {
    public long ReportID { get; set; }

    public string ReportString {get; set; }
    public DateTime TimeStamp {get; set;}
  }
}