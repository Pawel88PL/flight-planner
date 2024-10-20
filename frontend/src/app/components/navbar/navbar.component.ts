import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { RouterModule } from '@angular/router';
import { MatButton } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButton,
    MatIconModule,
    MatTooltipModule
  ],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})

export class NavbarComponent implements OnInit {

  userName: string | null = null;

  constructor(
    public authService: AuthService,
  ) { }

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.initializeNavbar();
    }

    this.authService.loginSuccess$.subscribe(() => {
      this.initializeNavbar();
    });
  }

  initializeNavbar(): void {
    this.userName = this.authService.getName();
  }
}
