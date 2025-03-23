import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminFlightPlansComponent } from './admin-flight-plans.component';

describe('AdminFlightPlansComponent', () => {
  let component: AdminFlightPlansComponent;
  let fixture: ComponentFixture<AdminFlightPlansComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminFlightPlansComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminFlightPlansComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
