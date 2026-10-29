# Pád Android prototypu 0.1.0 při spuštění

## Potvrzené údaje z telefonu

- Android ukončoval proces hry s důvodem `LOW_MEMORY` / `OOM KILL`.
- Před ukončením systém zaznamenal RSS přibližně 6,9–7,5 GB.
- Log Unity před načtením herního světa hlásil nesoulad serializace `UniversalRenderPipelineAsset` a `UniversalRendererData` (například 452 přečtených bajtů místo očekávaných 600).
- Podpis APK a zarovnání nativních knihoven byly v pořádku. Tyto kontroly samy o sobě nepotvrzují funkčnost aplikace.

## Oprava sestavení

Předchozí APK vzniklo dokončením Gradle exportu i přesto, že Unity build report obsahoval chyby TypeDB. Tento postup nepoužívat pro distribuci dalších verzí.

Verze 0.1.1 se sestavuje z nového checkoutu ve skutečné ASCII cestě, bez kopírování staré Library/Bee cache. Grafická nastavení a scéna byly znovu serializovány aktuálním Editorem. Diagnostický build používá volbu Development a zvýšený Android versionCode 2, aby šel nainstalovat jako aktualizace.

## Podmínky přijetí

1. Unity BuildPipeline skončí výsledkem Succeeded bez chyb; ruční zabalení chybného exportu není náhrada.
2. Aktualizace APK se nainstaluje bez mazání uživatelských dat.
3. Na připojeném telefonu se zobrazí hratelný svět a rozhraní.
4. Hra zůstane spuštěná a spotřeba paměti se nebude nekontrolovaně zvyšovat.
5. Nový log nebude obsahovat uvedené chyby serializace.

## Výsledek dosavadních kontrol

Čistá verze 0.1.1 byla nainstalovaná na S26. Proces zůstal spuštěný a deset vzorků PSS mělo maximum 451,8 MB. Původní chyba serializace zmizela. Dodatečná kontrola ale našla `InvalidOperationException: URP Lit shader unavailable`, která zastavila vytváření světa a vedla k černorůžovému obrazu.

Verze 0.1.2 přidává explicitně referencovaný materiál v Resources a načítá jej namísto samotného Shader.Find. Unity build uspěl bez chyb a vygenerovaný IL2CPP kód obsahuje Resources.Load pro materiál. Závěrečný obraz a ovládání na telefonu čekají na uživatelský test staženého APK; USB ladění již nebylo dostupné.
