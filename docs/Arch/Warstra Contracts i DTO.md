## Zasady
Ten projekt ma być tylko zawierać modele danych, bez żadnych referencji
Enumy też tam są napisane (powielone, ale wymagane jeśli projekt ma być czysty) - testy są w `Tests.Unit/EnumMappingTests`
Mappery znajdują się w projekcie Api, bo łączy projekt Data i Contracts.
Czasem też potrzebna jest dodatkowa logika do zmapowania encji na DTO.


## Nazewnictwo
Pliki są podzielone odpowiedzialnościami (np. GameDto), a w plikach są odpowiednie DTO.
XXXDetailDto - szczegóły czegoś
XXXListItemDto - uproszczone dto do użycia w listach
XXXDto - gdy jest małe / nie potrzeba oddzielnych szczegółów i listy.
