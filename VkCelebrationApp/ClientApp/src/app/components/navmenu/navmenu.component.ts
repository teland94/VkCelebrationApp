import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { VkUser } from '../../models/vk-user.model';
import { AuthService } from 'src/app/services/auth.service';
import { Router } from '@angular/router';
import { UtilitiesService } from 'src/app/services/utilities.service';

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent implements OnInit {
    isExpanded = false;

    baseUrl = 'http://vk.com/id';
    currentUser: VkUser;

    constructor(private readonly router: Router,
      private readonly authService: AuthService,
      private readonly userService: UserService,
      private utilitiesService: UtilitiesService) {
    }

    ngOnInit() {
      this.authService.authNavStatus$.subscribe(res => {
        if (res) {
          this.userService.getUserInfo().subscribe((user: VkUser) => {
            this.currentUser = user;
          });
        } else {
          this.currentUser = null;
        }
      });
    }

    isLoggedIn() {
      return this.authService.isLoggedIn();
    }

    collapse() {
      this.isExpanded = false;
    }

    toggle() {
      this.isExpanded = !this.isExpanded;
    }

    getImageData(url: string) {
      return this.utilitiesService.getImageData(url);
    }

    logout() {
      this.authService.logout();
      this.currentUser = null;
      this.router.navigate(['/login']);
    }
}
