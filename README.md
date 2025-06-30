# 🚗 Wypożyczalnia Samochodów – Projekt Semestralny

**Projekt grupowy:** Patryk Sylwerski, Oktawian Radziszewski, Filip Wawrysiewicz  
**Semestr letni 2024/2025** – Grupa 2

---

## 📝 Opis projektu

Konsolowa aplikacja napisana w C#, symulująca prosty system wypożyczalni samochodów. Pozwala na:

- **Rejestrację** (dodawanie) nowych pojazdów  
- **Zarządzanie** klientami  
- **Wypożyczanie** i **zwracanie** samochodów  
- **Śledzenie** stanu floty i dostępności pojazdów  
- **Logowanie** operacji do pliku tekstowego

---

## 🚀 Technologie

- **C# .NET 6+** (CLI: `dotnet`)  
- **SQLite** – lokalna baza danych (`CarRental.sqlite`, `carrental.db`)  
- **Logi** w formacie tekstowym (`CarRentalLog.txt`)  
- **Struktura projektu:**
Models/ – klasy reprezentujące encje (Car, Customer, Rental)
Interfaces/ – kontrakty dla usług i repozytoriów
Services/ – logika biznesowa (wypożyczenia, obsługa danych)
Utils/ – funkcje pomocnicze (logowanie, walidacja)
Forms/ – ewentualne komponenty UI (rozszerzenie)
Program.cs – punkt startowy aplikacji
---

## 🛠 Instalacja i uruchamianie

1. **Sklonuj repozytorium:**

 git clone https://github.com/Sylw3rr/WypozyczalniaSamochodProjekt2.git
 cd WypozyczalniaSamochodProjekt2

2. **Sprawdź, że masz zainstalowany .NET SDK (wersja 6 lub nowsza).**

3. **Uruchom aplikację:**
```
dotnet run --project WypozyczalniaSamochodProjekt2.csproj
```
Aplikacja zbuduje projekt, stworzy (jeśli nie istnieją) bazę danych i plik logów, a następnie wyświetli konsolowe menu.
---
## 🧩 Użycie
Po uruchomieniu zobaczysz konsolowe menu z opcjami:

Dodaj samochód – wpisz dane pojazdu
Dodaj klienta – zarejestruj nowego użytkownika
Wypożycz – wybierz klienta i samochód
Zwróć – oddaj pojazd, aktualizując jego stan
Pokaż dostępność – lista dostępnych lub wypożyczonych pojazdów
Wyjdź – zakończenie programu
