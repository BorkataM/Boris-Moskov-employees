import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { CollaborationResult } from '../models/collaboration.model';

@Injectable({ providedIn: 'root' })
export class CollaborationService {
  private readonly http = inject(HttpClient);

  getCollaborations(file: File): Observable<CollaborationResult[]> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<CollaborationResult[]>(environment.apiUrl, formData);
  }
}
