# Instrukcja uruchomienia aplikacji
Do uruchomienia aplikacji potrzebny jest .NET w wersji 10.0
### Krok 1. Klonowanie repozytorium
Użyj komendy `git clone`, aby pobrać kod źródłowy na swój komputer:

```
git clone --branch lab3 https://github.com/KamilRusinRepo/Programowanie_na_platformie_.NET.git
```

### Krok 2. Przejdz do folderu z programem
```
cd Programowanie_na_platformie_.NET/TransportApi/TransportApi
```

### Krok 3. Uruchom program
```
dotnet run
```


# Przykłady API

### Dodawanie pojazdu
Endpoint `/api/vehicles` z metodą `POST`

Ciało żądania:
```
{
    "VehicleType": "Truck",
    "RegistrationNumber": "kos123",
    "MaxLoadKg": 1000,
    "TrailerLength": 3
}
```

### Wyświetlanie wszystkich pojazdów
Endpoint `/api/vehicles` z metodą `GET`

Wynik:
```
[
    {
        "id": 1,
        "registrationNumber": "kos123",
        "maxLoadKg": 1000,
        "isAvailable": true
    }
]
```

### Dodawanie kierowcy
Enpoint `/api/drivers` z metodą `POST`

Ciało żadania:
```
{
    "Name": "New Driver",
    "LicenseNumber": "12345A"
}
```

### Wyświetlanie wszystkich kierowców
Enpoint `/api/drivers` z metodą `GET`

Wynik:
```
[
    {
        "id": 1,
        "name": "New Driver",
        "licenseNumber": "12345A",
        "isAvailable": true
    }
]
```

### Dodawanie zlecenia
Enpoint `/api/orders` z metodą `POST`

Ciało żądania:
```
{
    "CargoDescription": "Elektronika",
    "Weight": 1000,
    "VehicleId": 1,
    "DriverId": 1
}
```

### Wyświetlanie wszystkich zleceń
Enpoint `/api/orders` z metodą `GET`

Wynik:
```
[
    {
        "id": 1,
        "cargoDescription": "Elektronika",
        "weight": 1000,
        "createdAt": "2025-12-05T16:35:38.3267428",
        "isCompleted": false,
        "vehicleId": 1,
        "vehicle": {
            "id": 1,
            "registrationNumber": "kos123",
            "maxLoadKg": 1000,
            "isAvailable": false
        },
        "driverId": 1,
        "driver": {
            "id": 1,
            "name": "New Driver",
            "licenseNumber": "12345A",
            "isAvailable": false
        }
    }
]
```

### Zakończenie zlecenia
Enpoint `/api/orders/{id}/complete` z metodą `PUT`
