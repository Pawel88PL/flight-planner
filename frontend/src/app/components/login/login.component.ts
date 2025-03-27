import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';

import { Router } from '@angular/router';
import { RouterModule } from '@angular/router';
import gsap from 'gsap';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,

    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressBarModule,

    RouterModule,
    ReactiveFormsModule,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})

export class LoginComponent implements OnInit, AfterViewInit {

  isLoading: boolean = false;
  errorMessage: string | null = null;
  loginForm!: FormGroup;

  // Referencja do pola input dla userName
  @ViewChild('emailInput') emailInput!: ElementRef<HTMLInputElement>;

  constructor(
    private authService: AuthService,
    private formBuilder: FormBuilder,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.initialeLoginForm();
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.emailInput.nativeElement.focus();
      gsap.from('.loginForm', { duration: 0.5, y: -100, opacity: 0, stagger: 0.2 });
    });
  }

  initialeLoginForm() {
    this.loginForm = this.formBuilder.group({
      username: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    })
  }

  onSubmit() {
    if (this.loginForm.valid) {
      this.isLoading = true;
      const loginData = this.loginForm.value;
      this.authService.login(loginData.username, loginData.password).subscribe({
        next: () => {
          this.errorMessage = null;
          this.isLoading = false;
          if (this.authService.isAdmin()) {
            this.router.navigate(['/admin']);
          } else {
            this.router.navigate(['/home']);
          }
        },
        error: (message) => {
          this.isLoading = false;
          this.errorMessage = message;
        }
      });
    }
  }

}