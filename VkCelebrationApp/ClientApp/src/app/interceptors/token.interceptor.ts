import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpHandler, HttpRequest, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {

  constructor(private readonly authService: AuthService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!this.authService.isLoggedIn()) { next.handle(req); }

    const dummyrequest = req.clone({
      setHeaders: {
        'Authorization': 'Bearer ' + this.authService.getToken()
      }
    });

    return next.handle(dummyrequest);
  }
}
