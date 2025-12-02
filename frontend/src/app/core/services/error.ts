import { Injectable, signal } from '@angular/core';

export interface ErrorMessage {
  id: number;
  message: string;
  type: 'error' | 'success' | 'warning' | 'info';
}

@Injectable({
  providedIn: 'root',
})
export class Error {
  private errorId = 0;
  errors = signal<ErrorMessage[]>([]);

  showError(message: string): void {
    this.addMessage(message, 'error');
  }

  showSuccess(message: string): void {
    this.addMessage(message, 'success');
  }

  showInfo(message: string): void {
    this.addMessage(message, 'info');
  }

  showWarning(message: string): void {
    this.addMessage(message, 'warning');
  }

  private addMessage(message: string, type: ErrorMessage['type']): void {
    const id = this.errorId++;
    const newError: ErrorMessage = { id, message, type };

    this.errors.update((errors) => [...errors, newError]);

    // Automatically remove the message after 5 seconds
    setTimeout(() => this.removeError(id), 5000);
  }

  removeError(id: number): void {
    this.errors.update((errors) => errors.filter((error) => error.id !== id));
  }

  clearAll(): void {
    this.errors.set([]);
  }
}
