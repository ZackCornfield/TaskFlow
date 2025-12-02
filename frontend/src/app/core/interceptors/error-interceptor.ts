import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Error } from '../services/error';
import { catchError } from 'rxjs/internal/operators/catchError';
import { throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const errorService = inject(Error);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'An unknown error occurred';

      if (error.error instanceof ErrorEvent) {
        // Client-side error
        errorMessage = `Error: ${error.error.message}`;
      } else {
        // Server-side error
        errorMessage = error.error?.message || error.error || error.statusText;

        // Handle specific status codes
        switch (error.status) {
          case 400:
            errorMessage = error.error || 'Bad Request';
            break;
          case 401:
            errorMessage = 'Unauthorized. Please log in.';
            break;
          case 403:
            errorMessage = 'Access Denied.';
            break;
          case 404:
            errorMessage = error.error || 'Resource not found.';
            break;
          case 409:
            errorMessage = error.error || 'Conflict occurred.';
            break;
          case 500:
            errorMessage = 'Internal Server Error. Please try again later.';
            break;
        }
      }

      // Show the error using the Error service
      errorService.showError(errorMessage);

      return throwError(() => errorMessage);
    })
  );
};
