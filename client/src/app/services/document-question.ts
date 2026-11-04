
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DocumentQuestionResponse {
  answer: string;
}

@Injectable({
  providedIn: 'root',
})
export class DocumentQuestion {
  private readonly apiUrl =
    'http://localhost:5034/api/DocumentQuestion';

  constructor(private http: HttpClient) {}

  askQuestion(
    text: string,
    question: string
  ): Observable<DocumentQuestionResponse> {
    return this.http.post<DocumentQuestionResponse>(
      this.apiUrl,
      { text, question }
    );
  }
}
