import { BrowserModule } from '@angular/platform-browser';
import { NgModule, enableProdMode } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { CreateReportComponent } from './create-report/create-report.component';
import { OverviewComponent } from './overview/overview.component';
import { CreateMultireportComponent } from './create-multireport/create-multireport.component';
import { CreateQueuereportComponent } from './create-queuereport/create-queuereport.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { BoardComponent } from './board/board.component';
import { TvComponent } from './tv/tv.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { WidgetComponent } from './widget/widget.component';
import { LogReportComponent } from './log-report/log-report.component';

enableProdMode();


@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CreateReportComponent,
    OverviewComponent,
    CreateMultireportComponent,
    CreateQueuereportComponent,
    SidebarComponent,
    CounterComponent,
    FetchDataComponent,
    DashboardComponent,
    BoardComponent,
    TvComponent,
    WidgetComponent,
    LogReportComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'overview', component: OverviewComponent },
      { path: 'create-report', component: CreateReportComponent },
      { path: 'create-multireport', component: CreateMultireportComponent },
      { path: 'create-queuereport', component: CreateQueuereportComponent },
      { path: 'counter', component: CounterComponent },
      { path: 'fetch-data', component: FetchDataComponent },
      { path: 'board', component: BoardComponent },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'tv', component: TvComponent },
      { path: 'widget', component: WidgetComponent },
      { path: 'log-report', component: LogReportComponent },
    ]),
    BrowserAnimationsModule
  ],
  providers: [
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
