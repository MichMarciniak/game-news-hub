import { inject, Injectable } from '@angular/core';
import { from, Observable, map } from 'rxjs';

import { UserInterestApiService } from '@/core/api/generated/services';

@Injectable({
  providedIn: 'root',
})
export class UserInterestService {
  private api = inject(UserInterestApiService);

  getFollowedGameIds(): Observable<number[]> {
    return from(this.api.followedGamesGet$Json()).pipe(
      map((ids) => ids.map((id) => Number(id)))
    );
  }

  toggleFollowGame(gameId: number): Observable<void> {
    return from(this.api.followGameGameIdPost({ gameId }));
  }

  toggleFollowEvent(eventId: number): Observable<void> {
    return from(this.api.followEventEventIdPost({eventId}))
  }
}