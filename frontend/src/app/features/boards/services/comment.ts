import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Comment, CommentRequest } from '../modals/board';
import { Observable } from 'rxjs';
@Injectable({
  providedIn: 'root',
})
export class CommentService {
  private http = inject(HttpClient);
  private readonly API_URL = 'http://localhost:5287/api/comment';

  createComment(taskId: number, request: CommentRequest): Observable<Comment> {
    return this.http.post<Comment>(`${this.API_URL}/${taskId}`, request);
  }

  updateComment(id: number, request: CommentRequest): Observable<Comment> {
    return this.http.put<Comment>(`${this.API_URL}/${id}`, request);
  }

  deleteComment(id: number): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/${id}`);
  }
}
