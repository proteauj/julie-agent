import { Component } from '@angular/core';
import { Location } from '@angular/common';
import { TranslateService } from '@ngx-translate/core';
import { Observable } from 'rxjs';
import { AuthService, AuthUser } from './services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'Aline Écoute';
  isLogged$: Observable<boolean>;
  currentUser$: Observable<AuthUser | null>;

  constructor(
    private translate: TranslateService,
    private auth: AuthService,
    private location: Location
  ) {
    this.isLogged$ = this.auth.isLogged$;
    this.currentUser$ = this.auth.currentUser$;
  }

  switchLang(lang: string) {
    this.translate.use(lang);
  }

  goBack() {
    this.location.back();
  }

  logout() {
    this.auth.logout();
  }
}