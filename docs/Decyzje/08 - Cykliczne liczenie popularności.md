## Kontekst
Przydały by się opcje do sprawdzenia popularności eventów i gier (po liczbie obserwacji)

## Opcje
- Dodanie popularności
- Obliczanie ich przy każdej obserwacji
- Cykliczne obliczanie

## Wybór
Dodanie popularności + cyklicznie obliczanie.

## Powód
Dostaje wyznacznik popularności po liczbie obserwacji, a jest to liczone cyklicznie, w SQL'em. Ogranicza to operacje na bazie danych.
Obliczanie przy każdej obserwacji jest złym pomysłem - nie potrzebuję dokładnych danych do co sekundy. 

## Konsekwencje
- SQL w kodzie
- Mam liczby obserwacji w eventach i grach
- Background service w api