## Kontekst
Access token ma długi czas życia (15 min), jak na admina to dużo jeśli zostałby ukradziony
## Opcje
- Dodanie security_stamp do access tokena - implementacja oddzielnego endpointu
- Zmniejszenie TTL - access token żyje ~3 minuty zamiast 15 
- Oddzielne logowanie a claimem admina - implementacja oddzielnego endpointu
- 2FA przy każdym wyborze - bezpieczne, ale utrudniło by cokolwiek adminowi

## Wybór
Zmniejszenie TTL

## Powód
Zmniejszenie TTL ogranicza możliwości atakującego, oraz nie utrudnia implementacji.

## Konsekwencje
- Więcej refreshy przy adminie (mało znaczące, nie ma ich dużo w porównaniu ze zwykłymi użytkownikami)