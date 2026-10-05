## Kontekst
Aplikacja składa się z SPA i API, które są/będą wdrażane razem i obsługiwane przez jednego klienta pod jedną domeną (za nginx), więce nie ma scenariuszy międzydomenowych. Dotychczasowe uwierzytelnianie opierało się na JWT: access token + refresh cookie. Wymagało to własnej implementacji generowania i rotacji tokenów oraz mechanizmu unieważnienia, czyli sporo kodu którym trzeba zarządzać.
## Opcje
1. pozostanie przy jwt
2. pełne uwierzytelnianie ciasteczkowe z core identity
3. wbudowane `mapIdentityApi` z tokenami bearer (odrzucone wcześniej)

## Wybór
Uwierzytelnianie ciasteczkowe z core identity

## Powód
1. Aplikacja jest spójna (SPA + api), gdzie zalety JWT nie były wykorzystywane, a wprowadzały większy poziom skomplikowania (np. konieczność obsługi rotacji tokenów, obsługa generowania tokenów)

## Konsekwencje
- Mniej własnego kodu uwierzytelniającego
- gotowa obsługa wylogowania, blokady konta i security stamp
- brak tokenów w js i potrzeby zarządzania nimi
negatywne:
- pojawia się ryzyko CSRF (`sameSite` częściowo chroni)

## Dotyczy
[[02 - Auth]]
[[03 - Logowanie admina]]
