import { Component, inject, input, output } from '@angular/core';
import { Task } from '../../modals/board';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DateService } from '../../services/date';

@Component({
  selector: 'app-task-details',
  imports: [CommonModule, FormsModule],
  templateUrl: './task-details.html',
  styleUrl: './task-details.css',
})
export class TaskDetails {
  task = input<Task | null>(null);
  taskClick = output<void>();
  deleteTask = output<void>();
  addComment = output<string>();
  toggleComplete = output<void>();
  private dateService = inject(DateService);

  newComment: string = '';

  onDelete(event: Event) {
    event.stopPropagation();
    this.deleteTask.emit();
  }

  onToggleComplete(event: Event) {
    event.stopPropagation();
    this.toggleComplete.emit();
  }

  onAddComment(): void {
    if (!this.newComment.trim()) return;
    this.addComment.emit(this.newComment.trim());
    this.newComment = '';
  }

  formatDueDate(date: Date | string): string {
    const parsedDate = new Date(date);
    if (isNaN(parsedDate.getTime())) {
      return 'Invalid date';
    }

    const now = new Date();
    const diffTime = parsedDate.getTime() - now.getTime();
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

    if (diffDays < 0) {
      return 'Overdue';
    } else if (diffDays === 0) {
      return 'Today';
    } else if (diffDays === 1) {
      return 'Tomorrow';
    } else if (diffDays < 7) {
      return `${diffDays} days`;
    } else {
      return parsedDate.toLocaleDateString('en-US', {
        month: 'short',
        day: 'numeric',
      });
    }
  }

  isOverdue(date: Date | string): boolean {
    const parsedDate = new Date(date);
    if (isNaN(parsedDate.getTime())) {
      return false; // Treat invalid dates as not overdue
    }

    const now = new Date();
    return parsedDate < now;
  }

  formatDateFull(date: Date | string): string {
    const parsedDate = new Date(date);
    if (isNaN(parsedDate.getTime())) {
      return 'Invalid date';
    }

    return this.dateService.toDateString(parsedDate);
  }
}
