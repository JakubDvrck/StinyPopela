# Stíny popela 0.2.0 — souboje, výbava a atributy

## Nové funkce

- Lišta životů a číselný stav zdraví nad nepřítelem.
- Čísla skutečně uděleného poškození při základním útoku i seku. Červená čísla označují ztrátu životů hráče.
- Základní útok je dostupný od začátku, automaticky zamíří na nejbližší cíl. Sek je označený jako zamčený, dokud jej neodemkneš skill pointem.
- Inventář s náhledem modelu a aktuální výbavou: hlava, zbroj, rukavice, kalhoty, boty, zbraň, druhá ruka, dva prsteny a amulet.
- Vybavení a sundání předmětů, porovnání bonusů a ukládání výbavy. Zbroj a zbraně se promítají do pracovního modelu postavy.
- Atributový panel pod inventářem: aktuální hodnota, navržená hodnota a přesný dopad na poškození, rychlost nebo životy.
- Body se utratí až tlačítkem ULOŽIT ATRIBUTY. ZRUŠIT ZMĚNY a zavření panelu nepotvrzené body vrátí.
- Otevřený inventář pozastaví souboje.

## Opravy

- Vyřešen konflikt CapsuleCollider / CharacterController, který blokoval ovládání a vypisoval chyby.
- NAČÍST obnoví postavu i bez existujícího uložení a vrátí ji do bezpečí. Po obnovení je krátká ochrana před zraněním.
- Zachovány opravy startu, serializace a přibaleného shaderu z předchozí verze.

## Instalace a test

Stáhni StinyPopela-0.2.0.apk a nainstaluj jako aktualizaci. Původní aplikaci není potřeba odinstalovat. Potvrzené body a vybavení se ukládají; nepotvrzené body se neukládají.

Ověřeno v Unity Editoru: útok tlačítkem, zamíření, poškození, kolize, pozastavení, návrh a potvrzení atributů včetně uložení, rozpočet bodů, výměna a sundání vybavení a obnovení po smrti. Konzole bez chyb. Obraz nové verze na S26 ještě čeká na uživatelský test.

Modely jsou zatím pracovní, další povolání a plná tvorba postavy nejsou součástí prototypu. Inventář zatím nepodporuje přetahování předmětů. Stav nepřátel a bosse se neukládá.
