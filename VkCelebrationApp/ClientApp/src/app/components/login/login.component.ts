import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { Router } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { Credentials } from 'src/app/models/credentials.model';
import { ToastrService } from 'ngx-toastr';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  errors: string;
  isRequesting: boolean;
  submitted = false;

  form: FormGroup;
  login = new FormControl('');
  password = new FormControl('');

  constructor(private readonly router: Router,
    private fb: FormBuilder,
    private readonly authService: AuthService,
    private readonly toastr: ToastrService) { }

  ngOnInit() {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/home'], { replaceUrl: true });
    }

    this.form = this.fb.group({
      login: this.login,
      password: this.password
    });
  }

  // Handling after autofill from browser
  focus(event: any) {
    const elem = this.form.controls[event.srcElement.name];
    if (!elem.validator) {
      elem.setValidators([Validators.required]);
      elem.updateValueAndValidity();
    }
  }

  submit() {
    this.submitted = true;
    this.isRequesting = true;
    this.errors = '';
    const credentials = this.form.value as Credentials;
    if (this.form.valid) {
      this.authService.auth(credentials.login, credentials.password)
        .pipe(finalize(() => this.isRequesting = false))
        .subscribe(
        result => {
          if (result) {
            this.router.navigate(['/home'], { replaceUrl: true });
          }
        },
        error => {
          console.log(error);
          const loginFailure = error.error.login_failure;
          if (loginFailure && loginFailure[0]) {
            this.errors = loginFailure[0];
          } else {
            this.toastr.error(error.message);
          }
        });
    }
  }
}

