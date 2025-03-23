import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';

import { AircraftService } from '../../services/aircraft.service';
import { DataService } from '../../services/data.service';
import { ToastrService } from 'ngx-toastr';
import { AircraftModel } from '../../models/aircraft.model';

@Component({
  selector: 'app-aircraft-edit',
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
  templateUrl: './aircraft-edit.component.html',
  styleUrl: './aircraft-edit.component.css'
})

export class AircraftEditComponent implements OnInit {

  isLoading: boolean = false;

  aircraftId: number | null = null;
  aircraftEditForm!: FormGroup;
  errorMessage: string | null = null;
  successMessage: string = 'Zapisano zmiany';

  constructor(
    private aircraftService: AircraftService,
    private fb: FormBuilder,
    private dataService: DataService,
    private route: ActivatedRoute,
    private router: Router,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    this.setAircraftId();
    this.initialeForm();
  }

  goBack(): void {
    window.history.back();
  }

  initialeForm(): void {
    this.aircraftEditForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      manufacturer: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      model: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      cruiseSpeed: ['', [Validators.required, Validators.min(0), Validators.max(1000)]],
      range: ['', [Validators.required, Validators.min(0), Validators.max(10000)]],
      maxCrosswind: ['', [Validators.required]],
    });
  }

  loadAircraftData(id: number): void {
    this.isLoading = true;
    this.aircraftService.getAircraftById(id).subscribe({
      next: (data) => {
        this.isLoading = false;
        this.errorMessage = null;
        this.aircraftEditForm.patchValue(data);
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error.message;
        if (this.errorMessage) {
          this.toastr.error(this.errorMessage, 'Błąd');
        } else {
          this.toastr.error('Wystąpił błąd podczas pobierania danych samolotu', 'Błąd');
        }
        console.error(error);
      }
    });
  }

  prepareAircraftModel(): AircraftModel {
    return {
      id: this.aircraftId || 0,
      name: this.aircraftEditForm.get('name')?.value,
      manufacturer: this.aircraftEditForm.get('manufacturer')?.value,
      model: this.aircraftEditForm.get('model')?.value,
      cruiseSpeed: this.aircraftEditForm.get('cruiseSpeed')?.value,
      range: this.aircraftEditForm.get('range')?.value,
      maxCrosswind: this.aircraftEditForm.get('maxCrosswind')?.value
    };
  }

  onSubmit(): void {
    if (this.aircraftEditForm.valid) {
      this.isLoading = true;

      const aircraftModel = this.prepareAircraftModel();

      this.aircraftService.editAircraft(aircraftModel).subscribe({
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
            this.toastr.error(this.errorMessage, 'Błąd');
          } else {
            this.toastr.error('Wystąpił błąd podczas edycji samolotu', 'Błąd');
          }
          console.error(error);
        }
      });
    }
  }

  setAircraftId() {
    this.aircraftId = Number(this.route.snapshot.paramMap.get('id'));
    if (this.aircraftId) {
      this.loadAircraftData(this.aircraftId);
    }
  }
}