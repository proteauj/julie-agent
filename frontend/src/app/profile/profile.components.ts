import { Component, OnInit } from '@angular/core';
import { UserService, User } from '../services/user.service';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  user: User = {
    email: '',
    nom: '',
    languePreferree: 'fr'
  };
  loaded = false;
  error = '';

  constructor(
    private userService: UserService,
    private auth: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.userService.getMe().subscribe({
      next: user => { this.user = user; this.loaded = true; },
      error: err => {
        this.loaded = true;
        this.error = "Impossible de récupérer le profil (non connecté ou erreur API)";
        if (err.status === 401) {
          this.auth.logout();
          this.router.navigate(['/login']);
        }
      }
    });
  }

  save() {
    if (this.user.id) {
      this.userService.updateUser(this.user.id, this.user).subscribe({
        next: user => this.user = user,
        error: () => this.error = "Échec de sauvegarde"
      });
    }
  }
}