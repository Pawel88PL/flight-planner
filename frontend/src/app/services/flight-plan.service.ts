import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';
import { Observable } from 'rxjs';
import { FlightPlanRequest } from '../models/request-model';
import { FlightPlanResponse } from '../models/response-model';

@Injectable({
  providedIn: 'root'
})

export class FlightPlanService {

  private apiUrl = `${environment.apiUrl}/flight-plan`;

  constructor(private authService: AuthService, private http: HttpClient) { }

  createFlightPlan(request: FlightPlanRequest): Observable<any> {
    const token = this.authService.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post<any>(`${this.apiUrl}/create`, request, { headers });
  }

  getFlightPlanById(id: string): Observable<FlightPlanResponse> {
    const token = this.authService.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<FlightPlanResponse>(`${this.apiUrl}/get/${id}`, { headers });
  }

  getFlightPlans(): Observable<FlightPlanResponse[]> {
    const token = this.authService.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<FlightPlanResponse[]>(`${this.apiUrl}/get-flight-plans-by-userId`, { headers });
  }
}
