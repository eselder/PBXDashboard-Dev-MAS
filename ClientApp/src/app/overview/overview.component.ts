import { Component, OnInit, Inject } from '@angular/core';
import { Call } from "../models/call.model";
import { Extension } from "../models/extension.model";
import { Event } from "../models/event.model";
import { HttpClient } from '@angular/common/http';
import { Queue } from '../models/queue.model';
import { QueueLog } from '../models/queueLog';
import { TalkTimeRecord } from '../models/talkTimeRecord';

@Component({
  selector: 'app-overview',
  templateUrl: './overview.component.html',
  styleUrls: ['./overview.component.css']
})

export class OverviewComponent {
  public calls: Call[];
  public scopedCalls: Call[];
  public extensions: Extension[];
  public mainExtensions: Extension[] = [];
  public callQueues: Extension[] = [];
  public ivrs: Extension[] = [];
  public queues: Queue[] = [];
  public parkingLots: Extension[] = [];
  public queueLogs: QueueLog[];
  public numberOfQueueLogs = 0;
  public talkTimeRecords: TalkTimeRecord[];
  public scopedTalkTimeRecords: TalkTimeRecord[];
  public timePeriod: string = "today";
  public today: Date;
  public lastBusinessDay: Date;
  public daysThisWeek: Date[];
  public daysLastWeek: Date[];
  public daysThisMonth: Date[];
  public daysLastMonth: Date[];
  public daysThisQuarter: Date[];
  public daysLastQuarter: Date[];
  public daysInScope: Date[];
  public displayIdleTime: boolean = true;
  public numberOfDays: number = 1;
  public lastCall: Call;
  public lastEvent: Event;
  public noLastCall: boolean;
  public eventList: Event[];
  public str: string;
  public arr: string[];
  public numberOfQueueCallsList: number[];
  public avgQueueWaitTimeList: number[];
  public longestWaitTimeList: number[];
  public longestWaitTimeCallList: Call[];
  public abandonedQueueCallsList: Call[][];
  public abandonedQueueCallPctList: string[];
  public avgQueueParkTimeList: number[];
  public scopedQueueLogs: QueueLog[];

