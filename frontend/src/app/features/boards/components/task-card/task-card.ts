import { Component, input, output } from '@angular/core';
import { Task } from '../../modals/board';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-task-card',
  imports: [CommonModule, FormsModule],
  templateUrl: './task-card.html',
  styleUrl: './task-card.css',
})
export class TaskCard {
  task = input<Task>();
  taskClick = output<void>();
  deleteTask = output<void>();
  addComment = output<string>();

  newComment: string = '';

  onDelete(event: Event) {
    event.stopPropagation();
    this.deleteTask.emit();
  }

  onAddComment(): void {
    if (!this.newComment.trim()) return;
    this.addComment.emit(this.newComment.trim());
    this.newComment = '';
  }

  formatDueDate(date: Date): string {
    const now = new Date();
    const diffTime = date.getTime() - now.getTime();
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
      return date.toLocaleDateString('en-US', {
        month: 'short',
        day: 'numeric',
      });
    }
  }

  isOverdue(date: Date): boolean {
    const now = new Date();
    return date < now;
  }
}
