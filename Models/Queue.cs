using System.Collections.Generic;

namespace PBXDashboard_Dev.Models
{
  public class Queue
  {
    public long ID { get; set; }
    public string Extension { get; set; }
    public string Strategy { get; set; }
    public string AccountID {get; set;}
    public List<Extension> Members { get; set; }
    public List<Call> WaiteringCalls { get; set; }
  }
}