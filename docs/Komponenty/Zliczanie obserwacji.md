## Cel
Do zliczania obserwacji (oczywiste) gier i eventów.

## Główne elementy
`Api/Services`
- `FollowRecalculationJob` - zdefiniowane sql'e do zliczania obserwacji
- `CountFollowsBackgroundService` - serwis okresowo odpalający zliczanie obserwacji
- `BackgroundRegistration` dodanie do `IServiceCollection`

`Api/Options`
- `FollowServiceOptions` - [[#Konfiguracja]] okresu czasowego
## Przepływ
1. `CountFollowsBackgroundService` wywołuje `FollowRecalculationJob`
2. `FollowRecalculationJob` robi update sql'em
3. `CountFollowsBackgroundService` czeka i wraca do kroku 1.

## Konfiguracja
`appsettings.Development.json : FollowService`
- `Timespan`

## Powiązane
[[08 - Cykliczne liczenie popularności]]
