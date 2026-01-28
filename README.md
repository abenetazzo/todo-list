# MINIMAL WEB-API

Questo progetto ha come obiettivo l'apprendimento base del framework .NET CORE e della creazione di un nuovo progetto Web-API.

## Diario di bordo

### Step 1

Creazione del progetto col template Web-API, prima esecuzione del programma e aggiunta di un primo endpoint "/ping" che risponde semplicemente "pong".

### Step 2

Aggiunta lista di TodoItem in memory e endpoint GET "/todos" che restituisce la lista dei TodoItem presenti nella lista.

### Step 3

Aggiunto endpoint POST "/todos" per l'aggiunta di un nuovo elemento TodoItem alla lista.

### Step 4

Aggiunti endpoint GET "/todos/{id}" per ottenere singolo TodoItem e DELETE "/todos/{id}" per eliminare un elemento dalla lista.

### Step 5

Aggiunto endpoint PUT "/todos/{id}" per aggiornare un TodoItem esistente.

### Step 6

Fatto refactor della repository e aggiunto progetto di test

### Step 7

Aggiunti test di integrazione per tutti gli endpoint

### Step 8

Aggiunto report di copertura dei test.

### Step 9

Aggiunti test di unità e fatto refacto di Program.cs spostando la logica in un servizio esterno.

### Step 10

Aggiunta persistenza dei dati con EntitiFrameworkCore e database SqLite. Sostituiti tutti i metodi sincroni con corrispondenti metodi asincroni. Aggiornate le classi di test.

### Step 11

Aggiunta interfaccia grafica Blazor hosted (pagina index default).

### Step 12

Interfaccia grafica per la visualizzazione della Todo list e gestite le CRUD.

### Step 13

Aggiunta interfaccia grafica Blazor in client separato (pagina index default).