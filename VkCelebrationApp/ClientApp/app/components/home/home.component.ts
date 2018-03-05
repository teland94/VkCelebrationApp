import { Component, ViewChild, TemplateRef, EventEmitter } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { ModalDirective, BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { VkUser } from '../../models/vk-user.model';
import { VkCelebrationService } from '../../services/vk-celebration.service';
import { CongratulationTemplatesService } from '../../services/congratulation-templates.service';
import { VkCollection } from '../../models/vk-collection.model';
import { UserCongratulation } from '../../models/user-congratulation.model';
import { CongratulationTemplate } from '../../models/congratulation-template.model';

import 'rxjs/add/operator/distinctUntilChanged';
import 'rxjs/add/operator/debounceTime';
import 'rxjs/add/operator/switchMap';

@Component({
    selector: 'home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css']
})
export class HomeComponent {
    baseUrl = 'http://vk.com/id';

    usersCollection: VkCollection;
    congratulationTemplates: CongratulationTemplate[];
    selectedUser: VkUser;

    @ViewChild(ModalDirective) congratulationModal: ModalDirective;
    typeahead = new EventEmitter<string>();
    isModalShown: boolean = false;

    messageText: string;
    template: string;

    public loading = false;

    constructor(private readonly vkCelebrationService: VkCelebrationService,
        private readonly congratulationTemplatesService: CongratulationTemplatesService,
        private readonly toastrService: ToastrService) {
    }

    ngOnInit() {
        this.seachUsers();

        this.typeahead
            .distinctUntilChanged()
            .debounceTime(200)
            .switchMap((text: string) => this.loadTemplates(text))
            .subscribe((items: CongratulationTemplate[]) => {
                this.congratulationTemplates = items;
            }, (err: any) => {
                this.congratulationTemplates = [];
                this.showErrorToast('Ошибка загрузки заготовок поздравлений', err);
            });
    }

    loadTemplates(text: string) {
        return this.congratulationTemplatesService.findCongratulationTemplates(text);
    }

    seachUsers() {
        this.vkCelebrationService.search().subscribe((data: VkCollection) => {
            this.usersCollection = data;

            window.scrollTo(0, 0);
        }, err => {
            this.showErrorToast('Ошибка загрузки пользователей', err);
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

    detectAge(userId: number, firstName: string, lastName: string) {
        this.vkCelebrationService.detectAge(userId, firstName, lastName).subscribe((data: number) => {
            const user = this.usersCollection.items.find(u => u.id === userId);
            if (user) {
                user.age = data;
            }

            this.toastrService.success('Возраст успешно определен');
        }, err => {
            this.showErrorToast('Ошибка определения возраста', err);
        });
    }

    sendMessageOpen(user: VkUser) {
        if (this.selectedUser !== user) {
            this.messageText = '';
            this.template = '';
        }
        this.selectedUser = user;
        this.isModalShown = true;

        this.loadTemplates('').subscribe((items: CongratulationTemplate[]) => {
            this.congratulationTemplates = items;
        }, (err: any) => {
            this.congratulationTemplates = [];
            this.showErrorToast('Ошибка загрузки заготовок поздравлений', err);
        });
    }

    editTemplateMessage() {
        if (this.template) {
            this.messageText = this.template;
        }
    }

    saveCongratulationTemplate() {
        this.loading = true;
        this.congratulationTemplatesService.createCongratulationTemplate(
            new CongratulationTemplate(this.messageText)).subscribe(() => {
                this.loading = false;
                this.toastrService.success('Заготовка для поздравления успешно сохранена');
            }, err => {
                this.loading = false;
                this.showErrorToast('Ошибка сохранения заготовки поздравления', err);
            });
    }

    sendCongratulation() {
        this.loading = true;
        let resultMessage = this.messageText;
        if (this.template && !this.messageText) {
            resultMessage = this.template;
        }
        this.vkCelebrationService.sendCongratulation
            (new UserCongratulation(resultMessage, this.selectedUser.id)).subscribe((data: number) => {
                this.congratulationModal.hide();
                this.toastrService.success('Поздравление успешно отправлено');
                this.seachUsers();
            }, err => {
                this.loading = false;
                this.showErrorToast('Ошибка отправки поздравления', err);
            });
    }

    onHidden(event: any) {
        this.loading = false;
        this.isModalShown = false;
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

    private showErrorToast(message: string, error: any) {
        const errToast = this.toastrService.error(message, 'Ошибка');
        console.log(error);
        if (errToast) {
            errToast.onTap.subscribe(() => {
                this.seachUsers();
            });
        }
    }
}
