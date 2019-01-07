import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs'; // do not delete
import { AuthService } from './auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(public auth: AuthService) { }
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    if (this.auth.isAuthenticated()) {
      request = request.clone({
        setHeaders: {
          Authorization: this.auth.getAuthorizationHeader()
        }
      });
    }
    return next.handle(request);
  }
}
