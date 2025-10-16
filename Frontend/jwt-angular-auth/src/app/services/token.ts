//File này giúp tách riêng logic kiểm tra token. /ng g service token/
import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class TokenService {
  isTokenExpired(): boolean {
    const token = localStorage.getItem("accessToken");
    if(!token) return true; // không có token => coi như hết hạn luôn
    try{
      const decoded: any = jwtDecode(token);
      const currentTime = Math.floor(Date.now()/1000) // second

      // nếu token đã hết hạn
      return decoded.exp < currentTime;

    }catch(error){
      return true; // lỗi token => coi như hết hạn luôn
    }
  }
  clearToken(){
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken')
  }
}
