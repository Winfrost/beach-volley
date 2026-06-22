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
- **Principio Fase 3**: confini puliti, non infrastruttura anticipata. Si costruisce
  la cosa piccola che funziona, con i layer separati, cosi il pezzo grosso si aggiunge
  AL BORDO senza riscrivere il cuore. (Online/pagamenti/evoluzioni = lunghissimo periodo,
  NON si architettano ora.)

## Piano per fasi
- **Fase 0** — Setup ambiente ✅
- **Fase 1** — Ricostruzione prototipo in Unity ✅
- **Fase 2** — Game feel + arte + audio + AI + input ✅
- **Fase 3** — Contenuto (personaggi, stadi, modalità torneo, salvataggi) ← IN CORSO
- **Fase 4** — Audio (musica chiptune, pass SFX completo)
- **Fase 5** — Mobile e polish (build su device, performance, vari schermi)
- **Fase 6** — Pubblicazione (account store, asset, beta, submission)

---

## STATO ATTUALE: FASE 3 IN CORSO

Architettura dei contenuti avviata. Fatti i primi due passi (iniezione personaggi +
seam di setup match). Prossimo: scena menu + caricamento scene + selezione personaggio.

Base end-to-end della Fase 2 intatta:
- 2 giocatori locali da tastiera (frecce+Spazio / WASD)
- 1 giocatore vs CPU (predizione balistica della palla)
- Controlli touch (pulsanti a schermo)
- Game feel completo via eventi (consolidato in ImpactFeedback)
- Pixel art pixel-perfect (PPU 25, 480x270 upscale x4)
- Tre sorgenti input dietro IPlayerInput

---

## Struttura del progetto (aggiornata Fase 3)

```
Assets/_Project/
  Animations/  Audio/  Fonts/  Materials/  Prefabs/  Sprites/
  Scenes/      Gameplay.unity, MainMenu.unity (creata, da riempire)
  Settings/    *_Default.asset, PixelArtSprite.preset, BallBouncy.physicsMaterial2D
    Characters/  Character_Scatto, Character_Bastione,
                 PlayerStats_Scatto, PlayerStats_Bastione
  Scripts/
    AI/           AIPlayerInput, AIStats
    Content/      CharacterDefinition, PlayerCharacter, MatchConfig, MatchSession
    Core/         GameManager, GameplayBootstrap, GameState,
                  GameStateDebugListener, MatchController
    Gameplay/     Ball, BallStats, PlayerController, PlayerStats, IPlayerInput,
                  KeyboardPlayerInput, TouchButton, TouchPlayerInput
    Presentation/ BallSquashStretch, CameraShake, HitStop, ImpactFeedback,
                  SfxPlayer, SpriteAnimator
    UI/           HUDController, WinScreenController
    Utils/
```

Namespace allineati alle cartelle: `BeachVolley.Core/.Gameplay/.AI/.Content/.Presentation/.UI`.
**Simmetria contenuto**: Scripts/Content (codice) <-> Settings/Characters (dati) <->
Sprites/Characters (arte). Una sola natura di file per cartella; stadi/modalita
replicheranno (Settings/Stages, ecc.).

Gerarchia GameObject scena Gameplay: _Bootstrap, Main Camera, Environment (Background,
LeftWall, RightWall, NetPlaceholder, Ground), Gameplay (Player1+Visual, Player2+Visual,
Ball+Visual), UI (Canvas: ScoreText, WinPanel, BtnLeft/Right/Jump), EventSystem,
FX (FX_SandPuff, FX_HitSpark), Audios.

---

## Strategia chat con Claude
- Una chat per FASE; chat nuova al confine di fase. Questo `progresso.md` è il ponte:
  incollarlo in cima al primo messaggio di ogni chat nuova.
- A fine di ogni micro-passo: commit git + aggiornamento di questo file.

---

## FASE 3 — Contenuti: avanzamento

### Passo 1 — Personaggi come contenuto iniettabile ✅
- **CharacterDefinition** (SO, namespace BeachVolley.Content, cartella Scripts/Content):
  unita di CONTENUTO = identita (displayName, portrait, tint) + PlayerStats (umano)
  + AIStats (CPU). NON gonfia PlayerStats: lo CONTIENE per riferimento.
