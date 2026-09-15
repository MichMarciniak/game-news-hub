## Tryby uruchomienia
- default: Pobiera dane okresowo, sprzed 30 dni
- bulk: Pobiera dane sprzed 600 dni.
> Czas (dni) da się zmienić w `appsettings.Development.json`, lub ustawiając zmienną przy uruchomieniu: `SyncDaysRange`.


## Różnica
Bulk pobiera max. 500 rekordów, default max. 100 (domyślny limit igdb)

## Pobierane dane
### Eventy
- Nazwa
- Opis (może być pusty)
- Start time
- End time (może być pusty)
- Id gier (może być pusty)
Na koniec pobierania, przekazywana jest lista id gier do pobrania.
### Gry
- Nazwa
- Summary
- Url obrazka
- Nazwy gatunków
- Nazwy platform
- Kategorie
- Parent game (może być puste)
- Typ gry

## Gatunki
- Nazwa
## Platformy
- Nazwa