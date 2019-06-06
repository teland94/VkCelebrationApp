import { Injectable } from '@angular/core';
import { DataService } from './data.service';
import { UserCongratulation } from '../models/user-congratulation.model';
import { SearchParams } from '../models/search-params.model';

@Injectable()
export class VkCelebrationService {
    private url = 'vkcelebration';

    constructor(private readonly dataService: DataService) {

    }

    getFriendsSuggestions() {
        return this.dataService.get(`${this.url}/getFriendsSuggestions`);
    }

    search(searchParams: SearchParams, pageNumber?: number, pageSize?: number) {
        return this.dataService.post(`${this.url}/search`, {
          searchParams,
          pageNumber,
          pageSize
        });
    }

    sendCongratulation(userCongratulation: UserCongratulation) {
        return this.dataService.post(`${this.url}/sendCongratulation`, userCongratulation);
    }

    sendRandomCongratulation(vkUserId: number) {
      return this.dataService.post(`${this.url}/sendRandomCongratulation`, { vkUserId });
    }
}
