import { Injectable } from '@angular/core';
import { DataService } from './data.service';
import { UserCongratulation } from '../models/user-congratulation.model';

@Injectable()
export class VkCelebrationService {
    private url = 'vkcelebration';

    constructor(private readonly dataService: DataService) {

    }

    search(ageFrom: number, ageTo: number) {
        return this.dataService.get(`${this.url}/search`, {
                ageFrom: ageFrom,
                ageTo: ageTo,
            });
    }

    detectAge(userId: number, firstName: string, lastName: string, ageFrom: number, ageTo: number) {
        return this.dataService.get(`${this.url}/detectAge`, {
            userId: userId,
            firstName: firstName,
            lastName: lastName,
            ageFrom: ageFrom,
            ageTo: ageTo
        });
    }

    sendCongratulation(userCongratulation: UserCongratulation) {
        return this.dataService.post(`${this.url}/sendCongratulation`, userCongratulation);
    }
}