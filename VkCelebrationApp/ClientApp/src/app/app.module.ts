import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ToastrModule } from 'ngx-toastr';
import { ModalModule } from 'ngx-bootstrap/modal';
import { NgSelectModule } from '@ng-select/ng-select';
import { ClipboardModule } from 'ngx-clipboard';
import { BsDatepickerModule, CarouselModule, CarouselConfig, BsDropdownModule } from 'ngx-bootstrap';
import { NgxLoadingModule } from 'ngx-loading';
import { NgProgressModule } from '@ngx-progressbar/core';
import { NgProgressHttpModule } from '@ngx-progressbar/http';
import { routing } from './app.routing';

import { defineLocale } from 'ngx-bootstrap/chronos';
import { ruLocale } from 'ngx-bootstrap/locale';
defineLocale('ru', ruLocale);

import { AppComponent } from './components/app/app.component';
import { AutofocusDirective } from './directives/autofocus.directive';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { LoginComponent } from './components/login/login.component';
import { HomeComponent } from './components/home/home.component';
import { VkUsersComponent } from './components/vk-users/vk-users.component';
import { CongratulationTemplatesComponent } from './components/congratulation-templates/congratulation-templates.component';
import { UserCongratulationsComponent } from './components/user-congratulations/user-congratulations.component';
import { AboutComponent } from './components/about/about.component';

import { DataService } from './services/data.service';
import { UserService } from './services/user.service';
import { VkCelebrationService } from './services/vk-celebration.service';
import { CongratulationTemplatesService } from './services/congratulation-templates.service';
import { UserCongratulationsService } from './services/user-congratulations.service';
import { AuthService } from './services/auth.service';
import { AuthGuard } from './guards/auth.guard';
import { TokenInterceptor } from './interceptors/token.interceptor';
import { UnauthorizedInterceptor } from './interceptors/unauthorized.interceptor';

@NgModule({
  declarations: [
    AppComponent,
    AutofocusDirective,
    NavMenuComponent,
    LoginComponent,
    HomeComponent,
    VkUsersComponent,
    CongratulationTemplatesComponent,
    UserCongratulationsComponent,
    AboutComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    ToastrModule.forRoot(),
    ModalModule.forRoot(),
    NgSelectModule,
    ClipboardModule,
    BsDatepickerModule.forRoot(),
    NgxLoadingModule,
    NgProgressModule,
    NgProgressHttpModule,
    CarouselModule.forRoot(),
    routing
  ],
  providers: [
    DataService,
    AuthService,
    AuthGuard,
    UserService,
    VkCelebrationService,
    CongratulationTemplatesService,
    UserCongratulationsService,
    { provide: CarouselConfig, useValue: { interval: 0 } },
    { provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: UnauthorizedInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
