import { Event } from "./event.model";
import { QueueLog } from "./queueLog";

export class Call {
  constructor(
    public callId?: number,
    public pbxid?: number,
    public uniqueID?: string,
    public origination?: string,
    public startTime?: string,
    public from?: string,
    public fromAccountId?: number,
    public toAccountId?: number,
    public fromName?: string,
    public fromNumber?: string,
    public to?: string,
    public toNumber?: string,
    public totalDuration?: number,
    public totalDurationstr?: string,
    public talkDuration?: number,
    public cdrCallId?: number,
    public events?: Event[],
    public queueLogs?: QueueLog[],
    public waitTime?: number,
    public queueOutcome?: string,
    public answeredBy?: string,
    public isCurrent?: boolean
  ) { }
}
