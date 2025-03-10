import { Injectable } from '@angular/core';
import { PagedRequestParams } from '../models/paged-request-params';
import { Observable } from 'rxjs';
import { UserListModel } from '../models/user-model';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { JwtService } from './jwt.service';

@Injectable({
  providedIn: 'root'
})

export class UsersService {

  private apiUrl = `${environment.apiUrl}`;

  constructor(
    private http: HttpClient,
    private jwtService: JwtService
  ) { }

  getUsersPaged(request: PagedRequestParams): Observable<UserListModel> {
    const token = this.jwtService.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    const params = new HttpParams()
      .set('pageNumber', request.pageNumber)
      .set('pageSize', request.pageSize)
      .set('sortColumn', request.sortColumn)
      .set('sortDirection', request.sortDirection)
      .set('searchQuery', request.searchQuery ?? '');

    return this.http.get<UserListModel>(`${this.apiUrl}/users/paged`, { headers, params });
  }
}
