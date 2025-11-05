## Zarządzanie, aktualizowanie pakietów i zależności:
polecenie ``` dotnet add package Newtonsoft.Json ``` służy do dodawania lub aktualizowania pakietu w aktualnej lokalizacji. 
Komenda ``` dotnet add src/TextAnalytics.App package Newtonsoft.Json ``` wskazuje konkretną lokalizację gdzie pakiet ma być zainstalowany.
Komenda ``` dotnet add src/TextAnalytics.App package Newtonsoft.Json --version 13.0.4 ``` aktualizuje pakiet do wskazanej wersji.

### SemVer 
To zasada, która pomaga rozumieć, jakie zmiany wprowadza aktualizacja pakietu i czy może coś zepsuć w Twoim kodzie.
Wyróznia się trzy typy aktualizacji:
##### Major 
np. 11.0.1 -> 12.0.1 - Wprowadza duże zmiany, które mogą być niekompatybilne z poprzednią wersją pakietu
##### Minor
np 12.0.1 -> 12.1.1 - Wprowadza nowe funkcje, które będą kompatybilne z poprzednimi wersjami
#### Patch
np 12.0.1 -> 12.0.2 - Wprowadza poprawki błędów, nadal wszystko będzie kompatybilne


