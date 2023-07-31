import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs'; // do not delete
import { AuthService } from './auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    constructor(public auth: AuthService) { }
    intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {

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
