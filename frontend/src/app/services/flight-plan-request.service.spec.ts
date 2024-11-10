import { TestBed } from '@angular/core/testing';

import { FlightPlanRequestService } from './flight-plan-request.service';

describe('FlightPlanRequestService', () => {
  let service: FlightPlanRequestService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FlightPlanRequestService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
