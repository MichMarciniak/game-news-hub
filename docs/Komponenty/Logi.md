## Cel
Rejestrowanie przebiegu i błędów w procesach działających w tle, oraz błędów i przepływu w API.

## Główne elementy
- `ILogger<T>` - wbudowany w .NET, używany w przez DI
- `Api/Extensions/LoggingExtensions` - konfiguracja logów
- `Api/Options/SeqOptions` - [[#Konfiguracja]]
- `datalust/seq:2026.1` - kontener dockera

## Przykład
```csharp
_logger.LogInformation("Event {EventId} reassigned from series {OldSeriesId} to {NewSeriesId}");
```
Zawsze placeholder, nigdy interpolacja (`$"..."`) w wywołaniach loggera - structured logging pozwala później filtrować/agregować po nazwanych polach. 
## Konfiguracja
`appsettings.Development.json : Seq`
- `Url` - gdzie wysyłane logi

## Ograniczenia



## Powiązane
