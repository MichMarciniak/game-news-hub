## Cel
Pobieranie danych z zewnętrznego api

## Główne elementy


## Przepływ
### Event sync
1. ustawia zakres czasu z którego pobiera wydarzenia
2. pobiera wydarzenia i zbiera ich id
3. wyznacza nowe, nieistniejące wydarzenia w bazie
4. dla każdego tworzy nowy wiersz w bazie
5. [[#Wyznaczanie serii]]
6. jeśli istnieją, zapis do bazy

### Wyznaczanie serii
1. wyznacza znormalizowaną nazwę
2. (jeśli pusta, ustawia nazwę wydarzenia jako znormalizowana)
3. jeśli:
	1. jest exact match nazwy znormalizowanej z inną istniejącą -> ustawia serię
	2. trigram similarity jest >= 0.85 - sortuje wg. similarity i ustawia ustawia serię do pierwszego wydarzenia
	3. 

## Konfiguracja

## Api
| Metoda | Ścieżka | Opis |
| ------ | ------- | ---- |
|        |         |      |
|        |         |      |

## Ograniczenia

## Powiązane