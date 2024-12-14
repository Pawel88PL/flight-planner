import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';
import { Observable } from 'rxjs';
import { AIResponseModel } from '../models/ai-response-model';

@Injectable({
  providedIn: 'root'
})

export class AiService {

  private apiUrl = `${environment.apiUrl}/ai-response`;

  constructor(private authService: AuthService, private http: HttpClient) { }

  getAIResponseByFlightPlanById(id: string): Observable<AIResponseModel> {
    return this.http.get<AIResponseModel>(`${this.apiUrl}/get/${id}`);
  }
}