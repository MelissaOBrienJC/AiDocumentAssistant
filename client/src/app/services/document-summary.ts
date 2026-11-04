
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DocumentSummaryResponse {
  summary: string;
}

@Injectable({
  providedIn: 'root',
})
export class DocumentSummary {
  private readonly apiUrl = 'http://localhost:5034/api/DocumentSummary';

  constructor(private http: HttpClient) {}

  summarize(text: string): Observable<DocumentSummaryResponse> {
    return this.http.post<DocumentSummaryResponse>(
      this.apiUrl,
      { text }
    );
  }
}
