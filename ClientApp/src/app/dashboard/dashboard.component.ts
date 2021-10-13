import { Component, OnInit, Inject, ViewChild } from '@angular/core';
import { Call } from "../models/call.model";
import { Extension } from "../models/extension.model";
import { Event } from "../models/event.model";
import { HttpClient } from '@angular/common/http';
import { Queue } from '../models/queue.model';
import { QueueLog } from '../models/queueLog';
import { TalkTimeRecord } from '../models/talkTimeRecord';
import { CurrentCall } from '../models/currentCall.model';
import { Subscription, Observable, of, timer } from 'rxjs';
import { ExtensionStatus } from '../models/extensionStatus';
import { CSVRecord1 } from '../models/CSVModel1';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})

export class DashboardComponent {

  public loggedIn: Boolean;
  public calls: Call[];
  public scopedCalls: Call[];
  public extensions: Extension[];
  public extensionStatuses: ExtensionStatus[];
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
  public currentCalls: CurrentCall[];
  public baseUrl: string;
  public http: HttpClient;
  public subscription: Subscription;
  public totalICalls: number;
  public subscribeTimer: number;

  public showTable : boolean = false;

  earliestCallTime: Date = new Date();
  latestCallTime: Date = new Date(1970, 1, 1);

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
    this.http = http;
    http.get<Call[]>(baseUrl + 'api/calls').subscribe(result => {
      this.calls = result;
    }, error => console.error(error));
    http.get<Extension[]>(baseUrl + 'api/extensions').subscribe(result => {
      this.extensions = result;
    }, error => console.error(error));
    http.get<ExtensionStatus[]>(baseUrl + 'api/extensionstatuses').subscribe(result => {
      this.extensionStatuses = result;
    }, error => console.error(error));
    /* http.get<QueueLog[]>(baseUrl + 'api/queuelogs').subscribe(result => {
      this.queueLogs = result;
    }, error => console.error(error));
    http.get<TalkTimeRecord[]>(baseUrl + 'api/talktimerecords').subscribe(result => {
      this.talkTimeRecords = result;
    }, error => console.error(error));
    http.get<CurrentCall[]>(baseUrl + 'api/currentCalls').subscribe(result => {
      this.currentCalls = result;
    });
    this.today = new Date(Date.now());
    this.getLastBusinessDay();
    this.getDaysThisWeek();
    this.getDaysLastWeek();
    this.getDaysThisMonth();
    this.getDaysLastMonth();
    this.getDaysThisQuarter();
    this.getDaysLastQuarter(); */
  }

  dataRefresher: any;

  ngOnInit() {
    this.refreshData();
    this.subscription = this.getItems().subscribe
      (

      );
    let ticks: number = 0;
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  getItems(): Observable<any> {
    return of(this.refreshData());
  }

  getData(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<CurrentCall[]>(baseUrl + 'api/currentCalls').subscribe(result => {
      this.currentCalls = result;
    });
    http.get<ExtensionStatus[]>(baseUrl + 'api/extensionstatuses').subscribe(result => {
      this.extensionStatuses = result;
    });

    this.getListOfExtensions();
    this.tableTimer();
    /*     this.mainExtensions.forEach(ext => {
          this.getCallerStatus(ext.lastCall);
          this.getLastActivity(ext.lastCall);
          this.getAverageIdleTime(ext.accountID);
          this.getNumberOfICalls(ext.accountID);
          this.getNumberOfOCalls(ext.accountID);
          this.getNumberOfCalls(ext.accountID);
          this.getCallTime(ext);
          this.getTalkTime(ext);
          this.getTotalInb();
        }); */
  }

  refreshData() {
    this.dataRefresher =
      setInterval(() => {
        this.getData(this.http, this.baseUrl);
      }, 2000);
  }

  alert() {
    window.alert("Expand")
  }

  // **Overview methods**
  changeTimePeriod() {
    this.getTalkTimeRecords();
    this.getScopedQueueLogs();
    this.getScopedCalls();
    this.getDaysInScope();
  }

  getListOfExtensions() {
    let mainExtensions: Extension[] = [];
    let totalICalls: number = 0;
    this.extensions.forEach(e => {
      if (e.type == "sip") {
        if (e.firstName == "Yariza" || 
          e.firstName == "Jacqueline" || 
          e.firstName == "Ivan" ||
          e.firstName == "Fred"  
        ) {

        }
        else {
          mainExtensions.push(e);
          this.getStatusClass(e);
          this.getExtensionStatus(e);
          this.getExtensionATime(e);
          this.getLastActivity(e);
          this.getITime(e);
          this.getAverageIdleTime(e);
          this.getICalls(e);
          this.getOCalls(e);
          this.getTCalls(e);
          this.getTalkTime(e);
          this.getAverageCallTime(e);
          this.getLoggedInTime(e);
        }
      }
    });
    return mainExtensions.sort((a, b) => (a.number > b.number) ? 1 : -1);
  }
  getNumberOfEvents() {
    let total: number = 0;
    for (const call of this.calls) {
      let num: number = call.events.length;
      total += num;
    }
    return total;
  }
  getNumberOfQueueLogs() {
    return this.queueLogs.length;
  }
  getQueueMembers(accountId) {
    let total;
    total = this.queues[0].members;
    return total;
  }

  getEarliestCall() {
    for (const call of this.calls) {
      let date: Date = new Date(call.startTime);
      if (date < this.earliestCallTime) {
        this.earliestCallTime = date;
      }
    }
    return this.earliestCallTime;
  }

  getLastCall(ext: string) {
    let extNum: string;
    let ext0: Extension;
    this.extensions.forEach(extension => {
      if (extension.accountID == ext) {
        extNum = extension.number;
        ext0 = extension;
        ext0.isActive = false;
      }
    });
    let lastEventTime: Date = new Date('January 1, 1970 00:0:00');
    let lastCall: Call;
    let lastEvent: Event;
    extNum = `<${extNum}>`
    let extNum0: string = `(${extNum})`
    this.currentCalls.forEach(call => {
      if ((call.ToCallerIdNumber == ext0.number) || (call.FromCallerIdNumber == ext0.number)) {
        ext0.lastCall = new Call();
        ext0.lastCall.startTime = call.StartTime;
        let now: Date = new Date();
        let startTime: Date = new Date(call.StartTime);
        ext0.coolDownTime = now;
        ext0.lastCall.totalDurationstr = this.minSec((Number(now) - Number(startTime)) / 1000);
        ext0.lastCall.fromName = call.FromCallerIdName;
        ext0.lastCall.to = call.ToCallerIdNumber;
        ext0.lastCall.toNumber = call.ToCallerIdNumber;
        ext0.lastCall.fromNumber = call.FromCallerIdNumber;
        ext0.isActive = true;
        ext0.lastCall.isCurrent = true;
        if (call.ToCallerIdNumber == ext0.number) {
          ext0.lastCall.origination = "incoming";
        }
        else {
          ext0.lastCall.origination = "outgoing";
        }
        return;
      }
    });
    if (!ext0.isActive) {
      this.calls.forEach(call => {
        call.events.forEach(event => {
          let time: Date = new Date(event.startTime)
          if ((time > lastEventTime) && (event.display.includes(extNum || extNum0))) {
            lastEventTime = new Date(event.startTime);
            lastCall = call;
            lastEvent = event;
            this.noLastCall = false;
          }
        });
      });
      if (!lastCall) {
        this.noLastCall = true;
        return;
      }
      ext0.lastCall = lastCall;
      this.lastCall = lastCall;
      this.lastEvent = lastEvent;
    }

  }

  getNumberOfMainPhones() {
    let total: number = 0;
    this.mainExtensions = [];
    for (const ext of this.extensions) {
      if (ext.type === "sip") {
        this.mainExtensions.push(ext);
        total = total + 1;
      }
    }
    return total;
  }

  getNumberOfCallQueues() {
    let total: number = 0;
    this.callQueues = [];
    for (const ext of this.extensions) {
      if (ext.type === "call_queue") {
        total = total + 1;
        this.callQueues.push(ext);
      }
    }
    return total;
  }

  getNumberOfIVRs() {
    let total: number = 0;
    this.ivrs = [];
    for (const ext of this.extensions) {
      if (ext.type === "ivr") {
        total = total + 1;
        this.ivrs.push(ext);
      }
    }
    return total;
  }

  getTalkTimeRecords() {
    this.scopedTalkTimeRecords = [];
    if (this.timePeriod == "today") {
      this.today = new Date(Date.now());
      this.talkTimeRecords.forEach(record => {
        if (this.isSameDateAs(new Date(record.date), this.today)) {
          this.scopedTalkTimeRecords.push(record);
        }
      });
    }
    else if (this.timePeriod == "yesterday") {
      this.talkTimeRecords.forEach(record => {
        if (this.isSameDateAs(new Date(record.date), this.lastBusinessDay)) {
          this.scopedTalkTimeRecords.push(record);
        }
      });
    }
    else {
      this.daysInScope.forEach(day => {
        this.talkTimeRecords.forEach(record => {
          if (this.isSameDateAs(new Date(record.date), day)) {
            this.scopedTalkTimeRecords.push(record);
          }
        });
      });
    }
  }

  getScopedCalls() {
    this.scopedCalls = [];
    if (this.timePeriod == "today") {
      this.calls.forEach(call => {
        if (this.isSameDateAs(new Date(call.startTime), this.today)) {

          this.scopedCalls.push(call);
        }
      });
    }
    else if (this.timePeriod == "yesterday") {
      this.calls.forEach(call => {
        if (this.isSameDateAs(new Date(call.startTime), this.lastBusinessDay)) {

          this.scopedCalls.push(call);
        }
      });
    }
    else {
      this.daysInScope.forEach(day => {
        this.calls.forEach(call => {
          if (this.isSameDateAs(new Date(call.startTime), day)) {

            this.scopedCalls.push(call);
          }
        });
      });
    }
  }

  getNumberOfCalls(ext: string) {
    if (!this.scopedTalkTimeRecords || this.scopedTalkTimeRecords == []) {
      this.getTalkTimeRecords();
    }
    let num: number = 0;
    this.scopedTalkTimeRecords.forEach(record => {
      if (record.accountID == ext) {
        num += record.totalCalls;
      }
    });
    return num;
  }
  getNumberOfICalls(ext: string) {
    if (!this.scopedTalkTimeRecords || this.scopedTalkTimeRecords == []) {
      this.getTalkTimeRecords();
    }
    let num: number = 0;
    this.scopedTalkTimeRecords.forEach(record => {
      if (record.accountID == ext) {
        num += record.totalIncomingCalls;
      }
    });
    return num;
  }
  getNumberOfOCalls(ext: string) {
    if (!this.scopedTalkTimeRecords || this.scopedTalkTimeRecords == []) {
      this.getTalkTimeRecords();
    }
    let num: number = 0;
    this.scopedTalkTimeRecords.forEach(record => {
      if (record.accountID == ext) {
        num += record.totalOutgoingCalls;
      }
    });
    return num;
  }

  getTimeSinceLastCall(ext: string) {
    // this.getLastCall(ext);
    if (this.noLastCall == true) {
      return "(N/A)";
    }
    else {
      let now: number = Date.now();
      let diff: number = now - (new Date(this.lastEvent.startTime)).getTime();
      diff = Math.floor(diff / 1000);
      return this.hrsMinSec(diff);
    }
  }

  getScopedQueueLogs() {
    this.scopedQueueLogs = [];
    if (this.timePeriod == "today") {
      this.queueLogs.forEach(log => {
        if (this.isSameDateAs(new Date(log.startTime), this.today)) {
          this.scopedQueueLogs.push(log);
        }
      });
    }
    else if (this.timePeriod == "yesterday") {
      this.queueLogs.forEach(log => {
        if (this.isSameDateAs(new Date(log.startTime), this.lastBusinessDay)) {
          this.scopedQueueLogs.push(log);
        }
      });
    }
    else {
      this.daysInScope.forEach(day => {
        this.queueLogs.forEach(log => {
          if (this.isSameDateAs(new Date(log.startTime), day)) {
            this.scopedQueueLogs.push(log);
          }
        });
      });
    }
  }

  getQueueCallsAnswered() {
    if (!this.scopedQueueLogs || this.scopedQueueLogs == []) {
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
        if (log.queueAccountID == queue.accountID) {
          totalQueueAnswers++;
          specLogs.push(log);
        }
      });
      this.mainExtensions.forEach(ext => {
        let totalExtensionQAnswers: number = 0;
        specLogs.forEach(log => {
          if (log.memberAccountID == ext.accountID) {
            totalExtensionQAnswers++;
          }
        });
        ext.totalAnswersList.push(totalExtensionQAnswers);
        let percent: string = "0";
        percent = ((totalExtensionQAnswers / totalQueueAnswers) * 100).toFixed(2);
        if (percent == "NaN") {
          percent = "0";
        }
        ext.percentAnswersList.push(percent);
      });
    });
    return this.callQueues;
  }

  getQueueCallInfo() {
    if (!this.scopedQueueLogs || this.scopedQueueLogs == []) {
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
        if (queue.accountID == log.queueAccountID) {
          total++;
          totalWaitTime += log.waitTime;
          if (log.waitTime > longestWaitTime) {
            longestWaitTime = log.waitTime;
            longestLog = log;
          }
          if (log.type == "abandoned") {
            this.scopedCalls.forEach(call => {
              if (call.uniqueID == log.uniqueID) {
                abandonedCalls.push(call);
              }
            });
          }
        }
      });
      this.scopedCalls.forEach(call => {
        if (longestLog && call.uniqueID == longestLog.uniqueID) {
          this.longestWaitTimeCallList.push(call);
        }
      });
      this.numberOfQueueCallsList.push(total);
      let avgWaitTime: number;
      avgWaitTime = (totalWaitTime / total);
      if (isNaN(avgWaitTime)) {
        avgWaitTime = 0;
      }
      this.avgQueueWaitTimeList.push(avgWaitTime);
      this.longestWaitTimeList.push(longestWaitTime);
      this.abandonedQueueCallsList.push(abandonedCalls);
      let abandonedCallsNum: number = abandonedCalls.length;
      let abandonedCallsPct: string = ((abandonedCallsNum / total) * 100).toFixed(2);
      if (abandonedCallsPct == "NaN") {
        abandonedCallsPct = "0";
      }
      this.abandonedQueueCallPctList.push(abandonedCallsPct);
    });
    return this.callQueues;
  }

  getLastBusinessDay() {
    let date: Date = new Date(Date.now());
    while (true) {
      date = new Date(date.setDate(date.getDate() - 1));
      if ([1, 2, 3, 4, 5].includes(date.getDay())) {
        this.lastBusinessDay = date;
        return;
      }
    }
  }

  getDaysThisWeek() {
    let dates: Date[] = [];
    let date: Date = new Date(Date.now());
    while (true) {
      if ([1, 2, 3, 4, 5, 6].includes(date.getDay())) {
        let date0: Date = new Date(date.getTime());
        dates.push(date0);
        date = new Date(date.setDate(date.getDate() - 1));
      }
      else {
        this.daysThisWeek = dates;
        return;
      }
    }
  }

  getDaysLastWeek() {
    let dates: Date[] = [];
    let date: Date = new Date(Date.now());
    while (true) {
      if (date.getDay() == 0) {
        date = new Date(date.setDate(date.getDate() - 1));
        break;
      }
      else {
        date = new Date(date.setDate(date.getDate() - 1));
      }
    }
    while (true) {
      if ([1, 2, 3, 4, 5, 6].includes(date.getDay())) {
        let date0: Date = new Date(date.getTime());
        dates.push(date0);
        date = new Date(date.setDate(date.getDate() - 1));
      }
      else {
        this.daysLastWeek = dates;
        return;
      }
    }
  }

  getDaysThisMonth() {
    let dates: Date[] = [];
    let date: Date = new Date(Date.now());
    let month: number = date.getMonth();
    while (true) {
      if (date.getMonth() != month) {
        break;
      }
      else {
        let date0: Date = new Date(date.getTime());
        dates.push(date0);
        date = new Date(date.setDate(date.getDate() - 1));
      }
    }
    this.daysThisMonth = dates;
  }

  getDaysLastMonth() {
    let dates: Date[] = [];
    let date: Date = new Date(Date.now());
    let month: number = date.getMonth();
    while (true) {
      if (date.getMonth() != month) {
        month = date.getMonth();
        break;
      }
      else {
        date = new Date(date.setDate(date.getDate() - 1));
      }
    }
    while (true) {
      if (date.getMonth() != month) {
        break;
      }
      else {
        let date0: Date = new Date(date.getTime());
        dates.push(date0);
        date = new Date(date.setDate(date.getDate() - 1));
      }
    }
    this.daysLastMonth = dates;
  }

  getDaysThisQuarter() {
    let dates: Date[] = [];
    let date: Date = new Date(Date.now());
    while (true) {
      if (date.getDate() == 1 && (date.getMonth() == 0 || date.getMonth() == 3 || date.getMonth() == 6 || date.getMonth() == 9)) {
        let date0: Date = new Date(date.getTime());
        dates.push(date0);
        date = new Date(date.setDate(date.getDate() - 1));
        break;
      }
      else {
        let date0: Date = new Date(date.getTime());
        dates.push(date0);
        date = new Date(date.setDate(date.getDate() - 1));
      }
    }
    this.daysThisQuarter = dates;
  }

  getDaysLastQuarter() {
    let dates: Date[] = [];
    let date: Date = new Date(Date.now());
    while (true) {
      if (date.getDate() == 1 && (date.getMonth() == 0 || date.getMonth() == 3 || date.getMonth() == 6 || date.getMonth() == 9)) {
        date = new Date(date.setDate(date.getDate() - 1));
        break;
      }
      else {
        date = new Date(date.setDate(date.getDate() - 1));
      }
    }
    while (true) {
      if (date.getDate() == 1 && (date.getMonth() == 0 || date.getMonth() == 3 || date.getMonth() == 6 || date.getMonth() == 9)) {
        let date0: Date = new Date(date.getTime());
        dates.push(date0);
        date = new Date(date.setDate(date.getDate() - 1));
        break;
      }
      else {
        let date0: Date = new Date(date.getTime());
        dates.push(date0);
        date = new Date(date.setDate(date.getDate() - 1));
      }
    }
    this.daysLastQuarter = dates;
  }

  getDaysInScope() {
    if (this.timePeriod == "thisweek") {
      this.displayIdleTime = true;
      this.daysInScope = this.daysThisWeek;
    }
    else if (this.timePeriod == "lastweek") {
      this.displayIdleTime = true;
      this.daysInScope = this.daysLastWeek;
    }
    else if (this.timePeriod == "thismonth") {
      this.daysInScope = this.daysThisMonth;
      this.displayIdleTime = false;
    }
    else if (this.timePeriod == "lastmonth") {
      this.daysInScope = this.daysLastMonth;
      this.displayIdleTime = false;
    }
    else if (this.timePeriod == "thisquarter") {
      this.daysInScope = this.daysThisQuarter;
      this.displayIdleTime = false;
    }
    else if (this.timePeriod == "lastquarter") {
      this.daysInScope = this.daysLastQuarter;
      this.displayIdleTime = false;
    }
  }

  sideClick(ext: Extension) {
    let extID: string = ext.accountID;
    let call: Call;
    this.extensions.forEach(extension => {
      if (extension.accountID == extID) {
        call = extension.lastCall;
      }
    });
    this.eventList = call.events;
    this.str = `Call ${call.pbxid} events:`
    this.arr = [];
    let i: number = 0;

    call.events.forEach(event => {
      i++;
      this.arr.push(`${(new Date(event.startTime)).toLocaleString()}-${event.display}`);
    });
  }

  longestClick(call: Call) {
    this.eventList = call.events;
    this.str = `Call ${call.pbxid} events:`
    this.arr = [];
    call.events.forEach(event => {
      this.arr.push(`${(new Date(event.startTime)).toLocaleString()}-${event.display}`);
    });
  }

  abandonedClick(calls: Call[], queue: Extension) {
    this.str = `Abandoned Calls for <${queue.display}>`;
    this.arr = [];
    calls.forEach(call => {
      let dateString: string = (new Date(call.startTime)).toLocaleString();
      this.arr.push(`From: ${call.from} - ${call.fromName} on ${dateString}`);
    });
  }

  setScopeButton(scope: string) {
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

  hrsMinSec(sec: number) {
    let hh = Math.floor(sec / 60 / 60);
    if (hh.toString().length == 1) {
      var hhs: string = "0" + hh;
    }
    else {
      hhs = hh.toString();
    }
    sec -= hh * 60 * 60;
    let mm = Math.floor(sec / 60);
    if (mm.toString().length == 1) {
      var mms: string = "0" + mm;
    }
    else {
      mms = mm.toString();
    }
    sec -= mm * 60;
    let ss = Math.floor(sec);
    if (ss.toString().length == 1) {
      var sss: string = "0" + ss;
    }
    else {
      sss = ss.toString();
    }
    return `${hhs}:${mms}:${sss}`;
  }

  getExtensionStatus(ext: Extension) {
    this.extensionStatuses.forEach(es => {
      if (es.ID == ext.number) {
        if (es.Status == "idle") {
          ext.status = "Idle";
          ext.subStatus = "";
        }
        else if (es.LastActivity.substring(0, 8) == "Incoming" && es.Status == "talking")
        {
          ext.status = "Incoming";
        }
        else if (es.LastActivity.substring(0, 8) == "Outgoing" && es.Status == "talking")
        {
          ext.status = "Outgoing";
        }
        else {
          ext.status = es.Status;
          ext.status = es.SubStatus ? (es.SubStatus) : es.Status;
          if (es.SubStatus == "Restroom") {
            ext.status = "Away";
            ext.subStatus = "Restroom";
          }
          if (ext.glyphiconClass == "fas fa-phone") {
            ext.subStatus = "";
          }
        }
      }
    });
  }

  minSec(sec: number) {
    let mm = Math.floor(sec / 60);
    if (mm.toString().length == 1) {
      var mms: string = "0" + mm;
    }
    else {
      mms = mm.toString();
    }
    sec -= mm * 60;
    let ss = Math.floor(sec);
    if (ss.toString().length == 1) {
      var sss: string = "0" + ss;
    }
    else {
      sss = ss.toString();
    }
    return `${mms}:${sss}`;
  }

  lastCallDuration(ext: Extension) {
    let lastCall: Call = ext.lastCall;
    if (lastCall != undefined) {
      return ext.lastCall.totalDurationstr;
    }
    else {
      return "N/A";
    }
  }

  getCallerStatus(call: Call) {
    if (call.origination == "incoming") {
      return `Incoming: ${call.fromName} ${call.fromNumber}`;
    }
    else {
      return `Outgoing: ${call.to}`;
    }
  }

  getLastEventTimeDiff(ext: Extension) {
    let call = ext.lastCall;
    let noLastCall: boolean = true;
    let lastEventTime: Date = new Date('January 1, 1970 00:0:00');
    call.events.forEach(event => {
      let time: Date = new Date(event.startTime)
      if ((time > lastEventTime)) {
        lastEventTime = new Date(event.startTime);
        noLastCall = false;
      }
    });
    if (this.isSameDateAs((lastEventTime), this.today)) {
      let now: Date = new Date();
      if (((Number(now) - Number(ext.coolDownTime)) / 1000) < 50) {
        return "< 1 min";
      }
      let timeDiff: number = (Number(now) - Number(lastEventTime)) / 1000;
      return this.minSec(timeDiff);
    }
    else {
      return "N/A";
    }
  }

  getCallTime(ext0: Extension) {
    let ext = ext0.accountID;
    if (!this.scopedTalkTimeRecords || this.scopedTalkTimeRecords == []) {
      this.getTalkTimeRecords();
    }
    let time: number = 0;
    this.scopedTalkTimeRecords.forEach(record => {
      if (record.accountID == ext) {
        time += record.callDuration;
      }
    });
    ext0.callTime = this.hrsMinSec(time);
    return this.hrsMinSec(time);
  }

  getStatusClass(ext: Extension) {
    this.extensionStatuses.forEach(es => {
      if (es.ID == ext.number) {
        if (es.Status == "idle") {
          ext.extensionClass = "text-muted";
          ext.glyphiconClass = "fas fa-user-check";
          return true;
        }
        else if (es.Status == "away") {
          ext.extensionClass = "table-dark";
          ext.glyphiconClass = "fas fa-clock";
        }
        else if (es.Status == "dnd") {
          ext.extensionClass = "table-dark";
          ext.glyphiconClass = "fas fa-stop-circle";
        }
        else if (es.Status == "xa") {
          ext.extensionClass = "table-dark";
          ext.glyphiconClass = "fas fa-plane";
        }
        else {
          ext.extensionClass = "table-success";
          ext.glyphiconClass = "fas fa-phone";
          return true;
        }
      }
    });
  }

  getTotalInb() {
    let total: number = 0;
    this.mainExtensions.forEach(ext => {
      total += this.getNumberOfICalls(ext.accountID);
    });
    this.totalICalls = total;
    return total;
  }

  getExtensionATime(ext: Extension) {
    this.extensionStatuses.forEach(es => {
      if (es.ID == ext.number) {
        if (es.ATime < 3600)
        {
          ext.activeTime = this.minSec(es.ATime);
        }
        else
        {
          ext.activeTime = this.hrsMinSec(es.ATime);
        }
        if (es.Status == "idle" && es.ATime > 180) {
          ext.activeIClass = "table-danger";
        }
        else {
          ext.activeIClass = ext.extensionClass;
        }
      }
    });
  }

  getLastActivity(ext: Extension) {
    this.extensionStatuses.forEach(es => {
      if (es.ID == ext.number) {
        ext.lastActivity = es.LastActivity;
      }
    });
  }

  getITime(ext: Extension) {
    this.extensionStatuses.forEach(es => {
      if (es.ID == ext.number) {
        ext.idleTime = this.hrsMinSec(es.ITime);
      }
    });
  }

  getAverageIdleTime(ext: Extension) {
    this.extensionStatuses.forEach(es => {
      if (es.ID == ext.number) {
        if (!es.IdleTimeList) {
          ext.averageIdleTime = "N/A";
        }
        else {
          let sum: number = 2;
          let num: number = 0;
          let average: number;
          es.IdleTimeList.forEach(time => {
            if (time > 12) {
              sum += time;
              num += 1;
            }
          });
          average = sum / num;
          if (num > 0) {
            ext.averageIdleTime = this.minSec(average);
          }
          else {
            ext.averageIdleTime = "N/A";
          }
          if (average > 180 && ext.averageIdleTime != "N/A")
          {
            ext.idleClass = "table-danger";
          }
          else {
            ext.idleClass = "";
          }
        }

      }
    });
  }

  getActiveIClass(ext: Extension) {
    this.extensionStatuses.forEach(es => {
      if (es.ID == ext.number) {
        if (es.Status == "idle") {
          ext.extensionClass = "text-muted";
          ext.glyphiconClass = "fas fa-user-check";
          return true;
        }
        else if (es.Status == "away") {
          ext.extensionClass = "table-dark";
          ext.glyphiconClass = "fas fa-clock";
        }
        else if (es.Status == "dnd") {
          ext.extensionClass = "table-dark";
          ext.glyphiconClass = "fas fa-stop-circle";
        }
        else if (es.Status == "xa") {
          ext.extensionClass = "table-dark";
          ext.glyphiconClass = "fas fa-plane";
        }
        else {
          ext.extensionClass = "table-success";
          ext.glyphiconClass = "fas fa-phone";
          return true;
        }
      }
    });
  }

  getICalls(ext: Extension) {
    this.extensionStatuses.forEach(es => {
      if (es.ID == ext.number) {
        ext.iCalls = es.TotalICalls;
      }
    });
  }

  getOCalls(ext: Extension) {
    this.extensionStatuses.forEach(es => {
      if (es.ID == ext.number) {
        ext.oCalls = es.TotalOCalls;
      }
    });
  }

  getTCalls(ext: Extension) {
    this.extensionStatuses.forEach(es => {
      if (es.ID == ext.number) {
        ext.tCalls = es.TotalCalls;
      }
    });
  }

  getAverageCallTime(ext: Extension) {
    this.extensionStatuses.forEach(es => {
      if (es.ID == ext.number) {
        if (es.TotalCalls == 0) {
          ext.averageCallTime = "N/A";
        }
        else {
          ext.averageCallTime = this.minSec(es.CTime / es.TotalCalls);
        }
      }
    });
  }

  getTalkTime(ext: Extension) {
    this.extensionStatuses.forEach(es => {
      if (es.ID == ext.number) {
        ext.talkTime = this.hrsMinSec(es.CTime);
      }
    });
  }

  getLoggedInTime(ext: Extension) {
    this.extensionStatuses.forEach(es => {
      if (es.ID == ext.number) {
        ext.loggedInTime = this.hrsMinSec(es.LoggedInTime);
      }
    });
  }

  title = 'Angular7-readCSV';  
  
  public records: any[] = [];  
  @ViewChild('csvReader', {static: false}) csvReader: any;  
  
  uploadListener($event: any): void {  
  
    let text = [];  
    let files = $event.srcElement.files;  
  
    if (this.isValidCSVFile(files[0])) {  
  
      let input = $event.target;  
      let reader = new FileReader();  
      reader.readAsText(input.files[0]);  
  
      reader.onload = () => {  
        let csvData = reader.result;  
        let csvRecordsArray = (<string>csvData).split(/\r\n|\n/);  
  
        let headersRow = this.getHeaderArray(csvRecordsArray);  
  
        this.records = this.getDataRecordsArrayFromCSVFile(csvRecordsArray, headersRow.length);  
      };  
  
      reader.onerror = function () {  
        console.log('error is occured while reading file!');  
      };  
  
    } else {  
      alert("Please import valid .csv file.");  
      this.fileReset();  
    }  
  }  
  
  getDataRecordsArrayFromCSVFile(csvRecordsArray: any, headerLength: any) {  
    let csvArr = [];  
  
    for (let i = 1; i < csvRecordsArray.length; i++) {  
      let curruntRecord = (<string>csvRecordsArray[i]).split(',');  
      if (curruntRecord.length == headerLength) {  
        let csvRecord: CSVRecord1 = new CSVRecord1();  
        csvRecord.id = curruntRecord[0].trim();  
        csvRecord.firstName = curruntRecord[1].trim();  
        csvRecord.lastName = curruntRecord[2].trim();  
        csvRecord.total = curruntRecord[3].trim();  
        csvRecord.data1 = curruntRecord[4].trim();  
        csvRecord.data2 = curruntRecord[5].trim();  
        csvArr.push(csvRecord);  
      }  
    }  
    return csvArr;  
  }  
  
  isValidCSVFile(file: any) {  
    return file.name.endsWith(".csv");  
  }  
  
  getHeaderArray(csvRecordsArr: any) {  
    let headers = (<string>csvRecordsArr[0]).split(',');  
    let headerArray = [];  
    for (let j = 0; j < headers.length; j++) {  
      headerArray.push(headers[j]);  
    }  
    return headerArray;  
  }  
  
  fileReset() {  
    this.csvReader.nativeElement.value = "";  
    this.records = [];  
  } 
  
  tableTimer() {
    this.showTable = false;
    let d = new Date();
    let s = d.getSeconds();
    if (s > 30) {
      this.showTable = true;
    }
  }
}