- **Separazione dato/identita**: PlayerStats = "come si muove il corpo" (fisica,
  riusabile, si tocca per bilanciare); CharacterDefinition = "chi e il personaggio".
  Due file per personaggio (Character_X + PlayerStats_X), legati per riferimento.
  Asset condiviso = stesso oggetto in memoria (NON template che si istanzia): stat
  condivise tra due Character li renderebbe identici -> serve un PlayerStats per ognuno.
- **PlayerController.SetStats(newStats)**: swap del riferimento stats. Funziona perche
  il controller legge le stat DAL VIVO (mai cachate in Awake) -> vale dal FixedUpdate
  successivo. Campo stats nell'inspector resta come fallback/testabilita.
- **PlayerCharacter** (componente, Content, RequireComponent PlayerController):
  MECCANISMO per "indossare" un personaggio. Apply(definition) spinge PlayerStats sul
  controller e tint sullo SpriteRenderer (GetComponentInChildren -> figlio Visual).
  QUALE personaggio lo decide altri.
- **Iniezione per RIFERIMENTO non per valore**: si passa QUALE PlayerStats usare, non
  si copiano i valori. Stessa mossa di IPlayerInput. Dipendenze: Content -> Gameplay,
  mai il contrario (no ciclo).
- Personaggi di prova: Scatto (veloce, arioso) e Bastione (lento, ancorato), tint diversi.

### Passo 2 — MatchConfig + MatchSession: seam di setup match ✅
- **MatchConfig**: data class (NON SO), namespace Content. La FORMA del setup di un
  match (i 2 CharacterDefinition; poi stadio/regole/contesto torneo). Stato di SESSIONE
  (transitorio, runtime, valido per un match), non tuning autoriale -> plain class, cosi
  non sporca il test "boot dritto in Gameplay" (un SO conserverebbe i valori runtime).
- **MatchSession**: statico, Content. Il TRASPORTO del config pendente tra la scena che
  lo scrive (menu/torneo) e quella che lo consuma (Gameplay). Unico seam deliberato per
  passare dati tra scene. Auto-reset a ogni Play via [RuntimeInitializeOnLoadMethod
  (SubsystemRegistration)] -> sessione sempre fresca, a prescindere da "Reload Domain".
- **Forma vs trasporto** separati: cambiare CHI riempie il config (menu oggi, torneo
  domani) non tocca la forma ne il codice che la legge.
- **GameplayBootstrap = composition root**: UNICA eccezione legittima alla regola
  "dipendenze in una direzione" — il suo mestiere e assemblare, quindi conosce tutti i
  layer di proposito. Legge config-o-fallback: MatchSession.Pending se presente, altrimenti
  BuildFallbackConfig() dai 4 campi serializzati (= i default di test).
- Verifica: comportamento VISIBILE invariato bootando in Gameplay (prova che il fallback
  funziona). Lo step costruisce il binario, non aggiunge gioco.

### Passo 3 — Primo menu + transizione di scena ✅
- **SceneNames** (statico, Core): costanti dei nomi scena (MainMenu, Gameplay). No magic
  string. NB: ogni nome deve combaciare col file scena E con l'entry in Build Settings
  (LoadScene per nome lo richiede).
- **MainMenuController** (UI): bottone Gioca -> scrive un MatchConfig (2 personaggi di
  default serializzati) via MatchSession.Set() -> SceneManager.LoadScene(Gameplay).
  I default verranno sostituiti dalla selezione personaggio (3b); il job del controller
  (costruisci config + carica) resta.
- **Flusso completo che si chiude**: boot da MainMenu -> GameManager Boot->MainMenu ->
  click Gioca -> config in MatchSession + load Gameplay -> GameManager PERSISTE (stato
  MainMenu) -> GameplayBootstrap legge il config e fa MainMenu->Playing. Il binario dei
  passi 1-2 si accende qui.
- **GameManager NON toccato**: la state machine aveva gia MainMenu/Loading/Playing e la
  transizione MainMenu->Playing. Buona lungimiranza passata.
