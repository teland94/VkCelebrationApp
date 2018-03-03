import { Injectable } from '@angular/core';
import { DataService } from './data.service';

@Injectable()
export class UserCongratulationsService {
    private url = 'UserCongratulations';

    constructor(private readonly dataService: DataService) {

    }

    getUserCongratulations(congratulationDate: Date) {
        return this.dataService.post(this.url + '/getUserCongratulations', {
            congratulationDate: congratulationDate,
            timezoneOffset: -(congratulationDate.getTimezoneOffset() / 60)
        });
    }
}