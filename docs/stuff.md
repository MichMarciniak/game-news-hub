użytkownik:
- logowanie
- rejestracja
- wybór gatunków (rpg, simulator, itd.)
- wybór platform (pc, xbox...)
- blacklista??? nwm może jakaś blokada drm
- kalendarz z preferencjami
- kalendarz ogólny/trending
- jakiś trending top 10 wydarzeń w tygodniu/miesiącu???
- formularz dodania eventu czy coś
- ?zgłoszenie błędu
- dodawanie like
- synchronizacja kalendarza pojedynczych wydarzen
- synchronizacja (pełna) z kalendarzem google (albo export do .ics, łatwiej bo nie trzeba oauth z google'a)
- patrzenie na promocje
- ?obserwowanie poszczególnych dev czy gier?
- powiadomienia (mail/browser)
- rekomendacja treści (ten top 10):
    - co uważać za "trending"? % więcej like'ów od innych wydarzeń w tym tygodniu? Czy też porównanie z poprzednim tygodniem?
    - np. jak w danym tygodniu mamy 10,000 i kilka po 1,000 to będzie jedno
    - ale jak w następnym mamy kilka 10,000 to wtedy niby wszystkie trending?
- ?jakiś search?

service worker:
- auto pobieranie danych z api (np. 1x dziennie)
- obsługa promocji (steamAPI itp.)
- ?łączenie danych (np. igdb mówi że gra wychodzi a steam ma cenę/link do steama)

admin:
- zarządzanie treścią: akceptacja wydarzeń dodanych przez ludzi itp.
- zarządzanie kategoriami: możliwość dodania nowego gatunku/platformy jak się pojawi

jakość:
- szybkie ładowanie kalendarza (żeby nie było długiego czekania bo to nieprzyjemne)
- ?kalendarz musi być czytelny na telefonie
- bezpieczeństwo -  hashowanie haseł itp.
- dostępność -  co się dzieje jak service worker aktualizuje dane, lub duży ruch?

potencjalne problemy:
- zmiana api zewnętrznego - DTO
- data gry się zmieni - jak to połączyć?
- timezone
- gra anulowana/przesunięta?
- trzymać url'a a nie obrazek do gier, ale wtedy czas ładowania może się zwiększyć :/
    - image proxy - backend pobiera obraz, ale kompresuje go do webp i ma lekki plik
- export .ics przez url - stały link, użytkownik wkleja do google'a "by url" i kalendarz sam się odświeża - wtedy nie trzeba oauth
- offile mode (PWA)??? service worker na froncie potrzebny wtedy
- score decay: score = likes/(timeSinceRelease+2)^1.8 ??
- 


