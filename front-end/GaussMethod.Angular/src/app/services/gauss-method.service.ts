import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GaussMethodInput } from '../models/gaussMethodInput';

@Injectable({
  providedIn: 'root'
})
export class GaussMethodService {
  private apiUrl = 'http://localhost:5153/api/gaussmethod';

  constructor(private http: HttpClient) { }

  solveFromFile(file: File, precision: number): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    const params = new HttpParams().set('precision', precision.toString());

    return this.http.post(`${this.apiUrl}/solveFromFile`, formData, { params });
  }

  solveManually(model: GaussMethodInput, precision: number): Observable<any> {
    const params = new HttpParams().set('precision', precision.toString());

    return this.http.post(`${this.apiUrl}/solveManually`, model, { params });
  }

  generateAndSimplifySystem(n: number, precision: number): Observable<any> {
    const params = new HttpParams()
      .set('n', n.toString())
      .set('precision', precision.toString());

    return this.http.post(`${this.apiUrl}/generateAndSimplifySystem`, null, { params });
  }  
}