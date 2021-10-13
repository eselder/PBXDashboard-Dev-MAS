import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateMultireportComponent } from './create-multireport.component';

describe('CreateMultireportComponent', () => {
  let component: CreateMultireportComponent;
  let fixture: ComponentFixture<CreateMultireportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateMultireportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateMultireportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
