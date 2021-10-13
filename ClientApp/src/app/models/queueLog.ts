export class QueueLog {
  constructor(
    public id: number,
    public queueAccountID?: string,
    public queueExtension?: string,
    public enterPosition?: number,
    public memberAccountID?: string,
    public waitTime?: number,
    public queueName?: string,
    public memberExtension?: string,
    public memberName?: string,
    public talkTime?: string,
    public type?: string,
    public memberMisses?: number,
    public uniqueID?: string,
    public startTime?: string
  ) {}
}