import { Injectable } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(private auth: AuthService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.auth.getToken();
    let observable = next.handle(token ? req.clone({
        setHeaders: { Authorization: `Bearer ${token}` }
    }) : req);

    return observable.pipe(
        catchError(err => {
        if (err.status === 401) {
            this.auth.logout();
            // (Optionnel: naviguer automatiquement vers /login ici)
        }
        return throwError(() => err);
        })
    );
  }
}