Powiązane: [[05 - Serie eventów]]
## Kontekst
Przechowywanie znormalizowanych (po regexie) nazw serii wprowadza problem Semantic Drift (A ~ B, B ~ C, A !~ C).
Igdb nie ma serii eventów.

## Opcje
- Zaakceptować problem
- Porównywać do wszystkich eventów w serii
- Porównywać do ostatniego eventu

## Wybór
Porównanie do ostatniego eventu

## Powód
Gdy nowy event jest pobrany, i ma podobną znormalizowaną nazwę w bazie, to jest dodawany do serii.
Porównanie do wszystkich eventów w serii nie jest aż tak potrzebne.
Gdy nazwa się zmieni (np. rebranding), to admin może dostosować serię tego eventu, i następny cykl tej serii będzie już dobry.

## Konsekwencje
- Admin może musieć dostosować
- Możliwe jest wykrycie serii eventu