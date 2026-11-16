Głównym celem jest zaprojektowanie oraz zaimplementowanie aplikacji internetowej umożliwiającej agregację, przeglądanie oraz personalizację informacji o wydarzeniach branżowych i zapowiedziach w branży gier wideo.

Szczególnym wyzwaniem jest to, że zewnętrzne źródło danych (IGDB) przypisuje gry do wydarzeń dopiero po ich zakończeniu, zwykle po 2-3 dniach. Nadchodzące wydarzenia nie mają więc informacji potrzebnych do rekomendacji. Praca zakłada rozwiązanie tego problemu przez wykrywanie powtarzalnych edycji tego samego wydarzenia (serii) i estymację profilu nowego wydarzenia na podstawie poprzednich edycji.

**Obecny, bazowy zakres funkcjonalny projektu obejmuje:**

- **Agregację i przetwarzanie danych:** integracja z zewnętrznym źródłem danych (IGDB) w celu pobierania informacji o wydarzeniach i grach, ich oczyszczania oraz automatycznego łączenia powtarzalnych wydarzeń w serie.

- **Kalendarz wydarzeń:** przeglądanie nadchodzących i minionych wydarzeń w widoku miesięcznym oraz tygodniowym, wraz ze szczegółami i listą prezentowanych gier.

- **Wyszukiwarkę i katalog:** wyszukiwanie tekstowe wydarzeń oraz gier z filtrowaniem według gatunków i platform oraz sortowaniem według trafności, popularności i daty.

- **System kont i personalizacji:** rejestracja i logowanie użytkowników, zarządzanie profilem oraz możliwość obserwowania wybranych gier, wydarzeń, gatunków i grup platform.

- **Moduł rekomendacji:** generowanie spersonalizowanego zestawienia wydarzeń na podstawie preferencji użytkownika.

- **Integrację z platformą Steam:** powiązanie gier z bazą Steam, bezpośrednie odnośniki do platformy oraz możliwość zaimportowania gier użytkownika w celu szybkiej personalizacji profilu.

- **Panel administracyjny:** narzędzia umożliwiające weryfikację i ręczną korektę przypisania wydarzeń do serii oraz zarządzanie grupami platform.