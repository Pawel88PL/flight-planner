import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';
import { Observable } from 'rxjs';
import { FlightPlanRequest } from '../models/flight-plan-request.model';

@Injectable({
  providedIn: 'root'
})
export class FlightPlanRequestService {

  private apiUrl = `${environment.apiUrl}/flight-plan-request`;

  constructor(private authService: AuthService, private http: HttpClient) { }

  addNewFlightPlanRequest(flightPlanRequest: FlightPlanRequest): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/create`, flightPlanRequest);
  }
}
