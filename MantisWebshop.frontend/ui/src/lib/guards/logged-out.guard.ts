import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { TokenService } from '../services/token-service.service';
import { Router } from '@angular/router';

export const loggedOutGuard: CanActivateFn = (route, state) => {
  const tokenService = inject(TokenService);
  const router = inject(Router);
  if (tokenService.isLoggedIn()) {
    return router.navigate(['/']);
  }

  return true;
};
