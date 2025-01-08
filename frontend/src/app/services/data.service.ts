import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DataService {

  private responseData: any;

  private successMessage = new BehaviorSubject<string | null>(null);
  successMessage$ = this.successMessage.asObservable();

  constructor() { }

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
  }

  setResponseData(data: any): void {
    this.responseData = data;
  }

  setSuccessMessage(message: string | null) {
    this.successMessage.next(message);
  }
}
