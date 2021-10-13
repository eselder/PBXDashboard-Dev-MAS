export class CurrentCall {
  constructor(
    public ID: number,
    public PBXID?: string,
    public StartTime?: string,
    public Duration?: number,
    public State?: string,
    public Format?: string,
    public FromCallerIdName?: string,
    public ToCallerIdName?: string,
    public FromCallerIdNumber?: string,
    public ToCallerIdNumber?: string
  ) { }
}
