import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { MoveTaskRequest, Task, TaskRequest } from '../modals/board';
import { Observable } from 'rxjs';
@Injectable({
  providedIn: 'root',
})
export class TaskService {
  private http = inject(HttpClient);
  private readonly API_URL = 'http://localhost:5287/api/task';

  createTask(columnId: number, request: TaskRequest): Observable<Task> {
    return this.http.post<Task>(`${this.API_URL}/${columnId}`, request);
  }

  updateTask(id: number, request: TaskRequest): Observable<Task> {
    return this.http.put<Task>(`${this.API_URL}/${id}`, request);
  }

  deleteTask(id: number): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/${id}`);
  }

  moveTask(id: number, request: MoveTaskRequest): Observable<Task> {
    return this.http.patch<Task>(`${this.API_URL}/${id}/move`, request);
  }
}
