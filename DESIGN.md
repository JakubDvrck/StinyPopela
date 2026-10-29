# Návrh mobilního akčního RPG — verze 0.1

Datum: 29. října 2026  
Pracovní název: Stíny popela (návrh, zatím nepotvrzený)

## 1. Koncept

Akční RPG pro telefon, inspirované principy Diabla II: Lord of Destruction: boj s nepřáteli, náhodná kořist, budování vlastní postavy a průchod pěti prostředími. Hra bude mít vlastní svět, příběh, postavy a výtvarné zpracování.

Potvrzená je izometrická kamera. Navržené provedení je 3D, hraní na šířku a zpočátku hra pro jednoho hráče. Android/iOS, offline režim a obchodní model zatím nejsou rozhodnuté.

Základní herní smyčka: příprava a výbava → průzkum mapy → souboje a kořist → rozvoj postavy → dungeon → hlavní boss → další oblast.

## 2. Tvorba postavy

Hráč zadá jméno, určí pohlaví, zvolí rasu a povolání a upraví vzhled postavy.

Rasy: člověk, elf, trpaslík, temný elf.

Povolání: válečník, templář, čaroděj, nekromant, lovec, druid.

Každou rasu lze spojit s každým povoláním: celkem 24 kombinací bez omezení.

Navržené úpravy vzhledu: obličej, odstín pleti, účes, barva vlasů a očí, vousy, jizvy a tetování. Výběr odpovídá rase. Náhled postavy lze otáčet. Přesný rozsah úprav a vliv rasy na hraní zatím nejsou schválené; rasové bonusy nejsou potvrzenou součástí pravidel.

## 3. Atributy a úrovně

| Atribut | Hodnota na 1. úrovni | Navržený účinek |
|---|---:|---|
| Síla | 5 | Účinnost vybraných zbraní na blízko a požadavky výbavy |
| Obratnost | 5 | Účinnost vybraných střelných zbraní, přesnost a úhyb |
| Inteligence | 5 | Účinnost kouzel a zásoba many |
| Vitalita | 5 | Maximální počet životů |

Počáteční hodnoty jsou stejné pro všechny rasy a povolání. Za každý postup na další úroveň hráč získá 5 bodů k libovolnému rozdělení mezi atributy a 1 dovednostní bod. První příděl dostane při dosažení 2. úrovně.

Bez výbavy a dalších bonusů má postava na úrovni L celkem 20 + 5 × (L − 1) atributových bodů včetně nerozdělených a získala L − 1 dovednostních bodů. Například na úrovni 10 jde o 65 atributových bodů a 9 dovednostních bodů.

Maximální úroveň, zkušenostní křivka, přesné přepočty atributů a možnost resetu bodů zůstávají otevřené.

## 4. Stromy dovedností

Každé povolání má vlastní strom s větvemi navazujících schopností. Hráč si vybírá směr rozvoje, odemyká schopnosti a dalšími body je vylepšuje. Konkrétní stromy doplníme později.

| Povolání | Navržená identita |
|---|---|
| Válečník | Fyzický boj zblízka, zbraně a odolnost |
| Templář | Boj zblízka, svatá magie, aury a ochrana |
| Čaroděj | Živelná kouzla a plošné útoky |
| Nekromant | Nemrtví služebníci, kletby a magie smrti |
| Lovec | Střelné zbraně, pasti a pohyb |
| Druid | Přírodní magie, zvířata a proměny |

K rozhodnutí: počet větví, kombinování větví, předpoklady odemčení, maximální hodnost schopností, aktivní a pasivní schopnosti a jejich zdroje. Mana pro všechny nebo odlišné zdroje podle povolání zatím nejsou rozhodnuté.

## 5. Svět

Potvrzených je pět map. Každá má vlastní dungeon, v němž se nachází hlavní boss dané mapy: celkem pět hlavních dungeonů a pět hlavních bossů.

| Mapa | Navržená podoba dungeonu | Navržené zaměření bosse |
|---|---|---|
| Travnatá a lesní | Podzemí zarostlé pevnosti | Čitelné útoky zblízka a přivolání posil |
| Pouštní | Pohřbená hrobka | Pasti, pohyb a útoky z písku |
| Pralesní | Zapomenutý chrám | Jed, kořeny a nebezpečné zóny |
| Zimní hornatá | Ledová pevnost | Zpomalení a útoky s přípravou |
| Pekelná | Démonická citadela | Vícefázový boj, oheň a láva |

Navržené pořadí odpovídá tabulce. Každá oblast by fungovala jako jeden akt s úkoly a bezpečným zázemím. Poražení bosse by otevřelo další oblast. Toto odemykání, příběh, názvy a konkrétní bossové jsou zatím návrhy.

Zda budou mapy a dungeony ručně vytvořené, procedurální nebo kombinované, zatím není rozhodnuté. Pro prototyp doporučujeme ručně sestavenou malou oblast.

## 6. Souboje a ovládání

Návrh pro telefon na šířku: vlevo virtuální joystick, vpravo základní útok a tlačítka přiřazených schopností, poblíž tlačítko lektvaru a interakce. Počet současně dostupných schopností se ověří na telefonu; první prototyp nabídne tři sloty.

Izometrická kamera sleduje postavu. Doporučený je pevný úhel bez povinného otáčení, průhlednění překážek zakrývajících hrdinu a mírná pomoc se zaměřením. Plošné schopnosti mohou podporovat zamíření podržením tlačítka. Jde o návrh k praktickému ověření.

