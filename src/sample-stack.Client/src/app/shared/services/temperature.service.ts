import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { TemperatureResult } from '../../models/temperature-result';
import { BehaviorSubject, catchError, map, Observable, tap } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class TemperatureService {
  private STATUS_CODES = {
    Unknown: 0,
    OK: 200,
    NoContent: 204,
    BadRequest: 400,
    Unauthorized: 401,
    Forbidden: 403,
    Duplicate: 409,
    UnhandledException: 500
  };

  protected baseUrl: string;

  constructor(protected http: HttpClient) {
    this.baseUrl = environment.backend.apiUrl;
  }

  public currentTemperature(): Observable<TemperatureResult> {
    return this.http.get<number>(this.Url())
    .pipe(
      tap(result => {
        console.log(`Completed GET server request.  Response:`, result);
      }),
      map(result => {
        let returnedResult = new TemperatureResult();
        returnedResult.temp = result;

        return returnedResult;
      }),
      catchError(e => {
        const returnResult = new BehaviorSubject<any>(null);
        console.log(e);
        returnResult.error(e);

        return returnResult;
      })
    );
  }

    /**
   * The base URL to use in api calls.
   * @returns {string}
   */
    protected Url(): string {
      return `${this.baseUrl}/api/temperature`;
    }
}
