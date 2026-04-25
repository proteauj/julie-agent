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

const pickerOptions: any = {
  enableTime: false,
  dateFormat: 'Y-m-d',
  minDate: 'today'
};

@Component({
  selector: 'app-agenda',
  templateUrl: './agenda.component.html',
  styleUrls: ['./agenda.component.scss']
})
export class AgendaComponent implements OnInit {
  appointments: Appointment[] = [];
  loading = false;
  error = '';

  form = {
    title: '',
    description: '',
    start: '',
    durationMinutes: 60,
    type: 'personal'
  };

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadAppointments();
  }

  loadAppointments(): void {
    this.loading = true;
    this.error = '';

    this.http.get<Appointment[]>(`${environment.apiUrl}/appointments/mine`).subscribe({
      next: appointments => {
        const now = new Date();

        this.appointments = [...appointments]
          .filter(a => new Date(a.end || a.start) >= now)
          .sort((a, b) => new Date(a.start).getTime() - new Date(b.start).getTime());
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

    const startDate = new Date(this.form.start);
    const now = new Date();

    if (!this.form.start) {
      this.error = 'La date de début est requise.';
      return;
    }

    if (startDate < now) {
      this.error = 'La date doit être dans le futur.';
      return;
    }

    const endDate = new Date(startDate);
    endDate.setMinutes(endDate.getMinutes() + Number(this.form.durationMinutes || 60));

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

    this.http.post<Appointment>(`${environment.apiUrl}/appointments`, payload).subscribe({
      next: () => {
        this.form = {
          title: '',
          description: '',
          start: '',
          durationMinutes: 60,
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

  typeLabel(type?: string): string {
    switch (type) {
      case 'personal':
        return 'TYPES.PERSONAL';
      case 'medical':
        return 'TYPES.MEDICAL';
      case 'activity':
        return 'TYPES.ACTIVITY';
      default:
        return 'COMMON.NONE';
    }
  }
}