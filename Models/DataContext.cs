using Microsoft.EntityFrameworkCore;

namespace PBXDashboard_Dev.Models
{
  public class DataContext : DbContext
  {
    public DataContext(DbContextOptions<DataContext> opts) : base(opts) { }
    public DbSet<Call> Calls {get; set;}
    public DbSet<Event> Events {get; set;}
    public DbSet<Report> Reports {get; set;}
    public DbSet<Extension> Extensions {get; set;}
    public DbSet<Queue> Queues { get; set; }
    public DbSet<QueueLog> QueueLogs {get; set;}
    public DbSet<CurrentCall> CurrentCalls {get; set;}
    public DbSet<TalkTimeRecord> TalkTimeRecords {get; set;}
  }
}