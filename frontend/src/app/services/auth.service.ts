import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, Subject, catchError, tap, throwError } from 'rxjs';
import { TwoFactorRequest, User } from '../models/user-model';
import { JwtService } from './jwt.service';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl = `${environment.apiUrl}/auth`;
  private loggedIn: boolean = false;
  private loginSuccess = new Subject<void>();
  loginSuccess$ = this.loginSuccess.asObservable();


  constructor(private http: HttpClient, private jwtService: JwtService, private router: Router) { }

  checkUserExists(email: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/checkUserExists`, { email: email });
  }

  private clearToken(): void {
    localStorage.removeItem('authToken');
    this.loggedIn = false;
  }

  getName(): string | null {
    const token = this.getToken();
    if (!token) return null;
    const decodedToken = this.jwtService.decodeToken(token);
    return decodedToken.unique_name + ' ' + decodedToken.surname;
  }

  getRoles(): Observable<any[]> {
    const token = this.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any[]>(`${this.apiUrl}/roles`, { headers });
  }

  getToken(): string | null {
    if (typeof localStorage !== 'undefined') {
      return localStorage.getItem('token');
    }
    return null;
  }

  getUsers(): Observable<User[]> {
    const token = this.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<User[]>(`${this.apiUrl}/getUsers`, { headers });
  }

  getUserById(id: string): Observable<User> {
    const token = this.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<User>(`${this.apiUrl}/getUser/${id}`, { headers });
  }

  getUserRole(): string[] {
    const token = this.getToken();
    if (!token) return [];
    const decodedToken = this.jwtService.decodeToken(token);
    return Array.isArray(decodedToken.role) ? decodedToken.role : [decodedToken.role];
  }

  getUserId(): string | null {
    const token = this.getToken();
    if (!token) return null;
    const decodedToken = this.jwtService.decodeToken(token);
    return decodedToken.sub;
  }

  handleTokenExpiration(token: string | null): void {
    if (token && this.jwtService.isTokenExpired(token)) {
      this.logout();
      console.log('Token expired. User logged out.');
      this.router.navigate(['/login']);
    }
  }

  isAdmin(): boolean {
    const roles = this.getUserRole();
    return roles.includes('Administrator');
  }

  isOperator(): boolean {
    const roles = this.getUserRole();
    return roles.includes('Operator');
  }

  isReporter(): boolean {
    const roles = this.getUserRole();
    return roles.includes('Zgłaszający');
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    return !!token && !this.jwtService.isTokenExpired(token);
  }

  login(username: string, password: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, { username, password }).pipe(
      tap(res => {
        // Zmiana sprawdzenia odpowiedzi na poprawną wiadomość
        if (res.message === "2FA") {
          // Przekierowanie do strony 2FA
          if (res.id) {
            this.router.navigate(['/two-factor-auth'], { queryParams: { id: res.id } });
            return;
          }
        }

        // Obsługa logowania, jeśli 2FA nie jest wymagane
        const token = res.token?.result;
        if (token) {
          this.setToken(token);
          this.jwtService.decodeToken(token);
          this.loggedIn = true;
        } else {
          this.clearToken();
          this.loggedIn = false;
          throw new Error('Token is missing');
        }
      }),
      catchError(error => {
        // Obsługa błędów logowania
        const errorMessage = error.error?.message || 'Wystąpił błąd podczas logowania. Spróbuj ponownie.';
        return throwError(() => errorMessage);
      })
    );
  }

  logout(): void {
    const token = localStorage.getItem('token');
    if (token) {
      this.http.post(`${this.apiUrl}/logout`, {}).subscribe({
        next: () => {
          localStorage.removeItem('token');
          this.router.navigate(['/home']);
        },
        error: (error) => {
          console.error('Error during logout:', error);
        }
      });
    }
  }

  createUser(userData: User): Observable<User> {
    return this.http.post(`${this.apiUrl}/create`, userData);
  }

  register(userData: any): Observable<any> {
    const token = this.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post(`${this.apiUrl}/register`, userData, { headers });
  }

  verifyTwoFactorCode(request: TwoFactorRequest): Observable<TwoFactorRequest> {
    return this.http.post<any>(`${this.apiUrl}/verify-2fa`, request).pipe(
      tap(res => {
        const token = res.token?.result;
        if (token) {
          this.setToken(token);
          this.jwtService.decodeToken(token);
          this.loggedIn = true;
          this.loginSuccess.next();
        } else {
          this.clearToken();
          this.loggedIn = false;
          throw new Error('Token is missing');
        }
      }),
      catchError(error => {
        let message = 'Nieprawidłowy kod 2FA.';
        if (error.status === 401) {
          message = error.error.message || 'Wystąpił błąd podczas weryfikacji kodu 2FA.';
        }
        return throwError(() => message);
      })
    );
  }


  update(user: User): Observable<User> {
    const token = this.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.put(`${this.apiUrl}/update`, user, { headers });
  }

  setToken(token: string): void {
    localStorage.setItem('token', token);
  }
}
