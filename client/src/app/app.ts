
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';

import { DocumentText } from './services/document-text';
import { DocumentSummary } from './services/document-summary';
import { DocumentQuestion } from './services/document-question';

@Component({
  selector: 'app-root',
  imports: [FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  documentText = '';
  preparedText = '';
  summary = '';

  question = '';
  answer = '';

  errorMessage = '';

  isLoading = false;
  isSummarizing = false;
  isAsking = false;

  constructor(
    private documentTextService: DocumentText,
    private documentSummaryService: DocumentSummary,
    private documentQuestionService: DocumentQuestion
  ) {}

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];

    if (!file) {
      return;
    }

    this.errorMessage = '';
    this.summary = '';
    this.answer = '';
    this.preparedText = '';

    if (file.size > 1024 * 1024) {
      this.errorMessage = 'The file cannot exceed 1 MB.';
      input.value = '';
      return;
    }

    const reader = new FileReader();

    reader.onload = () => {
      this.documentText = reader.result as string;
    };

    reader.onerror = () => {
      this.errorMessage = 'Unable to read the selected file.';
    };

    reader.readAsText(file);
  }

  prepareDocument(): void {
    this.preparedText = '';
    this.errorMessage = '';
    this.isLoading = true;

    this.documentTextService.prepareText(this.documentText).subscribe({
      next: (response) => {
        this.preparedText = response.text;
        this.isLoading = false;
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage =
          error.error?.error ??
          'Unable to prepare the document. Check that the API is running.';
        this.isLoading = false;
      }
    });
  }

  summarizeDocument(): void {
    this.summary = '';
    this.errorMessage = '';
    this.isSummarizing = true;

    this.documentSummaryService.summarize(this.documentText).subscribe({
      next: (response) => {
        this.summary = response.summary;
        this.isSummarizing = false;
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage =
          error.error?.error ??
          'Unable to summarize the document. Check that the API and Ollama are running.';
        this.isSummarizing = false;
      }
    });
  }

  askQuestion(): void {
    this.answer = '';
    this.errorMessage = '';

    if (!this.question.trim()) {
      this.errorMessage = 'Please enter a question.';
      return;
    }

    this.isAsking = true;

    this.documentQuestionService
      .askQuestion(this.documentText, this.question)
      .subscribe({
        next: (response) => {
          this.answer = response.answer;
          this.isAsking = false;
        },
        error: (error: HttpErrorResponse) => {
          this.errorMessage =
            error.error?.error ??
            'Unable to answer the question. Check that the API and Ollama are running.';
          this.isAsking = false;
        }
      });
  }
}
