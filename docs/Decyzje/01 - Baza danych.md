## Kontekst
Potrzebowałem bazy danych do przechowywania użytkowników i danych z zewnętrznego api

## Wybór
PostgreSQL

## Powód
1. Potrzebowałem relacyjnej bazy danych
2. Postgres ma funkcje: `similarity` z `pg_trgm` ~~oraz `levenshtein` z `fuzzystrmatch`

## Konsekwencje
- Dostęp do funkcji postgresa
- Musi działać kontener dockera
- Testy też muszą być wykonywane z kontenerem