- **Scena MainMenu vestita**: Canvas (sotto UI, Scale With Screen Size 1920x1080),
  BtnGioca->OnPlayPressed, EventSystem (sotto Managers). GameManager a RADICE
  (obbligatorio per DontDestroyOnLoad; il suo Awake stacca a forza se figlio).
- **Build Settings**: MainMenu (indice 0) + Gameplay aggiunte.
- Test "boot dritto in Gameplay" ancora intatto (MatchSession vuoto -> fallback).

### Passo 4 — Modalita 1P/2P + difficolta CPU ✅
- **Due assi separati**: FORMA FISICA (PlayerStats, del personaggio, mai toccata dalla
  difficolta) vs BRAVURA (AIStats, quanto bene la CPU guida quel corpo). La difficolta
  e un avversario "piu bravo", non "piu potente".
- **MatchConfig cresce**: mode (enum MatchMode 1P/2P = bivio strutturale) + cpuStats
  (AIStats = difficolta; un riferimento, non un enum). cpuStats null -> fallback all'aiStats
  del personaggio (config-or-fallback a livello di campo).
- **Difficolta SOSTITUISCE (non modula)**: scelta -> AIStats_Easy/Medium/Hard usato al
  posto del cervello del personaggio. Leva piu forte = usePrediction (off su Easy: insegue
  dove la palla E', non dove SARA -> sempre in ritardo). In 2P l'asse bravura non esiste
  (nessuna CPU, AIStats non letto).
- **PlayerController.SetInput(IPlayerInput)**: sceglie la sorgente di input a runtime.
  Serve perche il controller cachea input in Awake (troppo presto); il Bootstrap (Start,
  dopo gli Awake, prima del primo Tick) lo sovrascrive. Stessa mossa di SetStats.
- **AIPlayerInput.SetStats(AIStats)**: inietta la difficolta. Legge stats dal vivo ogni
  Tick -> applica subito.
- **Evoluzione regola "un solo IPlayerInput"**: Player2 ora ospita ENTRAMBI Keyboard+AI
  (configurati in inspector); il Bootstrap ne SELEZIONA uno via SetInput. Da scelta
  manuale a automatica. Chiude la rifinitura Fase 2 "selettore input automatico".
- **GameplayBootstrap**: risolve i pezzi di Player2 in Awake (GetComponent), in Start
  legge config e fa ConfigurePlayer2Input: 1P -> AI (cervello scelto o default) +
  SetInput(ai); 2P -> SetInput(keyboard). Campi fallback nuovi (fallbackMode,
  fallbackCpuStats) per testare tutto da boot-direct.
- **AIStats Easy/Medium/Hard** (Settings/Difficulty): tarati sui campi reali
  (chaseDeadzone, usePrediction, maxPredictTime, hitReachX, jumpTriggerHeight, jumpCooldown).
- Da menu, per ora, mode default = TwoPlayers (i bottoni 1P/2P sono il prossimo step).

### Passo 5a — Comandi menu: modalita + difficolta ✅
- **MainMenuController cresce**: raccoglie mode (1P/2P) e difficolta (3 AIStats) e li
  scrive nel MatchConfig insieme ai 2 personaggi. Pura policy/comandi, nessuna logica di
  gioco: il motore (applicare difficolta, scegliere input P2) e gia nel Bootstrap (passo 4).
- **Riga difficolta condizionale**: visibile solo in 1P via SetActive(difficultyRow). In 2P
  cpuStats scritto null (asse bravura inesistente senza CPU).
- **Selezione "uno tra N" = pattern radio**: per ora con Button + tint manuale nel controller
  (Transition = None perche il tint automatico di Unity darebbe contro). Default: 2P, e
  Medium quando si entra in 1P. Alternativa futura: Toggle + ToggleGroup.
- **Layout UI**: Vertical Layout Group su MenuColumn (impila ModeRow/DifficultyRow/BtnGioca);
  Horizontal Layout Group su ciascuna riga (affianca i bottoni). Child Force Expand Width off.
  La gerarchia fa la parentela, il Layout Group fa la disposizione.
- Verifica: 2P -> riga difficolta sparisce, Gioca -> 2 umani; 1P -> riga appare (Medium
  evidenziato), scelta difficolta -> CPU corrispondente in partita.
  
### Passo 5b — Selezione personaggi guidata dai dati ✅
- **UI generata dai dati**: niente bottoni piazzati a mano. Un roster (lista di
  CharacterDefinition) per colonna -> il selector genera uno swatch per elemento. Aggiungere
  un personaggio = aggiungerlo al roster, la UI segue. Stessa filosofia dei ScriptableObject.
- **CharacterSwatch** (componente su prefab, UI): la singola opzione toccabile. Setup(def,
  callback) tinge l'Image col tint del personaggio (placeholder; domani Image.sprite =
  ritratto) e scrive displayName; al click invoca la callback. SetSelected(bool) per il mark.
