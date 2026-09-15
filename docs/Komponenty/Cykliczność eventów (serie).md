## Cel
Wykrycie cykliczności eventów (np. `The Game Awards 2024`,`The Game Awards 2025`), aby mieć lepsze rekomendacje.

## Główne elementy
`Sync/Sync/Events`
- `EventNameNormalizer` - regex do normalizacji nazw
- `EvenySyncService` - serwis zarządzający zapisem eventów do bazy
`Tests.Unit`
- `NormalizeNameTests` - test normalizacji wczytujący plik json
- `resources/test_normalize.json` - lista prawdziwych eventów i oczekiwanych nazw

## Przepływ
1. `EventSyncService.DiscoverNewEvents` - składa listę nowych eventów
2. `EventSyncService.AssignToSeries` - bierze listę eventów
	1. Normalizuje nazwy
	2. Sprawdza czy jest exact match
	3. Sprawdza `similarity` z `pg_trgm` z ostatnim eventem w serii (po dacie)
	4. Jeśli 2 i 3 nie spełnione - tworzy nową serię

## Regex
1. daty w formacie liczbowym
2. wyliczenia (1st/2nd/3rd/Xth)
3. angielskie nazwy miesięcy
4. pory roku + "Edition"
5. vol. ...
6. znaki specjalne, interpunkcyjne
7. liczby na końcu nazwy
8. liczby oddzielone spacjami itp (wolno stojące)
9. 2+ spacji
10. Spacje na początku i końcu

## Ograniczenia
Regex nie jest idealny
Eventy mogą nagle zmienić nazwę
Admin musi czasem poprawić przydzielenie eventów
Nazwy muszą być angielskie, inaczej byłoby to zbyt skomplikowane regexem, nie-angielski przypadek:
```json
{
"id": 96,
"name": "Festival Jogatório: 3° Edição",
"expected": "Festival Jogatório ° Edição"
},
```

## Testy
Test jednostkowy do normalizacji wczytuje prawdziwe dane, zapisane w json'ie.

## Powiązane
[[05 - Serie eventów]]