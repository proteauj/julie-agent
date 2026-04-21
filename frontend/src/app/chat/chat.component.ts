import { Component } from '@angular/core';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent {
  messages: {author: string, text: string}[] = [];
  input = '';

  send() {
    if (this.input.trim()) {
      this.messages.push({ author: 'Vous', text: this.input });
      // Simulation réponse bot
      setTimeout(() => {
        this.messages.push({ author: 'Julie', text: 'Réponse automatique 🎉' });
      }, 700);
      this.input = '';
    }
  }
}