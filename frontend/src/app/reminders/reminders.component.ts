import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

export interface Reminder {
  id: number;
  userId: number;
  title: string;
  description?: string;
  scheduledAt: string;
  type: string;
  channel: string;
  isDone: boolean;
  completedAt?: string | null;
  isRecurring: boolean;
  recurrenceRule?: string | null;
  sourceId?: number | null;
  sourceType?: string | null;
  createdAt?: string;
}

@Component({
  selector: 'app-reminders',
  templateUrl: './reminders.component.html',
  styleUrls: ['./reminders.component.scss']
})
export class RemindersComponent implements OnInit {
  reminders: Reminder[] = [];
  loading = false;
  error = '';

  form = {
    title: '',
    description: '',
    scheduledAt: '',
    type: 'general',
    channel: 'sms',
    isRecurring: false,
    recurrenceRule: ''
  };

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadReminders();
  }

  loadReminders(): void {
    this.loading = true;
    this.error = '';

    this.http
      .get<Reminder[]>(`${environment.apiUrl}/reminders/mine`)
      .subscribe({
        next: reminders => {
          this.reminders = [...reminders].sort(
            (a, b) => new Date(a.scheduledAt).getTime() - new Date(b.scheduledAt).getTime()
          );
          this.loading = false;
        },
        error: err => {
          this.error = err?.error || 'Impossible de charger les rappels.';
          this.loading = false;
        }
      });
  }

  createReminder(): void {
    this.error = '';

    if (!this.form.title.trim()) {
      this.error = 'Le titre est requis.';
      return;
    }

    if (!this.form.scheduledAt) {
      this.error = 'La date du rappel est requise.';
      return;
    }

    this.loading = true;

    const payload = {
      title: this.form.title.trim(),
      description: this.form.description.trim(),
      scheduledAt: new Date(this.form.scheduledAt).toISOString(),
      type: this.form.type,
      channel: this.form.channel,
      isRecurring: this.form.isRecurring,
      recurrenceRule: this.form.isRecurring ? this.form.recurrenceRule || null : null
    };

    this.http
      .post(`${environment.apiUrl}/reminders`, payload)
      .subscribe({
        next: () => {
          this.form = {
            title: '',
            description: '',
            scheduledAt: '',
            type: 'general',
            channel: 'sms',
            isRecurring: false,
            recurrenceRule: ''
          };
          this.loadReminders();
        },
        error: err => {
          this.error = err?.error || 'Impossible de créer le rappel.';
          this.loading = false;
        }
      });
  }

  markDone(reminderId: number): void {
    this.loading = true;
    this.error = '';

    this.http
      .post(`${environment.apiUrl}/reminders/${reminderId}/complete`, {})
      .subscribe({
        next: () => this.loadReminders(),
        error: err => {
          this.error = err?.error || 'Impossible de compléter le rappel.';
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