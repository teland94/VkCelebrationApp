import { Injectable } from '@angular/core';
import { DataService } from './data.service';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { BaseService } from './base.service';

import 'rxjs/add/operator/catch';

@Injectable()
export class UserService extends BaseService {
    private url = "user";

    baseUrl: string = '';

    // Observable navItem source
    private authNavStatusSource = new BehaviorSubject<boolean>(false);
    // Observable navItem stream
    authNavStatus$ = this.authNavStatusSource.asObservable();

    private loggedIn = false;

    constructor(private readonly dataService: DataService) {
        super();
        this.loggedIn = !!localStorage.getItem('auth_token');
        // ?? not sure if this the best way to broadcast the status but seems to resolve issue on page refresh where auth status is lost in
        // header component resulting in authed user nav links disappearing despite the fact user is still logged in
        this.authNavStatusSource.next(this.loggedIn);
    }

    getUserInfo() {
        return this.dataService.get(this.url + '/' + 'getUserInfo');
    }


    logout() {
        localStorage.removeItem('auth_token');
        this.loggedIn = false;
        this.authNavStatusSource.next(false);
    }

    isLoggedIn() {
        return this.loggedIn;
    }

    facebookLogin(accessToken: string) {
        let headers = new Headers();
        headers.append('Content-Type', 'application/json');
        let body = JSON.stringify({ accessToken });
        return this.dataService
            .post(
            this.baseUrl + '/externalauth/facebook', body)
            .map((res:any) => {
                localStorage.setItem('auth_token', res.auth_token);
                this.loggedIn = true;
                this.authNavStatusSource.next(true);
                return true;
            })
            .catch(this.handleError);
    }
}