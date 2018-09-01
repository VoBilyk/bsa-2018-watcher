import {Injectable} from '@angular/core';
import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpErrorResponse, HttpResponse} from '@angular/common/http';
import {AuthService} from '../services/auth.service';
import {Observable, from} from 'rxjs';
import { flatMap } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class TokensInterceptor implements HttpInterceptor {
  headersConfig = {
    'Content-Type': 'application/json; charset=utf-8',
    'Accept': 'application/json'
  };

  constructor(public auth: AuthService) {
  }
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
  // check for preventing infinite loop while getting new token from backend
    if (req.url.match(/\/Tokens\/Login/)) {
      return from(this.auth.getFirebaseToken()).pipe(
        flatMap<string[], HttpEvent<any>>((firebaseToken) => {
          if (firebaseToken) {
            this.headersConfig['Authorization'] = `Bearer ${firebaseToken}`;
          }
          const request = req.clone({setHeaders: this.headersConfig, responseType: 'json'});
          return next.handle(request);
        }));
    }
    return this.auth.getTokens().pipe(
      flatMap<string[], HttpEvent<any>>(([firebaseToken, watcherToken]) => {
        if (firebaseToken) {
          this.headersConfig['Authorization'] = `Bearer ${firebaseToken}`;
        }
        if (watcherToken) {
          this.headersConfig['WatcherAuthorization'] = watcherToken;
        }
        const request = req.clone({setHeaders: this.headersConfig, responseType: 'json'});
        return next.handle(request);
      })
    );
  }
}
