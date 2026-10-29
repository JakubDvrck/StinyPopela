# Kolizní chyby a obnovení po smrti — 0.1.3

Uživatel na S26 potvrdil vykreslený svět, nulové zdraví, nefunkční NAČÍST a výpis chyb CapsuleCollider/CharacterController.

Příčina kolizní chyby: primitivní kapsle už obsahovala CapsuleCollider. Destroy odstraňuje komponentu až později, ale CharacterController se přidával ihned na stejný objekt. Pracovní model je nyní samostatný potomek, kořen hráče obsahuje pouze CharacterController. Collider modelu je ihned vypnutý a následně odstraněný.

Příčina nefunkčního tlačítka: LoadGame při chybějícím uložení pouze vracel řízení. Nová metoda Revive obnoví zdraví, bezpečně přesune ovladač na začátek mapy, vynuluje joystick a poskytne tři sekundy ochrany. LoadGame ji používá i bez uložení.

Ověřeno v běžící Unity scéně: CharacterController existuje, CapsuleCollider na kořenu není, nulové zdraví se přes LoadGame bez uložení obnoví na maximum, poloha se vrátí na začátek a ovladač umožní pohyb. Konzole po těchto kontrolách obsahovala nula chyb. Test nové verze na Android telefonu je samostatný následný krok.
