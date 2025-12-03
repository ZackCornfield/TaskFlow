import { inject } from '@angular/core/primitives/di';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth';

export const guestGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // If user is authenticated, redirect to dashboard
  if (authService.isAuthenticated() && !authService.isTokenExpired()) {
    router.navigate(['dashboard']);
    return false;
  }

  return true;
};
