import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateQueuereportComponent } from './create-queuereport.component';

describe('CreateQueuereportComponent', () => {
  let component: CreateQueuereportComponent;
  let fixture: ComponentFixture<CreateQueuereportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateQueuereportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateQueuereportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
