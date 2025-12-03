import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { LoadingService } from '../services/loading';
import { finalize } from 'rxjs';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const loadingService = inject(LoadingService);

  // Start loading
  loadingService.show();

  return next(req).pipe(
    // Stop loading on both success and error
    finalize(() => loadingService.hide())
  );
};
