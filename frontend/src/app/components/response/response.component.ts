import { ActivatedRoute } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatCardModule } from '@angular/material/card';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MarkdownModule } from 'ngx-markdown';

import { AiService } from '../../services/ai.service';
import { FlightPlanService } from '../../services/flight-plan.service';

import { AIResponseModel } from '../../models/ai-response-model';
import { FlightPlanResponse } from '../../models/response-model';

@Component({
  selector: 'app-response',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatProgressBarModule,
    MarkdownModule
  ],
  templateUrl: './response.component.html',
  styleUrl: './response.component.css'
})

export class ResponseComponent implements OnInit {

  errorMessage: string | null = null;
  successMessage: string | null = null

  aiResponseId: string | null = null;
  aiResponse: AIResponseModel | null = null;

  flightPlanId: string | null = null;
  flightPlan: FlightPlanResponse | null = null;

  isLoading: boolean = true;
  isLoadingAI: boolean = true;

  constructor(
    private aiService: AiService,
    private route: ActivatedRoute,
    private flightPlanService: FlightPlanService) { }

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
        this.isLoading = false;
        this.getAIResponse();
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error.message;
      }
    });
  }

  getAIResponse() {
    if (!this.flightPlanId) {
      console.error('Flight Plan ID is not set');
      return
    }
    this.isLoadingAI = true;

    this.aiService.getAIResponseByFlightPlanById(this.flightPlanId).subscribe({
      next: (response) => {
        this.aiResponse = response;
        this.isLoadingAI = false;
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
