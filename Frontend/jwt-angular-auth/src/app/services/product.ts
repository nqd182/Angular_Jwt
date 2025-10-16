import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MyProduct {
  private baseUrl = 'https://localhost:7002/api/Product'; // link backend
  constructor(private http: HttpClient){};
  getAll(): Observable<any>{
    return this.http.get<any[]>(this.baseUrl);
  }
  getById(id: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/${id}`);
  }
  create(product: any): Observable<any>{
    return this.http.post(this.baseUrl, product);
  }
  update(id: number, product: any): Observable<any>{
    return this.http.put(`${this.baseUrl}/${id}`, product);
  }
  delete(id: number) {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }
}
