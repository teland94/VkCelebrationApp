import { Component, OnInit, EventEmitter, Input, Output } from '@angular/core';
import { VkCollection } from '../../models/vk-collection.model';
import { VkUser } from '../../models/vk-user.model';
import { ToastrService } from 'ngx-toastr';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-vk-users',
  templateUrl: './vk-users.component.html',
  styleUrls: ['./vk-users.component.css']
})
export class VkUsersComponent implements OnInit {

  baseUrl = 'http://vk.com/id';

  @Input() usersCollection: VkCollection<VkUser>;
  @Input() writeCongratulationButton: boolean;

  @Output() writeCongratulationClick = new EventEmitter<VkUser>();

  constructor(private readonly userService: UserService,
    private readonly toastrService: ToastrService) { }

  ngOnInit() {
  }

  writeCongratulation(user) {
    this.writeCongratulationClick.emit(user);
  }

  detectAge(userId: number, firstName: string, lastName: string) {
    this.userService.detectAge(userId, firstName, lastName).subscribe((data: number) => {
      const user = this.usersCollection.items.find(u => u.id === userId);
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
