import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ChatService {
  sendMessage(msg: string): Promise<string> {
    return new Promise(resolve => {
      setTimeout(() => resolve('Réponse automatique 🎉 : ' + msg), 700);
    });
  }
}