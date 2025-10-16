import { inject, Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, Observable, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth';

export const AuthInterceptor: HttpInterceptorFn = (req, next) => {
   const authService = inject(AuthService)
    const accessToken = localStorage.getItem('accessToken');

    let authReq = req;
    if(accessToken) {
        authReq = req.clone({
        setHeaders: { Authorization: `Bearer ${accessToken}` }
      });
    }
     return next(authReq).pipe(
      catchError((error: HttpErrorResponse) => {
        // Nếu bị 401  thử refresh token
        if (error.status === 401) {
          const accessToken = localStorage.getItem("accessToken");
          const refreshToken = localStorage.getItem("refreshToken");
          return authService.renew({accessToken, refreshToken}).pipe(
            switchMap((newToken: any) => {
              // Cập nhật accessToken mới
              localStorage.setItem('accessToken', newToken.data.accessToken);
              localStorage.setItem('refreshToken', newToken.data.refreshToken);
              // Gửi lại request cũ với token mới
              const clonedReq = req.clone({
                headers: req.headers.set('Authorization', `Bearer ${newToken.data.accessToken}`)
              });

              return next(clonedReq);
            }),
            catchError((err) => {
              // Nếu refresh thất bại  đăng xuất
              authService.logout();
              return throwError(() => err);
            })
          );
        }

        return throwError(() => error);
      })
    );
  }

