import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatCardModule } from '@angular/material/card';
import { FlightPlanResponse } from '../../models/response-model';
import { FlightPlanService } from '../../services/flight-plan.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-flight-plans-list',
  standalone: true,
  imports: [
    CommonModule,

    MatCardModule
  ],
  templateUrl: './flight-plans-list.component.html',
  styleUrl: './flight-plans-list.component.css'
})

export class FlightPlansListComponent implements OnInit {
  
  flightPlans: FlightPlanResponse[] | null = null;

  constructor(
    private router: Router,
    private flightPlanService: FlightPlanService
  ) { }

  ngOnInit() {
    this.getFlightPlans();
  }

  getFlightPlans(): void {
    this.flightPlanService.getFlightPlans().subscribe({
      next: (flightPlans: FlightPlanResponse[]) => {
        this.flightPlans = flightPlans;
      },
      error: (error: any) => {
        console.error(error);
      }
    });
  }

  showFlightPlanDetails(flightPlanId: number): void {
    this.router.navigate(['/response', flightPlanId]);
  }
}