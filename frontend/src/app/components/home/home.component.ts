import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { Subscription } from 'rxjs';

import { MatCardModule } from '@angular/material/card';

import { InsertFlyDataComponent } from '../insert-fly-data/insert-fly-data.component';

import { DataService } from '../../services/data.service';

import gsap from 'gsap';

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
    this.animateComponents();
    this.animateAirplane();

    this.subscription = this.dataService.successMessage$.subscribe(message => {
      this.successMessage = message;
    });
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  animateComponents() {
    const timeline = gsap.timeline({
      onComplete: () => this.scrollToTop() // Wywołanie scrollToTop po zakończeniu wszystkich animacji
    });

    // Animacja dla całej karty mat-card
    timeline.from(".main-card", {
      duration: 0.8,  // Skrócony czas trwania
      opacity: 0,
      y: 30,
      ease: "power3.out"
    });

    // Animacja dla głównego nagłówka
    timeline.from(".main-card h1", {
      duration: 0.6,  // Skrócony czas trwania
      opacity: 0,
      y: -20,
      ease: "power3.out"
    }, "-=0.4");  // Przesunięcie czasowe, aby animacja zaczęła się wcześniej

    // Animacja dla alertu z opisem aplikacji
    timeline.from(".alert-primary", {
      duration: 0.8,
      opacity: 0,
      scale: 0.9,
      ease: "power3.out"
    }, "-=0.2");
  }

  // Osobna animacja dla ikony samolotu, aby kontynuowała ruch w nieskończoność
  animateAirplane() {
    gsap.to(".airplane", {
      x: -60,  // Ruch w lewo
      yoyo: true,
      repeat: -1,
      duration: 1.5,
      ease: "power1.inOut",
      rotation: 10,  // Delikatna rotacja dla efektu lotu
    });
  }

  scrollToTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
}