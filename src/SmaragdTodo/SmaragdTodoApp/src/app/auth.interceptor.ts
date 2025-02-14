import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const idToken = localStorage.getItem('jwt');

  if (idToken) {
    const cloned = req.clone({
      setHeaders: {
        Authorization: `Bearer ${idToken}`
      }
    });

    return next(cloned);
  }

  return next(req);
};
