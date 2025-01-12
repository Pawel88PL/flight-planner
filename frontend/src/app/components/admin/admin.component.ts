import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatCardModule } from '@angular/material/card';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [
    CommonModule,

    MatCardModule
  ],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.css'
})

export class AdminComponent implements OnInit {

  constructor(
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    this.welcomeAdmin();
  }

  welcomeAdmin(): void {
    this.toastr.success('Witaj w panelu administratora!', 'Witaj!');
    console.log('Welcome admin');
  }
}