## Cel
Rejestracja, logowanie i zarządzanie stanem użytkownika przez JWT access token (stateless) + refresh token (httpOnly cookie, z rewokacją przez security_stamp).

## Główne elementy
`Api/Features/Auth`
- `AuthService` - logika biznesowa: Login, Register, ConfirmEmail, ResetPassword, RefreshToken, LogoutFull
- `TokenService` - generowanie i walidacja tokenów (access i refresh)
- `AuthController` - endpointy HTTP, zarządzanie cookie refresh tokena
- `TokenResult` - dto do przekazywania tokenów między tymi komponentami
- `AuthRegistration` - dodanie do `IServiceCollection`

`Api/Options` - zmienne do wczytania z `appsettings`
- `JwtTokenOptions` - [[#Konfiguracja]]

`Api/` - inne
- `Constants/Roles` - zdefiniowane role
- `Extensions/AuthExtension` - dodanie tokena do dokumentacji (scalara), dodanie opcji auth
- `Seeder/RoleSeeder` - do tworzenia roli w bazie, jeśli nie istnieją

## Przepływ
### Logowanie
1. POST `/login` z emailem i hasłem
2. `AuthService.Login` - wyszukanie użytkownika po email'u, sprawdzenie hasła/lockout przy zbyt wielu nieudanych próbach
3. Jeśli OK i potwierdzony email -> `TokenService` - wygenerowanie tokenów
4. Kontroler zwraca access w body, refresh jako cookie httpOnly

### Rejestracja
1. POST `/register` z emailem i hasłem
2. `AuthService.Register` - wyszukanie czy email nie istnieje, sprawdzenie siły hasła, dodanie roli `User`
3. `EmailService` - wysłanie emaila z potwierdzeniem
4. POST `/confirm-email` z tokenem i userId -> ustawienie emailConfirmed
5. Jeśli wygasł, POST `/resend-email`, powrót do 4.

### Refresh
1. POST `/refresh` z cookie refresh
2. `TokenService.ValidateRefreshToken` - sprawdzenie czy refresh poprawny
3. `TokenService.RefreshToken + GenerateAccess/RefreshToken` - wygenerowanie nowych tokenów
4. Zwrócenie access tokena, ustawienie nowego refresh

### Wylogowanie
1. POST `/logout` - usuwa cookie refresh
2. POST `/logout-all` - usuwa cookie refresh, ustawia nowy security stamp na na użytkowniku, poprzednie refresh tokeny są nieważne

### Zmiana hasła
1. Gdy użytkownik zalogowany - POST `/change-password` ze starym i nowym hasłem
	1. `AuthService.ChangePassword` - waliduje i zmienia hasło
	2. Aktualizuje security_stamp, refresh tokeny unieważnione
2. Gdy użytkownik zapomniał - POST `/request-reset-passord` - email na `/reset-password` z userId, tokenem i nowym hasłem
	1. `AuthService.ResetPassword` - ustawia nowe hasło
	2. Aktualizuje security_stamp

## Konfiguracja
- `appsettings.Development.json : Token`
	- `Issuer`
	- `Audience`
	- `AccessTokenLifetime`
	- `AdminAccessTokenLifetime`
	- `RefreshTokenLifetime`
- User secrets: `Token:SigningKey` - min. 32 znaki, sekret
## Api
| Metoda | Ścieżka                 | Opis                                                                                                                                |
| ------ | ----------------------- | ----------------------------------------------------------------------------------------------------------------------------------- |
| POST   | /register               | Rejestracja, wysyła email potwierdzający                                                                                            |
| POST   | /login                  | Logowanie, zwraca access token + ustawia refresh cookie                                                                             |
| POST   | /refresh                | Odświeża access token i refresh cookie, waliduje refresh cookie                                                                     |
| POST   | /logout                 | Wylogowanie z danego urządzenia, usuwa ciastko refresh                                                                              |
| POST   | /logout-all             | Wylogowanie ze wszystkich urządzeń, usuwa ciastko refresh i aktualizuje security_stamp, unieważniając dotychczasowe refresh ciastka |
| POST   | /confirm-email          | Przyjmuje userId i token, z wygenerowanej wiadomości przy rejestracji, ustawia `EmailConfirmed` na true                             |
| POST   | /resend-email           | Wysyła potwierdzenie konta na email, bo token może stracić ważność, lub wiadomość nie dojść                                         |
| POST   | /request-password-reset | W przypadku gdy użytkownik zapomni hasła, wysyła email z linkiem do resetu hasła                                                    |
| POST   | /reset-password         | Ustawia nowe hasło na podstawie tokena i wprowadzonego hasła                                                                        |
| POST   | /change-password        | Zmienia hasło zalogowanego użytkownika                                                                                              |
## Ograniczenia
- Refresh token nie jest przechowywany w bazie, zamiast tego jest security_stamp weryfikujący jego "wersję", rotacja nie unieważnia
- Access token dla zwykłego usera ma opóźnienie rewokacji do 15 minut (wygaśnięcie)
- Dla roli admin: krótszy TTL (3 min) w celu bezpieczeństwa

## Powiązane
[[02 - Auth]]
[[03 - Logowanie admina]]
[[Emaile]]

