import { Component, OnInit } from '@angular/core';

import { MatCardModule } from '@angular/material/card';

import { InsertFlyDataComponent } from '../insert-fly-data/insert-fly-data.component';
import { Router, RouterModule } from '@angular/router';
import gsap from 'gsap';
import { CommonModule } from '@angular/common';
import { DataService } from '../../services/data.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    InsertFlyDataComponent,

    MatCardModule,
    RouterModule
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})

export class HomeComponent implements OnInit {

  errorMessage: string | null = null;
  successMessage: string | null = null;

  private subscription: Subscription | null = null;

  constructor(
    private dataService: DataService,
    private router: Router
  ) { }

  ngOnInit() {
    this.performGsapAnimation();

    this.subscription = this.dataService.successMessage$.subscribe(message => {
      this.successMessage = message;
    });
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  performGsapAnimation(): void {
    const tl = gsap.timeline({ defaults: { ease: 'power3.out', duration: 1 } });

    // Animacja dla nagłówka
    tl.from('.main-card', { y: -100, opacity: 0 });
  }

}
