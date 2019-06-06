import { Component, ViewChild, TemplateRef, EventEmitter, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { ModalDirective, BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { VkUser } from '../../models/vk-user.model';
import { VkCelebrationService } from '../../services/vk-celebration.service';
import { CongratulationTemplatesService } from '../../services/congratulation-templates.service';
import { VkCollection } from '../../models/vk-collection.model';
import { UserCongratulation } from '../../models/user-congratulation.model';
import { CongratulationTemplate } from '../../models/congratulation-template.model';
import { distinctUntilChanged, debounceTime, switchMap } from 'rxjs/operators';
import { SearchParams } from 'src/app/models/search-params.model';
import { UserService } from 'src/app/services/user.service';
import { PagedVkCollection } from 'src/app/models/paged-vk-collection';

@Component({
  selector: 'home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  private searchSettingsKey = 'searchSettings';

  friendsSuggestionsCollection: VkCollection<VkUser>;
  usersCollection: PagedVkCollection<VkUser>;
  congratulationTemplates: CongratulationTemplate[];
  selectedUser: VkUser;
  searchParams: SearchParams;
  pageNumber: number;

  @ViewChild(ModalDirective) congratulationModal: ModalDirective;
  @ViewChild(ModalDirective) userInfoModal: ModalDirective;

  typeahead = new EventEmitter<string>();
  isModalShown = false;
  isUserInfoModalShown = false;
  isCongratulationTemplatesExists = false;

  messageText: string;
  template: string;

  loading = false;

  constructor(private readonly vkCelebrationService: VkCelebrationService,
    private readonly congratulationTemplatesService: CongratulationTemplatesService,
    private userService: UserService,
    private readonly toastrService: ToastrService) {
  }

  async ngOnInit() {
    await this.loadSearchSettings();

    this.typeahead
      .pipe(
        distinctUntilChanged(),
        debounceTime(200),
        switchMap((text: string) => this.congratulationTemplatesService.findCongratulationTemplates(text))
      )
      .subscribe((items: CongratulationTemplate[]) => {
        this.congratulationTemplates = items;
      }, (err: any) => {
        this.congratulationTemplates = [];
        this.showErrorToast('Ошибка загрузки заготовок поздравлений', err);
      });

    this.congratulationTemplatesService.congratulationTemplatesExists().subscribe((data: boolean) => {
      this.isCongratulationTemplatesExists = data;
    });
  }

  loadFriendsSuggestions() {
    this.vkCelebrationService.getFriendsSuggestions().subscribe((data: VkCollection<VkUser>) => {
      this.friendsSuggestionsCollection = data;
    }, err => {
      this.showErrorToast('Ошибка загрузки возможных друзей - именинников', err);
    });
  }

  seachUsers() {
    this.vkCelebrationService.search(this.searchParams, this.pageNumber)
      .subscribe((data: PagedVkCollection<VkUser>) => {
      this.usersCollection = data;
    }, err => {
      this.showErrorToast('Ошибка загрузки именинников из поиска', err);
    });
  }

  settingsChanged() {
    this.seachUsers();
  }

  pageChanged() {
    this.seachUsers();
  }

  isMobileDevice() {
    return (typeof window.orientation !== 'undefined') || (navigator.userAgent.indexOf('IEMobile') !== -1);
  }

  sendMessageOpen(user: VkUser) {
    if (this.selectedUser !== user) {
      this.messageText = '';
      this.template = '';
    }
    this.selectedUser = user;
    this.isModalShown = true;

    this.congratulationTemplatesService.getRandomCongratulationTemplates().subscribe((items: CongratulationTemplate[]) => {
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
        // this.loadFriendsSuggestions();
      }, err => {
        this.loading = false;
        this.showErrorToast('Ошибка отправки поздравления', err);
      });
  }

  sendRandomCongratulation(user: VkUser) {
    this.vkCelebrationService.sendRandomCongratulation(user.id).subscribe((data: number) => {
      this.toastrService.success('Поздравление успешно отправлено');
      this.seachUsers();
    }, err => {
      this.loading = false;
      if (err.status === 404) {
        this.showErrorToast('Отсутствуют заготовки поздравлений', err);
      } else {
        this.showErrorToast('Ошибка отправки поздравления', err);
      }
    });
  }

  onHidden(event: any) {
    this.loading = false;
    this.isModalShown = false;
    this.isUserInfoModalShown = false;
  }

  saveSearchSettings() {
    localStorage.setItem(this.searchSettingsKey, JSON.stringify(this.searchParams));
    this.toastrService.success('Настройки поиска успешно сохранено', 'Успех');
  }

  async resetSearchSettings() {
    localStorage.removeItem(this.searchSettingsKey);
    this.searchParams = new SearchParams();
  }

  private async loadSearchSettings() {
    const userInfo = <VkUser>await this.userService.getUserInfo().toPromise();
    this.searchSettingsKey += '_' + userInfo.id;

    const settings = localStorage.getItem(this.searchSettingsKey);
    if (settings) {
      this.searchParams = JSON.parse(settings);
    } else {
      this.searchParams = new SearchParams(userInfo.cityId);
    }
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
