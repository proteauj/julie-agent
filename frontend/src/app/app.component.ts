import { Component } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from './services/auth.service';
import { Observable } from 'rxjs/internal/Observable';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'Julie-Agent';
  isLogged$: Observable<boolean>;
  constructor(private translate: TranslateService, private auth: AuthService) {
    this.isLogged$ = this.auth.isLogged$;
  }
  switchLang(lang: string) { this.translate.use(lang); }
  logout() { this.auth.logout(); }
}