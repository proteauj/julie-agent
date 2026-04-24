import { Component } from '@angular/core';
import { ChatService } from '../services/chat.service';
import { ChatMessage } from '../services/chat.service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent {
  messages: ChatMessage[] = [];
  input = '';
  loading = false;

  constructor(private chat: ChatService) {}

  send() {
    const msg = this.input.trim();
    if (!msg) return;

    this.messages.push({
      role: 'user',
      content: msg
    });
    this.input = '';
    this.loading = true;

    this.chat.sendMessage(msg).subscribe({
      next: res => {
        this.messages.push({
          role: 'assistant',
          content: res.response
        });
        this.loading = false;
      },
      error: _ => {
        this.messages.push({
          role: 'assistant',
          content: "Erreur : échec communication API."
        });
        this.loading = false;
      }
    });
  }

  ngAfterViewChecked() {
    const el = document.querySelector('.messages');
    if (el) el.scrollTop = el.scrollHeight;
  }
}