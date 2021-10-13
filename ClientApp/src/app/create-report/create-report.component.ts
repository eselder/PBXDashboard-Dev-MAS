import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Call } from "../models/call.model";
import { Extension } from "../models/extension.model";
import { NgForm, NumberValueAccessor } from '@angular/forms';
import { Event } from "../models/event.model";

@Component({
  selector: 'app-create-report',
  templateUrl: './create-report.component.html',
  styleUrls: ['./create-report.component.css']
})
export class CreateReportComponent implements OnInit {

  public calls: Call[];
  public extensions: Extension[];
  public reportExtensions: Extension[];
  public reportExtension: Extension;
  public extIn: string = "Sample Extension";
  public reportCalls: Call[] = [];
  public reportCalled: boolean;
  public e: Event;
  public evs: Event[] = [];
  public eventSpanString: string = "";
  public callOrigin: string = "both";
  public submitted = false;
  public idleTime;
  public callScope: string = "both";
  public eventText: string = "";
  public isActive: boolean = false;
  public activeString: string = "";
  public extCalls: Call[] = [];
  public str: string = "Call details will display here";
  public eventList: Event[] = [];
  public arr: string[] = [];
  public startDate: Date;
  public endDate: Date;
  public filteredCalls: Call[] = [];
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
  public datesList: string[];
  public daysTotal: number;
  public totalCalls: number;
  public avgCallsPerDay: string;
  public totalDurationSum: number;
  public talkDurationSum: number;
  public idleTimePerDay: string;
  public totalTalkTime: string;
  public totalTotalTime: string;
  public dailyTalkTime: string;
  public dailyTotalTime: string;
  public filterByDate: boolean = false;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<Call[]>(baseUrl + 'api/calls').subscribe(result => {
      this.calls = result;
    }, error => console.error(error));
    http.get<Extension[]>(baseUrl + 'api/extensions').subscribe(result => {
      this.extensions = result;
    }, error => console.error(error));
  }

  ngOnInit() {
  }

  onSubmit(f: NgForm)
  {
    this.submitted = true;
    this.extIn = f.value.extension;
    let sDate: Date = new Date(f.value.start_date);
    sDate = new Date(sDate.setHours(sDate.getHours() + 7));
    let eDate: Date = new Date(f.value.end_date);
    eDate = new Date(eDate.setHours(eDate.getHours() + 7));
    console.log("report date: " + f.value.start_date);
    if (!f.value.call_scope)
    {
      f.value.call_scope = "both";
    }
    if (!f.value.call_origin)
    {
      f.value.call_origin = "both";
    }
    if (f.value.start_date)
    {
      this.startDate = new Date((sDate));
      this.filterByDate = true;
    }
    else
    {
      this.startDate = new Date(1970, 0, 1);
    }
    if (f.value.end_date)
    {
      this.endDate = new Date(eDate);
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
    this.callScope = f.value.call_scope;
    this.callOrigin = f.value.call_origin;
    this.filterCallsByTime();
    this.filterCallsByDays();
    if (this.filter_by_time)
    {
      this.filterCallsByHour();
    }
    this.createReport(f.value.call_origin);
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

  getListOfExtensionsLength()
  {
    return this.extensions.length;
  }

  onUpdateExtension(event: any)
  {
    this.extIn = event.target.value;
  }

  filterCallsByTime()
  {
    this.endDate = new Date (this.endDate.setDate(this.endDate.getDate() + 1));
    this.calls.forEach(call => {
      let date: Date = new Date(call.startTime);
      if ( date >= this.startDate && date <= this.endDate)
      {
        this.filteredCalls.push(call);
      }
    });
  }
  filterCallsByDays()
  {
    let tempFilteredCalls: Call[] = this.filteredCalls;
    this.filteredCalls = [];
    if (this.monday)
    {
      tempFilteredCalls.forEach(call => {
        let date = new Date(call.startTime)
        if (date.getDay() == 1)
        {
          this.filteredCalls.push(call);
        }
      });
    }
    if (this.tuesday)
    {
      tempFilteredCalls.forEach(call => {
        let date = new Date(call.startTime)
        if (date.getDay() == 2)
        {
          this.filteredCalls.push(call);
        }
      });
    }
    if (this.wednesday)
    {
      tempFilteredCalls.forEach(call => {
        let date = new Date(call.startTime)
        if (date.getDay() == 3)
        {
          this.filteredCalls.push(call);
        }
      });
    }
    if (this.thursday)
    {
      tempFilteredCalls.forEach(call => {
        let date = new Date(call.startTime)
        if (date.getDay() == 4)
        {
          this.filteredCalls.push(call);
        }
      });
    }
    if (this.friday)
    {
      tempFilteredCalls.forEach(call => {
        let date = new Date(call.startTime)
        if (date.getDay() == 5)
        {
          this.filteredCalls.push(call);
        }
      });
    }
    if (this.saturday)
    {
      tempFilteredCalls.forEach(call => {
        let date = new Date(call.startTime)
        if (date.getDay() == 6)
        {
          this.filteredCalls.push(call);
        }
      });
    }
    if (this.sunday)
    {
      tempFilteredCalls.forEach(call => {
        let date = new Date(call.startTime)
        if (date.getDay() == 7)
        {
          this.filteredCalls.push(call);
        }
      });
    }
  }
  filterCallsByHour()
  {
    let tempFilteredCalls: Call[] = this.filteredCalls;
    this.filteredCalls = [];
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
        this.filteredCalls.push(call);
      }
    });
  }

  createReport(callOrigin: string = "both")
  {
    this.reportCalled = true;
    if (callOrigin == "both" && this.callScope == "both")
    {
      this.filteredCalls.forEach(call =>
        {
          if (call.fromNumber == this.extIn || call.toNumber == this.extIn)
          {
            this.reportCalls.push(call);
          }
        }
      )
    }
    else if (this.callScope == "both") {
      this.filteredCalls.forEach(call =>
        {
          if ((call.fromNumber == this.extIn || call.toNumber == this.extIn) &&   callOrigin == call.origination)
          {
            this.reportCalls.push(call);
          }
        }
      )
    }
    else if (this.callScope == "internal") {
      if (callOrigin == "both") 
      {
        this.filteredCalls.forEach(call =>
          {
            let isInternal: boolean = (call.toNumber.toString().length == 4) && (call.fromNumber.toString().length == 4);
            if (isInternal && (call.toNumber == this.extIn || call.fromNumber == this.extIn ))
            {
              this.reportCalls.push(call);
            }
          }
        )
      }
      else{
        this.filteredCalls.forEach(call =>
          {
            let isExternal: boolean = (call.toNumber.toString().length != 4) || (call.fromNumber.toString().length != 4);
            if (!isExternal)
            {
              let correctNumber: boolean = (call.fromNumber == this.extIn || call.toNumber == this.extIn);
              if (correctNumber)
              {
                if (callOrigin == "outgoing" && call.from == this.extIn)
                {
                  this.reportCalls.push(call);
                }
                else if (callOrigin == "incoming" && call.toNumber == this.extIn)
                {
                  this.reportCalls.push(call);
                }
              }
            }
          })
        }
    }
    else {
      this.filteredCalls.forEach(call =>
        {
          let isExternal: boolean = (call.toNumber.toString().length != 4) || (call.fromNumber.toString().length != 4);
          if (isExternal)
          {
            let correctNumber: boolean = (call.fromNumber == this.extIn || call.toNumber == this.extIn);
            if (correctNumber)
            {
              if (callOrigin == "both")
              {
                this.reportCalls.push(call);
              }
              if (callOrigin == call.origination)
              {
                this.reportCalls.push(call);
              }
            }
          }
          
        }
      )
    }
    this.getTotalDurationSum();
    this.getTalkDurationSum();
    this.getDailyStats();
    let eventCount: number = 0;
    this.calls.forEach(call => {
      call.events.forEach(event => {
        if (event.type == "TALKING" && event.display.includes("Talked to Tracy Elder <101>") && eventCount < 20)
        {
          console.log(event.display);
          eventCount++;
        }
      });
    });
    console.log("TALKING eventCount: " + eventCount);
  }

  eventSpan(call0: Call)
  {
    call0.events.forEach( ev =>
      {
        this.eventSpanString += " => "
        this.eventSpanString += ev.display;
      }
    )
    return "Hello";
  }

  getTalkDuration()
  {
    let duration = this.talkDurationSum;
    let min = 0;
    let sec = 0;
    min = Math.floor((duration/(this.reportCalls.length))/60);
    sec = (((duration/(this.reportCalls.length))%60));
    return `${min}:${sec.toFixed(0)} (min:sec)`;
  }

  getTotalDuration()
  {
    let duration = this.totalDurationSum;
    let min = 0;
    let sec = 0;
    min = Math.floor((duration/(this.reportCalls.length))/60);
    sec = (((duration/(this.reportCalls.length))%60));
    return `${min}:${sec.toFixed(0)} (min:sec)`;
  }

  getExtension(extString: string)
  {
    return this.extensions.find(
      e => e.number == extString
    ).display;
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

  getLocalString(time: string)
  {
    let date = new Date(time);
    return date.toLocaleString();
  }

  getTotalDurationSum()
  {
    this.totalDurationSum = 0;
    this.reportCalls.forEach(call => {
      this.totalDurationSum += call.totalDuration;
    });
  }

  getTalkDurationSum()
  {
    this.talkDurationSum = 0;
    this.reportCalls.forEach(call => {
      this.talkDurationSum += call.talkDuration;
    });
  }

  getDailyStats()
  {
    this.datesList = [];
    this.reportCalls.forEach(call => {
      let date: Date = new Date(call.startTime);
      let dateString: string = date.toLocaleDateString();
      if (!this.datesList.includes(dateString))
      {
        this.datesList.push(dateString);
      }
    });
    this.daysTotal = this.datesList.length;
    this.totalCalls = this.reportCalls.length;
    this.avgCallsPerDay = (this.totalCalls / this.daysTotal).toFixed(1);
    if (this.filter_by_time)
    {
      let hours: number = Number((this.endTime).substring(0, 2)) - Number((this.startTime).substring(0, 2));
      let minutes: number = Number((this.endTime).substring(3, 5)) - Number((this.startTime).substring(3, 5));
      let minutesTotal: number = hours*60 + minutes;
      let secondsTotal: number = minutesTotal*60;
      let totalSeconds: number = secondsTotal*this.daysTotal;
      let idleSeconds: number = totalSeconds - this.totalDurationSum;
      this.idleTime = this.hrsMinSec(idleSeconds);
      let idleSecondsPerDay: number = idleSeconds / this.daysTotal;
      this.idleTimePerDay = this.hrsMinSec(idleSecondsPerDay);
    }
    this.totalTalkTime = this.hrsMinSec(this.talkDurationSum);
    this.totalTotalTime = this.hrsMinSec(this.totalDurationSum);
    this.dailyTalkTime = this.hrsMinSec(this.talkDurationSum / this.daysTotal);
    this.dailyTotalTime = this.hrsMinSec(this.totalDurationSum / this.daysTotal);
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
    return `${hhs}:${mms}:${sss} (hrs:min:sec)`;
  }

}
