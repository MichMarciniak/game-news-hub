import { Routes } from '@angular/router';

export const routes: Routes = [
    { path: '', pathMatch: 'full', redirectTo: 'events' },
    { path: 'login', pathMatch: 'full', redirectTo: 'events' },
    { 
        path: 'games',
        loadComponent: () =>
            import('./features/games/game-list/game-list.compoment')
            .then(m => m.GameListCompoment)
    },
    {
        path: 'games/:id',
        loadComponent: () =>
            import('./features/games/game-details/game-details.component')
            .then(m => m.GameDetailsComponent)
    },
    { path: 'genres', pathMatch: 'full', redirectTo: 'events' },
    {
        path: 'events',
        loadComponent: () =>
            import('./features/events/event-list-page/event-list-page.component')
                .then(m => m.EventListPageComponent)
    },
    // {
    //     path: 'events/:id',
    //     loadComponent: () =>
    //         import('./features/events/event-details')
    // }
    { path: '**', redirectTo: 'events' }
];
