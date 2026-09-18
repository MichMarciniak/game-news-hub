**Co jest dobre**

- Decyzje techniczne są zapisane sensownie i w większości zgadzają się z backendem, zwłaszcza baza, auth, serie eventów i liczenie popularności.
- Separacja projektów jest zgodna z kodem: Api, Sync, Contracts i Data faktycznie żyją osobno.
- Opis cykliczności eventów i grupowania platform ma dobry kierunek, bo odpowiada temu, co siedzi w Sync i w serwisach administracyjnych.

**Co bym poprawił**

- [Przegląd systemu.md:1](vscode-file://vscode-app/c:/Program%20Files/Microsoft%20VS%20Code/4fe60c8b1c/resources/app/out/vs/code/electron-browser/workbench/workbench.html) jest jeszcze niedomknięty: są puste sekcje, jest wtręt w stylu „ALE TO NIE MA SENSU?”, i w diagramie jest literówka w nazwie Data. To osłabia ten plik jako główny opis systemu.
- docs/Komponenty/Wyszukiwanie.md i docs/Komponenty/Sync z igdb.md są puste, mimo że backend już ma gotowe flow dla wyszukiwania i synchronizacji. To dziś największa luka dokumentacyjna.
- Opis wyszukiwania trzeba dopisać, bo backend robi teraz proste filtrowanie po nazwie przez ILike i sortowanie po dacie, nazwie albo popularności w [EventService.cs:94](vscode-file://vscode-app/c:/Program%20Files/Microsoft%20VS%20Code/4fe60c8b1c/resources/app/out/vs/code/electron-browser/workbench/workbench.html), a nie bardziej zaawansowane dopasowanie.
- Opis cykliczności eventów jest blisko prawdy, ale upraszcza algorytm. W kodzie w [EventSyncService.cs:160](vscode-file://vscode-app/c:/Program%20Files/Microsoft%20VS%20Code/4fe60c8b1c/resources/app/out/vs/code/electron-browser/workbench/workbench.html) jest exact match po znormalizowanej nazwie, potem fuzzy match po dowolnym wcześniejszym evencie z serią, a dopiero potem tworzenie nowej serii. To warto doprecyzować.
- Auth jest zgodny z backendem, ale ma kilka literówek i drobnych rozjazdów nazewniczych względem [AuthController.cs:1](vscode-file://vscode-app/c:/Program%20Files/Microsoft%20VS%20Code/4fe60c8b1c/resources/app/out/vs/code/electron-browser/workbench/workbench.html) i [AuthService.cs:1](vscode-file://vscode-app/c:/Program%20Files/Microsoft%20VS%20Code/4fe60c8b1c/resources/app/out/vs/code/electron-browser/workbench/workbench.html). Tu bardziej chodzi o porządek niż o błąd architektoniczny.

**Czego jeszcze brakuje**

- Osobnej strony o rekomendacjach. Ten obszar już istnieje w backendzie i korzysta z serii eventów przy niepewnych danych w [RecommendationService.cs:59](vscode-file://vscode-app/c:/Program%20Files/Microsoft%20VS%20Code/4fe60c8b1c/resources/app/out/vs/code/electron-browser/workbench/workbench.html), więc zasługuje na własny opis.
- Krótkiego opisu modelu danych: Event, Game, Platform, PlatformGroup, EventSeries i relacje między nimi.
- Jednej strony o konfiguracji runtime: Token, Seq, SMTP, FollowService, ApiOptions oraz wymagania środowiskowe dla Dockera.
- Mapa endpointów backendu po feature’ach: Auth, Events, Games, Genres, Platforms, Recommendations i Admin.
- Sekcji statusu: co jest już zaimplementowane, co jest tylko decyzją, a co jest jeszcze do zrobienia.