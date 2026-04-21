import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  email = '';
  password = '';
  nom = '';
  error = '';
  success = false;

  constructor(private auth: AuthService, private router: Router) {}

  register() {
    this.auth.register(this.email, this.password, this.nom).subscribe({
      next: () => { this.success = true; setTimeout(() => this.router.navigate(['/login']), 1000); },
      error: err => this.error = err.error || 'Erreur à la création du compte'
    });
  }
}