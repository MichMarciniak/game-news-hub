## Cel
Do ułatwienia użytkownikowi obserwowanie platform (np. PlayStation zamiast ps5, ps4 itd.)
Do alg. rekomendacji
## Główne elementy
`Sync/Sync/Platforms`
- `PlatformSyncService` - "zgadywacz" grup

`Sync/Seeder`
- `PlatformGroupSeeder` - dodaje grupy do bazy, jeśli nie istnieją

`Api/Features/Admin`
- `AdminController` - akcje admina do ręcznej zmiany grup w przypadku błędu

`Api/Features/Platform`
- `PlatformService` - logika zmiany grupy platformy 
## Przepływ
### Zgadywanie
1. `PlatformSyncService.GetOrCreateBatchAsync` - zbiera platformy, których nie ma w bazie
2. `PlatformSyncService.GuessGroup` - dopasowuje grupę po nazwie

### Zmiana grupy
1. POST `/admin/platform/reassign` przyjmuje id platformy i grupy
2. `PlatformService.ReassignPlatformGroup` szuka platformy i grupy i je łączy

## Konfiguracja
Grupy:
`Playstation, Xbox, Nintendo, PC, Mobile, Other`

## Api
| Metoda | Ścieżka                  | Opis                                                                   |
| ------ | ------------------------ | ---------------------------------------------------------------------- |
| POST   | /admin/platform/reassign | Przyjmuje id grupy i platformy, zmienia grupę platformy jeśli istnieją |

## Ograniczenia
Zgadywanie grup nie jest idealne (string matching)
Grupy są ograniczone do najbardziej znanych (inne - `Other`)
Admin musi dostosować jeśli coś pójdzie nie tak


## Powiązane
[[04 - Grupowanie platform]]