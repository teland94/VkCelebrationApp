import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

@Injectable()
export class DataService {
    private baseUrl = 'api';

    constructor(private readonly httpClient: HttpClient) {

    }

    get(methodName: string, apiParameters?: any) {
        const parameters = this.buildHttpParams(apiParameters);
        return this.httpClient.get(this.baseUrl + '/' + methodName, {
            params: parameters
        });
    }

    post(url: string, data?: any) {
        return this.httpClient.post(this.baseUrl + '/' + url, data);
    }

    put(url: string, data?: any) {
        return this.httpClient.put(this.baseUrl + '/' + url, data);
    }

    delete(url: string) {
        return this.httpClient.delete(this.baseUrl + '/' + url);
    }

    private buildHttpParams(params?: any): HttpParams {
        let httpParams = new HttpParams();
        for (const key in params) {
            if (params.hasOwnProperty(key)) {
                httpParams = httpParams.append(key, params[key]);
            }
        }
        return httpParams;
    }
}