import { Extension } from "./extension.model";
import { Call } from "./call.model";

export class Queue {
  constructor(
    public id: number,
    public extension?: string,
    public strategy?: string,
    public accountId?: string,
    public members?: Extension[],
    public waiteringCalls?: Call[]
  ) {}
}