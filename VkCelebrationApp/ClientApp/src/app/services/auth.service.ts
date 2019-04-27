import { Injectable } from '@angular/core';
import { DataService } from './data.service';
import { map } from 'rxjs/operators';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private url = 'auth';
  private tokenName = 'auth_token';

  // Observable navItem source
  private _authNavStatusSource = new BehaviorSubject<boolean>(false);

  private loggedIn = false;

  constructor(private dataService: DataService) {
    this.loggedIn = !!localStorage.getItem(this.tokenName);
    this._authNavStatusSource.next(this.loggedIn);
  }

  // Observable navItem stream
  authNavStatus$ = this._authNavStatusSource.asObservable();

  getToken() {
    return localStorage.getItem(this.tokenName);
  }

  isLoggedIn() {
    return this.loggedIn;
  }

  auth(login: string, password: string) {
    return this.dataService.post(this.url, {
      login,
      password
    }).pipe(
      map((res: any) => {
        localStorage.setItem(this.tokenName, res.auth_token);
        this.loggedIn = true;
        this._authNavStatusSource.next(true);
        return true;
      })
    );
  }

  logout() {
    localStorage.removeItem(this.tokenName);
    this.loggedIn = false;
    this._authNavStatusSource.next(false);
  }
}
