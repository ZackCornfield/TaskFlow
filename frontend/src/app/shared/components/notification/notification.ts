import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Error, ErrorMessage } from '../../../core/services/error';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="notification-container">
      @for (error of errorService.errors(); track error.id) {
      <div
        class="notification"
        [class.error]="error.type === 'error'"
        [class.success]="error.type === 'success'"
        [class.info]="error.type === 'info'"
        [class.warning]="error.type === 'warning'"
      >
        <div class="notification-content">
          <span class="notification-icon">
            @switch (error.type) { @case ('error') { ❌ } @case ('success') { ✅
            } @case ('info') { ℹ️ } @case ('warning') { ⚠️ } }
          </span>
          <span class="notification-message">{{ error.message }}</span>
        </div>
        <button
          class="notification-close"
          (click)="errorService.removeError(error.id)"
        >
          ✕
        </button>
      </div>
      }
    </div>
  `,
  styles: [
    `
      .notification-container {
        position: fixed;
        top: 1rem;
        right: 1rem;
        z-index: 9999;
        display: flex;
        flex-direction: column;
        gap: 0.5rem;
        max-width: 400px;
      }

      .notification {
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 1rem;
        border-radius: 0.5rem;
        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
        animation: slideIn 0.3s ease-out;
        background: white;
        border-left: 4px solid;
      }

      @keyframes slideIn {
        from {
          transform: translateX(100%);
          opacity: 0;
        }
        to {
          transform: translateX(0);
          opacity: 1;
        }
      }

      .notification.error {
        border-left-color: #ef4444;
        background-color: #fef2f2;
      }

      .notification.success {
        border-left-color: #10b981;
        background-color: #f0fdf4;
      }

      .notification.info {
        border-left-color: #3b82f6;
        background-color: #eff6ff;
      }

      .notification.warning {
        border-left-color: #f59e0b;
        background-color: #fffbeb;
      }

      .notification-content {
        display: flex;
        align-items: center;
        gap: 0.75rem;
        flex: 1;
      }

      .notification-icon {
        font-size: 1.25rem;
      }

      .notification-message {
        color: #374151;
        font-size: 0.875rem;
        line-height: 1.25rem;
      }

      .notification-close {
        background: none;
        border: none;
        color: #9ca3af;
        cursor: pointer;
        font-size: 1.25rem;
        padding: 0;
        margin-left: 0.5rem;
        transition: color 0.2s;
      }

      .notification-close:hover {
        color: #374151;
      }
    `,
  ],
})
export class NotificationComponent {
  errorService = inject(Error);
}
