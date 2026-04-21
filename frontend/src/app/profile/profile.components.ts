import { Component, OnInit } from '@angular/core';
import { UserService, User } from '../services/user.service';

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

  constructor(private userService: UserService) {}

  ngOnInit() {
    // Exemple : récupère user avec id 1 (à adapter selon tes besoins)
    this.userService.getUser(1).subscribe({
      next: user => { this.user = user; this.loaded = true; },
      error: () => { this.loaded = true; } // Gestion d’erreur simplifiée
    });
  }

  save() {
    if (this.user.id) {
      this.userService.updateUser(this.user.id, this.user).subscribe();
    } else {
      this.userService.createUser(this.user).subscribe(r => this.user = r);
    }
  }
}