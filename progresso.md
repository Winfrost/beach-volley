# Beach Volley - Diario di progetto

## Visione del progetto
Sviluppo di un videogioco di beach volley per Android e iOS.
Obiettivo finale: pubblicazione sugli store come prodotto serio
(possibile monetizzazione). Stile grafico: pixel art retro.
Modalità: 1 vs CPU e 2 giocatori sullo stesso schermo.

## Stack tecnologico
- **Motore:** Unity 6 (LTS)
- **Linguaggio:** C#
- **IDE:** Visual Studio Community con workload "Game development with Unity"
- **Versionamento:** Git + GitHub (repo privato `beach-volley`, utente Winfrost)
- **OS sviluppo:** Windows — progetto in `C:\Dev\BeachVolley`
- **Dispositivo test:** Android (fisico, via USB)
- **iOS:** affrontato a fine progetto (servirà Mac in cloud o fisico)
- TextMeshPro (HUD), PhysicsMaterial2D, Unity Particle System, 2D Pixel Perfect

## Decisioni di design e strategia
- Si è ripartiti da ZERO (vecchio progetto solo "memoria visiva")
- Scope ridotto e finibile: ~3 personaggi con stat diverse, 2-3 stadi,
  singleplayer vs CPU + 2 giocatori locali. NIENTE multiplayer online per v1.0.
- Approccio: solide fondamenta tecniche prima del gameplay
- Tre regole assolute:
  1. Niente magic numbers (ScriptableObject o costanti)
  2. Un MonoBehaviour fa UNA cosa (obiettivo ~150 righe)
  3. Commit frequenti, piccoli e descrittivi (refactor separati dalle feature)

## Piano per fasi
- **Fase 0** — Setup ambiente ✅
- **Fase 1** — Ricostruzione prototipo in Unity ✅
- **Fase 2** — Game feel + arte + audio + AI + input ✅ (QUESTA APPENA CHIUSA)
- **Fase 3** — Contenuto (personaggi, stadi, modalità torneo, salvataggi) ← PROSSIMA
- **Fase 4** — Audio (musica chiptune, pass SFX completo)
- **Fase 5** — Mobile e polish (build su device, performance, vari schermi)
- **Fase 6** — Pubblicazione (account store, asset, beta, submission)

---

## STATO ATTUALE: FASE 2 COMPLETATA ✅

Il gioco è completo end-to-end su tutti i fronti principali:

**Giocabilità**
- Modalità 2 giocatori locali da tastiera (frecce+Spazio / WASD)
- Modalità 1 giocatore vs CPU (la CPU prevede la traiettoria della palla)
- Controlli touch per mobile (pulsanti a schermo)

**Game feel** (tutto via eventi, consolidato in `ImpactFeedback`)
- Colpo direzionale del giocatore (punto di contatto + velocità)
- Squash & stretch della palla
- Screen shake, hit-stop, particelle d'impatto (sabbia + scintilla), SFX d'impatto

**Grafica (pixel art)**
- Pipeline pixel-perfect: PPU 25, risoluzione interna 480x270, upscale x4 a 1080p
- Scena vestita: sfondo cielo/mare, sabbia, palla, due giocatori tintati
- Giocatori animati (idle / walk / air) via frame singoli

**Architettura input**
- Tre sorgenti (tastiera, CPU, touch) dietro l'interfaccia `IPlayerInput`;
  il `PlayerController` le guida tutte senza saperlo

---

## Struttura del progetto

```
Assets/_Project/
  Animations/  Audio/  Fonts/  Materials/  Prefabs/  Scenes/  Settings/  Sprites/
  Scripts/
    AI/           AIPlayerInput, AIStats
    Core/         GameManager, GameplayBootstrap, state machine
    Gameplay/     Ball, BallStats, PlayerController, PlayerStats, MatchController,
                  IPlayerInput, KeyboardPlayerInput, TouchButton, TouchPlayerInput
    Presentation/ BallSquashStretch, CameraShake, HitStop, ImpactFeedback,
                  SfxPlayer, SpriteAnimator
    UI/           HUDController, WinScreenController
    Utils/
```

Namespace allineati alle cartelle: `BeachVolley.Core/.Gameplay/.AI/.Presentation/.UI`.

**Rimossi durante il consolidamento:** `GameplayShakeTrigger`, `HitStopTrigger`,
`ImpactParticleTrigger` (fusi in `ImpactFeedback`).

---

## Strategia chat con Claude
- Una chat per FASE del progetto; chat nuova al confine di fase
- Questo `progresso.md` è il ponte: incollarlo in cima al primo messaggio di ogni chat nuova
- Prossima chat: "Beach Volley - Fase 3 - Contenuti"

## Prossima Fase: 3 - Contenuti
Obiettivo: personaggi con stat differenti, stadi, modalità (torneo), salvataggi.
L'architettura costruita è la base giusta: stat in ScriptableObject (PlayerStats,
BallStats, AIStats), comunicazione a eventi, sorgenti di input intercambiabili.

---