- **CharacterSelector** (componente, una colonna P1/P2): roster + swatchPrefab +
  swatchContainer. In Start istanzia gli swatch, traccia Selected (default roster[0]),
  evidenzia il selezionato + preview opzionale. Due istanze (sinistra P1, destra P2),
  stesso roster.
- **MainMenuController**: legge player1Selector.Selected / player2Selector.Selected e li
  scrive nel MatchConfig (sostituiti i campi default serializzati).
- **Layout annidati (lezione UI)**: la catena Layout Element ("io figlio voglio essere
  grande X") -> Layout Group ("io contenitore dispongo i figli") -> Content Size Fitter
  ("io contenitore mi adatto ai figli"). Ogni livello annidato deve leggere l'altezza dei
  figli (Control Child Size Height) E dichiararla al genitore (Content Size Fitter), o la
  catena si spezza e gli elementi si sovrappongono. "Contenitore" = GameObject vuoto +
  RectTransform + Layout Group (NON un Panel: il Panel porta un'Image che ruba i click).
- Verifica: scegli P1 e P2 diversi -> Gioca -> in campo giocano proprio quei due.

### Passo 5c — Marcatore P1/P2 (nodo tint risolto) ✅
- **Due canali separati**: il tint resta IDENTITA del personaggio (sul corpo, da
  PlayerCharacter); il marcatore e DISTINZIONE tra giocatori (sopra la testa, da slot).
  Stesso personaggio in 2P -> stesso colore corpo (corretto) ma marcatori diversi ->
  distinguibili. Un'informazione = un canale.
- **PlayerMarker** (componente, Presentation): colora il SpriteRenderer del figlio Marker
  in base a PlayerIndex (P1/P2 color). Intrinseco allo SLOT (Player1 e sempre P1) ->
  settato in inspector + applicato in Awake, NON passa da MatchConfig/Bootstrap. Principio:
  cio che dipende dal config passa dal Bootstrap, cio che e fisso del player sta sul player.
- **Marcatore = figlio del player** (GameObject vuoto + SpriteRenderer), Sorting Layer
  Foreground (sopra il player), Scale (1,1,1). Stesso pattern "logica sul root, grafica
  sul figlio". PlayerMarker sul root, markerRenderer -> SpriteRenderer del figlio.
- **Sprite marcatore** (pixel art 16x16): corpo BIANCO + contorno SCURO. Il tinting
  moltiplica: bianco x colore = colore (corpo prende P1/P2 color), scuro x colore = scuro
  (contorno resta netto). Un solo sprite tintabile per entrambi.
- Verifica: stesso personaggio in 2P -> triangoli rosso/blu sopra le teste, distinguibili.

### Passo 6a — Stadi estetici (motore) ✅
- **StageDefinition** (SO, Content): lo stadio = il "vestito" del campo. v1 ESTETICO:
  background sprite + sand sprite. Strutturato a sezioni: identita / estetica / sezione
  gameplay COMMENTATA non implementata (geometria, rete, musica, physics material) -> porta
  aperta per il funzionale futuro senza ripensare l'asset.
- **Estetico vs funzionale**: lo stadio tocca SOLO gli SpriteRenderer (vestito), MAI il
  BoxCollider2D del Ground (geometria di gioco). Scoperta: il Ground ha sprite sabbia E
  collider sullo STESSO GameObject -> si scambia solo .sprite, collider intatto.
- **StageDresser** (componente, Content): Apply(stage) scambia background + sand sprite.
  Meccanismo; QUALE stadio lo decide il Bootstrap. Sand sprite dei nuovi stadi devono avere
  le stesse dimensioni (512x64) del corrente (il Ground ha scala X20 -> dimensioni diverse
  si deformerebbero).
- **MatchConfig** cresce: campo stage (era gia previsto nei commenti). null -> scena tiene
  gli sprite correnti.
- **GameplayBootstrap** applica lo stadio (config-o-fallback, campo fallbackStage). Stesso
  schema dei personaggi: contenuto iniettato.
- Verifica: boot-direct con fallbackStage = sprite attuali -> scena identica (prova del
  percorso). Secondo stadio con sprite diversi -> si vede lo scambio.

### Passo 6b-1 — Refactor: selettore di contenuto generico ✅ (refactor, no feature)
- **ISelectableContent** (interfaccia, Content): DisplayName + SwatchColor + Preview.
  Il minimo che una cella di selezione serve. CharacterDefinition la implementa (espone
  displayName/tint/portrait). StageDefinition la implementera in 6b-2.
- **ContentSwatch** (era CharacterSwatch): lavora su ISelectableContent, non sul tipo
  concreto. Mostra Preview se c'e, altrimenti SwatchColor (placeholder).
- **ContentSelector<T>** (base generica astratta, where T : ScriptableObject,
  ISelectableContent): tutta la logica (genera swatch dal roster, traccia/evidenzia la
  selezione, preview). Le sottoclassi fissano solo il tipo -> Unity serializza il roster
  tipizzato e Selected ritorna T. UNA logica, tanti tipi.
- **CharacterSelector** ora = ContentSelector<CharacterDefinition> { } (2 righe).
- **Perche generica e non interfaccia "spogliata"**: roster tipizzato in inspector +
  Selected tipizzato senza cast. Astrazione leggera, non over-engineering.
- Refactor INVISIBILE dall'esterno: selezione personaggi identica a prima. Commit separato
  dalle feature.
- Migrazione editor: prefab swatch ricreato con ContentSwatch; selector ri-cablati
  (swatchPrefab). SelectedMark (cornice/brackets) aggiunto come visual di selezione.

### PROSSIMO PASSO (6b-2)
Feature: StageSelector = ContentSelector<StageDefinition> { } (2 righe). StageDefinition
implementa ISelectableContent (+ swatchColor per il chip menu). Colonna stadi nel menu,
MainMenuController scrive config.stage. Riusa tutto il selettore generico appena fatto.

### NODO APERTO (da risolvere alla selezione personaggio)
- **Tint identita vs distinzione**: oggi il tint viene dal CharacterDefinition. In 2P
  stesso personaggio = stesso colore. Da risolvere alla schermata di selezione (es.
  tint-giocatore sovrapposto, contorno, o blocco del duplicato).

### Ordine pianificato Fase 3
1. CharacterDefinition + iniezione ✅
2. MatchConfig + MatchSession ✅
3. Scena menu + caricamento scene ✅
4. Modalita 1P/2P + difficolta CPU ✅
1-5c ✅ (personaggi, modalita, difficolta, selezione, marcatore)
6a. Stadi estetici (motore) ✅
6b. Scelta stadio nel menu  ← prossimo
7. Modalita torneo
8. Salvataggi (PER ULTIMI)

---

## Fase 2 — rifiniture RIMANDATE (non ancora fatte)
- **Rete vestita**: oggi barra bianca (placeholder con collider). Manca sprite pixel art
  (pali + maglia) dimensionato al collider.
- **Mira della CPU**: rimanda la palla ma non sceglie *dove* (no mira al campo scoperto).
- **Predizione CPU con rimbalzi sui muri**: ignora le carambole laterali (si auto-corregge
  dopo il rimbalzo, ma non anticipa).
- **Selettore input automatico** per piattaforma/modalita (ora si cambia a mano il
  componente IPlayerInput; un menu lo fara in Fase 3).
- **Polish mobile vero** (sconfina in Fase 5): build su device, performance, multi-aspect,
  layout/trasparenza pulsanti touch.

## Fase 3 — rifiniture RIMANDATE (non ancora fatte)
- **Reset punteggio dal menu**: il GameManager persiste col punteggio tra le scene. Prima
  partita dal menu = 0-0 ok. Quando ci sara "torna al menu e rigioca" servira ResetMatch()
  al punto giusto (metodo gia presente) per non trascinare il punteggio.
  
- **Estetica menu**: oggi e funzionale ma "grezzo da programmatore" (bottoni di sistema,
  niente titolo/tema/centratura). Rimandato di proposito a Fase 5 / dopo 5b: il menu
  cambiera comunque con ritratti e colonne di selezione -> si lucida tutto insieme, una volta.
  
- **Layout menu da rifinire**: funziona ma grezzo — ordine righe invertito (Gioca in cima
  invece che in fondo), spaziature/centratura da sistemare, catena Content Size Fitter
  SelectorsRow->Play da raffinare. Polish insieme all'estetica menu (Fase 5 / dopo i
  ritratti veri).

- **Cornice selezione (SelectedMark) da 9-slice**: oggi la 48x48 viene stirata e il bordo
  risulta spesso/sgranato. Impostare Border=6 nello Sprite Editor + Image Type=Sliced.
  Rinviato col polish menu.

---

## Decisioni di architettura — FASE 1 (Sessioni 3-5)
- GameManager: Singleton con DontDestroyOnLoad, root GameObject, Script Execution Order -100
- State Machine con transizioni validate esplicitamente (no transizioni "magiche")
- Comunicazione tra sistemi via event Action<T> (no riferimenti diretti)
- Sottoscrizione: OnEnable iscrive, OnDisable disiscrive, sempre
- Player NON e Singleton, NON ha riferimenti al GameManager (accoppiamento minimo)
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
- **Colpo direzionale** ("combinato"): punto di contatto + velocita del player.
  Contatto normalizzato su collider.bounds.extents.x (indipendente da sprite/scala).
  Ball legge la velocita del player da collision.rigidbody (no dipendenza da PlayerStats).
  aim in [-1,+1] -> angolo da verticale = aim * maxHitAngle; dir = (sin, cos) -> palla
  sempre verso l'alto. Il colpo sostituisce il rimbalzo fisico (rb.linearVelocity diretto);
  PhysicsMaterial2D solo per muri/terreno/rete. Parametri in BallStats.
- **BallSquashStretch**: pattern "fisica sul root, grafica su figlio Visual". Componente
  reattivo, anima localScale del figlio (snap + recover via coroutine + AnimationCurve).
  Squash ad assi fissi. Separato da ImpactFeedback (grafica locale della palla).
- **Separazione meccanismo / policy** per il feedback di scena:
  - CameraShake = meccanismo (Shake(intensity), offset decrescente in LateUpdate)
  - HitStop = meccanismo (Time.timeScale=0 per N secondi REALI; WaitForSecondsRealtime;
    OnDestroy ripristina timeScale=1). Squash/shake usano Time.deltaTime apposta.
  - Particelle: nessun meccanismo custom (il ParticleSystem E il meccanismo;
    Simulation Space=World, Play On Awake=OFF, burst singolo)
  - SfxPlayer = meccanismo (AudioSource + PlayOneShot; audio non influenzato da timeScale)
  - CameraShake/HitStop/SfxPlayer = singleton SCOPED SULLA SCENA (no DontDestroyOnLoad)
- **ImpactFeedback** = coordinatore/policy unico: si iscrive UNA volta agli eventi Ball,
  config raggruppata per evento (hit / ground), smista a tutti i meccanismi. Ha sostituito
  i tre trigger separati. Zero modifiche a Ball lungo tutto il game feel.

### Pixel art (Sessione 7)
- PPU di progetto = 25; risoluzione interna 480x270 (upscale x4 a 1080p); ortho size 5.4
  (270/2/25). Framing invariato. Pixel Perfect Camera sulla Main Camera (PPU 25, ref 480x270).
- Import sprite: Single, Point, Compression None, no MipMaps, PPU 25, Full Rect.
  Preset "PixelArtSprite" salvato e impostato come Default for Texture Importer.
- Sorting Layers: Default, Background, Midground, Gameplay, Ball, Foreground, FX, UI.
  Particelle -> layer FX via modulo Renderer del Particle System (NON SpriteRenderer).
- Pattern "fisica sul root, grafica su figlio Visual" anche per il giocatore.
- Pivot giocatore = Center. Sprite: palla 16x16; giocatore 24x48 tintabile (P1 rosso,
  P2 blu); sfondo 480x270; sabbia 512x64 (larghezza nativa). pixel = unita_mondo x PPU.

### Input, AI, animazione (Sessione 8)
- **IPlayerInput**: interfaccia "sorgente di intenzione" (Tick, Horizontal, JumpHeld,
  ConsumeJumpPressed). PlayerController la ottiene via GetComponent, non legge la tastiera.
  Meccanismo (movimento) / policy (input). Un solo componente IPlayerInput per giocatore.
- **KeyboardPlayerInput / AIPlayerInput / TouchPlayerInput**: stessa interfaccia.
- **AIPlayerInput** (cartella AI): insegue/prevede la palla; salta quando e sopra, entro
  hitReachX e jumpTriggerHeight; jumpCooldown anti-rimbalzo. Config in AIStats. Modalita
  1P = sostituire KeyboardPlayerInput con AIPlayerInput su Player2.
- **Predizione balistica**: risolve y(t)=altezza_intercetto (gravita effettiva =
  Physics2D.gravity.y * ballRb.gravityScale), poi X(t). Ricalcolata ogni frame ->
  si auto-corregge dopo ogni rimbalzo. Salto sulla posizione REALE.
- **Touch**: pulsanti UI a schermo. TouchButton (IPointerDown/Up -> IsPressed + edge)
  letto da TouchPlayerInput. Testabile in editor col mouse.
- **SpriteAnimator** (Presentation): cicla array di frame singoli per stato idle/walk/air,
  stato dedotto dalla velocita del Rigidbody2D del padre. flipX pronto per arte di profilo.

---

## Lezioni dagli intoppi (Fase 2)
- **Scala dei placeholder**: Player aveva Scale (5, -3.5) ereditata dai rettangoli ->
  pixel distorti, offset/Y invertiti, piedi nella sabbia, ground check fuori posto.
  Fix: normalizzare a (1,1,1); dimensione dai PIXEL a PPU 25, non dalla scala. Collider/offset
  SEMPRE in unita reali (Size x Scale). Normalizzare la scala PRIMA della pixel art.
- **Preset contaminato**: un Preset salva TUTTI i campi (Sprite Mode, ritagli). Crearlo da
  uno sprite Single mode pulito, altrimenti avvelena ogni import ("rect outside texture").
- **Reimport e riferimenti sprite**: cambiare Preset/PPU forza il reimport; se lo sprite
  cambia identita (Single<->Multiple) i SpriteRenderer perdono il riferimento e diventano
  invisibili. Dopo reimport massivi, verificare gli sprite assegnati.
- **Sabbia visiva vs collider**: il bordo superiore del collider del terreno deve coincidere
  con la superficie VISIBILE della sabbia.
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
*(idee emerse ma da NON implementare subito)*
- Variazione potenza del colpo in base alla velocita in ingresso (smash)
- Mira deliberata della CPU verso il campo scoperto
- Animazioni piu ricche (Aseprite, sprite di profilo con flip)
- Evoluzione personaggi: il CharacterDefinition resta TEMPLATE immutabile; la progressione
  (livello/modificatori) vivra nel layer profilo. Stat effettive = base + modificatori
  composte a runtime -> NON mutare mai l'asset condiviso (lo cambierebbe per tutti).

## Note utili
- Vecchio progetto (>2 anni fa, solo APK) NON recuperato: solo ispirazione visiva.
- Prototipo HTML5 `beach-volley.html` come riferimento di game design.