Nepřátelské silné útoky mají mít čitelnou přípravu. Náročnost má vznikat kombinací nepřátel, pohybu a schopností, ne pouze vysokým počtem životů.

## 7. Kořist a výbava

Potvrzené druhy kořisti: náhodné zbraně, brnění, prsteny, amulety a unikátní předměty.

Síla předmětu se odvozuje od úrovně postavy, pro kterou kořist v okamžiku dropu vznikla. Úroveň omezuje základní statistiky i rozsah bonusů. Postava na 1. úrovni nesmí získat předmět se silou určenou pro úroveň 60. Vzácnost může zvýšit kvalitu v rámci příslušného rozsahu, nesmí obejít úrovňový limit.

Navržený postup generování:

1. Zachytit úroveň postavy v okamžiku dropu.
2. Vybrat vhodný typ a základ předmětu z povoleného úrovňového rozsahu.
3. Vybrat vzácnost a kompatibilní bonusy.
4. Vygenerovat hodnoty v mezích dané úrovně.
5. Uložit výsledné hodnoty do konkrétního předmětu.

Návrh: běžné předměty po získání automaticky nerostou s postavou. Unikáty mají vlastní jméno a zvláštní efekt, jejich číselná síla respektuje úroveň dropu. Úrovňové varianty unikátů, šance dropu, stupně vzácnosti a případné přizpůsobení kořisti povolání ještě určíme.

Navržené sloty výbavy: hlava, trup, ruce, nohy, boty, hlavní ruka, vedlejší ruka, dva prsteny a amulet. Přesný seznam ani pravidla jednoručních a obouručních zbraní zatím nejsou potvrzené.

## 8. Mřížkový inventář

Potvrzený je mřížkový inventář. Návrh počítá s různě velkými předměty: prsten 1 × 1, meč 1 × 3, brnění 2 × 3 políčka. Konečné rozměry předmětů a celé mřížky se ověří na mobilním displeji.

Navržené interakce: klepnutí zobrazí detail a srovnání s výbavou, podržení umožní přesun, tlačítko umožní vybavení bez přesného přetahování. Doplníme automatické uspořádání. Při plném inventáři se předmět nesmí tiše ztratit; zůstane na zemi s informací o nedostatku místa.

Rotace předmětů, úložiště ve městě, obchodníci, prodej a skládání spotřebních předmětů zatím nejsou rozhodnuté.

## 9. První hratelný prototyp

Cílem je ověřit, zda jsou na telefonu zábavné a pohodlné pohyb, boj, sbírání kořisti a rozvoj postavy. První prototyp je omezený výřez celé hry.

Navržený obsah:

- Jeden hratelný válečník a jednoduché zadání jména; úplná tvorba postavy přijde později.
- Izometrická kamera a dotykové ovládání s možností testovat i na počítači.
- Malá travnatá a lesní oblast, jeden krátký dungeon a jeden boss.
- Tři běžné typy nepřátel: bojovník zblízka, střelec a odolný nepřítel.
- Základní útok a malá větev tří odemykatelných a vylepšitelných schopností.
- Úrovně 1–10 s potvrzeným přidělováním bodů.
- Kořist odpovídající úrovni, nasazení výbavy a mřížkový inventář.
- Uložení a načtení postavy, výbavy a postupu.

Kritéria dokončení: hráč vytvoří postavu, projde oblast, získá úroveň, rozdělí body, odemkne nebo vylepší schopnost, použije nalezenou výbavu a porazí bosse. Po restartu se obnoví uložený postup. Vygenerované předměty dodrží úrovňové limity. Ovládání a čitelnost se ověří na skutečném telefonu; cílový výkon stanovíme podle vybraného zařízení.

## 10. Doporučený postup vývoje

1. Potvrdit 3D provedení, cílovou mobilní platformu a režim hry; vybrat Unity Editor a založit projekt.
2. Ověřit pohyb, kameru a souboj na telefonu.
3. Přidat atributy, zkušenosti a malý strom válečníka.
4. Zavést kořist, výbavu, inventář a ukládání.
5. Dokončit první oblast, dungeon a bosse jako hratelný celek.
6. Rozšířit tvorbu postavy na všechny rasy a vzhled; postupně přidávat povolání a jejich stromy.
7. Doplnit zbývající čtyři oblasti a bossy, příběh a úkoly.
8. Vyvážit postup, kořist a obtížnost, optimalizovat a otestovat cílová zařízení.

Pro navrženou 3D variantu je technickým základem Unity s URP. Číselné definice předmětů, schopností a nepřátel oddělíme od herní logiky, aby šly upravovat bez přepisování systémů. Toto je návrh hry, nikoli již vytvořený Unity projekt.

## 11. Otevřená rozhodnutí

- 3D nebo 2D grafika, výtvarný styl a název hry.
- Android, iOS nebo obojí; minimální podporované zařízení.
- Hra pro jednoho hráče, offline režim a případný budoucí multiplayer.
- Obchodní model.
- Maximální úroveň a pravidla smrti, návratu do oblasti a opakovaného zabíjení bossů.
- Účinky atributů, rasové vlastnosti a stromy povolání.
- Rozměry inventáře, sloty výbavy, vzácnosti a pravidla unikátů.
- Příběh, úkoly a ruční či procedurální tvorba map.

Nejbližší krok po odsouhlasení návrhu: určit provedení grafiky a cílový telefonní systém, poté připravit Unity projekt pro první hratelný prototyp.
