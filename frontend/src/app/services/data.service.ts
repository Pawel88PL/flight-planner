import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DataService {

  // BehaviorSubject do przechowywania wiadomości sukcesu
  private successMessage = new BehaviorSubject<string | null>(null);
  successMessage$ = this.successMessage.asObservable();

  constructor() { }

  // Metoda do pobierania i czyszczenia wiadomości sukcesu (opcjonalnie)
  clearSuccessMessage() {
    this.successMessage.next(null);
  }

  // Metoda do ustawiania wiadomości sukcesu
  setSuccessMessage(message: string | null) {
    this.successMessage.next(message);
  }
}
