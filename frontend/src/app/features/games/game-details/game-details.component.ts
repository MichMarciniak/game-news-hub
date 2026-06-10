import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { GameDetailsResponse } from '@/core/api/generated/models';
import { GamesService } from '../games.service';
import { UserInterestService } from '../../user-interest/user-interest.service';

@Component({
  selector: 'app-game-details',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './game-details.component.html',
  styleUrl: './game-details.component.scss',
})
export class GameDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private service = inject(GamesService);
  private userInterestService = inject(UserInterestService);

  game = signal<GameDetailsResponse | null>(null);
  followed = signal(false);
  loading = signal(false);
  followLoading = signal(false);
  error = signal<string | null>(null);

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (Number.isNaN(id)) {
      this.error.set('Invalid game id');
      return;
    }

    this.loading.set(true);
    forkJoin({
      game: this.service.getGameDetails(id),
      followedIds: this.userInterestService.getFollowedGameIds(),
    }).subscribe({
      next: ({ game, followedIds }) => {
        this.game.set(game);
        this.followed.set(followedIds.includes(id));
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load game details');
        this.loading.set(false);
      },
    });
  }

  toggleFollow(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (Number.isNaN(id)) {
      return;
    }

    this.followLoading.set(true);
    this.userInterestService.toggleFollowGame(id).subscribe({
      next: () => {
        this.followed.set(!this.followed());
        this.followLoading.set(false);
      },
      error: () => {
        this.error.set('Could not update follow state');
        this.followLoading.set(false);
      },
    });
  }
}