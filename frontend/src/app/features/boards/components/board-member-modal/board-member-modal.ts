import {
  Component,
  inject,
  input,
  OnInit,
  output,
  signal,
} from '@angular/core';
import { BoardMemberService } from '../../services/board-member';
import { AddBoardMemberRequest, BoardMember } from '../../modals/board';
import { ErrorService } from '../../../../core/services/error';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-board-member-modal',
  imports: [CommonModule, FormsModule],
  templateUrl: './board-member-modal.html',
  styleUrl: './board-member-modal.css',
})
export class BoardMemberModal implements OnInit {
  boardId = input.required<number>();
  ownerId = input.required<string>();
  currentUserId = input.required<string>();
  closeModal = output<void>();
  membersUpdated = output<void>();

  private boardMemberService = inject(BoardMemberService);
  private errorService = inject(ErrorService);

  members = signal<BoardMember[]>([]);
  isLoading = false;
  isAdding = false;
  newMemberEmail = '';
  newMemberRole: 'admin' | 'member' = 'member';

  ngOnInit(): void {
    this.loadBoardMembers();
  }

  loadBoardMembers(): void {
    this.isLoading = true;
    this.boardMemberService.getMembers(this.boardId()).subscribe({
      next: (members) => this.members.set(members),
      error: () => {
        this.errorService.showError('Failed to load board members');
      },
      complete: () => {
        this.isLoading = false;
      },
    });
  }

  addMember(): void {
    if (!this.newMemberEmail.trim()) {
      this.errorService.showError('Please enter a valid email address');
      return;
    }

    this.isAdding = true;

    const request: AddBoardMemberRequest = {
      email: this.newMemberEmail.trim(),
      boardId: this.boardId(),
      role: this.newMemberRole,
    };

    this.boardMemberService.addMember(request).subscribe({
      next: (member) => {
        if (member) {
          this.members.update((members) => [...members, member]);
          this.newMemberEmail = '';
          this.newMemberRole = 'member';
          this.membersUpdated.emit();
        }
      },
      error: () => {
        this.errorService.showError('Failed to add board member');
      },
      complete: () => {
        this.isAdding = false;
      },
    });
  }

  removeMember(member: BoardMember): void {
    if (!confirm(`Are you sure you want to remove ${member.user.displayName}?`))
      return;

    this.boardMemberService
      .removeMember(this.boardId(), member.userId)
      .subscribe({
        next: () => {
          this.members.update((members) =>
            members.filter((m) => m.userId !== member.userId)
          );
          this.errorService.showSuccess('Board member removed successfully');
          this.membersUpdated.emit();
        },
        error: () => {
          this.errorService.showError('Failed to remove board member');
        },
      });
  }

  canRemoveMember(member: BoardMember): boolean {
    // Owner can remove anyone except themselves
    // Admins can remove non-admins
    if (member.userId === this.ownerId()) {
      return false; // Can't remove the owner
    }

    if (this.currentUserId() === this.ownerId()) {
      return true; // Owner can remove anyone
    }

    const currentMember = this.members().find(
      (m) => m.userId === this.currentUserId()
    );
    if (currentMember?.role === 'admin' && member.role !== 'admin') {
      return true; // Admin can remove non-admins
    }

    return false;
  }

  getInitials(name: string): string {
    return name
      .split(' ')
      .map((n) => n[0])
      .join('')
      .toUpperCase()
      .substring(0, 2);
  }

  close(): void {
    this.closeModal.emit();
  }
}
