## Kontekst
Potrzebowałem wykrywania cykliczności eventów do rekomendacji

## Opcje
- Embeddingi - dodatkowa złożoność
- String matching - może być trudne do implementacji
- LLM - dodatkowa złożoność

## Wybór
String matching: `similarity` z `pg_trgm`, z czyszczeniem nazw z dat, edycji itp. przez regex.
Matching przez same nazwy, bez opisów.

Powiązane: [[01 - Baza danych]]

## Powód
Inne opcje niż string matching są bardziej skomplikowane, zbyt duże jak na takie proste zadanie.
Opisy są generyczne, nic nie wprowadzają unikalnego do eventów, czasem brakuje opisu, dlatego przez same nazwy.
## Konsekwencje
- Zbudowanie własnego regexa
- Jeśli similarity się pomyli, admin musi dostosować.