import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FlightPlansListComponent } from './flight-plans-list.component';

describe('FlightPlansListComponent', () => {
  let component: FlightPlansListComponent;
  let fixture: ComponentFixture<FlightPlansListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FlightPlansListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FlightPlansListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
