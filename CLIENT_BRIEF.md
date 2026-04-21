# Inventoria — Klant-opdracht

Een C# console-applicatie voor een kleine computerwinkel (Alternate-stijl) die lokaal voorraad wil bijhouden zonder externe database of cloud service.
De applicatie moet simpel, robuust en direct bruikbaar zijn op één machine.

## Wat de klant verwacht

### Opstart

Bij starten van de applicatie ziet de gebruiker een  hoofdmenu.
De lokale JSON database wordt automatisch geladen.
Als er nog geen database bestaat wordt die aangemaakt.

### Hoofdmenu

Het hoofdmenu bevat volgende genummerde opties:

```
=== Inventoria ===

1. Product toevoegen
2. Product aanpassen
3. Product verwijderen
4. Product zoeken
5. Alle producten tonen
6. Low-stock overzicht
0. Afsluiten

Keuze:
```

De gebruiker typt een cijfer en drukt enter.
Ongeldige input wordt opgevangen met een duidelijke foutmelding waarna het menu opnieuw getoond wordt.

### Optie 1 — Product toevoegen

De gebruiker wordt stap voor stap gevraagd naar:

- Naam
- Beschrijving
- Categorie (Laptop, Pre-build, CPU, Motherboard, Cooler, Memory, GPU, SSD, Case, Fans, Power supply, Monitor, Peripherals)
- Prijs
- Beginvoorraad (quantity)
- Minimum voorraad (minQuantity, waarbij alert moet getriggerd worden)

Na elke invoer wordt gevalideerd.
Na bevestiging wordt het product toegevoegd.

### Optie 2 — Product aanpassen

Gebruiker geeft product-id.
App toont het huidige product met alle velden.
Gebruiker kan kiezen welk veld hij wil aanpassen:

```
=== Product 1 ===
Naam:         Asus Laptop
Beschrijving: Gaming Laptop
Categorie:    Laptops
Prijs:        1600.00
Voorraad:     10 
min: 2

Wat wil je aanpassen?
1. Naam
2. Beschrijving
3. Categorie
4. Prijs
5. Voorraad aanpassen
6. Minimum voorraad aanpassen
0. Terug

Keuze:
```

Na aanpassing wordt een bevestiging getoond en keert de app terug naar het hoofdmenu.

### Optie 3 — Product verwijderen

Gebruiker geeft product-id.
App toont het product en vraagt bevestiging (y/n) voor de verwijdering.
Dit voorkomt accidentele verwijdering.

### Optie 4 — Product zoeken

Twee submogelijkheden:

- Zoeken op id
- Zoeken op naam

Bij meerdere resultaten worden alle matches getoond.

### Optie 5 — Alle producten tonen

Overzicht in tabelvorm, gesorteerd op id:

```
Id   Naam                              Categorie      Prijs      Voorraad
---- --------------------------------- -------------- ---------- ---------
1    Asus Laptop                       Laptops        1600.00    10
2    AMD Ryzen 7 7800X3D               Processors     419.00     18
...
```

Keuzen om te filteren op categorie.

### Optie 6 — Low-stock overzicht

Toont enkel producten waar `quantity <= minQuantity`.
Deze lijst helpt de zaakvoerder beslissen wat er bijbesteld moet worden.
Als er geen producten onder minimum zitten, toont de app een bevestiging dat alles in orde is.
Als er producten onder minimum zitten, toont de app deze producten.


## Eisen

- **Menu komt altijd terug** na elke voltooide actie, tot de gebruiker expliciet afsluit
- **Foutbestendig** — Controleer elke input of deze geldig is.


## Wat nog gebouwd moet worden

- [ ] Hoofdmenu met loop
- [ ] Input-validatie (cijfers, prijzen, bestaande id's)
- [ ] Product toevoegen flow
- [ ] Product aanpassen flow
- [ ] Product verwijderen met bevestiging
- [ ] Product zoeken (id + naam)
- [ ] Alle producten tabel-overzicht
- [ ] Low-stock overzicht
- [ ] Nette afsluit-flow

## Wat al gedaan is

- [x] Lokale JSON database met CRUD-methodes (ReadNode, Read, Write, Add, Remove)
