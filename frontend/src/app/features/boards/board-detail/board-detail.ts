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
  Tag,
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
import { TagService } from '../services/tag';
import { Navbar } from '../../../shared/components/navbar/navbar';

@Component({
  selector: 'app-board-detail',
  imports: [
    CommonModule,
    FormsModule,
    DragDropModule,
    TaskCard,
    TaskDetails,
    Navbar,
  ],
  templateUrl: './board-detail.html',
  styleUrl: './board-detail.css',
})
export class BoardDetail implements OnInit {
  // Services
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly boardService = inject(BoardService);
  private readonly columnService = inject(ColumnService);
  private readonly taskService = inject(TaskService);
  private readonly commentService = inject(CommentService);
  private readonly tagService = inject(TagService);
  private readonly errorService = inject(ErrorService);
  readonly authService = inject(AuthService);

  // State signals
  readonly board = signal<Board | null>(null);
  readonly selectedColumn = signal<Column | null>(null);
  readonly selectedTask = signal<Task | null>(null);

  // UI state
  showAddColumnModal = false;
  showAddTaskModalFlag = false;
  newColumnTitle = '';
  newTaskTitle = '';
  newTaskDescription = '';
  newTaskDueDate: Date | undefined;

  ngOnInit(): void {
    this.loadBoardFromRoute();
  }

  // ========== Board Management ==========

