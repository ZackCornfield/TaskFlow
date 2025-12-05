import {
  Component,
  inject,
  input,
  OnInit,
  output,
  signal,
} from '@angular/core';
import { Tag, Task } from '../../modals/board';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DateService } from '../../services/date';
import { TagService } from '../../services/tag';
import { ErrorService } from '../../../../core/services/error';

@Component({
  selector: 'app-task-details',
  imports: [CommonModule, FormsModule],
  templateUrl: './task-details.html',
  styleUrl: './task-details.css',
})
export class TaskDetails implements OnInit {
  task = input<Task | null>(null);
  taskClick = output<void>();
  deleteTask = output<void>();
  addComment = output<string>();
  toggleComplete = output<void>();
  addTag = output<{ taskId: number; tagId: number }>();
  removeTag = output<{ taskId: number; tagId: number }>();

  private dateService = inject(DateService);
  private tagService = inject(TagService);
  private errorService = inject(ErrorService);

  newComment: string = '';
  availableTags = signal<Tag[]>([]);
  showTagSelector = false;
  selectedTagIds = signal<number[]>([]);

  ngOnInit(): void {
    this.loadAvailableTags();
  }

  private loadAvailableTags(): void {
    this.tagService.getTags().subscribe({
      next: (tags) => this.availableTags.set(tags),
      error: () => this.errorService.showError('Failed to load tags'),
    });
  }

  onAddTag(tagId: number): void {
    const task = this.task();
    if (!task) return;

    this.addTag.emit({ taskId: task.id, tagId });
    this.showTagSelector = false;
  }

  onRemoveTag(tagId: number, event: Event): void {
    event.stopPropagation();
    const task = this.task();
    if (!task) return;

    this.removeTag.emit({ taskId: task.id, tagId });
  }

  isTagSelected(tagId: number): boolean {
    const task = this.task();
    if (!task) return false;
    return task.tags.some((tag) => tag.id === tagId);
  }

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
