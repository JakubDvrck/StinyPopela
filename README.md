# Stíny popela — prototyp 0.2.0

Unity 6000.3.27f1 LTS, URP, Android arm64. Budoucí iOS verze používá společný projekt.

Stažení: https://github.com/JakubDvrck/StinyPopela/releases/tag/v0.2.0-prototype

## Co obsahuje tato verze

- Izometrický les, dungeon a boss, válečník, joystick, základní útok a odemykatelný sek.
- Základní útok funguje od začátku a zamíří na nejbližšího nepřítele v dosahu. Sek zasahuje okolí.
- Lišty životů nepřátel a plovoucí čísla skutečného poškození. Poškození hráče je červené.
- Panel POSTAVA / VÝBAVA: náhled aktuálního modelu, deset slotů výbavy a inventář 8 × 4.
- Klepnutí na předmět v mřížce jej vybaví a ukáže porovnání; klepnutí na vybavený předmět jej sundá, pokud je místo.
- Náhodné zbraně, části zbroje, prsteny a amulety podle úrovně; boss má zvláštní meč.
- Rozdělování atributů pod inventářem s náhledem a tlačítky ULOŽIT ATRIBUTY a ZRUŠIT ZMĚNY. Zavření panelu nepotvrzené změny zahodí.
- Při otevřeném panelu se hra pozastaví. Výbava a potvrzené atributy se uloží automaticky.
- NAČÍST funguje i bez uložení: obnoví zdraví a vrátí postavu do bezpečí.

## Atributy prototypu

Na úrovni 1 jsou všechny atributy 5. Za další úroveň hráč získá 5 volitelných bodů a 1 skill point.

- Síla: +2 základního poškození za bod; ovlivňuje také výchozí sílu seku.
- Obratnost: +0,05 rychlosti pohybu za bod.
- Inteligence: +2 poškození seku za bod.
- Vitalita: +10 maximálních životů za bod.

Jde o počáteční vyvážení pro hratelného válečníka. Náhled zobrazuje skutečné současné přepočty.

## Spuštění a sestavení

Otevři Assets/Scenes/Prototype.unity a stiskni Play. WASD pohyb, mezerník útok, Q sek. Na telefonu používej joystick a tlačítka. Sek se odemyká skill pointem po získání úrovně.

Pro Android klonuj do skutečné cesty bez diakritiky, například C:\UnityProjects\StinyPopela. Starou Library/Bee cache nekopíruj. Použij Android Build Support, SDK/NDK a OpenJDK z Unity instalace. Metoda StinyPopela.Editor.PrototypeSetup.BuildAndroid vytvoří vývojové APK. Build musí skončit Succeeded bez chyb; ruční balení chybného exportu není náhrada.

## Ověření a omezení

Integrační metoda StinyPopela.Editor.PrototypeVerification.Run se spouští v Play režimu. Ověřuje skutečné tlačítko základního útoku, zamíření, poškození, kolize, pozastavení, návrh/zrušení/potvrzení a uložení atributů, rozpočet bodů, výměnu výbavy bez ztráty a obnovení po smrti. Kontroly i vizuální kontrola menu prošly v Editoru s nulovým počtem chyb konzole. Test 0.2.0 na skutečném telefonu je následný krok.

Grafika je pracovní. Chybí úplná tvorba postavy, další povolání, ruční přetahování předmětů a další mapy. Uložení zachovává postavu, schopnost, kořist a výbavu; neobnovuje polohu, nepřátele ani stav bosse. Cloudové sestavování není nastavené. Repo je veřejné.
