
## Kontekst
Potrzebowałem opcji logowania dla użytkowników, aby mogli dostawać spersonalizowane rekomendacje, oraz obserwować eventy

## Opcje
- Logowanie tokenowe - dostępne w Identity, proste do zaimplementowania, podatne na XSS
- Logowanie sesyjne - dostępne w Identity, trzeba przechowywać stan w backendzie
- Access token + refresh cookie - nie ma w Identity, Access przekazywany w body, refresh jako ciasteczko, ogranicza atak XSS, minimalny stan

## Wybór
Access token + refresh cookie z security_stamp

## Powód
1. Refresh zapisany do ciasteczka zapobiega XSS
2. Security stamp w refresh cookie pozwala na wylogowanie ze wszystkich urządzeń

## Konsekwencje
- Własna implementacja logowania i generowania tokenów
- Większe bezpieczeństwo przez dodanie security_stamp
- Access może wciąż być ukradziony, ale ma mniejszy czas dostępu (15 min)