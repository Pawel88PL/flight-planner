import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';


import { MatCardModule } from '@angular/material/card';
import { Subscription } from 'rxjs';
import { DataService } from '../../services/data.service';

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

export class ResponseComponent implements OnInit, OnDestroy {

  errorMessage: string | null = null;
  successMessage: string | null = null

  private subscription: Subscription | null = null;
  
  constructor(
    private dataService: DataService
  ) { }

  ngOnInit() {
    this.subscription = this.dataService.successMessage$.subscribe(message => {
      this.successMessage = message;
    });

    setTimeout(() => {
      this.dataService.clearSuccessMessage();
    }, 5000);
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

}
