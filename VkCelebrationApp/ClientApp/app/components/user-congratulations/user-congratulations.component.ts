import { Component, OnInit } from '@angular/core';
import { UserCongratulationsService } from '../../services/user-congratulations.service';
import { UserCongratulation } from '../../models/user-congratulation.model';
import { ToastrService } from 'ngx-toastr';
import { ruLocale } from 'ngx-bootstrap/locale';
import { BsLocaleService } from 'ngx-bootstrap';

@Component({
    selector: 'app-user-congratulations',
    templateUrl: './user-congratulations.component.html',
    styleUrls: ['./user-congratulations.component.css']
})
export class UserCongratulationsComponent implements OnInit {
    baseUrl = 'http://vk.com/id';

    userCongratulations: UserCongratulation[];

    minDate = new Date(2018, 1, 1);
    maxDate = new Date();

    currentDate = new Date();

    constructor(private readonly userCongratulationsService: UserCongratulationsService,
        private readonly toastr: ToastrService,
        private readonly localeService: BsLocaleService) {

    }

    ngOnInit() {
        this.localeService.use('ru');
        this.load();
    }

    load() {
        let date = new Date(this.currentDate.getFullYear(), this.currentDate.getMonth(), this.currentDate.getDate(), 0, 0, 0);
        this.userCongratulationsService.getUserCongratulations(date).subscribe((data: UserCongratulation[]) => {
            this.userCongratulations = data;
        }, () => {
            this.showErrorToast('Ошибка загрузки истории поздравлений');
        });
    }

    dateChanged(date: Date) {
        if (this.currentDate !== date) {
            this.currentDate = date;
            this.load();
        }
    }

    showErrorToast(message: string) {
        const errToast = this.toastr.error(message, 'Ошибка');
        if (errToast) {
            errToast.onTap.subscribe(() => {
                this.load();
            });
        }
    }
}