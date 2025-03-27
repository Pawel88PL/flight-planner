import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';

import { MatCardModule } from '@angular/material/card';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';

import { Router, RouterModule } from '@angular/router';

import { AircraftService } from '../../services/aircraft.service';
import { AuthService } from '../../services/auth.service';
import { DataService } from '../../services/data.service';
import { FlightPlanService } from '../../services/flight-plan.service';

import { AircraftModel } from '../../models/aircraft.model';
import { FlightPlanRequest } from '../../models/request-model';

import Swal from 'sweetalert2';

@Component({
  selector: 'app-insert-fly-data',
  standalone: true,
  imports: [
    CommonModule,

    MatCardModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatInputModule,
    MatNativeDateModule,
    MatProgressBarModule,
    MatRadioModule,
    MatSelectModule,

    RouterModule,
    ReactiveFormsModule
  ],
  templateUrl: './insert-fly-data.component.html',
  styleUrl: './insert-fly-data.component.css'
})

export class InsertFlyDataComponent implements OnInit {

  flyDataForm!: FormGroup;
  isLoading: boolean = false;
  request: FlightPlanRequest | null = null;
  userId: string | null = null;

  errorMessage: string | null = null;
  successMessage: string | null = null;

  aircrafts: AircraftModel[] = [];

  constructor(
    private aircraftService: AircraftService,
    private authService: AuthService,
    private dataService: DataService,
    private fb: FormBuilder,
    private flightPlanService: FlightPlanService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.getAircrafts();
    this.initializeRegisterForm();
    this.getUserId();
    this.request = this.dataService.getFlyDataForm();

    if (this.request && localStorage.getItem('flyDataForm')) {
      this.flyDataForm.setValue({
        departureICAO: this.request.departureICAO,
        arrivalICAO: this.request.arrivalICAO,
        departureTime: this.request.departureTime,
        flightDuration: this.request.flightDuration,
        aircraftId: this.request.aircraftId
      });

      this.onSubmit();
    }
  }

  notLoggedInAlert(): void {
    Swal.fire({
      icon: 'info',
      title: 'Zaloguj się, aby kontynuować!',
      text: 'Aby wykonać analizę warunków pogodowych, zaloguj się na swoje konto lub zarejestruj, jeśli jeszcze go nie masz.',
      showCancelButton: true,
      confirmButtonText: 'Zaloguj się',
      cancelButtonText: 'Zarejestruj się'
    }).then((result) => {
      if (result.isConfirmed) {
        this.dataService.setFlyDataForm(this.flyDataForm.value);
        // Przekierowanie do strony logowania
        this.router.navigate(['/login']);
      } else if (result.dismiss === Swal.DismissReason.cancel) {
        // Przekierowanie do strony rejestracji
        this.router.navigate(['/register']);
      }
    });
  }

  clearSuccessMessage() {
    if (this.successMessage)
      this.successMessage = null;
  }

  getAircrafts(): void {
    this.aircraftService.getAircrafts().subscribe({
      next: (response) => {
        this.aircrafts = response;
      },
      error: (error) => {
        this.errorMessage = error.error.message
      }
    });
  }

  getUserId(): void {
    this.userId = this.authService.getUserId();
  }

  initializeRegisterForm(): void {
    this.flyDataForm = this.fb.group({
      departureICAO: ['', [
        Validators.required,
        Validators.minLength(4),
        Validators.maxLength(4),
        Validators.pattern(/^[A-Z]{4}$/)
      ]],
      arrivalICAO: ['', [
        Validators.required,
        Validators.minLength(4),
        Validators.maxLength(4),
        Validators.pattern(/^[A-Z]{4}$/)
      ]],
      departureTime: ['', [
        Validators.required,
        Validators.minLength(4),
        Validators.maxLength(4),
        Validators.pattern(/^[0-9]{4}$/),
        this.validateTime
      ]],
      flightDuration: ['', [
        Validators.required,
        Validators.minLength(4),
        Validators.maxLength(4),
        Validators.pattern(/^[0-9]{4}$/),
        this.validateFlightDuration
      ]],
      aircraftId: ['', Validators.required]
    });
  }

