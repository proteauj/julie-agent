import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app.routing';
import { AppTranslateModule } from './app.translate.module';
import { JwtInterceptor } from './services/jwt.interceptor';

import { ProfileComponent } from './profile/profile.components';
import { ChatComponent } from './chat/chat.component';
import { AdminComponent } from './admin/admin.component';
import { AgendaComponent } from './agenda/agenda.component';
import { RemindersComponent } from './reminders/reminders.component';
import { ActivitiesComponent } from './activities/activities.component';
import { SeniorHomeComponent } from './senior-home/senior-home.component';
import { Ng2FlatpickrModule } from 'ng2-flatpickr';

@NgModule({
  declarations: [
    AppComponent,
    ProfileComponent,
    ChatComponent,
    AdminComponent,
    AgendaComponent,
    RemindersComponent,
    ActivitiesComponent,
    SeniorHomeComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule,
    AppRoutingModule,
    AppTranslateModule,
    Ng2FlatpickrModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}