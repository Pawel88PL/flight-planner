import { AuthService } from '../../services/auth.service';
import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { Router, RouterModule } from '@angular/router';
import { Role } from '../../models/user-model';
import { DataService } from '../../services/data.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    MatButton,
    MatFormFieldModule,
    MatInputModule,
    MatProgressBarModule,
    MatSelectModule,
    RouterModule,
    ReactiveFormsModule
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})

export class RegisterComponent implements OnInit, AfterViewInit {

  @ViewChild('autoFocusInput') autoFocusInput!: ElementRef;

  registerForm!: FormGroup;
  errorMessage: string | null = null;
  isLoading: boolean = false;
  successMessage: string | null = null;

  roles: Role[] = [
    { id: 1, name: 'Admin' },
    { id: 2, name: 'Pilot' }
  ];

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private dataService: DataService,
    private router: Router) { }

  ngOnInit(): void {
    this.initialeRegisterForm();
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.autoFocusInput.nativeElement.focus();
    }, 0);
  }

  emailMatchValidator(form: FormGroup): ValidationErrors | null {
    const email = form.get('email')?.value;
    const confirmEmail = form.get('confirmEmail')?.value;
    if (email && confirmEmail && email !== confirmEmail) {
      form.get('confirmEmail')?.setErrors({ mismatch: true });
      return { mismatch: true };
    } else if (email && !confirmEmail) {
      form.get('confirmEmail')?.setErrors({ mismatch: true });
      return { mismatch: true };
    } else {
      form.get('confirmEmail')?.setErrors(null);
      return null;
    }
  }

  initialeRegisterForm(): void {
    this.registerForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50), this.lettersOnly]],
      lastName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50), this.lettersOnly]],
      email: ['', [Validators.required, Validators.email]],
      confirmEmail: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required],
      role: ['', Validators.required]
    }, { validator: [this.passwordMatchValidator, this.emailMatchValidator] });
  }

  lettersOnly(control: AbstractControl): ValidationErrors | null {
    const letters = /^[A-Za-ząćęłńóśźżĄĆĘŁŃÓŚŹŻ]+$/;
    return letters.test(control.value) ? null : { 'lettersOnly': true };
  }

  passwordMatchValidator(form: FormGroup): ValidationErrors | null {
    const password = form.get('password')?.value;
    const confirmPassword = form.get('confirmPassword')?.value;
    if (password && confirmPassword && password !== confirmPassword) {
      form.get('confirmPassword')?.setErrors({ mismatch: true });
      return { mismatch: true };
    } else if (password && !confirmPassword) {
      form.get('confirmPassword')?.setErrors({ mismatch: true });
      return { mismatch: true };
    } else {
      form.get('confirmPassword')?.setErrors(null);
      return null;
    }
  }

  onSubmit(): void {
    if (this.registerForm.valid) {
      this.isLoading = true;
      this.authService.register(this.registerForm.value).subscribe({
        next: () => {
          this.isLoading = false;
          this.errorMessage = null;
          // Ustaw widok na listę użytkowników i wiadomość o sukcesie
          this.dataService.setSuccessMessage('Dodano nowego użytkownika');
          this.router.navigate(['/home']);
        },
        error: error => {
          this.isLoading = false;
          this.errorMessage = this.extractErrorMessage(error);
          console.error(error);
        }
      });
    }
  }


  private extractErrorMessage(error: any): string {
    // Sprawdzenie, czy obiekt błędu zawiera tablicę w polu 'error'
    if (error && Array.isArray(error.error) && error.error.length > 0 && error.error[0].description) {
      return error.error[0].description; // Zwrócenie opisu błędu
    }
    return 'Wystąpił błąd podczas rejestracji użytkownika. Spróbuj ponownie później.';
  }
}
