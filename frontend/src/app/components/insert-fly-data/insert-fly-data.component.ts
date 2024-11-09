import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';

import { MatButton } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';

import { Router, RouterModule } from '@angular/router';

import { AuthService } from '../../services/auth.service';
import { DataService } from '../../services/data.service';

@Component({
  selector: 'app-insert-fly-data',
  standalone: true,
  imports: [
    CommonModule,

    MatButton,
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
  errorMessage: string | null = null;
  isLoading: boolean = false;
  successMessage: string | null = null;

  aircrafts = [
    { id: 1, name: 'Cessna 172' },
    { id: 2, name: 'Piper PA-28' },
    { id: 3, name: 'Diamond DA40' },
    { id: 4, name: 'Beechcraft Bonanza' }
  ];

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private dataService: DataService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.initializeRegisterForm();
  }

  initializeRegisterForm(): void {
    this.flyDataForm = this.fb.group({
      departure: ['', [
        Validators.required,
        Validators.minLength(4),
        Validators.maxLength(4),
        Validators.pattern(/^[A-Z]{4}$/)
      ]],
      arrival: ['', [
        Validators.required,
        Validators.minLength(4),
        Validators.maxLength(4),
        Validators.pattern(/^[A-Z]{4}$/)
      ]],
      flightDay: ['today', Validators.required],
      departureTime: ['', [
        Validators.required,
        Validators.minLength(4),
        Validators.maxLength(4),
        Validators.pattern(/^[0-9]{4}$/)
      ]],
      flightDuration: ['', [
        Validators.required,
        Validators.minLength(4),
        Validators.maxLength(4),
        Validators.pattern(/^[0-9]{4}$/)
      ]],
      aircraft: ['', Validators.required]
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

  onSubmit(): void { }
}
