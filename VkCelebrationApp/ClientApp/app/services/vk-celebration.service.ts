import { Injectable } from '@angular/core';
import { DataService } from './data.service';

@Injectable()
export class VkCelebrationService {
    private url = 'vkcelebration';

    constructor(private readonly dataService: DataService) {

    }

    search(ageFrom: number, ageTo: number) {
        return this.dataService.get(`${this.url}/search`,
            {
                ageFrom: ageFrom,
                ageTo: ageTo,
            });
    }

    detectAge(uid: number, firstName: string, lastName: string, ageFrom: number, ageTo: number) {
        return this.dataService.get(`${this.url}/detectAge`);
    }
}