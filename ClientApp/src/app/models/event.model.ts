import { Call } from "./call.model";

export class Event {
  constructor(
    public eventId?: number,
    public startTime?: string,
    public type?: string,
    public display?: string,
    public call?: Call
  ) {}
}