import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

export interface SeniorSummary {
  id: number;
  email: string;
  nom?: string;
  phone?: string;
  notificationPreference?: string;
  facilityId?: number;
}

export interface CreateSeniorPayload {
  email: string;
  password: string;
  nom?: string;
  phone?: string;
  notificationPreference?: string;
}

export interface ResetPasswordPayload {
  newPassword: string;
}

export interface ActivityItem {
  id: number;
  name: string;
  description: string;
  start: string;
  maxParticipants: number;
  facilityId: number;
  registeredCount?: number;
}

export interface CreateActivityPayload {
  name: string;
  description: string;
  start: string;
  maxParticipants: number;
}

export interface DoctorItem {
  id?: number;
  name: string;
  specialty: string;
  bookingUrl: string;
  facilityId?: number;
}

@Injectable({ providedIn: 'root' })
export class AdminService {
  private adminApi = `${environment.apiUrl}/admin`;
  private doctorsApi = `${environment.apiUrl}/doctors`;

  constructor(private http: HttpClient) {}

  getSeniors(): Observable<SeniorSummary[]> {
    return this.http.get<SeniorSummary[]>(`${this.adminApi}/seniors`);
  }

  createSenior(payload: CreateSeniorPayload): Observable<SeniorSummary> {
    return this.http.post<SeniorSummary>(`${this.adminApi}/seniors`, payload);
  }

  resetSeniorPassword(userId: number, payload: ResetPasswordPayload): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(
      `${this.adminApi}/seniors/${userId}/reset-password`,
      payload
    );
  }

  getActivities(): Observable<ActivityItem[]> {
    return this.http.get<ActivityItem[]>(`${this.adminApi}/activities`);
  }

  createActivity(payload: CreateActivityPayload): Observable<ActivityItem> {
    return this.http.post<ActivityItem>(`${this.adminApi}/activities`, payload);
  }

  updateActivity(id: number, payload: CreateActivityPayload): Observable<ActivityItem> {
    return this.http.put<ActivityItem>(`${this.adminApi}/activities/${id}`, payload);
  }

  deleteActivity(id: number): Observable<void> {
    return this.http.delete<void>(`${this.adminApi}/activities/${id}`);
  }

  registerSeniorToActivity(activityId: number, userId: number): Observable<unknown> {
    return this.http.post(`${this.adminApi}/activities/${activityId}/register/${userId}`, {});
  }

  unregisterSeniorFromActivity(activityId: number, userId: number): Observable<void> {
    return this.http.delete<void>(`${this.adminApi}/activities/${activityId}/register/${userId}`);
  }

  getDoctors(): Observable<DoctorItem[]> {
    return this.http.get<DoctorItem[]>(this.doctorsApi);
  }

  createDoctor(payload: DoctorItem): Observable<DoctorItem> {
    return this.http.post<DoctorItem>(this.doctorsApi, payload);
  }

  updateDoctor(id: number, payload: DoctorItem): Observable<DoctorItem> {
    return this.http.put<DoctorItem>(`${this.doctorsApi}/${id}`, payload);
  }

  deleteDoctor(id: number): Observable<void> {
    return this.http.delete<void>(`${this.doctorsApi}/${id}`);
  }
}