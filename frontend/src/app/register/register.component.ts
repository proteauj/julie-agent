import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslateModule],
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  nom = '';
  email = '';
  password = '';
  error = '';
  success = '';

  constructor(private auth: AuthService, private router: Router) {}

  register() {
    this.error = '';
    this.success = '';

    this.auth.register(this.email, this.password, this.nom).subscribe({
      next: () => {
        this.success = 'Compte créé avec succès.';
        this.router.navigate(['/login']);
      },
      error: err => {
        this.error = err.error || 'Erreur lors de la création du compte';
      }
    });
  }
}