## Fase 2 — rifiniture RIMANDATE (non ancora fatte)
Lavoro opzionale lasciato indietro, da riprendere quando si vuole:
- **Rete vestita**: oggi è una barra bianca (placeholder funzionale con collider).
  Manca lo sprite pixel art (pali + maglia) dimensionato al collider.
- **Mira della CPU**: la CPU rimanda la palla ma non sceglie *dove* (no mira al
  campo scoperto). Va influenzata via posizionamento + velocità al contatto.
- **Predizione CPU con rimbalzi sui muri**: ora ignora le carambole laterali
  (si auto-corregge dopo il rimbalzo, ma non anticipa).
- **Selettore input automatico** per piattaforma/modalità (ora si cambia a mano
  il componente IPlayerInput sul giocatore; un menu lo farà in Fase 3).
- **Polish mobile vero** (sconfina in Fase 5): build su device, performance,
  gestione multi-aspect dei vari schermi, layout/trasparenza dei pulsanti touch.

---

## Decisioni di architettura — FASE 1 (Sessioni 3-5)
- GameManager: Singleton con DontDestroyOnLoad, root GameObject, Script Execution Order -100
- State Machine con transizioni validate esplicitamente (no transizioni "magiche")
- Comunicazione tra sistemi via event Action<T> (no riferimenti diretti)
- Sottoscrizione: OnEnable iscrive, OnDisable disiscrive, sempre
- Player NON è Singleton, NON ha riferimenti al GameManager (accoppiamento minimo)
- Movimento orizzontale: velocity diretta (feel arcade); salto: impulso + variable
  jump height + gravity multipliers; ground check via Physics2D.OverlapCircle
- Config gameplay tutta in ScriptableObject (PlayerStats, BallStats)
- Ball usa eventi (OnHitByPlayer, OnGroundTouched, OnNetTouched), no logica punteggio
- Lato campo determinato dalla X rispetto alla rete (X=0)
- PhysicsMaterial2D sul Rigidbody della palla
- Punteggio in GameManager (scope semplice); MatchController come "ponte" Ball<->GameManager
- Singleton lazy auto-creating + GameplayBootstrap per init esplicita
- Flag di stato critici resettati su TUTTI i percorsi di uscita (bug rematch)
- Active Input Handling = "Both" (legacy + nuovo Input System)

## Decisioni di architettura — FASE 2 (Sessioni 6-8)

### Game feel (Sessione 6)
- **Colpo direzionale** ("combinato"): punto di contatto + velocità del player.
  Contatto normalizzato su collider.bounds.extents.x (indipendente da sprite/scala).
  Ball legge la velocità del player da collision.rigidbody (no dipendenza da PlayerStats).
  aim in [-1,+1] -> angolo da verticale = aim * maxHitAngle; dir = (sin, cos) -> palla
  sempre verso l'alto. Il colpo sostituisce il rimbalzo fisico (rb.linearVelocity diretto);
  PhysicsMaterial2D solo per muri/terreno/rete. Parametri in BallStats.
- **BallSquashStretch**: pattern "fisica sul root, grafica su figlio Visual". Componente
  reattivo, anima localScale del figlio (snap + recover via coroutine + AnimationCurve).
  Squash ad assi fissi. Resta separato da ImpactFeedback (grafica locale della palla).
- **Separazione meccanismo / policy** per il feedback di scena:
  - CameraShake = meccanismo (Shake(intensity), offset decrescente in LateUpdate)
  - HitStop = meccanismo (Time.timeScale=0 per N secondi REALI; WaitForSecondsRealtime;
    OnDestroy ripristina timeScale=1). Squash/shake usano Time.deltaTime apposta, così
    si congelano/persistono coerenti durante il freeze.
  - Particelle: nessun meccanismo custom (il ParticleSystem È il meccanismo;
    Simulation Space=World, Play On Awake=OFF, burst singolo)
  - SfxPlayer = meccanismo (AudioSource + PlayOneShot; audio non influenzato da timeScale)
  - CameraShake/HitStop/SfxPlayer = singleton SCOPED SULLA SCENA (no DontDestroyOnLoad)
- **ImpactFeedback** = coordinatore/policy unico: si iscrive UNA volta agli eventi Ball,
  config raggruppata per evento (hit / ground), smista a tutti i meccanismi. Ha sostituito
  i tre trigger separati. Zero modifiche a Ball lungo tutto il game feel.

### Pixel art (Sessione 7)
- PPU di progetto = 25; risoluzione interna 480x270 (upscale x4 a 1080p);
  riproduce esattamente l'ortho size 5.4 esistente (270/2/25). Framing invariato.
- Pixel Perfect Camera sulla Main Camera (PPU 25, ref 480x270)
- Import sprite: Single, Point, Compression None, no MipMaps, PPU 25, Full Rect.
  Preset "PixelArtSprite" salvato e impostato come Default for Texture Importer.
- Sorting Layers: Default, Background, Midground, Gameplay, Ball, Foreground, FX, UI.
  Particelle -> layer FX via modulo Renderer del Particle System (NON SpriteRenderer).
