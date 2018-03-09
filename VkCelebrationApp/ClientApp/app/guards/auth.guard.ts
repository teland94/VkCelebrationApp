// auth.guard.ts
import { Injectable } from '@angular/core';
import { Router, CanActivate } from '@angular/router';
import { UserService } from '../services/user.service';

@Injectable()
export class AuthGuard implements CanActivate {
    constructor(private readonly user: UserService, private readonly router: Router) { }

    canActivate() {

        if (!this.user.isLoggedIn()) {
            this.router.navigate(['/account/login']);
            return false;
        }

        return true;
    }
}