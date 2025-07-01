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
![Projekt bez nazwy](https://github.com/user-attachments/assets/bb9284d5-cf1f-412e-ae65-743b15529780)
![zvasrpsqxxecyl2bxfz3](https://github.com/user-attachments/assets/47f4cbdb-0c74-4f66-8801-1cbf4fa01812)

## 🛠 Instalacja i uruchamianie

1. **Sklonuj repozytorium:**

 git clone https://github.com/Sylw3rr/WypozyczalniaSamochodProjekt2.git
 cd WypozyczalniaSamochodProjekt2

2. **Sprawdź, że masz zainstalowany .NET SDK (wersja 6 lub nowsza).**

3. **Uruchom aplikację:**
```
dotnet run --project WypozyczalniaSamochodProjekt2.csproj
```

Aplikacja zbuduje projekt, stworzy (jeśli nie istnieją) bazę danych i plik logów, a następnie wyświetli dashboard.

---

## 🧩 Użycie
Po uruchomieniu zobaczysz konsolowe menu z opcjami:

- **Dodaj samochód – wpisz dane pojazdu**
- **Dodaj klienta – zarejestruj nowego użytkownika**
- **Wypożycz – wybierz klienta i samochód**
- **Zwróć – oddaj pojazd, aktualizując jego stan**
- **Pokaż dostępność – lista dostępnych lub wypożyczonych pojazdów**
- **Wyjdź – zakończenie programu**

---
## 🚀 Roadmapa rozwoju
- Migracja do .NET 8 & EF Core
Modernizacja środowiska uruchomieniowego i wprowadzenie trwałej bazy danych zamiast kolekcji w pamięci.

- Konteneryzacja Docker + CI/CD (GitHub Actions)
Automatyczne budowanie i testowanie aplikacji przy każdym commicie oraz łatwe wdrażanie w chmurze.

- Nowoczesny interfejs (WPF / MAUI albo Blazor Web)
Lepszy UX, responsywność i możliwość uruchomienia aplikacji na wielu platformach.

- System powiadomień i płatności online
- E-maile lub SMS-y o rezerwacjach i zwrotach oraz integracja z bramką płatniczą dla pełnego procesu wynajmu.

- Zaawansowana analityka i raporty
- Dashboard KPI (obłożenie floty, przychody, popularność modeli) z opcją eksportu do CSV/PDF.

# 📊 Cztery Główne Paradygmaty Obiektowe w Projekcie

> **Analiza implementacji zasad programowania obiektowego w systemie wypożyczalni samochodów**

---

## 🔒 1. Enkapsulacja (Hermetyzacja)

**Definicja:** Ukrywanie danych i logiki wewnętrznej za prywatnymi polami, co chroni stan obiektów przed niepożądanym dostępem.

### 📋 Przykład z `Services/VehicleService.cs` (linie 8-10):

```csharp
private readonly ILogger _logger;
private readonly List<Vehicle> _vehicles;
private int _nextId = 1;
```

- ✅ Chroni listę pojazdów i logger przed zewnętrznym dostępem
- ✅ Zapewnia kontrolę nad modyfikacją stanu obiektu
- ✅ Redukuje ryzyko błędów wynikających z nieprawidłowego dostępu do danych

### 📋 Przykład z `Forms/VehicleForm.cs` (linie 18-30):

```csharp
// Wszystkie kontrolki UI są prywatne
private Button btnAdd;
private TextBox txtMarka;
private ComboBox cmbStatus;
// ...
```

- ✅ Chroni stan formularza przed zewnętrzną modyfikacją
- ✅ Ułatwia utrzymanie i refaktoryzację kodu UI

---

## 🧬 2. Dziedziczenie (Inheritance)

**Definicja:** Pozwala klasom współdzielić i rozszerzać wspólne właściwości bazowe, ułatwiając ponowne wykorzystanie kodu i jego organizację.

### 📋 Przykład z `Services/VehicleService.cs`:

```csharp
public class VehicleService : IVehicleService
{
    // Implementacja metod z IVehicleService
    public IEnumerable<Vehicle> GetAllVehicles() { /* ... */ }
    public void AddVehicle(Vehicle vehicle) { /* ... */ }
    // ...
}
```

**Analiza:**
- 🔄 **VehicleService** implementuje interfejs **IVehicleService**
- 📝 Wymusza zaimplementowanie wszystkich metod zadeklarowanych w interfejsie
- 🏗️ Dziedziczenie po interfejsie pozwala stworzyć kontrakt, który klasa musi spełnić

- ✅ Zapewnia spójność w projekcie
- ✅ Zwiększa elastyczność architektury
- ✅ Ułatwia dodawanie nowych implementacji

---

## 🎭 3. Polimorfizm (Polymorphism)

**Definicja:** Pozwala traktować różne implementacje w ten sam sposób dzięki interfejsom, co znacząco zwiększa elastyczność i testowalność kodu.

### 📋 Przykład z `Program.cs` (linie 16-19):

```csharp
ILogger logger = Logger.Instance;
IVehicleService vehicleService = new VehicleService(logger);
ICustomerService customerService = new CustomerService(logger);
IRentalService rentalService = new RentalService(vehicleService, customerService, logger);
```

- 🔄 Użycie interfejsów umożliwia łatwą wymienność implementacji
- 🧪 Ułatwia testowanie jednostkowe (mockowanie)
- 🔧 Zwiększa modularność systemu

### 📋 Przykład z `Services/RentalService.cs` (linie 13-17):

```csharp
public RentalService(IVehicleService vehicleService, ICustomerService customerService, ILogger logger)
{
    _vehicleService = vehicleService;
    _customerService = customerService;
    _logger = logger;
}
```

**Zastosowane wzorce:**
- 💉 **Dependency Injection**: wstrzykiwanie interfejsów w konstruktorze
- 🔄 **Polimorfizm**: pozwala na swobodną podmianę implementacji
- 🏗️ **Modularność**: tworzenie bardziej elastycznego i testowalnego kodu

---

## 🎯 4. Abstrakcja (Abstraction)

**Definicja:** Ukrywa szczegóły implementacji i prezentuje jedynie niezbędny kontrakt, pozwalając korzystać z funkcjonalności bez znajomości wewnętrznego działania.

### 📋 Przykład z `Interfaces/IVehicleService.cs` (linie 6-13):

```csharp
public interface IVehicleService
{
    IEnumerable<Vehicle> GetAllVehicles();
    IEnumerable<Vehicle> GetAvailableVehicles();
    Vehicle GetVehicleById(int id);
    void AddVehicle(Vehicle vehicle);
    void UpdateVehicle(Vehicle vehicle);
    void DeleteVehicle(int id);
    void SetVehicleAvailability(int id, bool isAvailable);
}
```

**Korzyści abstrakcji:**
- 📝 **Kontrakt**: Definiuje jasny zestaw operacji na pojazdach
- 🔒 **Ukrywanie implementacji**: Klient nie musi znać szczegółów działania
- 🔄 **Wymienność**: Łatwa podmiana implementacji bez zmiany kodu klienta
- 🧪 **Testowalność**: Możliwość tworzenia mock'ów dla testów

---

### 🔧 Wzorce projektowe wykorzystane:
- **Repository Pattern** - w serwisach danych
- **Dependency Injection** - w konstruktorach serwisów  
- **Interface Segregation** - dedykowane interfejsy dla każdego serwisu
- **Single Responsibility** - każda klasa ma jedną odpowiedzialność

---

*Dokument przygotowany w ramach analizy architektury systemu wypożyczalni samochodów*
