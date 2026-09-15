## Cel
Wysyłanie emaili użytkownikom do potwierdzenia go, resetu hasła, powiadomień

## Główne elementy
`Api/Features/Shared/Email`
- `EmailSender` - buduje i wysyła email
- `EmailTemplateService` - bierze template emaila i wstawia do niego wartości
- `ConsoleEmailSender` - debug do dev
- `EmailRegistration` - dodanie do `IServiceCollection`

`Api/Options` zmienne do wczytania z `appsettings`
- `SmtpOptions` - [[#Konfiguracja]] serwera smtp
- `EmailTemplateOptions` - [[#Konfiguracja]] ścieżki do templatek

`Api/Resources/EmailTemplates` - templatki email'i

## Przepływ
### Wysłanie emaila
1. `EmailTemplateService` - tworzy message body, wstawiając odpowiednie wartości do templatki
2. `EmailSender.SendEmailAsync` - przyjmuje destination, tytuł i html body
3. Ustawia opcje wiadomości i ją tworzy
4. Łączy się z SMTP
5. Wysyła wiadomość jeśli połączone

### Tworzenie template
0. Wymagane: `npm install -g mjml` Używana wersja: `(@5.4.0)`
1. Napisanie w mjml
2. `mjml template.mjml -o template.html`

## Konfiguracja
`appsettings.Development.json : Smtp`
- `Host`
- `Port`
- `EnableSsl`
- `FromEmail`
- `FromName`

`GameNewsHub.Api.csproj` - aby templatki szły do builda, inaczej ich nie ma
```xml
<ItemGroup>
	<None Include="Resources\EmailTemplates\**\*.html">
		<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
	</None>
</ItemGroup>
```
## Template'y
`confirm-email` - przy rejestracji, do potwierdzenia email
- username
- confirm_url

`reset-password` - przy requestcie zmiany hasła, w gdy zapomni
- reset_url

`password-changed` - powiadomienie przy każdej zmianie hasła

## Powiązane
