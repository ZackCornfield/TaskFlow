import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated() && !authService.isTokenExpired()) {
    return true;
  }

  // Redirect to login if not authenticated with return URL
  router.navigate(['/login'], {
    queryParams: { returnUrl: state.url },
  });
  return false;
};
