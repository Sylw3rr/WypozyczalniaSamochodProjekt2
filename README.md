🚗 Wypożyczalnia Samochodów – Projekt Semestralny
Projekt grupowy: Patryk Sylwerski, Oktawian Radziszewski, Filip Wawrysiewicz
Semestr letni 2024/2025 – Grupa 2

📝 Opis projektu
Konsolowa aplikacja napisana w C#, symulująca prosty system wypożyczalni samochodów. Pozwala na:

Rejestrację (dodawanie) nowych pojazdów

Zarządzanie klientami

Wypożyczanie i zwracanie samochodów

Śledzenie stanu floty i dostępności pojazdów

Logowanie operacji do pliku tekstowego

🚀 Technologie
C# .NET 6+ (CLI: dotnet)

SQLite – lokalna baza danych (CarRental.sqlite, carrental.db)

Logi w formacie tekstowym (CarRentalLog.txt)

Struktura projektu:

Models/ – klasy reprezentujące encje (Car, Customer, Rental)
Interfaces/ – kontrakty dla usług i repozytoriów
Services/ – logika biznesowa (wypożyczenia, obsługa danych)
Utils/ – pomocnicze funkcje, np. logowanie, walidacja
Forms/ – ewentualne komponenty UI (rozszerzenie)
Program.cs – punkt startowy aplikacji

🛠 Instalacja i uruchamianie
Sklonuj repozytorium:

git clone https://github.com/Sylw3rr/WypozyczalniaSamochodProjekt2.git
cd WypozyczalniaSamochodProjekt2

Upewnij się, że masz zainstalowany .NET SDK (wersja 6 lub nowsza).

Uruchom aplikację komendą:

dotnet run --project WypozyczalniaSamochodProjekt2.csproj

➤ To automatycznie zbuduje projekt, stworzy (jeśli nie istnieją) pliki bazy danych i logów, a następnie uruchomi konsolowe menu.

🧩 Użycie
Po uruchomieniu zobaczysz menu z opcjami:

Dodaj samochód – wpisz dane pojazdu

Dodaj klienta – zarejestruj nowego użytkownika

Wypożycz – wybierz klienta i samochód

Zwróć – oddaj pojazd, aktualizując stan

Pokaż dostępność – lista dostępnych/nieaktywnych pojazdów

Wyjdź – zakończenie programu

Przykładowe klasy i metody:

CarService.AddCar()
CustomerService.AddCustomer()
RentalService.RentCar()
RentalService.ReturnCar()

✅ Dodatkowe informacje
Pliki bazy danych i logów są tworzone w katalogu, z którego uruchamiasz aplikację.

W przypadku błędów (np. brak dostępnych samochodów), zostaniesz o tym powiadomiony komunikatem w konsoli.

Projekt został stworzony jako część zaliczenia z przedmiotu „Programowanie obiektowe” (C# + baza danych).

💡 Pomysły na rozwój
Dodanie UI (Windows Forms / WPF)

Eksport statystyk lub raportów (CSV / PDF)

Rozbudowana walidacja, wyjątki i obsługa błędów

Testy jednostkowe dla serwisów

📞 Kontakt
Masz pytania lub propozycje zmian? Skontaktuj się ze mną:

GitHub: Sylw3rr
E‑mail: sylwerski@example.com

📝 Licencja
Projekt udostępniany na licencji MIT – możesz swobodnie korzystać i modyfikować.

