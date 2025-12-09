import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Board, BoardRequest } from '../modals/board';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class BoardService {
  private http = inject(HttpClient);
  private readonly API_URL = environment.apiUrl + '/boards';

  getBoards(): Observable<Board[]> {
    return this.http.get<Board[]>(this.API_URL);
  }

  getBoardById(id: number): Observable<Board> {
    return this.http.get<Board>(`${this.API_URL}/${id}`);
  }

  createBoard(request: BoardRequest): Observable<Board> {
    return this.http.post<Board>(this.API_URL, request);
  }

  updateBoard(id: number, request: BoardRequest): Observable<Board> {
    return this.http.put<Board>(`${this.API_URL}/${id}`, request);
  }

  deleteBoard(id: number): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/${id}`);
  }

  getUserBoards(userId: string): Observable<Board[]> {
    return this.http.get<Board[]>(`${this.API_URL}/users/${userId}/boards`);
  }
}
