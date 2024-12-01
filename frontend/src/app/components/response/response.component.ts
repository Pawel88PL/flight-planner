import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';

import { MatCardModule } from '@angular/material/card';

import { DataService } from '../../services/data.service';
import { FlightPlanResponse } from '../../models/response-model';
import { ActivatedRoute } from '@angular/router';
import { FlightPlanService } from '../../services/flight-plan.service';

@Component({
  selector: 'app-response',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule
  ],
  templateUrl: './response.component.html',
  styleUrl: './response.component.css'
})

export class ResponseComponent implements OnInit {

  errorMessage: string | null = null;
  successMessage: string | null = null

  flightPlanId: string | null = null;
  flightPlan: FlightPlanResponse | null = null;


  constructor(private route: ActivatedRoute, private flightPlanService: FlightPlanService) { }

  ngOnInit() {
    this.setFlightPlanId();
    this.getFlightPlan();
  }

  getFlightPlan() {
    if (!this.flightPlanId) {
      console.error('Flight Plan ID is not set');
      return
    }

    this.flightPlanService.getFlightPlanById(this.flightPlanId).subscribe({
      next: (response) => {
        this.flightPlan = response;
      },
      error: (error) => {
        this.errorMessage = error.error.message;
      }
    });
  }

  
  
  setFlightPlanId() {
    this.flightPlanId = this.route.snapshot.paramMap.get('id'); 
  }
}
