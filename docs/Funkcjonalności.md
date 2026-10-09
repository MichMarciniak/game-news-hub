> Obecne funkcjonalności

> Mogą nie być kompletne
# Użytkownik końcowy
- **Konto i bezpieczeństwo**
	- rejestracja z potwierdzeniem e-mail
	- logowanie/wylogowanie
	- reset i zmiana hasła (email: confirm, reset, powierdzenie o zmianie)
	- cookie auth (Identity), role user/admin
- **Strona główna**
	- gość: treść bez personalizacji
	- zalogowany: lista rekomendacji + nadchodzący tydzień
	- nowe konto: zachęta do follow gatunków i platform, jeśli puste
- **Kalendarz i lista wydarzeń**
	- kalendarz miesięczny wydarzeń
	- lista wydarzeń w danym dniu
	- strona szczegółów wydarzenia
	- gry powiązane z wydarzeniem
	- inne edycje wydarzenia w serii
- **Wyszukiwanie wydarzeń**
	- zapytanie tekstowe
	- sortowanie: data, popularność, relevance (exact -> prefix -> początek słowa -> ILIKE -> similarity -> popularność)
	- paginacja
- **Gry**
	- wyszukiwanie (główne gry: Main/Remake/Remaster/Fork, żeby DLC nie zaśmiecały wyników)
	- sortowanie: tytuł, popularność, relevance (tak jak przy wydarzeniach)
	- filtry: gatunki i platformy (todo)
	- strona gry: typ, data premiery, gatunki, grupy platform, gra nadrzędna, dodatki/DLC
- Obserwowanie (follow)
	- gry, wydarzenia, gatunki, grupy platform (np. PlayStation zamiast ps4/5)
- Rekomendacje wydarzeń
- Profil
	- lista obserwowanych gier, eventów, gatunków, grup platform
	- ustawienia rekomendacji (czy pokazywać rekomendowane wydarzenia bez danych)
	- zmiana hasła
- Wygląd
	- widok desktop/mobile (kalendarz brzydki w mobile, reszta ok)
# Administrator
- Korekta serii wydarzeń
	- ręczne przypisanie eventu do innej serii (gdy regex/similarity się pomyli)
- Korekta grup platform
	- przypisanie platformy do grupy (ps, xbox, nintendo, pc, mobile, other)
- Logi systemu (kontener seq)

# Zbieranie i przetwarzanie danych
- Integracja z IGDB API
	- Oauth (twitch)
	- klient standardowy (limit ~100, zakres ~30 dni, do ustawienia w appsettings) i bulk (~500, zakres ~600 dni)
- Discovery eventów
	- pobranie eventów z zakresu
	- zapis nowych
	- status początkowy: Pending
	- normalizacja nazwy regexem (daty, 1st/2nd, miesiące, sezony, vol, znaki specjalne, liczby)
	- przypisanie do serii: exact match znormalizowanej nazwy, potem similarity do ostatniej edycji, inaczej nowa seria
- Hydracja (uzupełnienie danych) eventów
	- dociągnięcie gier do eventów, których status != Ready
	- aktualizacja powiązań gra-event i statusu (Ready/NoData)
- Gry, gatunki, platformy
	- sync gier: dane gry i parent (rekurencyjnie)
	- sync gatunków i platform
	- automatyczne zgadywanie grupy platformy po nazwie
- Popularność
	- cykliczne zliczanie follow'ów gier i eventów sql-em (czy dobrze? nie wiem)

