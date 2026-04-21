import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

export interface User {
  id?: number;
  email: string;
  nom?: string;
  languePreferree?: string;
  phone?: string;
  notificationPreference?: string;
}

@Injectable({ providedIn: 'root' })
export class UserService {
  private api = `${environment.apiUrl}/users`;
  constructor(private http: HttpClient) {}

  getUser(id: number): Observable<User> {
    return this.http.get<User>(`${this.api}/${id}`);
  }
  createUser(user: User): Observable<User> {
    return this.http.post<User>(this.api, user);
  }
  updateUser(id: number, user: User): Observable<any> {
    return this.http.put(`${this.api}/${id}`, user);
  }
  getMe() {
    return this.http.get<User>(`${environment.apiUrl}/users/me`);
  }
}