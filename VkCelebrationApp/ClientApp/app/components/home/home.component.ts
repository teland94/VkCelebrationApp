import { Component } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { User } from '../../models/user.model';
import { VkCelebrationService } from '../../services/vk-celebration.service';

@Component({
    selector: 'home',
    templateUrl: './home.component.html'
})
export class HomeComponent {
    baseUrl = 'http://vk.com/id';

    usersList: User[];
    selectedUser: User;
    usersCount: number;

    constructor(private readonly vkUsersService: VkCelebrationService,
        private readonly toastrService: ToastrService) {

    }

    ngOnInit() {
        this.seachUsers();
    }

    seachUsers() {
        this.vkUsersService.search(15, 25).subscribe((data: User[]) => {
            this.usersList = data;

            window.scrollTo(0, 0);
        });
    }

    navigate() {
        const date = new Date();
        const link = 'https://vk.com/friends' +
            '?act=find&c%5Bbday%5D=' + date.getDate() +
            '&c%5Bbmonth%5D=' + (date.getMonth() + 1) +
            '&c%5Bcity%5D=280&c%5Bcountry%5D=2&c%5Bname%5D=1&c%5Bonline%5D=1&c%5Bphoto%5D=1&c%5Bsection%5D=people&c%5Bsex%5D=1&c%5Bsort%5D=1';

        if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
            window.location.replace(link);
        } else {
            window.open(
                link,
                '_blank'
            );
        }
    }

    detectAge(uid: number, firstName: string, lastName: string) {
        this.vkUsersService.detectAge(uid, firstName, lastName, 15, 25).subscribe((data: number) => {
            const user = this.usersList.find(u => u.id === uid);
            if (user) {
                user.age = data;
            }

            this.toastrService.success('Возраст успешно определен');
        });
    }

    sendMessageOpen(user: any) {
        this.selectedUser = user;
    }

    isValidDate(dateStr: string) {
        let isValid = true;

        if (!dateStr) {
            isValid = false;
        } else {
            const dateParts = dateStr.split('.');
            if (dateParts.length < 3) {
                isValid = false;
            }
        }

        return isValid;
    }
}
