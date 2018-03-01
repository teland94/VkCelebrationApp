import { Injectable } from '@angular/core';
import { DataService } from './data.service';

@Injectable()
export class UserCongratulationsService {
    private url = 'UserCongratulations';

    constructor(private readonly dataService: DataService) {

    }

    getUserCongratulations() {
        return this.dataService.get(this.url + '/getUserCongratulations');
    }
}