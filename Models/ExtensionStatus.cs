using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBXDashboard_Dev.Models
{
    public class ExtensionStatus
    {
        public string ID { get; set; }
        public string Status { get; set; }
        public string LastActivity { get; set; }
        public string LastActivityIdle { get; set; }
        public double ITime { get; set; }
        public double ATime { get; set; }
        public double CTime { get; set; }
        public int TotalCalls { get; set; }
        public int TotalICalls { get; set; }
        public int TotalOCalls { get; set; }
        public double LoggedInTime { get; set; }
        public string SubStatus { get; set; }
        [NotMapped]
        public List<string> StatusList { get; set; }
        [NotMapped]
        public List<string> SubStatusList { get; set; }
        [NotMapped]
        public List<double> DurationList { get; set; }
        [NotMapped]
        public List<DateTime> LogoutTimesList { get; set; }
        [NotMapped]
        public List<double> IdleTimeList { get; set; }
    }
}