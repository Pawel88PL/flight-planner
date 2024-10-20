import { Component, OnInit } from '@angular/core';

import { MatCardModule } from '@angular/material/card';

import { InsertFlyDataComponent } from '../insert-fly-data/insert-fly-data.component';
import { RouterModule } from '@angular/router';
import gsap from 'gsap';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    InsertFlyDataComponent,
    
    MatCardModule,
    RouterModule
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})

export class HomeComponent implements OnInit {

  ngOnInit() {
    this.performGsapAnimation();
  }

  performGsapAnimation(): void {
    const tl = gsap.timeline({ defaults: { ease: 'power3.out', duration: 1 } });

    // Animacja dla nagłówka
    tl.from('.main-card', { y: -100, opacity: 0 });
  }

}
