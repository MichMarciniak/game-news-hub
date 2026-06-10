import { Component, inject, OnInit, signal } from '@angular/core';
import { EventsService } from '../events.service';
import { EventRecommendationResponse } from '@/core/api/generated/models/event-recommendation-response';
import { MatListModule } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { tap } from 'rxjs';

@Component({
  selector: 'app-event-list-page',
  standalone: true,
  imports: [MatListModule, MatProgressSpinnerModule],
  templateUrl: './event-list-page.component.html',
  styleUrl: './event-list-page.component.scss',
})
export class EventListPageComponent implements OnInit {
  private service = inject(EventsService);

  events = signal<EventRecommendationResponse[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);

  ngOnInit(): void {
    this.load()
  }

  load() {
    this.loading.set(true);
    this.error.set(null);

    this.service.getEvents()
    .pipe(
      tap(data => console.log("DATA", data)),
    )
    .subscribe({
      next: (data) => {
        this.events.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Couldn\'t load data');
        this.loading.set(false);
      }
    });
  }
}
