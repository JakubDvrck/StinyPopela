# Stíny popela — pracovní prototyp

Unity 6000.3.27f1 LTS, Universal Render Pipeline. Hlavní platforma Android; budoucí iOS verze používá společný projekt.

## Spuštění

Otevři projekt v odpovídající verzi Unity. Scéna `Assets/Scenes/Prototype.unity` bude po dokončení přípravy hlavní herní scénou. Lze ji vytvořit také přes menu **Stiny Popela → Create Prototype Scene**; tento příkaz nahrazuje otevřenou scénu, proto před použitím ulož svou práci.

Stiskni Play. Na počítači: WASD pohyb, mezerník útok, Q plošný sek. Na telefonu: joystick a tlačítka. Sek je nutné nejprve odemknout dovednostním bodem získaným po postupu úrovně. Tlačítka atributů utrácejí volné body. Kořist se sbírá přiblížením, pokud je v mřížce místo. V inventáři klepnutí na meč nastaví jeho bonus jako aktuálně vybavenou zbraň.

## Rozsah

Pracovní geometrie lesa a dungeonu, válečník, nájezdníci, boss, útok a vylepšitelný sek, zkušenosti a atributy, náhodné meče podle úrovně a inventář 8 × 4. Uložení zachovává atributy, zkušenosti, schopnost a kořist; zatím ne stav světa, pozici, životy nepřátel ani poražení bosse. Tlačítko načíst obnovuje zdraví.

Toto je technický prototyp. Neobsahuje zatím úplnou tvorbu postavy, všechny rasy a povolání, ostatní druhy výbavy, přesouvání předmětů v inventáři, příběh ani zbývající mapy.

## Android

Po ověření scény použij Android Build Support s SDK/NDK a OpenJDK z Unity instalace. Editor metoda `StinyPopela.Editor.PrototypeSetup.BuildAndroid` vytvoří vývojové APK v `Builds/Android/StinyPopela.apk`. APK a generované složky jsou vyloučené z Gitu. APK není podepsané produkčním klíčem; použije vývojový podpis Unity.

Android nástroje vyžadují skutečnou cestu projektu bez diakritiky. Pro další sestavování klonuj například do složky `C:\UnityProjects\StinyPopela`. Krátká cesta Windows nestačí: Gradle ji může převést zpět na původní cestu s diakritikou. První APK bylo připraveno dokončením balení exportovaného Gradle projektu v samostatné složce bez diakritiky.

## Pokračování z telefonu

Repozitář: https://github.com/JakubDvrck/StinyPopela

GitHub uchovává zdrojové soubory; sám o sobě nespouští Unity Editor. Pro testování na Androidu je nutné stáhnout vytvořené APK a povolit instalaci pro konkrétní použitý zdroj. Pro další sestavení musí být dostupný počítač s Unity nebo samostatně nastavená sestavovací služba. Automatické cloudové sestavování zatím není nastavené.

## Stav ověření

Ověřeno v Unity Editoru: kompilace skriptů, běžící herní smyčka, zobrazení scény a českých textů, zásah nepřítele a získání zkušeností. Samostatná kontrola ověřila 5 atributových bodů a 1 skill point při postupu z úrovně 1 na 2 a utracení atributového bodu.

Android APK 0.1.0 bylo sestaveno z exportovaného Gradle projektu. Podpis APK v2 byl ověřen nástrojem apksigner; manifest obsahuje `com.stinypopela.prototype`, minimální API 25, cílové API 36 a architekturu arm64-v8a. Velikost APK je 44 339 316 bajtů. Hra dosud nebyla spuštěna na skutečném telefonu; tento test musí následovat na Galaxy S26.
