import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ColumnRequest, MoveTaskRequest } from '../models/board';
import { Observable } from 'rxjs';
@Injectable({
  providedIn: 'root',
})
export class Task {
  private http = inject(HttpClient);
  private readonly API_URL = 'https://localhost:5287/api/task';

  createTask(columnId: number, request: ColumnRequest): Observable<Task> {
    return this.http.post<Task>(`${this.API_URL}/${columnId}`, request);
  }

  updateTask(id: number, request: ColumnRequest): Observable<Task> {
    return this.http.put<Task>(`${this.API_URL}/${id}`, request);
  }

  deleteTask(id: number): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/${id}`);
  }

  moveTask(id: number, request: MoveTaskRequest): Observable<Task> {
    return this.http.put<Task>(`${this.API_URL}/${id}/move`, request);
  }
}
