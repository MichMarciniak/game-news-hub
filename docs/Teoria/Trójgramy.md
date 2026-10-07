(W postgres)

np. słowo "kotek" dzieli się na:
```
"  k"
" ko"
"kot"
"ote"
"tek"
"ek "
```
Postgres dodaje 2 spacje z przodu, 1 z tyłu

```
`pg_trgm` ignores non-word characters (non-alphanumerics) when extracting trigrams from a string. Each word is considered to have two spaces prefixed and one space suffixed when determining the set of trigrams contained in the string. For example, the set of trigrams in the string “`cat`” is “ `c`”, “ `ca`”, “`cat`”, and “`at` ”. The set of trigrams in the string “`foo|bar`” is “ `f`”, “ `fo`”, “`foo`”, “`oo` ”, “ `b`”, “ `ba`”, “`bar`”, and “`ar` ”.
```
[PostgreSQL: Documentation: 18: F.35. pg_trgm — support for similarity of text using trigram matching](https://www.postgresql.org/docs/current/pgtrgm.html)

# Index
pg_trgm dostarcza `GiST` i `GIN`, które pozwalają na stworzenie indeksów na kolumnie tekstu dla szybkiego wyszukiwania po podobieństwie. Oprócz trójgramów, pozwalają też na wyszukiwanie przez `LIKE`,`ILIKE`, `~`, `~*`, `=`.

[PostgreSQL: Documentation: 18: F.35. pg_trgm — support for similarity of text using trigram matching](https://www.postgresql.org/docs/current/pgtrgm.html#PGTRGM-INDEX)

## GIN
Buduje odwrócony indeks: dla każdego unikalnego elementu (słowa, trigramu, elementu tablicy) trzyma listę wierszy, w których on występuje. Tak jak indeks na końcu książki: słowo -> strony w których się pojawia.

[PostgreSQL: Documentation: 18: 65.4. GIN Indexes](https://www.postgresql.org/docs/current/gin.html)

## GiST
Struktura drzewiasta, gdzie każdy węzeł to przybliżenie danych poniżej niego, i wyszukiwanie polega na przechodzeniu drzewa i odrzucaniu gałęzi, które na pewno nie pasują.

[PostgreSQL: Documentation: 18: 65.2. GiST Indexes](https://www.postgresql.org/docs/current/gist.html)

## W skrócie
GIN ma szybsze wyszukiwanie, ale musi aktualizować wszystko co dotknęło `INSERT`/`DELETE`.
GiST odwrotnie.
