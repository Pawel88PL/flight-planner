import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DataService {

  private responseData: any;

  private errorMessage = new BehaviorSubject<string | null>(null);
  errorMessage$ = this.errorMessage.asObservable();

  private successMessage = new BehaviorSubject<string | null>(null);
  successMessage$ = this.successMessage.asObservable();

  setSuccessMessage(message: string) {
    this.successMessage.next(message);
    setTimeout(() => this.successMessage.next(null), 5000);
  }

  setErrorMessage(message: string) {
    this.errorMessage.next(message);
    setTimeout(() => this.errorMessage.next(null), 5000);
  }

  clearSuccessMessage() {
    this.successMessage.next(null);
  }

  getFlyDataForm(): any {
    return this.responseData;
  }

  getResponseData(): any {
    return this.responseData;
  }

  setFlyDataForm(data: any): void {
    this.responseData = data;
    localStorage.setItem('flyDataForm', 'true');
  }

  setResponseData(data: any): void {
    this.responseData = data;
  }
}