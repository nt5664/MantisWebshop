import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { TokenService } from '../../services/token-service.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { InputText } from 'primeng/inputtext';
import { Password } from 'primeng/password';
import { Button } from 'primeng/button';
import { Message } from 'primeng/message';
import { ActionResponse } from '../../models/action-response';

interface RegistrationModel {
  email: string;
  name: string;
  password: string;
  confirmPassword: string;
}

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, InputText, Password, Button, Message],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  private readonly modelObj: RegistrationModel;

  errorMessage: string | null = null;

  constructor(
    private http: HttpClient,
    private router: Router,
    private tokenService: TokenService
  ) {
    this.modelObj = {
      email: '',
      name: '',
      password: '',
      confirmPassword: '',
    };
  }

  get registerModel(): RegistrationModel {
    return this.modelObj;
  }

  register() {
    const { confirmPassword, ...body } = this.modelObj;
    this.http.post<ActionResponse>('https://localhost:7153/signup', body).subscribe({
      next: res=> {
        if (res.message !== "Ok") {
          this.errorMessage = res.message;
          return;
        }

        return this.router.navigate(['/login']);
      },
      error: error => console.log(error)
    });
  }
}
