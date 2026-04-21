import { Component } from '@angular/core';
import { ChatService } from '../services/chat.service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent {
  messages: { author: string, text: string }[] = [];
  input = '';
  loading = false;

  constructor(private chat: ChatService) {}

  send() {
    const msg = this.input.trim();
    if (!msg) return;

    this.messages.push({ author: 'Vous', text: msg });
    this.input = '';
    this.loading = true;

    this.chat.sendMessage(msg).subscribe({
      next: res => {
        this.messages.push({ author: 'Julie', text: res.response });
        this.loading = false;
      },
      error: _ => {
        this.messages.push({ author: 'SYSTEM', text: "Erreur : échec communication API." });
        this.loading = false;
      }
    });
  }
}