import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TokenService } from '../../services/token-service.service';
import { Router } from '@angular/router';
import { Avatar } from 'primeng/avatar';
import { Button } from 'primeng/button';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-state',
  standalone: true,
  imports: [Avatar, Button, NgIf],
  templateUrl: './state.component.html',
  styleUrl: './state.component.css',
})
export class StateComponent {
  constructor(
    private http: HttpClient,
    private router: Router,
    private tokenService: TokenService
  ) {}

  isLoggedIn(): boolean {
    return this.tokenService.isLoggedIn();
  }

  login() {
    if (this.isLoggedIn()) return;

    return this.router.navigate(['/login']);
  }

  register() {
    if (this.isLoggedIn()) return;

    return this.router.navigate(['/register']);
  }

  logout() {
    if (!this.isLoggedIn()) return;

    this.tokenService.removeToken();
  }
}
