import { TestBed } from '@angular/core/testing';

import { FlightPlanService } from './flight-plan.service';

describe('FlightPlanRequestService', () => {
  let service: FlightPlanService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FlightPlanService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
