## Cel
Wyszukiwanie gier i eventów po nazwie, odporne na literówki, z filtrowaniem (platformy, gatunki, data, typ) i sortowaniem (trafność, popularność, data, alfabetycznie)

## Główne elementy
`Api/Features/Events`
- `EventService` - 
`Api/Features/Games`
- `GameService` - 

`Api/Options`
- `SearchOptions` - [[#Konfiguracja]] stałych progów

Rozszerzenia PostgreSQL:
- `pg_trgm` - similarity
- `fuzzystrmatch` - levenshtein

`FollowerCount` - kolumna gier i eventów do popularności


## Przepływ
### Wyszukiwanie tekstowe
1. Zapytanie użytkownika (`/search` query param)
2. Zapytanie SQL łączy:
	1. `ExactContains` - dokładny match
	2. `LevenshteinLessEqual` - literówki
	3. `TrigramsSimilarity` - próg 30%, łapie inną kolejność słów
3. Sortowanie: `ExactContains` -> `Distance` -> `Similarity` -> `FollowerCount`
4. Paginacja (`Skip`/`Take`) na posortowanym, przefiltrowanym wyniku

### Filtrowanie
[[todo]]

## Konfiguracja
### Baza danych
#### Rozszerzenia
```
CREATE EXTENSION IF NOT EXISTS pg_trgm;
CREATE EXTENSION IF NOT EXISTS fuzzystrmatch;
```
#### Indeksy
```
CREATE INDEX ix_games_title_trgm ON "Games" USING GIN ("Title" gin_trgm_ops);
```

### Zmienne
`appsettings.Development.json : Search`:
- [[todo]]


## Ograniczenia
Do poklikania i potestowania [[todo]] ale jak będzie frontend żeby było łatwiej

<details>
<summary>Gry z filtrowaniem przez ILIKE</summary>
<ul>
 <li>"The Witcher 3: Wild Hunt"
 <li>"Switcheroo"
 <li>"The Witcher 3: Wild Hunt Remastered"
</ul>

</details>

<details>
<summary>Gry z filtrowaniem przez % (similarity ~> 0.3)</summary>
<ul>
 <li>"The Witcher 3: Wild Hunt"
 <li>"Switcheroo"
 <li>"Witchy Tower"
 <li>"Dog Witch"
 <li>"Witchspire"
 <li>"Tiny Witch"
 <li>"Witchbrook"
 <li>"Warding Witches"
 <li>"The Witches' Whisk"
 <li>"Garden of Witches"
</ul>

</details>




## Powiązane
[[Zliczanie obserwacji]]
