0. zrób interfejsy do Service, HttpClient i cache
1. Konfiguracja i fundamenty
    1. ef core z diagramem bazy
    2. prosta usługa, która przy użyciu HttpClient pobiera token i robi zapytanie events
    3. background worker - usługa która pobiera 50? najbliższych wydarzeń


2. Personalizacja i API
    1. Profil użytkownika - rejestracja/logowanie, uzytkownik wybiera platformy i ulubione gatunki
    2. Serwis rekomendacji
        - pobiera eventy z bazy
        - filtruje je po platformach
        - oblicza score (gatunki + wagi za obserwowane gry)
    3. endpoint kalendarza - posortowana lista wydarzeń

3. frontend personalizowany - wszystko co dotychczas było zrobione w backendzie

4. kalendarz ogólny i trending
    - dla nowych użytkowników
    - sortuje wydarzenia po hypes gier
    - UI prosty switch kalendarza

5. Cache i testy

6?. admin board jeśli będzie potrzeba


