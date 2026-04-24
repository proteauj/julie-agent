import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

export interface ActivityItem {
  id: number;
  name: string;
  description?: string;
  start: string;
  maxParticipants: number;
  facilityId?: number;
  registeredCount?: number;
  isRegistered?: boolean;
  canRegister?: boolean;
}

@Component({
  selector: 'app-activities',
  templateUrl: './activities.component.html',
  styleUrls: ['./activities.component.scss']
})
export class ActivitiesComponent implements OnInit {
  activities: ActivityItem[] = [];
  loading = false;
  error = '';
  success = '';

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadActivities();
  }

  loadActivities(): void {
    this.loading = true;
    this.error = '';
    this.success = '';

    this.http.get<any>(`${environment.apiUrl}/admin/activities`).subscribe({
      next: res => {
        const activities = Array.isArray(res)
          ? res
          : Array.isArray(res.activities)
            ? res.activities
            : Array.isArray(res.data)
              ? res.data
              : [];

        this.activities = activities.sort(
          (a: ActivityItem, b: ActivityItem) =>
            new Date(a.start).getTime() - new Date(b.start).getTime()
        );

        this.loading = false;
      },
      error: err => {
          this.error = this.getErrorMessage(err, 'Impossible de charger les activités.');
          this.loading = false;
        }
    });
  }

  registerToActivity(activity: ActivityItem): void {
    this.loading = true;
    this.error = '';
    this.success = '';

    this.http
      .post(`${environment.apiUrl}/activities/${activity.id}/register`, {})
      .subscribe({
        next: () => {
          this.success = 'Inscription confirmée.';
          this.loadActivities();
        },
        error: err => {
          this.error = this.getErrorMessage(err, 'Impossible de vous inscrire à cette activité.');
          this.loading = false;
        }
      });
  }

  unregisterFromActivity(activity: ActivityItem): void {
    this.loading = true;
    this.error = '';
    this.success = '';

    this.http
      .delete(`${environment.apiUrl}/activities/${activity.id}/register`)
      .subscribe({
        next: () => {
          this.success = 'Désinscription confirmée.';
          this.loadActivities();
        },
        error: err => {
          this.error = this.getErrorMessage(err, 'Impossible de vous désinscrire de cette activité.');
          this.loading = false;
        }
      });
  }

  formatDate(value: any): string {
    if (!value) return '';

    const rawValue =
      typeof value === 'object'
        ? value.start ?? value.date ?? value.value ?? ''
        : value;

    const date = new Date(rawValue);

    if (isNaN(date.getTime())) return '';

    return date.toLocaleString('fr-CA', {
      dateStyle: 'medium',
      timeStyle: 'short'
    });
  }

  private getErrorMessage(err: any, fallback: string): string {
    if (typeof err?.error === 'string') {
      return err.error;
    }

    if (typeof err?.error?.message === 'string') {
      return err.error.message;
    }

    if (typeof err?.message === 'string') {
      return err.message;
    }

    return fallback;
  }
}