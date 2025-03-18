import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { JwtService } from './jwt.service';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AircraftListModel, AircraftModel } from '../models/aircraft.model';
import { PagedRequestParams } from '../models/paged-request-params';

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

  getAircraftsPaged(request: PagedRequestParams): Observable<AircraftListModel> {
    const token = this.jwtService.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    const params = new HttpParams()
      .set('pageNumber', request.pageNumber)
      .set('pageSize', request.pageSize)
      .set('sortColumn', request.sortColumn)
      .set('sortDirection', request.sortDirection)
      .set('searchQuery', request.searchQuery ?? '');

    return this.http.get<AircraftListModel>(`${this.apiUrl}/paged`, { headers, params });
  }
}
