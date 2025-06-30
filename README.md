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

- **Dodaj samochód – wpisz dane pojazdu**
- **Dodaj klienta – zarejestruj nowego użytkownika**
- **Wypożycz – wybierz klienta i samochód**
- **Zwróć – oddaj pojazd, aktualizując jego stan**
- **Pokaż dostępność – lista dostępnych lub wypożyczonych pojazdów**
- **Wyjdź – zakończenie programu**

---
## Paradygmaty obiektowe:
Cztery główne paradygmaty obiektowe w projekcie
1. Enkapsulacja (Hermetyzacja)
Ukrywanie danych i logiki wewnętrznej za prywatnymi polami, co chroni stan obiektów
przed niepożądanym dostępem.
Przykład – Services/VehicleService.cs (linie 8-10):
private readonly ILogger _logger;
private readonly List<Vehicle> _vehicles;
private int _nextId = 1;
Chroni listę pojazdów i logger przed zewnętrznym dostępem.
Przykład – Forms/VehicleForm.cs (linie 18-30):
Wszystkie kontrolki UI są prywatne, co chroni stan formularza i ułatwia utrzymanie.
2. Dziedziczenie (Inheritance)
Pozwala klasom współdzielić i rozszerzać wspólne właściwości bazowe poza UI,
ułatwiając ponowne wykorzystanie kodu i jego organizację.
Przykład: Services/VehicleService.cs
public class VehicleService : IVehicleService
{
 // Implementacja metod z IVehicleService
}
VehicleService implementuje interfejs IVehicleService, co wymusza
zaimplementowanie wszystkich metod zadeklarowanych w interfejsie. Dziedziczenie po
interfejsie pozwala stworzyć kontrakt, który klasa musi spełnić, zapewniając spójność i
elastyczność projektu.
3. Polimorfizm (Polymorphism)
Pozwala traktować różne implementacje w ten sam sposób dzięki interfejsom, co
znacznąco zwiększa elastyczność i testowalność kodu.
Przykład – Program.cs (linie 16-19):
ILogger logger = Logger.Instance;
IVehicleService vehicleService = new VehicleService(logger);
ICustomerService customerService = new CustomerService(logger);
IRentalService rentalService = new RentalService(vehicleService,
customerService, logger);
Użycie interfejsów umożliwia łatwą wymienność implementacji oraz testowanie.
Przykład – Services/RentalService.cs (linie 13-17):
public RentalService(IVehicleService vehicleService, ICustomerService
customerService, ILogger logger)
{
 _vehicleService = vehicleService;
 _customerService = customerService;
 _logger = logger;
}
Dependency Injection + Polimorfizm: wstrzykiwanie interfejsów w konstruktorze pozwala
na swobodną podmianę implementacji i tworzenie bardziej modularnego kodu.
4. Abstrakcja (Abstraction)
Ukrywa szczegóły implementacji i prezentuje jedynie niezbędny kontrakt, pozwalając
korzystać z funkcjonalności bez znajomości wewnętrznego działania.
Przykład – Interfaces/IVehicleService.cs (linie 6-13):
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
Definiuje kontrakt na operacje pojazdów bez ujawniania implementacji.
