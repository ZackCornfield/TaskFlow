import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TagService } from '../../services/tag';
import { ErrorService } from '../../../../core/services/error';
import { Tag, TagRequest } from '../../modals/board';

@Component({
  selector: 'app-tag-manager',
  imports: [CommonModule, FormsModule],
  templateUrl: './tag-manager.html',
  styleUrl: './tag-manager.css',
})
export class TagManager implements OnInit {
  private tagService = inject(TagService);
  private errorService = inject(ErrorService);

  tags = signal<Tag[]>([]);
  showAddTagModal = false;
  newTagName = '';
  newTagColor = '#667eea';
  editingTag = signal<Tag | null>(null);
  colorPresets: string[] = [
    '#ff6b6b',
    '#f06595',
    '#cc5de8',
    '#845ef7',
    '#5c7cfa',
    '#339af0',
    '#22b8cf',
    '#20c997',
    '#51cf66',
    '#94d82d',
    '#fcc419',
    '#ff922b',
  ];

  ngOnInit(): void {
    this.loadTags();
  }

  private loadTags(): void {
    this.tagService.getTags().subscribe({
      next: (tags) => this.tags.set(tags),
      error: () => this.errorService.showError('Failed to load tags'),
    });
  }

  createTag(): void {
    if (!this.newTagName.trim()) return;

    const tagRequest: TagRequest = {
      name: this.newTagName.trim(),
      color: this.newTagColor,
    };

    this.tagService.createTag(tagRequest).subscribe({
      next: (tag) => {
        this.tags.update((tags) => [...tags, tag]);
        this.errorService.showSuccess('Tag created successfully');
        this.closeAddTagModal();
      },
      error: () => this.errorService.showError('Failed to create tag'),
    });
  }

  deleteTag(tagId: number): void {
    if (!confirm('Are you sure you want to delete this tag?')) return;

    this.tagService.deleteTag(tagId).subscribe({
      next: () => {
        this.tags.update((tags) => tags.filter((tag) => tag.id !== tagId));
        this.errorService.showSuccess('Tag deleted successfully');
      },
      error: () => this.errorService.showError('Failed to delete tag'),
    });
  }

  openAddTagModal(): void {
    this.showAddTagModal = true;
  }

  closeAddTagModal(): void {
    this.showAddTagModal = false;
    this.newTagName = '';
    this.newTagColor = '#667eea';
    this.editingTag.set(null);
  }

  getColorPreviewStyle(color: string): any {
    return {
      'background-color': color,
    };
  }
}
