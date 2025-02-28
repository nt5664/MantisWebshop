import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { TokenService } from '../../services/token-service.service';
import { ActionResponse } from '../../models/action-response';
import { InputText } from 'primeng/inputtext';
import { FormsModule } from '@angular/forms';
import { Password } from 'primeng/password';
import { Button } from 'primeng/button';
import { Message } from 'primeng/message';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [InputText, FormsModule, Password, Button, Message, NgIf],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  public errorMessage: string | null = null;

  email = '';
  password = '';

  constructor(
    private http: HttpClient,
    private router: Router,
    private tokenService: TokenService
  ) {}

  login() {
    const body = {
      email: this.email,
      password: this.password,
    };
    this.http
      .post<ActionResponse>('https://localhost:7153/login', body)
      .subscribe({
        next: (res) => {
          this.tokenService.setToken(res.data);
          return this.router.navigate(['/']);
        },
        error: (error) => {
          console.log('***** ERROR *****', error);
          this.errorMessage = error.error.title || 'Invalid Credentials';
          this.email = '';
          this.password = '';
        },
      });
  }
}
