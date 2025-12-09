import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Tag, TagRequest } from '../modals/board';
import { environment } from '../../../../environments/environment';
@Injectable({
  providedIn: 'root',
})
export class TagService {
  private http = inject(HttpClient);
  private readonly API_URL = environment.apiUrl + '/tag';

  getTags(): Observable<Tag[]> {
    return this.http.get<Tag[]>(this.API_URL);
  }

  createTag(request: TagRequest): Observable<Tag> {
    return this.http.post<Tag>(this.API_URL, request);
  }

  deleteTag(id: number): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/${id}`);
  }

  addTagsToTask(taskId: number, tagIds: number[]): Observable<Tag[]> {
    return this.http.post<Tag[]>(`${this.API_URL}/${taskId}/tags`, tagIds);
  }

  removeTagsFromTask(taskId: number, tagIds: number[]): Observable<void> {
    return this.http.request<void>('delete', `${this.API_URL}/${taskId}/tags`, {
      body: tagIds,
    });
  }

  getTagsForTask(taskId: number): Observable<Tag[]> {
    return this.http.get<Tag[]>(`${this.API_URL}/${taskId}/tags`);
  }
}
