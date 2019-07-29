import { Injectable } from '@angular/core';
import { DataService } from './data.service';
import { map, } from 'rxjs/operators';
import { of } from 'rxjs';

@Injectable()
export class UserService {
  private url = 'user';
  private userInfoName = 'userInfo';

  constructor(private readonly dataService: DataService) {

  }

  getUserInfo() {
    const userInfo = localStorage.getItem(this.userInfoName);
    if (userInfo) { return of(JSON.parse(userInfo)); }
    return this.dataService.get(this.url + '/' + 'getUserInfo')
      .pipe(map((res: any) => {
        localStorage.setItem(this.userInfoName, JSON.stringify(res));
        return res;
      })
      );
  }

  getImageData(url: string) {
    return this.dataService.get('/utilities/getimage?url=' + url);
  }

  detectAge(userId: number, firstName: string, lastName: string) {
    return this.dataService.get(`${this.url}/detectAge`, {
      userId,
      firstName,
      lastName
    });
  }

  addToIgnoreList(vkUserId: number) {
    return this.dataService.post(`${this.url}/addToIngoreList`, {
      vkUserId
    });
  }
}
