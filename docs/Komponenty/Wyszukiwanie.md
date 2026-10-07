## Cel
Wyszukiwanie gier i eventów po nazwie, odporne na literówki, z filtrowaniem i sortowaniem.

## Główne elementy
`Api/Features/Events`
- `EventService` - 
`Api/Features/Games`
- `GameService` - 


Rozszerzenia PostgreSQL:
- `pg_trgm` - similarity

`FollowerCount` - kolumna gier i eventów do popularności

### Wszystkie ustawienia
- Query - string
- Sortowanie: alfabetycznie, relevance, popularność, data(E)
- Filtry: grupy platform, gatunki
- Paginacja: page, pageSize

## Przepływ
### Gry
1. Walidacja i przygotowanie paginacji
2. Baza zapytanie i filtrowanie wstępne - tylko GameType=Main
3. ([[todo]] Filtrowanie platform i gatunków)
4. (if query) oczyszczenie i przekazanie do zapytania
5. zliczenie wszystkich wyników
6. Sortowanie:
	1. Domyślne - Tytuł
	2. Tytuł - po tytule, alfabetycznie
	3. Popularność - po liczbach polubień (lokalnie w bazie)
	4. Dopasowanie (Relevance): [[#Relevance sort]]
7. Paginacja
8. Rzutowanie do Dto
9. Zwraca SearchDto zawierający wyniki i liczbę znalezionych (krok 4)

### Wydarzenia
1. Walidacja i przygotowanie paginacji
2. (if query) oczyszczenie i przekazanie do zapytania
3. zliczenie wszystkich wyników
4. Sortowanie:
	1. Domyślne - Data
	2. Data - po StartTime
	3. Popularność - liczba polubień (obserwacji)
	4. Dopasowanie (Relevance): [[#Relevance sort]]
5. Paginacja
6. Rzutowanie do Dto
7. SearchDto - wyniki, liczba znalezionych

## Relevance sort
5 warunków - od najważniejszego do najmniej:
1. exact match - query jest takie same jak nazwa wydarzenia
2. prefix - nazwa wydarzenia zaczyna się od query (`query*`)
3. wordStart - słowo w nazwie się od tego zaczyna (`* query*`)
4. like - nazwa zawiera gdzieś query (`*query*`)
5. similarity - [[Trójgramy]]
6. popularność - liczba obserwacji


## Filtrowanie
[[todo]]

## Konfiguracja
### Baza danych
#### Rozszerzenia
```sql
CREATE EXTENSION IF NOT EXISTS pg_trgm;
-- CREATE EXTENSION IF NOT EXISTS fuzzystrmatch; -- Już nie używane
```
#### Indeksy
[[Trójgramy#Index]]
```
CREATE INDEX ix_games_title_trgm ON "Games" USING GIN ("Title" gin_trgm_ops);
```


### Zmienne
`appsettings.Development.json : Search`:
- [[todo]]



## Powiązane
[[Zliczanie obserwacji]]
