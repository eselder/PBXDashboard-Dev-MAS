using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PBXDashboard_Dev.Models
{
    public class Extension
    {
        public long Id {get; set;}
        
        public string Number {get; set;}
        public string Status {get; set;}
        public DateTime DateCreated {get; set;}
        public string LastName {get; set;}
        public string Display {get; set;}
        public Boolean HasAdditionalPhones {get; set;}
        public string TypeDisplay {get; set;}
        public string AccountID {get; set;}
        public string FirstName {get; set;}
        public int MemberCount {get; set;}
        public string Strategy {get; set;}
        public string CallQueueName {get; set;}
        public string Type {get; set;}
        
    }
}
