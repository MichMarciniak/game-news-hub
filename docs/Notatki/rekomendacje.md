może się przyda
> https://primo.pw.edu.pl/permalink/48OMNIS_TUW/144220u/cdi_nii_cinii_1970867909773351814

# Co w rekomendacjach
Wszystko skalowane do 0-1 (lub podobnie)
- **Follow samego eventu** - od razu go pokazuje / nie uwzględniać w rekomendacjach?
- **dopasowanie gatunkowe** - `EventGenreWeight` i gatunki, które obserwuje użytkownik + przez obserwowane gry. Cosine sim?
- **Gra w evencie** - użytkownik obserwuje grę z eventu, albo grę która ma dlc w evencie, inna waga. (przy kilku dopasowaniach można brać max, a nie sumę)
- **Seria** - użytkownik obserwuje poprzednią edycję (albo średnie dopasowanie gatunkowe poprzednich edycji - może jeszcze inne np. follow gier?). Zastępuje dane, których nowy event jeszcze nie ma. Kluczowe dla `Pending`/`NoData`
- **Popularność** - mała waga, przydaje się gdy użytkownik jeszcze nic nie ma w profilu / nowe konto.
- **Czas** - event trwający obecnie - wyżej, potem zanik wokół StartTime/EndTime - zależy czy w tył czy w przód. Małe znaczenie, zakończone eventy można zawsze obejrzeć później.
> Ustawienie `ShowFutureRecommendations` 

Całość to $score = \sum{waga \cdot znormalizowany\_sygnał}$
Wagi ustawiane ręcznie na start (lub przekazywane przy testach?)



# Dlaczego nie uczenie maszynowe / AI
1. Skala i dane - duże systemy, np. youtube uczą się na miliardach interakcji użytkowników, a model potrzebuje takich danych, żeby cokolwiek wyciągnąć. Ja nie mam tylu użytkowników, więc nie ma czego się uczyć, model byłby gorszy

2. Charakter danych - dane są strukturalne i skończone - gatunki, relacje gra-dlc-parent, serie eventów, liczba obserwujących. LLM i embeddingi mają największą przewagę przy niestrukturalnych danych (np. tekst). Opisy eventów są często generyczne lub ich brakuje, więc nie ma czego wykorzystać

3. Interpretowalność - dla każdej rekomendacji będę mógł powiedzieć, dlaczego się pojawiła - wynik jest deterministyczny, testowalny i możliwy do wytłumaczenia użytkownikowi. Przy LLM jest trudniej - dane wchodzą, wynik wychodzi, trudno zrozumieć do się dzieje w środku.

4. Koszty i zależności - LLM to dodatkowa zależność zewnętrzna (koszt, opóźnienie, limity, awarie), a dane użytkowników trafiałyby do zewnętrznego API (prywatność). 

5. Prostota - zaczęcie od prostszego modelu jako punktu odniesienia jest lepsze, niż robienie czegoś na zaś (bo może się kiedyś przydać). [Zasada 1: nie bój się uruchomić produktu bez uczenia maszynowego](https://developers.google.com/machine-learning/guides/rules-of-ml?hl=pl)

6. Duże systemy często nie są czystym ML - mają etap kandydatów, scoring i re-ranking, czyli podobne do tego co mam. https://developers.google.com/machine-learning/recommendation/overview/types?hl=pl

## Dodatkowo
1. W rekomendacjach opartych na interakcji użytkowników trzeba taki model nauczyć. Wyjątki:
	1. Modele embeddingowe - uczą się języka, nie zachować użytkowników, ale u mnie zastosowanie ograniczone/brak
	2. Transfer learning i systemy cross-domain, gdzie próbuje się przenosić wiedzę między domenami ("Cross domain recommender systems"). Ale mają ograniczoną skuteczność, nadal badane a nie standard.