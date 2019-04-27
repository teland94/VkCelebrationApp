import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AuthGuard } from './guards/auth.guard';

import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/login/login.component';
import { CongratulationTemplatesComponent } from './components/congratulation-templates/congratulation-templates.component';
import { UserCongratulationsComponent } from './components/user-congratulations/user-congratulations.component';
import { AboutComponent } from './components/about/about.component';

const appRoutes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent, canActivate: [AuthGuard] },
  { path: 'login', component: LoginComponent },
  { path: 'congratulation-templates', component: CongratulationTemplatesComponent, canActivate: [AuthGuard] },
  { path: 'user-congratulations', component: UserCongratulationsComponent, canActivate: [AuthGuard] },
  { path: 'about', component: AboutComponent }
];

export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes);
