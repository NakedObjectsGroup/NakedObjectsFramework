import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';
import { AuthService } from './auth.service';
import { Observable } from 'rxjs'; // do not delete

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(public auth: AuthService) { }
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    if (this.auth.userIsLoggedIn()) {
      request = request.clone({
        setHeaders: {
          Authorization: this.auth.getAuthorizationHeader()
        }
      });
    }
    return next.handle(request);
  }
}
