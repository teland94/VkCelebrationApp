import { Injectable } from '@angular/core';
import { DataService } from './data.service';
import { CongratulationTemplate } from '../models/congratulation-template.model';

@Injectable()
export class CongratulationTemplatesService {
    private url = 'CongratulationTemplates';

    constructor(private dataService: DataService) {
    }

    getCongratulationTemplates() {
        return this.dataService.get(this.url);
    }

    getRandomCongratulationTemplates(count: number = 5) {
        return this.dataService.get(this.url + '/GetRandomCongratulationTemplates', {
            count: count
        });
    }

    createCongratulationTemplate(congratulationTemplate: CongratulationTemplate) {
        return this.dataService.post(this.url, congratulationTemplate);
    }

    updateCongratulationTemplate(congratulationTemplate: CongratulationTemplate) {
        return this.dataService.put(this.url + '/' + congratulationTemplate.id, congratulationTemplate);
    }

    deleteCongratulationTemplate(id: number) {
        return this.dataService.delete(this.url + '/' + id);
    }

    findCongratulationTemplates(text: string, maxItems: number = 5) {
        return this.dataService.get(this.url + '/find', {
            text: text,
            maxItems: maxItems
        });
    }
}
