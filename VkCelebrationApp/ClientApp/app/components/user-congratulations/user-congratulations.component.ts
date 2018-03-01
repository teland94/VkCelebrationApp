import { Component, OnInit } from '@angular/core';
import { UserCongratulationsService } from '../../services/user-congratulations.service';
import { UserCongratulation } from '../../models/user-congratulation.model';
import { ToastrService } from 'ngx-toastr';

@Component({
    selector: 'app-user-congratulations',
    templateUrl: './user-congratulations.component.html',
    styleUrls: ['./user-congratulations.component.css']
})
export class UserCongratulationsComponent implements OnInit {
    baseUrl = 'http://vk.com/id';

    userCongratulations: UserCongratulation[];

    constructor(private readonly userCongratulationsService: UserCongratulationsService,
        private toastr: ToastrService) {

    }

    ngOnInit() {
        this.load();
    }

    load() {
        this.userCongratulationsService.getUserCongratulations().subscribe((data: UserCongratulation[]) => {
            this.userCongratulations = data;
        }, () => {
            this.showErrorToast('Ошибка загрузки истории поздравлений');
        })
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