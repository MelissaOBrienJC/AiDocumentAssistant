
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface PrepareTextResponse {
  text: string;
}

@Injectable({
  providedIn: 'root',
})
export class DocumentText {
  private readonly apiUrl = 'http://localhost:5034/api/DocumentText/prepare';

  constructor(private http: HttpClient) {}

  prepareText(text: string): Observable<PrepareTextResponse> {
    return this.http.post<PrepareTextResponse>(
      this.apiUrl,
      { text }
    );
  }
}
