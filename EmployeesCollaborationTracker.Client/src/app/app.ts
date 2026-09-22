import { Component, inject, signal } from '@angular/core';
import { CollaborationService } from './services/collaboration.service';
import { ProjectCollaborationRow } from './models/collaboration.model';

@Component({
  imports: [],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
})
export class App {
  private readonly collaborationService = inject(CollaborationService);

  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly rows = signal<ProjectCollaborationRow[]>([]);
  protected readonly fileName = signal<string | null>(null);

  protected onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];

    if (!file) {
      return;
    }

    this.fileName.set(file.name);
    this.error.set(null);
    this.rows.set([]);
    this.loading.set(true);

    this.collaborationService.getCollaborations(file).subscribe({
      next: (results) => {
        this.rows.set(results.flatMap((result) => result.projects));
        this.loading.set(false);
      },
      error: (err) => {
        const message = typeof err?.error === 'string' && err.error.length > 0
          ? err.error
          : 'Failed to process the file.';
        this.error.set(message);
        this.loading.set(false);
      },
    });

    input.value = '';
  }
}
