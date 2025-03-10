import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, Subject, catchError, tap, throwError } from 'rxjs';
import { User } from '../models/user-model';
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


  constructor(
    private http: HttpClient,
    private jwtService: JwtService,
    private router: Router) { }

  generateNewToken(): Observable<any> {
    const token = this.jwtService.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get(`${this.apiUrl}/refresh`, { headers });
  }

  getName(): string | null {
    const token = this.jwtService.getToken();
    if (!token) return null;
    const decodedToken = this.jwtService.decodeToken(token);
    return decodedToken.unique_name + ' ' + decodedToken.surname;
  }

  getUserRole(): string[] {
    const token = this.jwtService.getToken();
    if (!token) return [];
    const decodedToken = this.jwtService.decodeToken(token);
    return Array.isArray(decodedToken.role) ? decodedToken.role : [decodedToken.role];
  }

  getUserId(): string | null {
    const token = this.jwtService.getToken();
    if (!token) return null;
    const decodedToken = this.jwtService.decodeToken(token);
    return decodedToken.sub;
  }

  isAdmin(): boolean {
    const roles = this.getUserRole();
    return roles.includes('Admin');
  }

  isPilot(): boolean {
    const roles = this.getUserRole();
    return roles.includes('Pilot');
  }

  isLoggedIn(): boolean {
    const token = this.jwtService.getToken();
    return !!token && !this.jwtService.isTokenExpired();
  }

  login(username: string, password: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, { username, password }).pipe(
      tap(res => {

        const token = res.token?.result;
        if (token) {
          this.jwtService.setToken(token);
          this.jwtService.decodeToken(token);
          this.loggedIn = true;
          this.loginSuccess.next();
        } else {
          this.jwtService.clearToken();
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
      localStorage.removeItem('token');
      this.http.post(`${this.apiUrl}/logout`, {}).subscribe({
        next: () => {
          this.router.navigate(['/home']);
        },
        error: (error) => {
          console.error('Error during logout:', error);
        }
      });
    }
  }

  deleteUser(id: string): Observable<any> {
    const token = this.jwtService.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.delete(`${this.apiUrl}/delete/${id}`, { headers });
  }

  register(userData: any): Observable<any> {
    const token = this.jwtService.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post(`${this.apiUrl}/register`, userData, { headers });
  }

  update(user: User): Observable<User> {
    const token = this.jwtService.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.put(`${this.apiUrl}/update`, user, { headers });
  }
}