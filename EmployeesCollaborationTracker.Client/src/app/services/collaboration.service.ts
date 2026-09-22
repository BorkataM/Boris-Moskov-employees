import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CollaborationResult } from '../models/collaboration.model';

const API_URL = 'http://localhost:5036/api/employees/collaborations';

@Injectable({ providedIn: 'root' })
export class CollaborationService {
  private readonly http = inject(HttpClient);

  getCollaborations(file: File): Observable<CollaborationResult[]> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<CollaborationResult[]>(API_URL, formData);
  }
}
