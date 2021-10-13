import { Component, OnInit, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Call } from '../models/call.model';
import { Extension } from '../models/extension.model';
import { QueueLog } from '../models/queueLog';
import { Event } from "../models/event.model";

@Component({
  selector: 'app-create-queuereport',
  templateUrl: './create-queuereport.component.html',
  styleUrls: ['./create-queuereport.component.css']
})
export class CreateQueuereportComponent implements OnInit {

  public calls: Call[] = [];
  public extensions: Extension[] = [];
  public queueLogs: QueueLog[] = [];
  public queueCalls: Call[] = [];
  public submitted: boolean = false;
  public extIn: string = "";
  public reportCalled = false;
  public reportLogs: QueueLog[];
  public queueDisplay: string;
  public queueStrategy: string;
  public queueExts: string[] = [];
  public queueMembers: string[] = [];
  public extsAnswers: number[] = [];
  public answersPercent: string[] = [];
  public averageWaitTime: string = '';
  public totalWaitTime: number = 0;
  public longestWaitTime: number = 0;
  public longestWaitTimeCall: Call;
  public longestWaitTimeCallString: string = "";
  public str: string = "Call details will display here";
  public eventList: Event[] = [];
  public arr: string[] = [];
  public startDate: Date;
  public endDate: Date;
  public monday: boolean = true;
  public tuesday: boolean = true;
  public wednesday: boolean = true;
  public thursday: boolean = true;
  public friday: boolean = true;
  public saturday: boolean = false;
  public sunday: boolean = false;
  public startTime: string = "08:00:00";
  public endTime: string = "18:00:00";
  public filter_by_time = false;
  public mainExtensions: Extension[] = [];
  public filter_by_ext: boolean = false;
  public extsFilter: string[];
  public filter_by_outcome: boolean = false;
  public call_outcome: string;
  public filterByDate: boolean = false;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) 
  {
    http.get<Call[]>(baseUrl + 'api/calls').subscribe(result => {
      this.calls = result;
    }, error => console.error(error));
    http.get<Extension[]>(baseUrl + 'api/extensions').subscribe(result => {
      this.extensions = result;
    }, error => console.error(error));
    http.get<QueueLog[]>(baseUrl + 'api/queuelogs').subscribe(result => {
      this.queueLogs = result;
    }, error => console.error(error));
    
  }

  ngOnInit()
  {
  }

  onSubmit(f: NgForm)
  {
    this.submitted = true;
    this.extIn = f.value.queue;
    this.filterByDate = false;
    if (f.value.start_date)
    {
      this.startDate = new Date(f.value.start_date);
      this.filterByDate = true;

    }
    else
    {
      this.startDate = new Date(1970, 0, 1);
    }
    if (f.value.end_date)
    {
      this.endDate = new Date(f.value.end_date);
      this.filterByDate = true;
    }
    else
    {
      this.endDate = new Date(Date.now());
    }
    if (f.value.start_time)
    {
      this.startTime = f.value.start_time;
    }
    if (f.value.end_time)
    {
      this.endTime = f.value.end_time;
    }
    if (f.value.call_outcome && this.filter_by_outcome)
    {
      this.call_outcome = f.value.call_outcome;
    }
    this.extsFilter = [];
    this.extsFilter = f.value.exts;
    this.filterCallsByTime();
    this.filterCallsByDays();
    if (this.filter_by_time)
    {
      this.filterCallsByHour();
    }
    if (this.filter_by_ext)
    {
      this.filterByExt();
    }
    if (this.filter_by_outcome)
    {
      this.filterByOutcome();
    }
    this.createReport();
  }

  getListOfQueues()
  {
    let queues: Extension[] = [];
    this.extensions.forEach(ext => {
      if (ext.type == "call_queue")
      {
        queues.push(ext);
      }
    });
    return queues;
  }

  createReport()
  {
    this.reportCalled = true;
    this.reportLogs = [];
    this.queueExts = [];
    this.queueMembers = [];
    this.queueLogs.forEach(log => {
      if (log.queueExtension == this.extIn)
      {
        this.reportLogs.push(log);
        let call: Call = this.calls.find(call => call.uniqueID == log.uniqueID);
        call.waitTime = log.waitTime;
        let capType: string = log.type.charAt(0).toUpperCase() + log.type.slice(1);
        call.queueOutcome = capType;
        call.answeredBy = `${log.memberName} <${log.memberExtension}>`;
        this.queueCalls.push(call);
        
        if (!this.queueExts.includes(log.memberExtension) && (log.memberExtension))
        {
          this.queueExts.push(log.memberExtension);
          this.queueMembers.push(log.memberName);
        }
      }
    });
    this.getExtsAnswers();
    this.getQueueName();
    this.getAnswerPercents();
    this.getAverageWaitTime();
    this.getLongestWaitTime();
    this.getLongestWaitTimeCallString();
  }

  // FILTERS
  filterCallsByTime()
  {
    let tempLogs: QueueLog[] = this.queueLogs;
    this.queueLogs = [];
    this.endDate = new Date (this.endDate.setDate(this.endDate.getDate() + 1));
    tempLogs.forEach(call => {
      let date: Date = new Date(call.startTime);
      if ( date >= this.startDate && date <= this.endDate)
      {
        this.queueLogs.push(call);
      }
    });
  }
  filterCallsByDays()
  {
    let tempFilteredCalls: QueueLog[] = this.queueLogs;
    this.queueLogs = [];
    if (this.monday)
    {
      tempFilteredCalls.forEach(call => {
        let date = new Date(call.startTime)
        if (date.getDay() == 1)
        {
          this.queueLogs.push(call);
        }
      });
    }
    if (this.tuesday)
    {
      tempFilteredCalls.forEach(call => {
        let date = new Date(call.startTime)
        if (date.getDay() == 2)
        {
          this.queueLogs.push(call);
        }
      });
    }
    if (this.wednesday)
    {
      tempFilteredCalls.forEach(call => {
        let date = new Date(call.startTime)
        if (date.getDay() == 3)
        {
          this.queueLogs.push(call);
        }
      });
    }
    if (this.thursday)
    {
      tempFilteredCalls.forEach(call => {
        let date = new Date(call.startTime)
        if (date.getDay() == 4)
        {
          this.queueLogs.push(call);
        }
      });
    }
    if (this.friday)
    {
      tempFilteredCalls.forEach(call => {
        let date = new Date(call.startTime)
        if (date.getDay() == 5)
        {
          this.queueLogs.push(call);
        }
      });
    }
    if (this.saturday)
    {
      tempFilteredCalls.forEach(call => {
        let date = new Date(call.startTime)
        if (date.getDay() == 6)
        {
          this.queueLogs.push(call);
        }
      });
    }
    if (this.sunday)
    {
      tempFilteredCalls.forEach(call => {
        let date = new Date(call.startTime)
        if (date.getDay() == 7)
        {
          this.queueLogs.push(call);
        }
      });
    }
  }
  filterCallsByHour()
  {
    let tempFilteredCalls: QueueLog[] = this.queueLogs;
    this.queueLogs = [];
    let afterStart: boolean = false;
    let beforeEnd: boolean = false;
    let startHourNum: number = parseInt(this.startTime.substring(0, 2));
    let startMinNum: number = parseInt(this.startTime.substring(3, 5));
    let endHourNum: number = parseInt(this.endTime.substring(0, 2));
    let endMinNum: number = parseInt(this.endTime.substring(3, 5));
    tempFilteredCalls.forEach(call => {
      afterStart = false;
      beforeEnd = false;
      let date: Date = new Date(call.startTime)
      let callHour: number = date.getHours();
      let callMin: number = date.getMinutes();
      if (callHour > startHourNum)
      {
        afterStart = true;
      }
      else if (callHour == startHourNum)
      {
        if (callMin >= startMinNum)
        {
          afterStart = true;
        }
      }
      if (callHour < endHourNum)
      {
        beforeEnd = true;
      }
      else if (callHour == endHourNum)
      {
        if (callMin <= endMinNum)
        {
          beforeEnd = true;
        }
      }
      if (afterStart && beforeEnd)
      {
        this.queueLogs.push(call);
      }
    });
  }
  filterByExt()
  {
    let tempFilteredCalls: QueueLog[] = this.queueLogs;
    this.queueLogs = [];
    tempFilteredCalls.forEach(log => {
      if (this.extsFilter.includes(log.memberExtension))
      {
        this.queueLogs.push(log);
      }
    });
  }
  filterByOutcome()
  {
    let tempFilteredCalls: QueueLog[] = this.queueLogs;
    this.queueLogs = [];
    tempFilteredCalls.forEach(log => {
      if (this.call_outcome == log.type)
      {
        this.queueLogs.push(log);
      }
    });
    console.log(this.queueLogs);
  }

  getAbandonedTotal()
  {
    let total: number = 0;
    this.reportLogs.forEach(log => {
      if (log.type == "abandoned")
      total++;
    });
    return total;
  }

  getQueueName()
  {
    this.extensions.forEach(e => {
      if (e.number == this.extIn)
      {
        this.queueDisplay = e.display;
        this.queueStrategy = e.strategy;
      }
    });
  }

  getAbandonedPercent()
  {
    return ((this.getAbandonedTotal() / this.reportLogs.length)*100).toFixed(2);
  }

  getExtsAnswers()
  {
    for(let i = 0; i < this.queueExts.length; i++)
    {
      let ext: string = this.queueExts[i];
      this.extsAnswers[i] = 0;
      this.queueLogs.forEach(log => {
        if (log.memberExtension == ext)
        {
          this.extsAnswers[i] += 1;
        }
      });
    }
  }

  getAnswerPercents()
  {
    this.extsAnswers.forEach(ext => {
      this.answersPercent.push(((ext / this.reportLogs.length)*100).toFixed(2));
    });
  }

  getAverageWaitTime()
  {
    this.queueLogs.forEach(log => {
      this.totalWaitTime += log.waitTime;
    });
    this.averageWaitTime = (this.totalWaitTime / this.reportLogs.length).toFixed(0);
  }

  getLongestWaitTime()
  {
    this.longestWaitTime = 0;
    this.queueLogs.forEach(log => {
      if (log.waitTime > this.longestWaitTime)
      {
        this.longestWaitTime = log.waitTime;
        this.longestWaitTimeCall = this.calls.find(call => call.uniqueID == log.uniqueID);
      }
    });
  }

  getLongestWaitTimeCallString()
  {
    this.longestWaitTimeCallString = "";
    this.longestWaitTimeCallString += "Longest Wait Time: ";
    this.longestWaitTimeCallString += this.longestWaitTime;
    this.longestWaitTimeCallString += " sec - Call: ";
    this.longestWaitTimeCallString += this.longestWaitTimeCall.pbxid;
    this.longestWaitTimeCallString += " ";
    this.longestWaitTimeCallString += (new Date(this.longestWaitTimeCall.startTime)).toLocaleString();
  }

  getLocalString(time: string)
  {
    let date = new Date(time);
    return date.toLocaleString();
  }

  getListOfExtensions()
  {
    this.mainExtensions = [];
    this.extensions.forEach(e => {
      if (e.type == "sip")
      {
        this.mainExtensions.push(e);
      }
    });
    return this.mainExtensions.sort((a, b) => (a.number > b.number) ? 1 : -1);
  }

  sideClick(call: Call)
  {
    this.eventList = call.events;
    this.str = `Call ${call.pbxid} events:`
    this.arr = [];
    call.events.forEach(event => {
      this.arr.push(`${(new Date(event.startTime)).toLocaleString()}-${event.display}` );
    });
  }

  getAnsweredBy(call: Call)
  {
    if (call.answeredBy.includes("undefined"))
    {
      return "(Not Applicable)";
    }
    else
    {
      return call.answeredBy;
    }
  }

}
