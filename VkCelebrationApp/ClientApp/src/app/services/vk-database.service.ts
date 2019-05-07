import { Injectable } from '@angular/core';
import { DataService } from './data.service';

@Injectable({
  providedIn: 'root'
})
export class VkDatabaseService {
  private url = 'VkDatabase';

  constructor(private readonly dataService: DataService) {

  }

  getCities(countryId: number, query: string = '') {
    return this.dataService.get(`${this.url}/GetCities`, {
      countryId,
      query
    });
  }

  getUniversities(countryId: number, cityId: number, query: string = '') {
    return this.dataService.get(`${this.url}/GetUniversities`, {
      countryId,
      cityId,
      query
    });
  }
}
