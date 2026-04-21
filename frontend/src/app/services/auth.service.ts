import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { tap } from 'rxjs/operators';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private jwtKey = 'token';
  public isLogged$ = new BehaviorSubject<boolean>(this.isAuthenticated());

  constructor(private http: HttpClient) {}

  login(email: string, password: string): Observable<any> {
    return this.http.post<{ token: string }>(
      `${environment.apiUrl}/auth/login`, { email, password }
    ).pipe(tap(res => {
      localStorage.setItem(this.jwtKey, res.token);
      this.isLogged$.next(true);
    }));
  }

  register(email: string, password: string, nom?: string): Observable<any> {
    return this.http.post(
      `${environment.apiUrl}/auth/register`, { email, password, nom }
    );
  }

  logout(): void {
    localStorage.removeItem(this.jwtKey);
    this.isLogged$.next(false);
  }

  getToken(): string | null {
    return localStorage.getItem(this.jwtKey);
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }
}