import { Component } from '@angular/core';
import { ChatService } from '../services/chat.service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent {
  messages: {author: string, text: string}[] = [];
  input = '';

  constructor(private chat: ChatService) {}

  async send() {
    if (this.input.trim()) {
      this.messages.push({ author: 'Vous', text: this.input });
      const reply = await this.chat.sendMessage(this.input);
      this.messages.push({ author: 'Julie', text: reply });
      this.input = '';
    }
  }
}