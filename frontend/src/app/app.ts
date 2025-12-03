import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LoadingComponent } from './shared/components/loading/loading';
import { NotificationComponent } from './shared/components/notification/notification';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, LoadingComponent, NotificationComponent],
  template: `
    <app-loading />
    <app-notification />
    <router-outlet />
  `,
  styleUrls: ['./app.css'],
})
export class App {
  protected readonly title = signal('TaskFlow');
}
