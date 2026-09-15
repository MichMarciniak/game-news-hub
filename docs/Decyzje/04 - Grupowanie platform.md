## Kontekst
Jest dużo platform, przydałoby się jakoś pogrupować je aby użytkownik mógł łatwiej je obserwować, oraz łączenie platform przy rekomendacji

## Opcje
- Skorzystanie z platforms_family zewnętrznego api - opcje są ograniczone: `nintendo, sega, xbox, playstation, linux` - brak ogólnego PC
- Skorzystanie z platforms_type zewnętrzengo api - podobnie, opcje są ograniczone, nie ma rozdzielenia między różne konsole. Opcje: `computer, portable console, operating system, platform, arcade, console`
- Zrobienie własnego grupowania - string matching

## Wybór
Własne grupowanie
`Playstation, Xbox, Nintendo, PC, Mobile, Other`

## Powód
Grupowanie z zewnętrznego api jest zbyt ograniczone/trudne do wykorzystania w moim przypadku.
Zbudowanie własnego grupowania daje mi bardziej dostosowane możliwości, zawsze mogę je rozbudować.

## Konsekwencje
- String matching jest ograniczony, czasem admin musi zmienić grupę ręcznie gdy źle jest przydzielony
- Dokładniejsze rekomendacje (np. po PlayStation zamiast dokładnie ps5)