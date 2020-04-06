import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ToastrModule } from 'ngx-toastr';
import { ModalModule } from 'ngx-bootstrap/modal';
import { NgSelectModule } from '@ng-select/ng-select';
import { NgProgressModule } from 'ngx-progressbar';
import { NgProgressHttpModule } from 'ngx-progressbar/http';
import { ClipboardModule } from 'ngx-clipboard';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { AccordionModule } from 'ngx-bootstrap/accordion';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { AutosizeModule } from 'ngx-autosize';
import { ServiceWorkerModule } from '@angular/service-worker';
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
import { SearchSettingsComponent } from './components/search-settings/search-settings.component';
import { CongratulationTemplatesComponent } from './components/congratulation-templates/congratulation-templates.component';
import { UserCongratulationsComponent } from './components/user-congratulations/user-congratulations.component';
import { AboutComponent } from './components/about/about.component';

import { DataService } from './services/data.service';
import { UserService } from './services/user.service';
import { VkCelebrationService } from './services/vk-celebration.service';
import { CongratulationTemplatesService } from './services/congratulation-templates.service';
import { UserCongratulationsService } from './services/user-congratulations.service';
import { AuthService } from './services/auth.service';
import { VkDatabaseService } from './services/vk-database.service';
import { UtilitiesService } from './services/utilities.service';
import { AuthGuard } from './guards/auth.guard';
import { TokenInterceptor } from './interceptors/token.interceptor';
import { UnauthorizedInterceptor } from './interceptors/unauthorized.interceptor';
import { environment } from '../environments/environment';

@NgModule({
  declarations: [
    AppComponent,
    AutofocusDirective,
    NavMenuComponent,
    LoginComponent,
    HomeComponent,
    VkUsersComponent,
    SearchSettingsComponent,
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
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-center',
      preventDuplicates: true,
    }),
    ModalModule.forRoot(),
    NgSelectModule,
    NgProgressModule,
    NgProgressHttpModule,
    ClipboardModule,
    BsDatepickerModule.forRoot(),
    AccordionModule.forRoot(),
    PaginationModule.forRoot(),
    TooltipModule.forRoot(),
    AutosizeModule,
    ServiceWorkerModule.register('ngsw-worker.js', { enabled: environment.production }),
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
    VkDatabaseService,
    UtilitiesService,
    { provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: UnauthorizedInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
