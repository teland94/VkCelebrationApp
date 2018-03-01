import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { VkUser } from '../../models/vk-user.model';

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent implements OnInit {
    baseUrl = 'http://vk.com/id';
    currentUser: VkUser;

    constructor(private readonly userService: UserService) {
        
    }

    ngOnInit() {
        this.userService.getUserInfo().subscribe((user: VkUser) => {
            this.currentUser = user;
        });
    }
}