import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { Storage } from './storage';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  displayName: string;
}

export interface UserDto {
  id: string;
  email: string;
  displayName: string;
  createdAt: string;
  token: string;
}

@Injectable({
  providedIn: 'root',
})
export class Auth {
  constructor(
    private http: HttpClient,
    private router: Router,
    private storage: Storage
  ) {
    this.loadUserFromStorage();
  }

  private readonly API_URL = 'http://localhost:5287/api/auth';

  currentUser = signal<UserDto | null>(null);
  isAuthenticated = signal<boolean>(false);

  login(request: LoginRequest): Observable<UserDto> {
    return this.http
      .post<UserDto>(`${this.API_URL}/login`, request)
      .pipe(tap((user) => this.handleAuthSuccess(user)));
  }

  register(request: RegisterRequest): Observable<UserDto> {
    return this.http
      .post<UserDto>(`${this.API_URL}/register`, request)
      .pipe(tap((user) => this.handleAuthSuccess(user)));
  }

  logout(): void {
    this.storage.removeToken();
    this.storage.removeUser();
    this.currentUser.set(null);
    this.isAuthenticated.set(false);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return this.storage.getToken();
  }

  private handleAuthSuccess(user: UserDto): void {
    this.storage.saveToken(user.token);
    this.storage.saveUser(user);
    this.currentUser.set(user);
    this.isAuthenticated.set(true);
  }

  private loadUserFromStorage(): void {
    const token = this.storage.getToken();
    const user = this.storage.getUser();

    if (token && user) {
      this.currentUser.set(user);
      this.isAuthenticated.set(true);
    }
  }

  // FIXED: Should return TRUE when token is missing or invalid
  isTokenExpired(): boolean {
    const token = this.getToken();
    if (!token) {
      return true; // No token = expired
    }

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expiry = payload.exp * 1000;
      return Date.now() > expiry;
    } catch (error) {
      return true; // Invalid token = expired
    }
  }
}
