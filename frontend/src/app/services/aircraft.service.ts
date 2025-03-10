import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { JwtService } from './jwt.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AircraftModel } from '../models/aircraft.model';

@Injectable({
  providedIn: 'root'
})

export class AircraftService {

  private apiUrl = `${environment.apiUrl}/aircraft`;

  constructor(private jwtService: JwtService, private http: HttpClient) { }

  addAircraft(request: AircraftModel): Observable<any> {
    const token = this.jwtService.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post(`${this.apiUrl}/add`, request, { headers });
  }
}
