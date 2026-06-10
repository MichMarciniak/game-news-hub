import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { GamesService } from '../games.service';
import { GameListResponse } from '@/core/api/generated/models';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatListModule } from "@angular/material/list";
import { tap } from 'rxjs';

@Component({
  selector: 'app-game-list',
  standalone: true,
  imports: [MatProgressSpinnerModule, MatListModule, RouterLink],
  templateUrl: './game-list.compoment.html',
  styleUrl: './game-list.compoment.scss',
})
export class GameListCompoment implements OnInit {
  private service = inject(GamesService);

  games = signal<GameListResponse[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);

  ngOnInit(): void {
    this.load()
  }

  load() {
    this.loading.set(true);
    this.error.set(null);

    this.service.getGames()
    .pipe(
      tap(data => console.log("games: ", data))
    )
    .subscribe({
      next: (data) => {
        this.games.set(data);
        this.loading.set(false);
      },
      error: (error) => {
        this.error.set(error);
        this.loading.set(false);
      },
    })
  }
}
