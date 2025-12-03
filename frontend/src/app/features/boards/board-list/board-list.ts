import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { BoardService } from '../services/board';
import { Board } from '../modals/board';
import { AuthService } from '../../../core/services/auth';
import { ErrorService } from '../../../core/services/error';
import { BoardRequest } from '../modals/board';

@Component({
  selector: 'app-board-list',
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './board-list.html',
  styleUrl: './board-list.css',
})
export class BoardList implements OnInit {
  private boardService = inject(BoardService);
  authService = inject(AuthService);
  private router = inject(Router);
  private errorService = inject(ErrorService);

  boards = signal<Board[]>([]);
  showCreateModal = false;
  newBoardTitle = '';

  ngOnInit(): void {
    this.loadBoards();
  }

  loadBoards(): void {
    this.boardService.getBoards().subscribe({
      next: (boards) => {
        this.boards.set(boards);
      },
      error: (err) => {
        this.errorService.showError('Failed to load boards');
      },
    });
  }

  createBoard(): void {
    if (!this.newBoardTitle.trim()) {
      return;
    }

    var newBoard: BoardRequest = {
      title: this.newBoardTitle,
      ownerId: this.authService.currentUser()!.id,
      createdAt: new Date(),
    };

    this.boardService.createBoard(newBoard).subscribe({
      next: (board) => {
        this.boards.update((boards) => [...boards, board]);
        this.errorService.showError('Board created successfully');
        this.closeModal();
        this.router.navigate(['/boards', board.id]);
      },
      error: () => {
        this.errorService.showError('Failed to create board');
      },
    });
  }

  openBoard(id: number): void {
    this.router.navigate(['/boards', id]);
  }

  closeModal(): void {
    this.showCreateModal = false;
    this.newBoardTitle = '';
  }

  logout(): void {
    this.authService.logout();
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString();
  }
}
