import { Injectable, Injector } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { UserService } from "../services/user.service";

@Injectable()
export class TokenInterceptor implements HttpInterceptor {

    constructor(private readonly userService: UserService) {
    }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

        request = request.clone({
            setHeaders: {
                Authorization: `Bearer ${this.userService.getToken()}`
            }
        });

        return next.handle(request);
    }
}