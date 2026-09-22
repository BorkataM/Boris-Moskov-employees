import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { CollaborationResult } from '../models/collaboration.model';

const API_URL = 'http://localhost:5036/api/employees/collaborations';

@Injectable({ providedIn: 'root' })
export class CollaborationService {
  private readonly http = inject(HttpClient);

  getCollaborations(file: File): Observable<CollaborationResult[]> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<CollaborationResult[]>(API_URL, formData).pipe(
      catchError((err: HttpErrorResponse) => {
        const message = typeof err.error === 'string' && err.error.length > 0
          ? err.error
          : 'Failed to process the file.';

        return throwError(() => new Error(message));
      }),
    );
  }
}
