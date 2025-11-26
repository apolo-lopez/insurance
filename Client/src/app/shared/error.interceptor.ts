import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { NotificationService } from './notification.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
    const notificationService = inject(NotificationService);

    return next(req).pipe(
        catchError((error: HttpErrorResponse) => {
            let userMessage = 'An unexpected error occurred.';
            if(error.error && error.error.error) {
                userMessage = error.error.error;
            }

            notificationService.showError(userMessage);
            return throwError(() => error);
        })
    )
};