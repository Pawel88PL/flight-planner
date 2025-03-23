import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';
import { Observable } from 'rxjs';
import { FlightPlanRequest } from '../models/request-model';
import { FlightPlanResponse } from '../models/response-model';
import { JwtService } from './jwt.service';
import { PagedRequestParams } from '../models/paged-request-params';

@Injectable({
  providedIn: 'root'
})

export class FlightPlanService {

  private apiUrl = `${environment.apiUrl}/flight-plan`;

  constructor(private jwtService: JwtService, private http: HttpClient) { }

  createFlightPlan(request: FlightPlanRequest): Observable<any> {
    const token = this.jwtService.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post<any>(`${this.apiUrl}/create`, request, { headers });
  }

  deleteFlightPlan(id: number): Observable<any> {
    const token = this.jwtService.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.delete<any>(`${this.apiUrl}/delete/${id}`, { headers });
  }

  getFlightPlanById(id: string): Observable<FlightPlanResponse> {
    const token = this.jwtService.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<FlightPlanResponse>(`${this.apiUrl}/get/${id}`, { headers });
  }

  getFlightPlans(): Observable<FlightPlanResponse[]> {
    const token = this.jwtService.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<FlightPlanResponse[]>(`${this.apiUrl}/get-flight-plans-by-userId`, { headers });
  }

  getFlightPlansPaged(request: PagedRequestParams): Observable<any> {
      const token = this.jwtService.getToken();
      const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  
      const params = new HttpParams()
        .set('pageNumber', request.pageNumber)
        .set('pageSize', request.pageSize)
        .set('sortColumn', request.sortColumn)
        .set('sortDirection', request.sortDirection)
        .set('searchQuery', request.searchQuery ?? '');
  
      return this.http.get(`${this.apiUrl}/paged`, { headers, params });
    }
}