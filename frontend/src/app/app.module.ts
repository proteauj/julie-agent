import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { AppComponent } from './app.component';
import { ProfileComponent } from './profile/profile.components';
import { ChatComponent } from './chat/chat.component';
import { AppRoutingModule } from './app.routing';
import { AppTranslateModule } from './app.translate.module';

@NgModule({
  declarations: [
    AppComponent,
    ProfileComponent,
    ChatComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule,
    AppRoutingModule,
    AppTranslateModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }