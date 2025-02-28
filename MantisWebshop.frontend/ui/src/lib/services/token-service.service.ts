import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TokenService {
  private static TOKEN_KEY = 'access_token';

  setToken(token: string): void {
    if (!token || token.length === 0) {
      throw new Error('Token is empty');
    }

    sessionStorage.setItem(TokenService.TOKEN_KEY, token);
  }

  getToken(): string | null {
    return sessionStorage.getItem(TokenService.TOKEN_KEY);
  }

  removeToken(): void {
    sessionStorage.removeItem(TokenService.TOKEN_KEY);
  }

  isLoggedIn(): boolean {
    return this.getToken() !== null;
  }
}
