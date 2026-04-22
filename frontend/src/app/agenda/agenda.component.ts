import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

export interface Appointment {
  id: number;
  title: string;
  description?: string;
  start: string;
  end: string;
  type?: string;
  userId?: number;
}

@Component({
  selector: 'app-agenda',
  templateUrl: './agenda.component.html',
  styleUrls: ['./agenda.component.scss']
})
export class AgendaComponent implements OnInit {
  appointments: Appointment[] = [];
  loading = false;
  error = '';

  form: {
    title: string;
    description: string;
    start: string;
    end: string;
    type: string;
  } = {
    title: '',
    description: '',
    start: '',
    end: '',
    type: 'personal'
  };

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadAppointments();
  }

  loadAppointments(): void {
    this.loading = true;
    this.error = '';

    this.http
      .get<Appointment[]>(`${environment.apiUrl}/appointments/mine`)
      .subscribe({
        next: appointments => {
          this.appointments = [...appointments].sort(
            (a, b) => new Date(a.start).getTime() - new Date(b.start).getTime()
          );
          this.loading = false;
        },
        error: err => {
          this.error = err?.error || 'Impossible de charger l’agenda.';
          this.loading = false;
        }
      });
  }

  createAppointment(): void {
    this.error = '';

    if (!this.form.title.trim()) {
      this.error = 'Le titre est requis.';
      return;
    }

    if (!this.form.start) {
      this.error = 'La date de début est requise.';
      return;
    }

    if (!this.form.end) {
      this.error = 'La date de fin est requise.';
      return;
    }

    const startDate = new Date(this.form.start);
    const endDate = new Date(this.form.end);

    if (endDate <= startDate) {
      this.error = 'La date de fin doit être après la date de début.';
      return;
    }

    this.loading = true;

    const payload = {
      title: this.form.title.trim(),
      description: this.form.description.trim(),
      start: startDate.toISOString(),
      end: endDate.toISOString(),
      type: this.form.type
    };

    this.http
      .post<Appointment>(`${environment.apiUrl}/appointments`, payload)
      .subscribe({
        next: () => {
          this.form = {
            title: '',
            description: '',
            start: '',
            end: '',
            type: 'personal'
          };
          this.loadAppointments();
        },
        error: err => {
          this.error = err?.error || 'Impossible de créer le rendez-vous.';
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