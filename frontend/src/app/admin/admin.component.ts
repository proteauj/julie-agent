import { Component, OnInit } from '@angular/core';
import {
  AdminService,
  ActivityItem,
  CreateActivityPayload,
  CreateSeniorPayload,
  DoctorItem,
  SeniorSummary
} from '../services/admin.service';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})
export class AdminComponent implements OnInit {
  loading = false;
  error = '';
  success = '';

  seniors: SeniorSummary[] = [];
  activities: ActivityItem[] = [];
  doctors: DoctorItem[] = [];

  seniorForm: CreateSeniorPayload = {
    email: '',
    password: '',
    nom: '',
    phone: '',
    notificationPreference: 'sms'
  };

  resetPasswordBySeniorId: Record<number, string> = {};

  activityForm: CreateActivityPayload = {
    name: '',
    description: '',
    start: '',
    maxParticipants: 10
  };

  selectedActivityId: number | null = null;
  selectedSeniorIdForRegistration: number | null = null;

  doctorForm: DoctorItem = {
    name: '',
    specialty: '',
    bookingUrl: ''
  };

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.refreshAll();
  }

  refreshAll(): void {
    this.loading = true;
    this.error = '';
    this.success = '';

    this.adminService.getSeniors().subscribe({
      next: seniors => {
        this.seniors = seniors;

        this.adminService.getActivities().subscribe({
          next: activities => {
            this.activities = activities;

            this.adminService.getDoctors().subscribe({
              next: doctors => {
                this.doctors = doctors;
                this.loading = false;
              },
              error: err => this.handleError(err, 'Impossible de charger les médecins.')
            });
          },
          error: err => this.handleError(err, 'Impossible de charger les activités.')
        });
      },
      error: err => this.handleError(err, 'Impossible de charger les séniors.')
    });
  }

  createSenior(): void {
    this.clearMessages();

    if (!this.seniorForm.email.trim() || !this.seniorForm.password.trim()) {
      this.error = 'Email et mot de passe sont requis.';
      return;
    }

    this.adminService.createSenior(this.seniorForm).subscribe({
      next: () => {
        this.success = 'Compte sénior créé.';
        this.seniorForm = {
          email: '',
          password: '',
          nom: '',
          phone: '',
          notificationPreference: 'sms'
        };
        this.refreshAll();
      },
      error: err => this.handleError(err, 'Erreur lors de la création du compte sénior.')
    });
  }

  resetPassword(seniorId: number): void {
    const newPassword = this.resetPasswordBySeniorId[seniorId]?.trim();

    if (!newPassword) {
      this.error = 'Entre un nouveau mot de passe.';
      return;
    }

    this.clearMessages();

    this.adminService.resetSeniorPassword(seniorId, { newPassword }).subscribe({
      next: () => {
        this.success = 'Mot de passe réinitialisé.';
        this.resetPasswordBySeniorId[seniorId] = '';
      },
      error: err => this.handleError(err, 'Erreur lors de la réinitialisation du mot de passe.')
    });
  }

  createActivity(): void {
    this.clearMessages();

    if (!this.activityForm.name.trim() || !this.activityForm.start) {
      this.error = 'Nom et date de l’activité sont requis.';
      return;
    }

    this.adminService.createActivity(this.activityForm).subscribe({
      next: () => {
        this.success = 'Activité créée.';
        this.activityForm = {
          name: '',
          description: '',
          start: '',
          maxParticipants: 10
        };
        this.refreshAll();
      },
      error: err => this.handleError(err, 'Erreur lors de la création de l’activité.')
    });
  }

  deleteActivity(activityId: number): void {
    this.clearMessages();

    this.adminService.deleteActivity(activityId).subscribe({
      next: () => {
        this.success = 'Activité supprimée.';
        this.refreshAll();
      },
      error: err => this.handleError(err, 'Erreur lors de la suppression de l’activité.')
    });
  }

  registerSeniorToActivity(): void {
    if (!this.selectedActivityId || !this.selectedSeniorIdForRegistration) {
      this.error = 'Choisis un sénior et une activité.';
      return;
    }

    this.clearMessages();

    this.adminService
      .registerSeniorToActivity(this.selectedActivityId, this.selectedSeniorIdForRegistration)
      .subscribe({
        next: () => {
          this.success = 'Sénior inscrit à l’activité.';
          this.refreshAll();
        },
        error: err => this.handleError(err, 'Erreur lors de l’inscription à l’activité.')
      });
  }

  createDoctor(): void {
    this.clearMessages();

    if (!this.doctorForm.name.trim() || !this.doctorForm.bookingUrl.trim()) {
      this.error = 'Nom et URL de prise de rendez-vous sont requis.';
      return;
    }

    this.adminService.createDoctor(this.doctorForm).subscribe({
      next: () => {
        this.success = 'Médecin ajouté.';
        this.doctorForm = {
          name: '',
          specialty: '',
          bookingUrl: ''
        };
        this.refreshAll();
      },
      error: err => this.handleError(err, 'Erreur lors de l’ajout du médecin.')
    });
  }

  deleteDoctor(doctorId?: number): void {
    if (!doctorId) return;

    this.clearMessages();

    this.adminService.deleteDoctor(doctorId).subscribe({
      next: () => {
        this.success = 'Médecin supprimé.';
        this.refreshAll();
      },
      error: err => this.handleError(err, 'Erreur lors de la suppression du médecin.')
    });
  }

  private clearMessages(): void {
    this.error = '';
    this.success = '';
  }

  private handleError(err: any, fallback: string): void {
    this.loading = false;
    this.error = err?.error?.message || err?.error || fallback;
  }
}