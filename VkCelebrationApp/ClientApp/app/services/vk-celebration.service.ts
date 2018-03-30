import { Injectable } from '@angular/core';
import { DataService } from './data.service';
import { UserCongratulation } from '../models/user-congratulation.model';

@Injectable()
export class VkCelebrationService {
    private url = 'vkcelebration';

    constructor(private readonly dataService: DataService) {

    }

    getFriendsSuggestions() {
        return this.dataService.get(`${this.url}/getFriendsSuggestions`);
    }

    search() {
        return this.dataService.get(`${this.url}/search`);
    }

    detectAge(userId: number, firstName: string, lastName: string) {
        return this.dataService.get(`${this.url}/detectAge`, {
            userId: userId,
            firstName: firstName,
            lastName: lastName
        });
    }

    sendCongratulation(userCongratulation: UserCongratulation) {
        return this.dataService.post(`${this.url}/sendCongratulation`, userCongratulation);
    }
}