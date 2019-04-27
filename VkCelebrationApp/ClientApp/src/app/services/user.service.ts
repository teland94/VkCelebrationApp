import { Injectable } from '@angular/core';
import { DataService } from './data.service';

@Injectable()
export class UserService {
    private url = 'user';

    constructor(private readonly dataService: DataService) {

    }

    getUserInfo() {
        return this.dataService.get(this.url + '/' + 'getUserInfo');
    }

    detectAge(userId: number, firstName: string, lastName: string) {
      return this.dataService.get(`${this.url}/detectAge`, {
          userId,
          firstName,
          lastName
      });
  }
}
