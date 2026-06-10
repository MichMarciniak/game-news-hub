import { GameDetailsResponse, GameListResponse } from '@/core/api/generated/models';
import { GameApiService } from '@/core/api/generated/services';
import { inject, Injectable } from '@angular/core';
import { from, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class GamesService {
  private api = inject(GameApiService);

  getGames(): Observable<GameListResponse[]> {
    return from(this.api.gameListGet$Json());
  }

  getGameDetails(id: number): Observable<GameDetailsResponse> {
    return from(this.api.gameGameIdGet$Json({ gameId: id }));
  } 
}
