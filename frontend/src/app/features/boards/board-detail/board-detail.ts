import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth';
import { BoardService } from '../services/board';
import { ColumnService } from '../services/column';
import { TaskService } from '../services/task';
import { ErrorService } from '../../../core/services/error';
import {
  Board,
  Column,
  ColumnRequest,
  CommentRequest,
  Task,
  TaskRequest,
} from '../modals/board';
import {
  CdkDragDrop,
  DragDropModule,
  moveItemInArray,
  transferArrayItem,
} from '@angular/cdk/drag-drop';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TaskCard } from '../components/task-card/task-card';
import { TaskDetails } from '../components/task-details/task-details';
import { CommentService } from '../services/comment';

@Component({
  selector: 'app-board-detail',
  imports: [CommonModule, FormsModule, DragDropModule, TaskCard, TaskDetails],
  templateUrl: './board-detail.html',
  styleUrl: './board-detail.css',
})
export class BoardDetail implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private boardService = inject(BoardService);
  private columnService = inject(ColumnService);
  private taskService = inject(TaskService);
  private commentService = inject(CommentService);
  authService = inject(AuthService);
  private errorService = inject(ErrorService);

  board = signal<Board | null>(null);
  showAddColumnModal = false;
  showAddTaskModalFlag = false;
  newColumnTitle = '';
  newTaskTitle = '';
  newTaskDescription = '';
  selectedColumn = signal<Column | null>(null);
  selectedTask = signal<Task | null>(null);

  ngOnInit(): void {
    const boardId = this.route.snapshot.paramMap.get('id');
    if (boardId) {
      this.loadBoard(Number(boardId));
    }
  }

  private loadBoard(boardId: number): void {
    this.boardService.getBoardById(boardId).subscribe({
      next: (board) => this.board.set(board),
      error: () => {
        this.errorService.showError('Failed to load board');
        this.router.navigate(['/boards']);
      },
    });
  }

  addColumn(): void {
    if (!this.newColumnTitle.trim() || !this.board()) return;

    const sortOrder = this.board()!.columns.length;

    const columnRequest: ColumnRequest = {
      boardId: this.board()!.id,
      title: this.newColumnTitle.trim(),
      sortOrder: sortOrder,
    };
    this.columnService.createColumn(this.board()!.id, columnRequest).subscribe({
      next: (column) => {
        this.board.update((b) => {
          if (!b) return b;
          return { ...b, columns: [...b.columns, column] };
        });
        this.errorService.showSuccess('Column added successfully');
        this.closeAddColumnModal();
      },
      error: () => {
        this.errorService.showError('Failed to add column');
      },
    });
  }

  addTask(): void {
    if (!this.newTaskTitle.trim() && !this.selectedColumn() && !this.board())
      return;

    const column = this.selectedColumn()!;
    const sortOrder = column.tasks.length;
    const currentUser = this.authService.currentUser();
    if (!currentUser) return;

    const taskRequest: TaskRequest = {
      columnId: column.id,
      title: this.newTaskTitle.trim(),
      description: this.newTaskDescription.trim(),
      sortOrder: sortOrder,
      createdById: currentUser.id,
      createdAt: new Date(),
      // assignedToId
      isCompleted: false,
    };

    this.taskService.createTask(column.id, taskRequest).subscribe({
      next: (task) => {
        this.board.update((b) => {
          if (!b) return b;
          const updatedColumns = b.columns.map((col) => {
            if (col.id == column.id) {
              return { ...col, tasks: [...col.tasks, task] };
            }
            return col;
          });
          return { ...b, columns: updatedColumns };
        });
        this.errorService.showSuccess('Task added successfully');
        this.closeAddTaskModal();
      },
      error: () => {
        this.errorService.showError('Failed to add task');
      },
    });
  }

  onTaskDrop(event: CdkDragDrop<Task[]>, targetColumn: Column): void {
    const task = event.previousContainer.data[event.previousIndex];

    if (event.previousContainer === event.container) {
      // Same column - just reorder
      moveItemInArray(
        event.container.data,
        event.previousIndex,
        event.currentIndex
      );
      this.updateTaskOrder(task, targetColumn.id, event.currentIndex);
    } else {
      // Different column - move task
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex
      );
      this.moveTask(task, targetColumn.id, event.currentIndex);
    }
  }

  moveTask(task: Task, newColumnId: number, sortOrder: number): void {
    this.taskService
      .moveTask(task.id, { targetColumnId: newColumnId, sortOrder: sortOrder })
      .subscribe({
        error: () => {
          this.errorService.showError('Failed to move task');
          this.loadBoard(this.board()!.id);
        },
      });
  }

  updateTaskOrder(task: Task, columnId: number, sortOrder: number): void {
    this.taskService
      .moveTask(task.id, { targetColumnId: columnId, sortOrder })
      .subscribe({
        error: () => {
          this.errorService.showError('Failed to update task order');
          this.loadBoard(this.board()!.id);
        },
      });
  }

  toggleTaskCompletion(task: Task, column: Column): void {
    if (!this.board()) return;

    const updatedTask: TaskRequest = {
      columnId: task.columnId,
      title: task.title,
      description: task.description || '',
      sortOrder: task.sortOrder,
      dueDate: task.dueDate,
      createdAt: task.createdAt,
      createdById: task.createdById,
      assignedToId: task.assignedToId,
      isCompleted: !task.isCompleted,
    };

    if (!column) {
      this.errorService.showError('Column not found');
      return;
    }

    const taskIndex = column.tasks.findIndex((t) => t.id === task.id);
    if (taskIndex === -1) {
      this.errorService.showError('Task not found');
      return;
    }

    this.taskService.updateTask(task.id, updatedTask).subscribe({
      next: (updated) => {
        this.board.update((b) => {
          if (!b) return b;
          const updatedColumns = b.columns.map((col) => {
            if (col.id === column.id) {
              const updatedTasks = col.tasks.map((t) => {
                if (t.id === task.id) {
                  return updated;
                }
                return t;
              });
              return { ...col, tasks: updatedTasks };
            }
            return col;
          });
          return { ...b, columns: updatedColumns };
        });

        // Also update selectedTask if it's the same task
        if (this.selectedTask()?.id === task.id) {
          this.selectedTask.set(updated);
        }
      },
      error: () => {
        this.errorService.showError('Failed to update task completion status');
      },
    });
  }

  deleteTask(taskId: number, column: Column): void {
    if (!confirm('Are you sure you want to delete this task?')) return;

    this.taskService.deleteTask(taskId).subscribe({
      next: () => {
        this.board.update((b) => {
          if (!b) return b;

          const updatedColumns = b.columns.map((col) => {
            if (col.id === column.id) {
              return {
                ...col,
                tasks: col.tasks.filter((t) => t.id !== taskId),
              };
            }
            return col;
          });
          return { ...b, columns: updatedColumns };
        });
        this.errorService.showSuccess('Task deleted successfully');
        this.closeTaskDetail();
      },
      error: () => {
        this.errorService.showError('Failed to delete task');
      },
    });
  }

  addCommentToTask(taskId: number, comment: string): void {
    if (!this.board()) return;

    const task = this.board()!
      .columns.flatMap((column) => column.tasks)
      .find((task) => task.id === taskId);

    if (!task) {
      this.errorService.showError('Task not found');
      return;
    }

    const newComment: CommentRequest = {
      taskId,
      content: comment,
      authorId: this.authService.currentUser()?.id || '',
      authorName: this.authService.currentUser()?.displayName || '',
      createdAt: new Date(),
    };

    this.commentService.createComment(taskId, newComment).subscribe({
      next: (comment) => {
        this.board.update((b) => {
          if (!b) return b;
          const updatedColumns = b.columns.map((col) => {
            const updatedTasks = col.tasks.map((t) => {
              if (t.id === taskId) {
                return { ...t, comments: [...t.comments, comment] };
              }
              return t;
            });
            return { ...col, tasks: updatedTasks };
          });
          return { ...b, columns: updatedColumns };
        });

        // ADD THIS: Update selectedTask if it's the same task
        if (this.selectedTask()?.id === taskId) {
          const updatedTask = this.board()!
            .columns.flatMap((column) => column.tasks)
            .find((task) => task.id === taskId);
          if (updatedTask) {
            this.selectedTask.set(updatedTask);
          }
        }

        this.errorService.showSuccess('Comment added successfully');
      },
      error: () => {
        this.errorService.showError('Failed to add comment');
      },
    });
  }

  showAddTaskModal(column: Column): void {
    this.selectedColumn.set(column);
    this.showAddTaskModalFlag = true;
  }

  closeAddColumnModal(): void {
    this.showAddColumnModal = false;
    this.newColumnTitle = '';
  }

  closeAddTaskModal(): void {
    this.showAddTaskModalFlag = false;
    this.newTaskTitle = '';
    this.newTaskDescription = '';
    this.selectedColumn.set(null);
  }

  openTaskDetail(task: Task, column: Column): void {
    this.selectedTask.set(task);
    this.selectedColumn.set(column);
  }

  closeTaskDetail(): void {
    this.selectedTask.set(null);
    this.selectedColumn.set(null);
  }

  goBack(): void {
    this.router.navigate(['/boards']);
  }

  logout(): void {
    this.authService.logout();
  }

  formatDate(date: Date): string {
    return date.toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
    });
  }
}
