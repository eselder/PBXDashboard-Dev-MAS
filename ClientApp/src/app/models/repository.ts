import { Call } from "./call.model";
import { Injectable } from "@angular/core";
import { HttpClient, HttpRequest, HttpResponse } from "@angular/common/http";
import { Observable } from "rxjs";
import { Filter } from "./configClasses.repository";

const callsUrl = "/api/calls";

@Injectable()
export class Repository { 
    private filterObject = new Filter();

    constructor(private http: HttpClient) {
        this.filter.related = true;
        this.getCalls();
    }

    getCalls() {
        let url = callsUrl +  "?related=" + this.filter.related;
    }

    call: Call;
    calls: Call[];	

    get filter(): Filter {
        return this.filterObject;
    }
  }