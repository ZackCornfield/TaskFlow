import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Column, ColumnRequest } from '../modals/board';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ColumnService {
  private http = inject(HttpClient);
  private readonly API_URL = environment.apiUrl + '/columns';

  createColumn(boardId: number, request: ColumnRequest): Observable<Column> {
    return this.http.post<Column>(`${this.API_URL}/${boardId}`, request);
  }

  updateColumn(id: number, request: ColumnRequest): Observable<Column> {
    return this.http.put<Column>(`${this.API_URL}/${id}`, request);
  }

  deleteColumn(id: number): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/${id}`);
  }
}
