import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';
import { guestGuard } from './core/guards/guest-guard';
import { TagManager } from './features/boards/components/tag-manager/tag-manager';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'boards',
    pathMatch: 'full',
  },
  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/login/login').then((m) => m.Login),
  },
  {
    path: 'register',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/register/register').then((m) => m.Register),
  },
  {
    path: 'boards',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/boards/board-list/board-list').then(
        (m) => m.BoardList
      ),
  },
  {
    path: 'boards/:id',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/boards/board-detail/board-detail').then(
        (m) => m.BoardDetail
      ),
  },
  {
    path: 'tags',
    component: TagManager,
    canActivate: [authGuard],
  },
  {
    path: '**',
    redirectTo: 'boards',
  },
];
