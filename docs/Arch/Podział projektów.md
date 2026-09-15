## Lista projektów
| Nazwa             | Odpowiedzialność                                   | Zależy od                      |
| ----------------- | -------------------------------------------------- | ------------------------------ |
| Api               | Główna część aplikacji, interakcja z użytkownikiem | Contracts, Data                |
| Contracts         | Definicje DTO wysyłane przez api                   | -                              |
| Data              | Definicje encji i konfiguracji bazy danych         | -                              |
| Sync              | Pobieranie danych z igdb api                       | Data                           |
| Tests.Integration | Testy aplikacji wymagające bazy danych             | Api, Contracts, <br>Data, Sync |
| Tests.Unit        | Testy jednostkowe prostych funkcji                 | Api, Contracts, Data, Sync     |

## Diagram



## Dlaczego Sync osobny
Api odpowiada za główną logikę aplikacji, interakcję z użytkownikiem itp. Sync jedynie komunikuje się z igdb i bazą danych - cyklicznie pobiera i zapisuje dane do bazy, nie potrzebuje endpointów, nie potrzebuje dużo zasobów
Dzięki temu, Api można może działać bez Sync, i możliwe będzie włączenie kilku Api, a 1 Sync wystarczy.