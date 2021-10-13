import { Component, OnInit, Input } from '@angular/core';
import { Event } from "../models/event.model";

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {
  callMessage: string;
  subscription;
  @Input() str: string;
  @Input() arr: string[];

  constructor() {

  }

  ngOnInit() {
  }


}
