import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

export interface AuthUser {
  id: number;
  email: string;
  nom?: string;
  role: string;
  facilityId?: number | null;
  notificationPreference?: string;
}

export interface LoginResponse {
  token: string;
  user: AuthUser;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private jwtKey = 'token';
  private userKey = 'current_user';

  public isLogged$ = new BehaviorSubject<boolean>(this.isAuthenticated());
  public currentUser$ = new BehaviorSubject<AuthUser | null>(this.getStoredUser());

  constructor(private http: HttpClient) {}

  login(email: string, password: string): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${environment.apiUrl}/auth/login`, { email, password })
      .pipe(
        tap(res => {
          localStorage.setItem(this.jwtKey, res.token);
          localStorage.setItem(this.userKey, JSON.stringify(res.user));
          this.isLogged$.next(true);
          this.currentUser$.next(res.user);
        })
      );
  }

  register(email: string, password: string, nom?: string): Observable<unknown> {
    return this.http.post(`${environment.apiUrl}/auth/register`, { email, password, nom });
  }

  logout(): void {
    localStorage.removeItem(this.jwtKey);
    localStorage.removeItem(this.userKey);
    this.isLogged$.next(false);
    this.currentUser$.next(null);
  }

  getToken(): string | null {
    return localStorage.getItem(this.jwtKey);
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  isAdmin(): boolean {
    return this.currentUser$.value?.role === 'admin';
  }

  private getStoredUser(): AuthUser | null {
    const raw = localStorage.getItem(this.userKey);
    if (!raw) return null;

    try {
      return JSON.parse(raw) as AuthUser;
    } catch {
      localStorage.removeItem(this.userKey);
      return null;
    }
  }
}