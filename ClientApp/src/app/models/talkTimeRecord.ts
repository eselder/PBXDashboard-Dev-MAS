export class TalkTimeRecord {
  constructor(
    public iD: number,
    public accountID: string,
    public date: Date,
    public talkingDuration: number,
    public totalOutgoingCalls: number,
    public totalIncomingCalls: number,
    public totalCalls: number,
    public callDuration: number
  ) {}
}