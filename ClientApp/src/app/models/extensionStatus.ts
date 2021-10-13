export class ExtensionStatus {
  constructor(
    public ID: string,
    public Status?: string,
    public SubStatus?: string,
    public LastActivity?: string,
    public LastActivityIdle?: string,
    public ITime?: number,
    public ATime?: number,
    public CTime?: number,
    public TotalCalls?: number,
    public TotalICalls?: number,
    public TotalOCalls?: number,
    public LoggedInTime?: number,
    public IdleTimeList?: number[],
    public StatusList?: string[],
    public SubStatusList?: string[],
    public DurationList?: number[],
    public LogoutTimesList?: Date[],
  ) { }
}
