import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UtilitiesService {
  private baseUrl = 'api/Utilities';

  constructor() { }

  getImageData(url: string) {
    return this.baseUrl + '/GetImageData?url=' + url;
  }
}
