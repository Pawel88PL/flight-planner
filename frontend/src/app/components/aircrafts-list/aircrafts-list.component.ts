import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatCardModule } from '@angular/material/card';

import { AircraftModel } from '../../models/aircraft.model';

import { AircraftService } from '../../services/aircraft.service';

@Component({
  selector: 'app-aircrafts-list',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule
  ],
  templateUrl: './aircrafts-list.component.html',
  styleUrl: './aircrafts-list.component.css'
})

export class AircraftsListComponent implements OnInit {

  
  constructor(
    private aircraftService: AircraftService
  ) { }

  ngOnInit(): void {
  }
}