- Pattern "fisica sul root, grafica su figlio Visual" anche per il giocatore.
- Pivot giocatore = Center (allinea a fisica esistente: transform al centro, piedi a -0.96).
- Sprite: palla 16x16; giocatore 24x48 tintabile (Color: P1 rosso, P2 blu);
  sfondo 480x270; sabbia 512x64 (larghezza nativa, no stretch).
- Formula dimensione sprite: pixel = unità_mondo x PPU.

### Input, AI, animazione (Sessione 8)
- **IPlayerInput**: interfaccia per la "sorgente di intenzione" del giocatore
  (Tick, Horizontal, JumpHeld, ConsumeJumpPressed). PlayerController NON legge più
  la tastiera: ottiene IPlayerInput via GetComponent. Meccanismo (movimento) / policy (input).
  Un solo componente IPlayerInput per giocatore.
- **KeyboardPlayerInput** (schema tasti P1/P2), **AIPlayerInput**, **TouchPlayerInput**
  implementano tutte la stessa interfaccia.
- **AIPlayerInput** (cartella AI): insegue/prevede la palla; salta quando è sopra,
  entro hitReachX e jumpTriggerHeight; jumpCooldown anti-rimbalzo. Config in AIStats.
  Modalità 1P = sostituire KeyboardPlayerInput con AIPlayerInput su Player2.
- **Predizione balistica**: risolve y(t)=altezza_intercetto per il moto del proiettile
  (gravità effettiva = Physics2D.gravity.y * ballRb.gravityScale), poi X(t). Ricalcolata
  ogni frame -> si auto-corregge dopo ogni rimbalzo. Salto sulla posizione REALE.
- **Touch**: pulsanti UI a schermo (no migrazione Input System ora). TouchButton
  (IPointerDown/Up -> IsPressed + edge) letto da TouchPlayerInput. Testabile in editor col mouse.
- **SpriteAnimator** (Presentation): cicla array di frame singoli per stato idle/walk/air,
  stato dedotto dalla velocità del Rigidbody2D del padre. Niente Animator né sprite-sheet.
  flipX pronto per arte di profilo.

---

## Lezioni dagli intoppi (Fase 2)
- **Scala dei placeholder**: il Player aveva Scale (5, -3.5) ereditata dai rettangoli
  -> pixel distorti, offset/Y invertiti, piedi nella sabbia, ground check fuori posto.
  Fix: normalizzare a (1,1,1); dimensione dai PIXEL a PPU 25, non dalla scala.
  Collider/offset SEMPRE in unità reali (Size x Scale). Normalizzare la scala PRIMA della pixel art.
- **Preset contaminato**: un Preset salva TUTTI i campi, inclusa Sprite Mode e i ritagli.
  Crearlo da uno sprite in Single mode pulito, altrimenti avvelena ogni import (warning
  "rect outside texture"). Fix per-sprite: Sprite Mode = Single, Apply.
- **Reimport e riferimenti sprite**: cambiare Preset/PPU forza il reimport; se lo sprite
  cambia identità (Single<->Multiple) i SpriteRenderer perdono il riferimento e diventano
  invisibili. Dopo reimport massivi, verificare sempre che gli sprite siano ancora assegnati.
- **Sabbia visiva vs collider**: il bordo superiore del collider del terreno deve coincidere
  con la superficie VISIBILE della sabbia, o i piedi sembrano affondare/fluttuare.
- **Nome file = nome classe** per i MonoBehaviour; un errore di compilazione in QUALSIASI
  script blocca l'aggiunta di TUTTI i componenti custom (controllare la Console).
- **Particle Sorting Layer** sta nel modulo Renderer, non su uno SpriteRenderer.

---

## Costanti tecniche chiave
- Pixel art: PPU 25; risoluzione interna 480x270 (x4 a 1080p); ortho size 5.4
- Pixel Perfect Camera: PPU 25, ref 480x270
- Player: Position Y -3.55, Scale (1,1,1), CapsuleCollider2D Size (0.9, 1.9) Offset (0,0)
- PlayerStats: groundCheckOffsetY -0.95
- Ground: Position Y -5, Scale (20,1,1), BoxCollider2D Size (1,1) -> bordo superiore a -4.5
- Sabbia: sprite 512x64, Position Y -5.78 (top a -4.5), Scale (1,1,1), layer Midground
- Sfondo: sprite 480x270, Position (0,0,0), Scale (1,1,1), layer Background
- Riferimento Canvas UI: Scale With Screen Size, ref 1920x1080
- Build: IL2CPP, ARMv7 + ARM64, Minimum API Android 7.0
- GameManager Script Execution Order: -100

## Backlog idee future
*(raccoglie idee emerse ma da NON implementare subito)*
- Variazione potenza del colpo in base alla velocità in ingresso (smash)
- Mira deliberata della CPU verso il campo scoperto
- Animazioni più ricche (Aseprite, sprite di profilo con flip)

## Note utili
- Vecchio progetto (>2 anni fa, solo APK) NON recuperato: solo ispirazione visiva.
- Prototipo HTML5 `beach-volley.html` come riferimento di game design.
