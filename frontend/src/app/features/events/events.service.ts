import { inject, Injectable } from '@angular/core';
import { from, Observable } from 'rxjs';

import { RecommendationApiService } from '../../core/api/generated/services/recommendation-api.service';
import { EventRecommendationResponse } from '../../core/api/generated/models/event-recommendation-response';

@Injectable({
  providedIn: 'root',
})
export class EventsService {
  private api = inject(RecommendationApiService);

  // return Observable to keep existing component code compatible
  getEvents(): Observable<EventRecommendationResponse[]> {
    return from(this.api.recommendationEventsGet$Json());
  }
}