  onICAOInput(controlName: string, event: Event): void {
    let input = (event.target as HTMLInputElement).value.toUpperCase();
    input = input.replace(/[^A-Z]/g, '').slice(0, 4);
    this.flyDataForm.controls[controlName].setValue(input, { emitEvent: false });
  }

  onTimeInput(event: Event): void {
    let input = (event.target as HTMLInputElement).value;
    input = input.replace(/[^0-9]/g, '').slice(0, 4);
    this.flyDataForm.controls['departureTime'].setValue(input, { emitEvent: false });
  }

  onFlightDurationInput(event: Event): void {
    let input = (event.target as HTMLInputElement).value;
    input = input.replace(/[^0-9]/g, '').slice(0, 4);
    this.flyDataForm.controls['flightDuration'].setValue(input, { emitEvent: false });
  }

  onSubmit(): void {
    if (this.flyDataForm.invalid) {
      return;
    }

    if (!this.userId && !this.authService.isLoggedIn()) {
      this.notLoggedInAlert();
      return;
    }

    this.isLoading = true;
    this.request = this.prepareFlightPlanRequest();
    localStorage.removeItem('flyDataForm');

    this.flightPlanService.createFlightPlan(this.request).subscribe({
      next: (response) => {
        this.isLoading = false;
        const responseId = response.responseId;
        this.router.navigate(['/response', responseId]);
      },
      error: (error) => {
        this.errorMessage = error.error.message;
        this.isLoading = false;
      }
    });
  }

  prepareFlightPlanRequest(): FlightPlanRequest {
    return {
      id: 0,
      departureICAO: this.flyDataForm.value.departureICAO,
      arrivalICAO: this.flyDataForm.value.arrivalICAO,
      departureTime: this.flyDataForm.value.departureTime,
      flightDuration: this.flyDataForm.value.flightDuration,
      aircraftId: this.flyDataForm.value.aircraftId,
      userId: this.userId!
    };
  }

  validateFlightDuration(control: any): { [key: string]: boolean } | null {
    const value = control.value;

    // Sprawdzenie, czy wartość jest czterocyfrową liczbą
    if (!/^[0-9]{4}$/.test(value)) {
      return { invalidFormat: true };
    }

    // Pobranie godzin i minut z wartości
    const hours = parseInt(value.substring(0, 2), 10);
    const minutes = parseInt(value.substring(2, 4), 10);

    // Sprawdzenie, czy godziny są w zakresie 0-5 i minuty w zakresie 00-59
    if (hours < 0 || hours > 6 || minutes < 0 || minutes > 59) {
      return { invalidDuration: true };
    }

    // Sprawdzenie, czy wartość nie jest 0000
    if (hours === 0 && minutes === 0) {
      return { zeroDuration: true };
    }

    return null;
  }


  validateTime(control: any): { [key: string]: boolean } | null {
    const value = control.value;

    // Sprawdzenie, czy wartość jest czterocyfrową liczbą
    if (!/^[0-9]{4}$/.test(value)) {
      return { invalidFormat: true };
    }

    // Pobranie godzin i minut z wartości
    const hours = parseInt(value.substring(0, 2), 10);
    const minutes = parseInt(value.substring(2, 4), 10);

    // Sprawdzenie, czy godziny są w zakresie 00-23 i minuty w zakresie 00-59
    if (hours < 0 || hours > 23 || minutes < 0 || minutes > 59) {
      return { invalidTime: true };
    }

    // Pobranie bieżącego czasu UTC
    const now = new Date();
    const currentUTCHours = now.getUTCHours();
    const currentUTCMinutes = now.getUTCMinutes();
    const currentUTCTime = currentUTCHours * 100 + currentUTCMinutes;

    const enteredTime = hours * 100 + minutes; // Wprowadzona godzina jako liczba

    if (enteredTime < currentUTCTime) {
      return { timePassed: true }; // Błąd: Wprowadzona godzina UTC już minęła
    }

    return null; // Wszystko OK
  }
}
