import { Component, OnInit, EventEmitter, Input, Output } from '@angular/core';
import { VkUser } from '../../models/vk-user.model';
import { ToastrService } from 'ngx-toastr';
import { UserService } from '../../services/user.service';
import { UtilitiesService } from 'src/app/services/utilities.service';
import { PagedVkCollection } from 'src/app/models/paged-vk-collection';
import { PaginationConfig } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-vk-users',
  templateUrl: './vk-users.component.html',
  styleUrls: ['./vk-users.component.css']
})
export class VkUsersComponent implements OnInit {

  baseUrl = 'http://vk.com/id';
  itemsPerPage: number;

  @Input() usersCollection: PagedVkCollection<VkUser>;
  @Input() currentPage: number;
  @Input() totalItems: number;
  @Input() disabled: boolean;
  @Input() sendRandomCongratulationEnabled: boolean;

  @Output() writeCongratulationClick = new EventEmitter<VkUser>();
  @Output() sendRandomCongratulationClick = new EventEmitter<VkUser>();
  @Output() userBlacklisted = new EventEmitter<VkUser>();
  @Output() currentPageChange = new EventEmitter();
  @Output() pageChanged = new EventEmitter();

  constructor(private readonly userService: UserService,
    private readonly utilitiesService: UtilitiesService,
    private readonly toastrService: ToastrService,
    private paginationConfig: PaginationConfig) {
      this.itemsPerPage = paginationConfig.main.itemsPerPage;
    }

  ngOnInit() {
  }

  writeCongratulation(user: VkUser) {
    this.writeCongratulationClick.emit(user);
  }

  sendRandomCongratulation(user: VkUser) {
    this.sendRandomCongratulationClick.emit(user);
  }

  addToIgnoreList(user: VkUser) {
    this.disabled = true;
    this.userService.addToIgnoreList(user.id)
      .pipe(finalize(() => this.disabled = false))
      .subscribe(data => {
        this.toastrService.success('Пользователь успешно заблокирован');
        this.userBlacklisted.emit(user);
      }, err => {
        this.toastrService.error('Ошибка блокировки пользователя', 'Ошибка');
        console.log(err);
      });
  }

  getImageData(url: string) {
    return this.utilitiesService.getImageData(url);
  }

  pageChange(event: any) {
    this.currentPageChange.emit(event.page);
    this.pageChanged.emit();
  }

  detectAge(userId: number, firstName: string, lastName: string) {
    this.disabled = true;
    this.userService.detectAge(userId, firstName, lastName)
      .pipe(finalize(() => this.disabled = false))
      .subscribe((data: number) => {
        const user = this.usersCollection.vkCollection.items.find(u => u.id === userId);
        if (user) {
          user.age = data;
        }

        this.toastrService.success('Возраст успешно определен');
      }, err => {
        this.toastrService.error('Ошибка определения возраста', 'Ошибка');
        console.log(err);
      });
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
