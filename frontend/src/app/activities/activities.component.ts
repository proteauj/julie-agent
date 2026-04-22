import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

export interface ActivityItem {
  id: number;
  name: string;
  description: string;
  start: string;
  maxParticipants: number;
  facilityId: number;
  registeredCount?: number;
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

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadActivities();
  }

  loadActivities(): void {
    this.loading = true;
    this.error = '';

    this.http
      .get<ActivityItem[]>(`${environment.apiUrl}/admin/activities`)
      .subscribe({
        next: activities => {
          this.activities = [...activities].sort(
            (a, b) => new Date(a.start).getTime() - new Date(b.start).getTime()
          );
          this.loading = false;
        },
        error: err => {
          this.error = err?.error || 'Impossible de charger les activités.';
          this.loading = false;
        }
      });
  }

  formatDate(value: string): string {
    return new Date(value).toLocaleString('fr-CA', {
      dateStyle: 'medium',
      timeStyle: 'short'
    });
  }
}