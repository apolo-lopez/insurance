import { HttpInterceptorFn } from '@angular/common/http';
import { AuthService } from './auth.service';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  // Aquí puedes usar un servicio/global
  const token = localStorage.getItem('auth_token'); // O usa tu almacenamiento real
  //console.log('JWT Token from functional interceptor:', token);

  if (token) {
    const authReq = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
    return next(authReq);
  }
  return next(req);
};