# ✈️ Flight Planner

**Flight Planner** to aplikacja internetowa wspierająca pilotów lotnictwa ogólnego w planowaniu lotów VFR. Na podstawie aktualnych danych pogodowych METAR/TAF, wybranego samolotu i planowanego czasu lotu, system ocenia czy lot można wykonać bezpiecznie — zgodnie z przepisami VFR oraz parametrami technicznymi statku powietrznego.

---

## 🌐 Dostępna wersja produkcyjna

[https://flight-planner.pl](https://flight-planner.pl)

---

## 📦 Technologie

- **Frontend:** Angular v18
- **Backend:** ASP.NET Core Web API (.NET 8)
- **Baza danych:** MS SQL Server
- **Zewnętrzne API:**
- [Aviation Weather Center](https://aviationweather.gov/data/api/) – dane METAR/TAF
- [OpenAI](https://platform.openai.com/) – analiza warunków pogodowych

---

## ⚙️ Uruchomienie lokalne

### 1. Backend (.NET)

1. Sklonuj repozytorium.
2. Skopiuj plik `appsettings.template.json` jako `appsettings.json`.
3. Uzupełnij w nim swój klucz OpenAI i dane połączenia do bazy danych.
4. W katalogu `backend/` uruchom:

   ```bash
   dotnet restore
   dotnet run
   ```

### 2. Frontend (Angular)

1. Przejdź do katalogu `frontend/`:

   ```bash
   cd frontend
   npm install
   ng serve
   ```

---

## 🧪 Przykładowe funkcje

- Wprowadzenie planu lotu (lotniska ICAO, data, czas, typ samolotu).
- Automatyczne pobranie i analiza warunków pogodowych METAR/TAF.
- Ocena możliwości wykonania lotu przez OpenAI (np. „warunki sprzyjające” lub „odradza się lot”).
- Historia planów lotu dla zalogowanych użytkowników.
- Możliwość rejestracji i logowania.

---

## 🔐 Pliki konfiguracyjne

Plik `appsettings.json` zawiera dane wrażliwe (klucze API, connection stringi) i **nie znajduje się w repozytorium**. Utwórz go lokalnie na podstawie szablonu:

```plaintext
appsettings.template.json
```

---

## 🧭 Plany rozwojowe

- Dodanie modułu wyświetlania NOTAM-ów.
- Generowanie planu lotu w formacie PDF.
- Obsługa wielu języków (np. angielski).
- Rozszerzenie bazy samolotów.
- Integracja z dodatkowymi źródłami danych pogodowych.

---

## 👨‍💻 Autorzy:

- **Anna Sroka**
- **Justyna Szofińska**
- **Paweł Staniul**

---

### 🎓 Uniwersytet:
**Uniwersytet WSB Merito w Poznaniu**
**Kierunek:** Informatyka