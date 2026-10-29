# Stíny popela — oprava prototypu 0.1.2

## Opravy

- Čisté sestavení bez staré cache odstraňuje nesoulad serializace grafických dat. Původní APK kvůli němu na S26 vyčerpalo přibližně 7 GB paměti a Android jej ukončil.
- Materiál `PrototypeLit` je explicitně přibalený v Resources a herní objekty jej načítají přímo. To opravuje chybějící shader a černorůžový obraz.
- Grafická nastavení byla znovu serializovaná aktuálním Unity Editorem.
- Android versionCode 3 umožňuje aktualizaci předchozí instalace bez mazání dat.

Unity sestavení 0.1.2 uspělo s nulovým počtem chyb. Verze 0.1.1 na S26 prošla kontrolou běhu a paměti kolem 452 MB, ale měla chybu shaderu. Verze 0.1.2 tuto chybu opravuje; její obraz a ovládání na telefonu ještě čekají na ověření.

První technická verze pro Android. Pracovní modely a prostředí slouží k ověření soubojů a ovládání.

APK 0.1.2: arm64-v8a, minimální Android API 25, velikost přibližně 52 MB.

- Izometrická kamera, virtuální joystick a tlačítka útoku.
- Válečník, lesní nepřátelé a boss v kamenném dungeonu na severu.
- Základní útok a plošný sek, který je potřeba odemknout skill pointem.
- Na 1. úrovni všechny atributy 5; za další úroveň 5 volitelných bodů a 1 skill point.
- Náhodné meče podle úrovně postavy v okamžiku porážky nepřítele.
- Automatický sběr do inventáře 8 × 4, vybavení klepnutím na meč.
- Ruční uložení a načtení atributů, zkušeností, schopnosti a kořisti.

## Instalace na Android

Stáhni přiložené `StinyPopela-0.1.2.apk` do Android telefonu a otevři ho. Nainstaluj je jako aktualizaci; původní hru není potřeba odinstalovat. Pokud Android vyžádá oprávnění k instalaci z tohoto zdroje, povol ho pro aplikaci, ze které APK otevíráš. Poté spusť Stíny popela. Instalace přes Google Play zatím není připravená.

## Co zkusit

1. Projdi lesem pomocí joysticku a bojuj tlačítkem ÚTOK.
2. Po získání úrovně rozděl atributové body a odemkni SEK.
3. Seber kořist, otevři inventář a klepnutím vybav nalezený meč.
4. Pokračuj severně do kamenné oblasti a poraz strážce.
5. Vyzkoušej ULOŽIT a NAČÍST.

## Známá omezení

Verze zatím nebyla ověřena na skutečném telefonu. Chybí tvorba postavy, ostatní rasy a povolání, zvuk, animace, plné stromy schopností a další druhy výbavy. Inventář zatím nepodporuje ruční přesouvání předmětů. Uložení neobnovuje polohu, nepřátele ani poražení bosse. Po smrti bez předchozího uložení ukonči a znovu otevři hru. Grafika je pracovní.

Při hlášení problému uveď model telefonu, verzi Androidu, postup k vyvolání chyby a případný snímek obrazovky.
