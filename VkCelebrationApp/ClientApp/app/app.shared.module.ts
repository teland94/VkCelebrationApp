import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { ToastrModule } from 'ngx-toastr';
import { ModalModule } from 'ngx-bootstrap/modal';
import { NgSelectModule } from '@ng-select/ng-select';
import { ClipboardModule } from 'ngx-clipboard';

import { AppComponent } from './components/app/app.component';
import { AutofocusDirective } from './directives/autofocus.directive';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { CongratulationTemplatesComponent } from './components/congratulation-templates/congratulation-templates.component';
import { AboutComponent } from './components/about/about.component';

import { DataService } from './services/data.service';
import { VkCelebrationService } from './services/vk-celebration.service';
import { CongratulationTemplatesService } from './services/congratulation-templates.service';

@NgModule({
    declarations: [
        AppComponent,
        AutofocusDirective,
        NavMenuComponent,
        HomeComponent,
        CongratulationTemplatesComponent,
        AboutComponent
    ],
    imports: [
        CommonModule,
        HttpClientModule,
        FormsModule,
        ToastrModule.forRoot(),
        ModalModule.forRoot(),
        NgSelectModule,
        ClipboardModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'congratulation-templates', component: CongratulationTemplatesComponent },
            { path: 'about', component: AboutComponent },
            { path: '**', redirectTo: 'home' }
        ])
    ],
    providers: [
        DataService,
        VkCelebrationService,
        CongratulationTemplatesService
    ]
})
export class AppModuleShared {
}
