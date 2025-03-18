import { CommonModule } from '@angular/common';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { DataService } from '../../services/data.service';
import { JwtService } from '../../services/jwt.service';
import { ToastrService } from 'ngx-toastr';
import { AircraftService } from '../../services/aircraft.service';
import { range } from 'rxjs';

@Component({
  selector: 'app-aircraft-add',
  standalone: true,
  imports: [
    CommonModule,

    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatSelectModule,

    RouterModule,
    ReactiveFormsModule
  ],
  templateUrl: './aircraft-add.component.html',
  styleUrl: './aircraft-add.component.css'
})
export class AircraftAddComponent implements OnInit {

  isLoading: boolean = false;

  aircraftAddForm!: FormGroup;
  errorMessage: string | null = null;
  successMessage: string = 'Dodano nowy samolot';

  constructor(
    private aircraftService: AircraftService,
    private fb: FormBuilder,
    private dataService: DataService,
    private router: Router,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    this.initialeAircraftAddForm();
  }

  goBack(): void {
    window.history.back();
  }

  initialeAircraftAddForm(): void {
    this.aircraftAddForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      manufacturer: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      model: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      cruiseSpeed: ['', [Validators.required, Validators.min(0), Validators.max(1000)]],
      range: ['', [Validators.required, Validators.min(0), Validators.max(10000)]],
      maxCrosswind: ['', [Validators.required]],
    });
  }

  onSubmit(): void {
    if (this.aircraftAddForm.valid) {
      this.isLoading = true;

      this.aircraftService.addAircraft(this.aircraftAddForm.value).subscribe({
        next: () => {
          this.isLoading = false;
          this.errorMessage = null;
          this.dataService.setSuccessMessage(this.successMessage);
          this.router.navigate(['/aircrafts-list']);
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = error.error.message;
          if (this.errorMessage) {
            this.toastr.error(this.errorMessage, 'Błąd rejestracji');
          } else {
            this.toastr.error('Wystąpił błąd podczas dodawania samolotu', 'Błąd rejestracji');
          }
          console.error(error);
        }
      });
    }
  }
}