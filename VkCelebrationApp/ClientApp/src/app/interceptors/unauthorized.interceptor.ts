import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpEvent, HttpRequest, HttpHandler, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, empty } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable()
export class UnauthorizedInterceptor implements HttpInterceptor {
  constructor(
    private router: Router,
    private authService: AuthService
  ) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
    catchError(err => {
      if (err instanceof HttpErrorResponse && err.status === 401) {
        this.authService.logout();
        this.router.navigate(['/login']);

        // this response is handled
        // stop the chain of handlers by returning empty
        return empty();
      }

      // rethrow so other error handlers may pick this up
      return throwError(err);
    }));
  }
}
