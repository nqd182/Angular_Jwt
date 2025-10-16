// service gọi api
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = 'https://localhost:7002/api/User'; // link backend
  constructor (private http: HttpClient){}
  login(userData:any): Observable<any>{
    return this.http.post(`${this.baseUrl}/Login`, userData);
    
  }
  register(userData:any): Observable<any>{
    return this.http.post(`${this.baseUrl}/Register`, userData);
  }
  renew(model: any): Observable<any>{
    return this.http.post(`${this.baseUrl}/RenewToken`, model);
  }
  logout() {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('loginToken');
    window.location.href = '/login'
  }
}
