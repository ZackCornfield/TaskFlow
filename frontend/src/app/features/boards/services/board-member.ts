import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { AddBoardMemberRequest, BoardMember } from '../modals/board';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class BoardMemberService {
  private http = inject(HttpClient);
  private readonly API_URL = 'http://localhost:5287/api/boardmember';

  addMember(request: AddBoardMemberRequest): Observable<BoardMember> {
    return this.http.post<BoardMember>(
      `${this.API_URL}/${request.boardId}`,
      request
    );
  }

  removeMember(boardId: number, userId: string): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/${boardId}/${userId}`);
  }

  getMembers(boardId: number): Observable<BoardMember[]> {
    return this.http.get<BoardMember[]>(`${this.API_URL}/${boardId}`);
  }
}