  private loadBoardFromRoute(): void {
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

  goBack(): void {
    this.router.navigate(['/boards']);
  }

  // ========== Column Operations ==========

  addColumn(): void {
    const currentBoard = this.board();
    if (!this.newColumnTitle.trim() || !currentBoard) return;

    const columnRequest: ColumnRequest = {
      boardId: currentBoard.id,
      title: this.newColumnTitle.trim(),
      sortOrder: currentBoard.columns.length,
    };

    this.columnService.createColumn(currentBoard.id, columnRequest).subscribe({
      next: (column) => this.handleColumnAdded(column),
      error: () => this.handleError('Failed to add column'),
    });
  }

  private handleColumnAdded(column: Column): void {
    this.board.update((board) => {
      if (!board) return board;
      return { ...board, columns: [...board.columns, column] };
    });

    this.errorService.showSuccess('Column added successfully');
    this.closeAddColumnModal();
  }

  // ========== Task Operations ==========

  addTask(): void {
    const column = this.selectedColumn();
    const currentUser = this.authService.currentUser();

    if (!this.validateTaskInput(column, currentUser)) return;

    const taskRequest = this.createTaskRequest(column!, currentUser!);

    this.taskService.createTask(column!.id, taskRequest).subscribe({
      next: (task) => this.handleTaskAdded(task, column!),
      error: () => this.handleError('Failed to add task'),
    });
  }

  private validateTaskInput(column: Column | null, user: any): boolean {
    return Boolean(this.newTaskTitle.trim() && column && this.board() && user);
  }

  private createTaskRequest(column: Column, user: any): TaskRequest {
    return {
      columnId: column.id,
      title: this.newTaskTitle.trim(),
      description: this.newTaskDescription.trim(),
      sortOrder: column.tasks.length,
      dueDate: this.newTaskDueDate ? new Date(this.newTaskDueDate) : undefined, // Ensure dueDate remains a Date object
      createdById: user.id,
      createdAt: new Date(), // Ensure createdAt remains a Date object
      isCompleted: false,
    };
  }

  private handleTaskAdded(task: Task, column: Column): void {
    this.board.update((board) => {
      if (!board) return board;

      const updatedColumns = board.columns.map((col) => {
        if (col.id === column.id) {
          return { ...col, tasks: [...col.tasks, task] };
        }
        return col;
      });

      return { ...board, columns: updatedColumns };
    });

    this.errorService.showSuccess('Task added successfully');
    this.closeAddTaskModal();
  }

  toggleTaskCompletion(task: Task, column: Column): void {
    if (!this.board()) return;

    const updatedTask: TaskRequest = {
      ...this.createTaskUpdatePayload(task),
      isCompleted: !task.isCompleted,
    };

    this.taskService.updateTask(task.id, updatedTask).subscribe({
      next: (updated) => this.updateTaskInState(updated, column),
      error: () => this.handleError('Failed to update task completion status'),
    });
  }

  private createTaskUpdatePayload(task: Task): TaskRequest {
    return {
      columnId: task.columnId,
      title: task.title,
      description: task.description || '',
      sortOrder: task.sortOrder,
      dueDate: task.dueDate,
      createdAt: task.createdAt,
      createdById: task.createdById,
      assignedToId: task.assignedToId,
      isCompleted: task.isCompleted,
    };
  }

  deleteTask(taskId: number, column: Column): void {
    if (!confirm('Are you sure you want to delete this task?')) return;

    this.taskService.deleteTask(taskId).subscribe({
      next: () => this.handleTaskDeleted(taskId, column),
      error: () => this.handleError('Failed to delete task'),
    });
  }

  private handleTaskDeleted(taskId: number, column: Column): void {
    this.board.update((board) => {
      if (!board) return board;

      const updatedColumns = board.columns.map((col) => {
        if (col.id === column.id) {
          return {
            ...col,
            tasks: col.tasks.filter((t) => t.id !== taskId),
          };
        }
        return col;
      });

      return { ...board, columns: updatedColumns };
    });

    this.errorService.showSuccess('Task deleted successfully');
    this.closeTaskDetail();
  }

  // ========== Drag & Drop Operations ==========

  onTaskDrop(event: CdkDragDrop<Task[]>, targetColumn: Column): void {
    const task = event.previousContainer.data[event.previousIndex];

    if (event.previousContainer === event.container) {
      this.handleTaskReorder(event, targetColumn, task);
    } else {
      this.handleTaskMove(event, targetColumn, task);
    }
  }

  private handleTaskReorder(
    event: CdkDragDrop<Task[]>,
    targetColumn: Column,
    task: Task
  ): void {
    moveItemInArray(
      event.container.data,
      event.previousIndex,
      event.currentIndex
    );
    this.updateTaskOrder(task, targetColumn.id, event.currentIndex);
  }

  private handleTaskMove(
    event: CdkDragDrop<Task[]>,
    targetColumn: Column,
    task: Task
  ): void {
    transferArrayItem(
      event.previousContainer.data,
      event.container.data,
      event.previousIndex,
      event.currentIndex
    );
    this.moveTask(task, targetColumn.id, event.currentIndex);
  }

  private moveTask(task: Task, newColumnId: number, sortOrder: number): void {
    this.taskService
      .moveTask(task.id, { targetColumnId: newColumnId, sortOrder })
      .subscribe({
        error: () => this.handleTaskMoveError(),
      });
  }

  private updateTaskOrder(
    task: Task,
    columnId: number,
    sortOrder: number
  ): void {
    this.taskService
      .moveTask(task.id, { targetColumnId: columnId, sortOrder })
      .subscribe({
        error: () => this.handleTaskMoveError(),
      });
  }

  private handleTaskMoveError(): void {
    this.errorService.showError('Failed to move task');
    const currentBoard = this.board();
    if (currentBoard) {
      this.loadBoard(currentBoard.id);
    }
  }

  // ========== Comment Operations ==========

  addCommentToTask(taskId: number, comment: string): void {
    const currentBoard = this.board();
    if (!currentBoard) return;

    const task = this.findTaskById(taskId, currentBoard);
    if (!task) {
      this.errorService.showError('Task not found');
      return;
    }

    const currentUser = this.authService.currentUser();
    const newComment: CommentRequest = {
      taskId,
      content: comment,
      authorId: currentUser?.id || '',
      authorName: currentUser?.displayName || '',
      createdAt: new Date(),
    };

    this.commentService.createComment(taskId, newComment).subscribe({
      next: (createdComment) => this.handleCommentAdded(taskId, createdComment),
      error: () => this.handleError('Failed to add comment'),
    });
  }

  private findTaskById(taskId: number, board: Board): Task | undefined {
    return board.columns
      .flatMap((column) => column.tasks)
      .find((task) => task.id === taskId);
  }

  private handleCommentAdded(taskId: number, comment: any): void {
    this.updateTaskInBoard(taskId, (task) => ({
      ...task,
      comments: [...task.comments, comment],
    }));

    this.updateSelectedTaskIfNeeded(taskId);
    this.errorService.showSuccess('Comment added successfully');
  }

  // ========== Tag Operations ==========

  addTagToTask(taskId: number, tagId: number): void {
    this.tagService.addTagsToTask(taskId, [tagId]).subscribe({
      next: (tags) => this.handleTagOperationSuccess(taskId, tags),
      error: () => this.handleError('Failed to add tag to task'),
    });
  }

  removeTagFromTask(taskId: number, tagId: number): void {
    this.tagService.removeTagsFromTask(taskId, [tagId]).subscribe({
      next: () => {
        this.updateTaskTags(taskId, tagId, false);
        this.errorService.showSuccess('Tag removed successfully');
      },
      error: () => this.handleError('Failed to remove tag'),
    });
  }

  private handleTagOperationSuccess(taskId: number, tags: Tag[]): void {
    this.updateTaskInBoard(taskId, () => ({ tags }));
    this.updateSelectedTaskIfNeeded(taskId);
    this.errorService.showSuccess('Tag added to task');
  }

  private updateTaskTags(taskId: number, tagId: number, add: boolean): void {
    this.updateTaskInBoard(taskId, (task) => {
      if (add) {
        const tagToAdd = this.getTagById(tagId);
        return tagToAdd ? { ...task, tags: [...task.tags, tagToAdd] } : task;
      } else {
        return {
          ...task,
          tags: task.tags.filter((tag) => tag.id !== tagId),
        };
      }
    });
  }

  private getTagById(tagId: number): Tag | null {
    const task = this.selectedTask();
    if (task) {
      const existingTag = task.tags.find((t) => t.id === tagId);
      if (existingTag) return existingTag;
    }

    // Placeholder - in production, maintain a list of all available tags
    return { id: tagId, name: `Tag ${tagId}`, color: '#667eea' };
  }

  // ========== State Update Helpers ==========

  private updateTaskInBoard(
    taskId: number,
    updateFn: (task: Task) => Partial<Task>
  ): void {
    this.board.update((board) => {
      if (!board) return board;

      const updatedColumns = board.columns.map((column) => {
        const updatedTasks = column.tasks.map((task) => {
          if (task.id === taskId) {
            return { ...task, ...updateFn(task) };
          }
          return task;
        });
        return { ...column, tasks: updatedTasks };
      });

      return { ...board, columns: updatedColumns };
    });
  }

  private updateTaskInState(updatedTask: Task, column: Column): void {
    this.board.update((board) => {
      if (!board) return board;

      const updatedColumns = board.columns.map((col) => {
        if (col.id === column.id) {
          const updatedTasks = col.tasks.map((t) =>
            t.id === updatedTask.id ? updatedTask : t
          );
          return { ...col, tasks: updatedTasks };
        }
        return col;
      });

      return { ...board, columns: updatedColumns };
    });

    if (this.selectedTask()?.id === updatedTask.id) {
      this.selectedTask.set(updatedTask);
    }
  }

  private updateSelectedTaskIfNeeded(taskId: number): void {
    if (this.selectedTask()?.id === taskId) {
      const updatedTask = this.findTaskById(taskId, this.board()!);
      if (updatedTask) {
        this.selectedTask.set(updatedTask);
      }
    }
  }

  // ========== UI State Management ==========

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

  // ========== Utility Methods ==========

  private handleError(message: string): void {
    this.errorService.showError(message);
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
