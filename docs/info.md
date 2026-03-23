2. Cache można do najbliższych wydarzeń (wszystkich). Lub tych najbardziej popularnych
Dwie rzeczy: (na dany miesiąc)
- użytkownik -> lista id eventów
- szczegóły eventów
Odświeżanie:
- użytkownik obserwuje nową grę
- zmieni swoje preferencje gatunków
- Minie np. 1 godzina (żeby uwzględnić nowe eventy z api)
Zacznij od IMemoryCache, później na redis to zmiana 2 linii przez IDistributedCache

3. AutoMapper - nie wiem jeszcze do końca co robi, ale brzmi ciekawie

4. Problem dominacji wag gatunków.
Rozwiązanie przez normalizację - zamiast brać sumę punktów, bierze się średnią lub wprowadza karę za popularność (mechanizm IDF - Inverse Document Frequency)
Gatunek wybrany ręcznie (np. +500 pkt)
Gatunek z obserwowanej gry (np. +50pkt za każdą grę)

5. Background Worker
Pobiera Event i przypisuje mu GameId. Jeśli gra nie istnieje, pobiera ją i zapisuje.
Join:
- Pobierz eventy
- dołącz tagi przypisane do tych eventów
- posortuj na podstawie wag w profilu użytkownika
Webhook nie ma sensu, potrzebny jest publiczny IP

6. Platformy
Użytkownik wybiera które posiada, a potem jest filtracja eventów tylko dla gier, które posiada.

7. Rekomendacje
- VIP - wydarzenie bezpośrednio dotyczy obserwowanej gry
- Personalizowane - Wydarzenie pasuje do platform i ma Score > 0 (z gatunków)
- Ogólne - wydarzenia bez przypisanych gier lub ze Score = 0

8. Nowy użytkownik
- dostaje kalendarz trending
- każda gra na hypes i follows
- powiązanie eventów z grami o najwyższym współczynniku hypes


9. INNE:
- sfery czasowe - angular wyświetla zgodnie z czasem lokalnym użytkownika
- obsługa obrazów - nie ma bezpośrednio z igdb, trzeba robić URL'e
- idempotentność workera - sprawdzać, czy wydarzenie już jest żeby nie wsadził tego samego 2x
- moderacja - nwm, może skip


SCHEMAT projektu
```
.
├── Configuration        # Konfiguracja (Identity, IGDB Settings, Swagger)
├── Controllers          # Endpointy API
├── Data                 # Entity Framework Core (DbContext, SeedData)
├── Migrations           # Migracje bazy danych
├── Mappings             # Profile AutoMappera
├── Models               
│   ├── Entities         # Klasy bazodanowe (Twoje "tabele")
│   └── DTOs             # Obiekty przesyłane przez API (Request/Response)
├── Services             # Logika biznesowa (Interfejsy + Implementacje)
│   ├── Interfaces       # Kontrakty (IRecommendationService, IIgdbClient)
│   ├── Implementations  # Logika (RecommendationService, IgdbClient)
│   └── Background       # Background Workers (Jobs)
├── Common               # (Opcjonalnie) Helpery, Stałe, Custom Exceptions
└── Program.cs
```
