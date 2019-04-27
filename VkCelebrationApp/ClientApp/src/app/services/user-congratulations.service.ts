import { Injectable } from '@angular/core';
import { DataService } from './data.service';
import { map } from 'rxjs/operators';
import { UserCongratulation } from '../models/user-congratulation.model';

@Injectable()
export class UserCongratulationsService {
    private url = 'UserCongratulations';

    constructor(private readonly dataService: DataService) {

    }

    getUserCongratulations(congratulationDate: Date) {
      const timezoneOffset = -(congratulationDate.getTimezoneOffset() / 60);
        return this.dataService.post(this.url + '/getUserCongratulations', {
            congratulationDate: congratulationDate,
            timezoneOffset: timezoneOffset
        }).pipe(map((data: UserCongratulation[]) => {
          data.forEach(uc => {
            uc.congratulationDate = new Date(uc.congratulationDate);
            uc.congratulationDate.setHours(uc.congratulationDate.getHours() + timezoneOffset);
          });
          return data;
        }));
    }
}
