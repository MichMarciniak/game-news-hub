## Cel
Ograniczenie szumu przy wyszukiwaniu gdy gra ma zbyt dużo dlc
Lepsze rekomendacje - jeśli ktoś obserwuje dlc, to ta gra i inne dlc też wliczają się do rekomendacji

## Główne elementy
`Sync/Sync/Games`
- `GameSyncService` - pobranie i dodanie parent game'ów

`Data/Entities
- `GameTypes` - zmapowane enum na to co zwraca igdb

`Api/Configuration`
- `GameTypePolicy` - które typy gry są uważane za "główne" (przy wyszukiwaniu)

## Przepływ
### Dodanie parent game
1. `GameSyncService.GetOrCreateGamesAsync` po pobraniu gier, ustawia parent game igdb id jeśli jest
2. `GameSyncService.PullParentGames` - sprawdza czy są parent gry
3. Jeśli tak, pobiera parent gry i przypisuje je do gier
4. Jeśli parent gra ma swój parent, to pobiera się ponownie
5. Kończy się, gdy gra nie ma parenta, lub trafia na visited

## Konfiguracja
Gry główne: (wyszukiwalne)
- `MainGame`
- `Remake`
- `Remaster`
- `Fork`

## Powiązane
