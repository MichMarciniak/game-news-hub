import { Component, inject, OnInit, signal } from '@angular/core';
import { EventsService } from '../events.service';
import { EventRecommendationResponse } from '@/core/api/generated/models/event-recommendation-response';
import { MatListModule } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { tap } from 'rxjs';
import { RouterLink } from "@angular/router";
import { UserInterestService } from '@/features/user-interest/user-interest.service';

@Component({
  selector: 'app-event-list-page',
  standalone: true,
  imports: [MatListModule, MatProgressSpinnerModule],
  templateUrl: './event-list-page.component.html',
  styleUrl: './event-list-page.component.scss',
})
export class EventListPageComponent implements OnInit {
  private service = inject(EventsService);
  private userInterestService = inject(UserInterestService);

  events = signal<EventRecommendationResponse[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);
  followLoading = signal(false);

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

  toggleFollow(id: number | string | undefined): void {
    id = Number(id);
    if (Number.isNaN(id)) {
      return;
    }

    this.followLoading.set(true);
    this.userInterestService.toggleFollowGame(id).subscribe({
      next: () => {
        this.events.update(events =>
          events.map(event =>
            event.id === id
              ? {...event, isFollowed: !event.isFollowed}
              : event
          )
        )
        this.followLoading.set(false);
      },
      error: () => {
        this.error.set('Could not update follow state');
        this.followLoading.set(false);
      },
    });
  }
}
