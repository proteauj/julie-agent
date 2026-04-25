import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import flatpickr from 'flatpickr';
import { French } from 'flatpickr/dist/l10n/fr';
import { Instance as FlatpickrInstance } from 'flatpickr/dist/types/instance';

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
export class AgendaComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('startPicker') startPicker?: ElementRef<HTMLInputElement>;

  private picker?: FlatpickrInstance;

  form = {
    title: '',
    description: '',
    start: '',
    durationMinutes: 60,
    type: 'personal'
  };

  ngAfterViewInit(): void {
    if (!this.startPicker) return;

    this.picker = flatpickr(this.startPicker.nativeElement, {
      enableTime: true,
      dateFormat: 'Y-m-d H:i',
      altInput: true,
      altFormat: 'd F Y à H:i',
      minDate: 'today',
      time_24hr: true,
      minuteIncrement: 15,
      locale: French,
      disableMobile: true,
      onChange: (_selectedDates, dateStr) => {
        this.form.start = dateStr;
      }
    });
  }

  ngOnDestroy(): void {
    this.picker?.destroy();
  }
  appointments: Appointment[] = [];
  loading = false;
  error = '';

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

    const startDate = new Date(this.form.start.replace(' ', 'T'));

    if (isNaN(startDate.getTime())) {
      this.error = 'La date est obligatoire.';
      return;
    }

    if (startDate < new Date()) {
      this.error = 'La date doit être dans le futur.';
      return;
    }

    const endDate = new Date(startDate);
    endDate.setMinutes(endDate.getMinutes() + this.form.durationMinutes);

    const payload = {
      title: this.form.title,
      description: this.form.description,
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