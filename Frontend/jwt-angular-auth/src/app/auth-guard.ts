// file để bảo vệ đường dẫn cần đăng nhập mới được truy cập (ng generate guard auth)
import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { TokenService } from './services/token';

export const authGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const tokenService = inject(TokenService);

  if( !tokenService.isTokenExpired()){
    return true;
  }else{
    //tokenService.clearToken();
    return false;
  }
};
