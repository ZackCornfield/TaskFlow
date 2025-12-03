import { Injectable } from '@angular/core';
import { UserDto } from './auth';

@Injectable({
  providedIn: 'root',
})
export class StorageService {
  private readonly TOKEN_KEY = 'auth_token';
  private readonly USER_KEY = 'auth_user';

  saveToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  saveUser(user: UserDto): void {
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  getUser(): UserDto | null {
    const userJson = localStorage.getItem(this.USER_KEY);
    return userJson ? JSON.parse(userJson) : null;
  }

  removeToken(): void {
    localStorage.removeItem(this.TOKEN_KEY);
  }

  removeUser(): void {
    localStorage.removeItem(this.USER_KEY);
  }

  clear(): void {
    this.removeToken();
    this.removeUser();
  }
}