  earliestCallTime: Date = new Date();
  latestCallTime: Date = new Date(1970, 1, 1);

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<Call[]>(baseUrl + 'api/calls').subscribe(result => {
      this.calls = result;
      console.log("PING");
    }, error => console.error(error));
    http.get<Extension[]>(baseUrl + 'api/extensions').subscribe(result => {
      this.extensions = result;
    }, error => console.error(error));
    http.get<Queue[]>(baseUrl + 'api/queues').subscribe(result => {
      this.queues = result;
    }, error => console.error(error));
    http.get<QueueLog[]>(baseUrl + 'api/queuelogs').subscribe(result => {
      this.queueLogs = result;
    }, error => console.error(error));
    http.get<TalkTimeRecord[]>(baseUrl + 'api/talktimerecords').subscribe(result => {
      this.talkTimeRecords = result;
    }, error => console.error(error));
    this.today = new Date(Date.now());
    this.getLastBusinessDay();
    this.getDaysThisWeek();
    this.getDaysLastWeek();
    this.getDaysThisMonth();
    this.getDaysLastMonth();
    this.getDaysThisQuarter();
    this.getDaysLastQuarter();
  }
  
  ngOnInit()
  {

  }



  alert()
  {
    window.alert("Expand")
  }

  // **Overview methods**
  changeTimePeriod()
  {
    this.getTalkTimeRecords();
    this.getScopedQueueLogs();
    this.getScopedCalls();
    this.getDaysInScope();
  }

  getListOfExtensions()
  {
    let mainExtensions: Extension[] = [];
    this.extensions.forEach(e => {
      if (e.type == "sip")
      {
        mainExtensions.push(e);
      }
    });
    return mainExtensions.sort((a, b) => (a.number > b.number) ? 1 : -1);
  }
  getNumberOfEvents()
  {
    let total: number = 0;
    for (const call of this.calls)
    {
      let num: number = call.events.length;
      total += num;
    }
    return total;
  }
  getNumberOfQueueLogs()
  {
    return this.queueLogs.length;
  }
  getQueueMembers(accountId)
  {
    let total;
    total = this.queues[0].members;
    return total;
  }

  getEarliestCall()
  {
    for (const call of this.calls)
    {
      let date: Date = new Date(call.startTime);
      if (date < this.earliestCallTime)
      {
        this.earliestCallTime = date;
      }
    }
    return this.earliestCallTime;
  }

  getLatestCall()
  {
    for (const call of this.calls)
    {
      let date: Date = new Date(call.startTime);
      if (date > this.latestCallTime)
      {
        this.latestCallTime = date;
      }
    }
    return this.latestCallTime;
  }

  getNumberOfMainPhones()
  {
    let total: number = 0;
    this.mainExtensions = [];
    for (const ext of this.extensions)
    {
      if (ext.type === "sip")
      {
        this.mainExtensions.push(ext);
        total = total + 1;
      }
    }
    return total;
  }

  getNumberOfCallQueues()
  {
    let total: number = 0;
    this.callQueues = [];
    for (const ext of this.extensions)
    {
      if (ext.type === "call_queue")
      {
        total = total + 1;
        this.callQueues.push(ext);
      }
    }
    return total;
  }

  getNumberOfIVRs()
  {
    let total: number = 0;
    this.ivrs = [];
    for (const ext of this.extensions)
    {
      if (ext.type === "ivr")
      {
        total = total + 1;
        this.ivrs.push(ext);
      }
    }
    return total;
  }

  getTalkTimeRecords()
  {
    this.scopedTalkTimeRecords = [];
    if (this.timePeriod == "today")
    {
      this.today = new Date(Date.now());
      this.talkTimeRecords.forEach(record => {
        if (this.isSameDateAs(new Date(record.date), this.today))
        {
          this.scopedTalkTimeRecords.push(record);
        }
      });
    }
    else if(this.timePeriod == "yesterday")
    {
      this.talkTimeRecords.forEach(record => {
        if (this.isSameDateAs(new Date(record.date), this.lastBusinessDay))
        {
          this.scopedTalkTimeRecords.push(record);
        }
      });
    }
    else
    {
      this.daysInScope.forEach(day => {
        this.talkTimeRecords.forEach(record => {
          if (this.isSameDateAs(new Date(record.date), day))
          {
            this.scopedTalkTimeRecords.push(record);
          }
        });
      });
    }
  }

  getScopedCalls()
  {
    this.scopedCalls = [];
    if (this.timePeriod == "today")
    {
      this.calls.forEach(call => {
        if (this.isSameDateAs(new Date(call.startTime), this.today))
        {

          this.scopedCalls.push(call);
        }
      });
    }
    else if (this.timePeriod == "yesterday")
    {
      this.calls.forEach(call => {
        if (this.isSameDateAs(new Date(call.startTime), this.lastBusinessDay))
        {

          this.scopedCalls.push(call);
        }
      });
    }
    else 
    {
      this.daysInScope.forEach(day => {
        this.calls.forEach(call => {
          if (this.isSameDateAs(new Date(call.startTime), day))
          {
  
            this.scopedCalls.push(call);
          }
        });
      });
    }
  }

  getNumberOfCalls(ext: string)
  {
    if (!this.scopedTalkTimeRecords || this.scopedTalkTimeRecords == [])
    {
      this.getTalkTimeRecords();
    }
    let num: number = 0;
    this.scopedTalkTimeRecords.forEach(record => {
      if (record.accountID == ext)
      {
        num += record.totalCalls;
      }
    });
    return num;
  }
  getNumberOfICalls(ext: string)
  {
    if (!this.scopedTalkTimeRecords || this.scopedTalkTimeRecords == [])
    {
      this.getTalkTimeRecords();
    }
    let num: number = 0;
    this.scopedTalkTimeRecords.forEach(record => {
      if (record.accountID == ext)
      {
        num += record.totalIncomingCalls;
      }
    });
    return num;
  }
  getNumberOfOCalls(ext: string)
  {
    if (!this.scopedTalkTimeRecords || this.scopedTalkTimeRecords == [])
    {
      this.getTalkTimeRecords();
    }
    let num: number = 0;
    this.scopedTalkTimeRecords.forEach(record => {
      if (record.accountID == ext)
      {
        num += record.totalOutgoingCalls;
      }
    });
    return num;
  }

  getTalkTime(ext: string)
  {
    if (!this.scopedTalkTimeRecords || this.scopedTalkTimeRecords == [])
    {
      this.getTalkTimeRecords();
    }
    let time: number = 0;
    this.scopedTalkTimeRecords.forEach(record => {
      if (record.accountID == ext)
      {
        time += record.talkingDuration;
      }
    });
    return this.hrsMinSec(time);
  }

  getIdleTime(ext: string)
  {
    if (!this.scopedTalkTimeRecords || this.scopedTalkTimeRecords == [])
    {
      this.getTalkTimeRecords();
    }
    let time: number = 0;
    this.scopedTalkTimeRecords.forEach(record => {
      if (record.accountID == ext)
      {
        time += record.talkingDuration;
      }
    });
    if (this.timePeriod == "today")
    {
      let begOfShift: number = (new Date (new Date (new Date(Date.now()).setHours(9)).setMinutes(0)).setSeconds(0));
      let now: number = (Date.now());
      let deff = now - begOfShift;
      if (deff > 28800000)
      {
        deff = 28800000;
      }
      var idleTime: number = Math.floor(deff / 1000) - time
    }
    else if(this.timePeriod == "yesterday")
    {
      idleTime = Math.floor(28800000 / 1000) - time
    }
    else if(this.timePeriod == "thisweek")
    {
      let begOfShift: number = (new Date (new Date (new Date(Date.now()).setHours(9)).setMinutes(0)).setSeconds(0));
      let now: number = (Date.now());
      let deff = now - begOfShift;
      if (deff > 28800000)
      {
        deff = 28800000;
      }
      let idleTime0 = Math.floor(deff / 1000) - time
      let idleTimeEarlier: number = (this.daysThisWeek.length-1)*28800;
      idleTime0 += idleTimeEarlier;
      if (idleTime0 > 144000)
      {
        idleTime0 = 144000;
      }
      return this.hrsMinSec(idleTime0);
    }
    else if (this.timePeriod == "lastweek")
    {
      idleTime = 140000 - time;
      return this.hrsMinSec(idleTime);
    }

    return this.hrsMinSec(idleTime);
  }

  getLastCall(ext: string)
  {
    let extNum: string;
    let ext0: Extension;
    this.extensions.forEach(extension => {
      if (extension.accountID == ext)
      {
        extNum = extension.number;
        ext0 = extension;
      }
    });
    let lastEventTime: Date = new Date('January 1, 1970 00:0:00');
    let lastCall: Call;
    let lastEvent: Event;
    extNum = `<${extNum}>`
    let extNum0: string = `(${extNum})`
    this.calls.forEach(call => {
      call.events.forEach(event => {
        let time: Date = new Date(event.startTime)
        if ((time > lastEventTime) && (event.display.includes(extNum || extNum0)))
        {
          lastEventTime = new Date(event.startTime);
          lastCall = call;
          lastEvent = event;
          this.noLastCall = false;
        }
      });
    });
    if (!lastCall)
    {
      this.noLastCall = true;
      return;
    }
    ext0.lastCall = lastCall;
    this.lastCall = lastCall;
    this.lastEvent = lastEvent;
  }

  getTimeSinceLastCall(ext: string)
  {
    this.getLastCall(ext);
    if(this.noLastCall == true)
    {
      return "(N/A)";
    }
    else
    {
      let now: number = Date.now();
      let diff: number = now - (new Date(this.lastEvent.startTime)).getTime();
      diff = Math.floor(diff / 1000);
      return this.hrsMinSec(diff);
    }
  }

  getScopedQueueLogs()
  {
    this.scopedQueueLogs = [];
    if (this.timePeriod == "today")
    {
      this.queueLogs.forEach(log => {
        if (this.isSameDateAs(new Date(log.startTime), this.today))
        {
          this.scopedQueueLogs.push(log);
        }
      });
    }
    else if (this.timePeriod == "yesterday")
    {
      this.queueLogs.forEach(log => {
        if (this.isSameDateAs(new Date(log.startTime), this.lastBusinessDay))
        {
          this.scopedQueueLogs.push(log);
        }
      });
    }
    else
    {
      this.daysInScope.forEach(day => {
        this.queueLogs.forEach(log => {
          if (this.isSameDateAs(new Date(log.startTime), day))
          {
            this.scopedQueueLogs.push(log);
          }
        });
      });
    }
  }

  getQueueCallsAnswered()
  {
    if (!this.scopedQueueLogs || this.scopedQueueLogs == [])
    {
      this.getScopedQueueLogs();
    }

    this.mainExtensions.forEach(ext => {
      ext.percentAnswersList = [];
      ext.totalAnswersList = [];
    });
    this.callQueues.forEach(queue => {
      let totalQueueAnswers: number = 0;
      let specLogs: QueueLog[] = [];
      this.scopedQueueLogs.forEach(log => {
        if (log.queueAccountID == queue.accountID)
        {
          totalQueueAnswers++;
          specLogs.push(log);
        }
      });
      this.mainExtensions.forEach(ext => {
        let totalExtensionQAnswers: number = 0;
        specLogs.forEach(log => {
          if (log.memberAccountID == ext.accountID)
          {
            totalExtensionQAnswers++;
          }
        });
        ext.totalAnswersList.push(totalExtensionQAnswers);
        let percent: string = "0";
        percent = ((totalExtensionQAnswers / totalQueueAnswers)*100).toFixed(2);
        if (percent == "NaN")
        {
          percent = "0";
        }
        ext.percentAnswersList.push(percent);
      });
    });
    return this.callQueues;
  }

  getQueueCallInfo()
  {
    if (!this.scopedQueueLogs || this.scopedQueueLogs == [])
    {
      this.getScopedQueueLogs();
    }
    this.numberOfQueueCallsList = [];
    this.avgQueueWaitTimeList = [];
    this.longestWaitTimeList = [];
    this.longestWaitTimeCallList = [];
    this.abandonedQueueCallsList = [];
    this.abandonedQueueCallPctList = [];
    this.callQueues.forEach(queue => {
      let total: number = 0;
      let totalWaitTime: number = 0;
      let longestWaitTime: number = 0;
      let longestCall: Call;
      let longestLog: QueueLog;
      let abandonedCalls: Call[] = [];
      this.scopedQueueLogs.forEach(log => {
        if (queue.accountID == log.queueAccountID)
        {
          total++;
          totalWaitTime += log.waitTime;
          if (log.waitTime > longestWaitTime)
          {
            longestWaitTime = log.waitTime;
            longestLog = log;
          }
          if (log.type == "abandoned")
          {
            this.scopedCalls.forEach(call => {
              if (call.uniqueID == log.uniqueID)
              {
                abandonedCalls.push(call);
              }
            });
          }
        }
      });
      this.scopedCalls.forEach(call => {
        if (longestLog && call.uniqueID == longestLog.uniqueID)
        {
          this.longestWaitTimeCallList.push(call);
        }
      });
      this.numberOfQueueCallsList.push(total);
      let avgWaitTime: number;
      avgWaitTime = (totalWaitTime / total);
      if (isNaN(avgWaitTime))
      {
        avgWaitTime = 0;
      }
      this.avgQueueWaitTimeList.push(avgWaitTime);
      this.longestWaitTimeList.push(longestWaitTime);
      this.abandonedQueueCallsList.push(abandonedCalls);
      let abandonedCallsNum: number = abandonedCalls.length;
      let abandonedCallsPct: string = ((abandonedCallsNum / total)*100).toFixed(2);
      if (abandonedCallsPct == "NaN")
      {
        abandonedCallsPct = "0";
      }
      this.abandonedQueueCallPctList.push(abandonedCallsPct);
    });
    return this.callQueues;
  }

  getLastBusinessDay()
  {
    let date: Date = new Date(Date.now());
    while(true)
    {
      date = new Date(date.setDate(date.getDate() - 1));
      if ([1,2,3,4,5].includes(date.getDay()))
      {
        this.lastBusinessDay = date;
        return;
      }
    }
  }

  getDaysThisWeek()
  {
    let dates: Date[] = [];
    let date: Date = new Date(Date.now());
    while(true)
    {
      if ([1,2,3,4,5,6].includes(date.getDay()))
      {
        let date0: Date = new Date(date.getTime());
        dates.push(date0);
        date = new Date(date.setDate(date.getDate() - 1));
      }
      else
      {
        this.daysThisWeek = dates;
        return;
      }
    }
  }

  getDaysLastWeek()
  {
    let dates: Date[] = [];
    let date: Date = new Date(Date.now());
    while(true)
    {
      if (date.getDay() == 0)
      {
        date = new Date(date.setDate(date.getDate() - 1));
        break;
      }
      else
      {
        date = new Date(date.setDate(date.getDate() - 1));
      }
    }
    while(true)
    {
      if ([1,2,3,4,5,6].includes(date.getDay()))
      {
        let date0: Date = new Date(date.getTime());
        dates.push(date0);
        date = new Date(date.setDate(date.getDate() - 1));
      }
      else
      {
        this.daysLastWeek = dates;
        return;
      }
    }
  }

  getDaysThisMonth()
  {
    let dates: Date[] = [];
    let date: Date = new Date(Date.now());
    let month: number = date.getMonth();
    while(true)
    {
      if (date.getMonth() != month)
      {
        break;
      }
      else
      {
        let date0: Date = new Date(date.getTime());
        dates.push(date0);
        date = new Date(date.setDate(date.getDate() - 1));
      }
    }
    this.daysThisMonth = dates;
  }

  getDaysLastMonth()
  {
    let dates: Date[] = [];
    let date: Date = new Date(Date.now());
    let month: number = date.getMonth();
    while(true)
    {
      if (date.getMonth() != month)
      {
        month = date.getMonth();
        break;
      }
      else
      {
        date = new Date(date.setDate(date.getDate() - 1));
      }
    }
    while(true)
    {
      if (date.getMonth() != month)
      {
        break;
      }
      else
      {
        let date0: Date = new Date(date.getTime());
        dates.push(date0);
        date = new Date(date.setDate(date.getDate() - 1));
      }
    }
    this.daysLastMonth = dates;
  }

  getDaysThisQuarter()
  {
    let dates: Date[] = [];
    let date: Date = new Date(Date.now());
    while(true)
    {
      if (date.getDate() == 1 && (date.getMonth() == 0 || date.getMonth() == 3 || date.getMonth() == 6 || date.getMonth() == 9))
      {
        let date0: Date = new Date(date.getTime());
        dates.push(date0);
        date = new Date(date.setDate(date.getDate() - 1));
        break;
      }
      else
      {
        let date0: Date = new Date(date.getTime());
        dates.push(date0);
        date = new Date(date.setDate(date.getDate() - 1));
      }
    }
    this.daysThisQuarter = dates;
  }

  getDaysLastQuarter()
  {
    let dates: Date[] = [];
    let date: Date = new Date(Date.now());
    while(true)
    {
      if (date.getDate() == 1 && (date.getMonth() == 0 || date.getMonth() == 3 || date.getMonth() == 6 || date.getMonth() == 9))
      {
        date = new Date(date.setDate(date.getDate() - 1));
        break;
      }
      else
      {
        date = new Date(date.setDate(date.getDate() - 1));
      }
    }
    while(true)
    {
      if (date.getDate() == 1 && (date.getMonth() == 0 || date.getMonth() == 3 || date.getMonth() == 6 || date.getMonth() == 9))
      {
        let date0: Date = new Date(date.getTime());
        dates.push(date0);
        date = new Date(date.setDate(date.getDate() - 1));
        break;
      }
      else
      {
        let date0: Date = new Date(date.getTime());
        dates.push(date0);
        date = new Date(date.setDate(date.getDate() - 1));
      }
    }
    this.daysLastQuarter = dates;
  }

  getDaysInScope()
  {
    if (this.timePeriod == "thisweek")
    {
      this.displayIdleTime = true;
      this.daysInScope = this.daysThisWeek;
    }
    else if (this.timePeriod == "lastweek")
    {
      this.displayIdleTime = true;
      this.daysInScope = this.daysLastWeek;
    }
    else if (this.timePeriod == "thismonth")
    {
      this.daysInScope = this.daysThisMonth;
      this.displayIdleTime = false;
    }
    else if (this.timePeriod == "lastmonth")
    {
      this.daysInScope = this.daysLastMonth;
      this.displayIdleTime = false;
    }
    else if (this.timePeriod == "thisquarter")
    {
      this.daysInScope = this.daysThisQuarter;
      this.displayIdleTime = false;
    }
    else if (this.timePeriod == "lastquarter")
    {
      this.daysInScope = this.daysLastQuarter;
      this.displayIdleTime = false;
    }
  }

  sideClick(ext: Extension)
  {
    let extID: string = ext.accountID;
    let call: Call;
    this.extensions.forEach(extension => {
      if (extension.accountID == extID)
      {
        call = extension.lastCall;
      }
    });
    this.eventList = call.events;
    this.str = `Call ${call.pbxid} events:`
    this.arr = [];
    let i: number = 0;

    call.events.forEach(event => {
      i++;
      this.arr.push(`${(new Date(event.startTime)).toLocaleString()}-${event.display}` );
    });
  }

  longestClick(call: Call)
  {
    this.eventList = call.events;
    this.str = `Call ${call.pbxid} events:`
    this.arr = [];
    call.events.forEach(event => {
      this.arr.push(`${(new Date(event.startTime)).toLocaleString()}-${event.display}` );
    });
  }

  abandonedClick(calls: Call[], queue: Extension)
  {
    this.str = `Abandoned Calls for <${queue.display}>`;
    this.arr = [];
    calls.forEach(call => {
      let dateString: string = (new Date(call.startTime)).toLocaleString();
      this.arr.push(`From: ${call.from} - ${call.fromName} on ${dateString}`);
    });
  }

  setScopeButton(scope: string)
  {
    this.timePeriod = scope;
    this.scopedTalkTimeRecords = [];
    this.displayIdleTime = true;
    this.getDaysInScope();
    this.changeTimePeriod();
  }

  isSameDateAs(pDate: Date, tDate: Date) {
    return (
      tDate.getFullYear() === pDate.getFullYear() &&
      tDate.getMonth() === pDate.getMonth() &&
      tDate.getDate() === pDate.getDate()
    );
  }

  hrsMinSec(sec: number)
  {
    let hh = Math.floor(sec / 60 / 60);
    if (hh.toString().length == 1)
    {
      var hhs: string = "0"+hh;
    }
    else
    {
      hhs = hh.toString();
    }
    sec -= hh * 60 * 60;
    let mm = Math.floor(sec / 60);
    if (mm.toString().length == 1)
    {
      var mms: string = "0"+mm;
    }
    else
    {
      mms = mm.toString();
    }
    sec -= mm * 60;
    let ss = Math.floor(sec);
    if (ss.toString().length == 1)
    {
      var sss: string = "0"+ss;
    }
    else
    {
      sss = ss.toString();
    }
    return `${hhs}:${mms}:${sss}`;
  }

}
