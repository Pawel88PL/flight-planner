import { Injectable } from '@angular/core';
import { interval, Subscription } from 'rxjs';
import { AuthService } from './auth.service';
import { ToastrService } from 'ngx-toastr';
import { JwtService } from './jwt.service';
import { MatDialog } from '@angular/material/dialog';


@Injectable({
  providedIn: 'root'
})

export class SessionService {

  shownAlert: boolean = false;
  private subscription: Subscription | null = null;

  constructor(
    private authService: AuthService,
    private jwtService: JwtService,
    private dialog: MatDialog,
    private toastr: ToastrService) { }

  startTokenExpirationCheck(): void {
    this.subscription = interval(30000).subscribe(() => {
      this.checkTokenExpiration();
    });
  }

  checkTokenExpiration(): void {
    const tokenExpiration = this.jwtService.getTokenExpirationDate();

    if (tokenExpiration) {
      const tokenExpirationDate = new Date(tokenExpiration);
      const currentDate = new Date();
      const timeDifference = tokenExpirationDate.getTime() - currentDate.getTime();
      const minutesDifference = Math.floor(timeDifference / 60000);

      console.log(`${minutesDifference} minutes to logout`);

      if (minutesDifference === 0) {
        this.shownAlert = true;
        this.showNewTokenAlert();
      } else if (minutesDifference < 0) {
        this.shownAlert = false;
        this.handleTokenExpiration(this.jwtService.getToken());
      }
    }
  }

  handleTokenExpiration(token: string | null): void {
    if (token && this.jwtService.isTokenExpired()) {
      console.log('Token expired');
      this.dialog.closeAll();
      this.authService.logout();
      this.toastr.error('Twoja sesja wygasła. Zaloguj się ponownie.', 'Sesja wygasła',
        {
          timeOut: 60000,
          extendedTimeOut: 6000,
          positionClass: 'toast-top-right',
          closeButton: true,
          tapToDismiss: true
        }
      );
    }
  }

  showNewTokenAlert(): void {
    let remainingTime = 60;

    const toast = this.toastr.info(
      `Za ${remainingTime} sekund wygaśnie twoja sesja. Kliknij tutaj, aby ją przedłużyć.`,
      'Sesja wygaśnie',
      {
        tapToDismiss: false,
        timeOut: 60000,
        extendedTimeOut: 0,
        progressBar: true,
        closeButton: true,
        enableHtml: true,
        positionClass: 'toast-top-right',
      }
    );

    const intervalId = setInterval(() => {

      if (!this.shownAlert) {
        clearInterval(intervalId);
        this.toastr.clear(toast.toastId);
        return;
      }

      remainingTime -= 1;

      const toastElement = document.getElementById(`toast-container`);
      if (toastElement) {
        const messageElement = toastElement.querySelector('.toast-message');
        if (messageElement) {
          messageElement.innerHTML = `Za ${remainingTime} sekund wygaśnie twoja sesja. Kliknij tutaj, aby ją przedłużyć.`;
        }
      }

      if (remainingTime <= 0) {
        clearInterval(intervalId);
        this.toastr.clear(toast.toastId);
      }
    }, 1000);

    toast.onTap.subscribe(() => {
      clearInterval(intervalId);
      this.toastr.clear(toast.toastId);

      this.authService.generateNewToken().subscribe({
        next: (res) => {
          this.jwtService.setToken(res.token?.result);
          this.toastr.success('Sesja została przedłużona.', 'Sukces');
          console.log('New token generated');
        },
        error: (error) => {
          console.error('Error generating new token:', error);
          this.toastr.error('Wystąpił błąd podczas przedłużania sesji.', 'Błąd');
        },
      });
    });
  }
}