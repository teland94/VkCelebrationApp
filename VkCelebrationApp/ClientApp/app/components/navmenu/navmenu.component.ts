import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { User } from '../../models/user.model';

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent implements OnInit {
    baseUrl = 'http://vk.com/id';
    currentUser: User;

    constructor(private readonly userService: UserService) {
        
    }

    ngOnInit() {
        this.userService.getUserInfo().subscribe((user: User) => {
            this.currentUser = user;
        });
    }
}