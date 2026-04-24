import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable, catchError, throwError } from 'rxjs';

export interface ChatResponse {
  response: string;
}

export interface ChatMessage {
  role: 'user' | 'assistant';
  content: string;
}

export interface ChatRequest {
  message: string;
}

@Injectable({ providedIn: 'root' })
export class ChatService {
  private api = `${environment.apiUrl}/chat`;

  constructor(private http: HttpClient) {}

  sendMessage(message: string): Observable<ChatResponse> {
    const body: ChatRequest = { message };

    return this.http.post<ChatResponse>(this.api, body).pipe(
      catchError(err => {
        console.error('Chat error', err);
        return throwError(() => err);
      })
    );
  }
}