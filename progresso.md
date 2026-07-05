# Beach Volley - Diario di progetto

> Questo file e il PONTE tra le chat: una chat per fase, lo si incolla in cima al
> primo messaggio di ogni chat nuova. A fine di ogni micro-passo: commit git +
> aggiornamento di questo file.

## Visione
Videogioco di beach volley per Android/iOS, pixel art retro, da pubblicare sugli store
(possibile monetizzazione). Modalita: 1 vs CPU e 2 giocatori locali. NIENTE online per v1.

## Stack
- Unity 6 (LTS), C#, Visual Studio Community (workload Unity)
- Git + GitHub (repo privato `beach-volley`, utente Winfrost)
- Windows, progetto in `C:\Dev\BeachVolley`. Test su Android fisico (USB). iOS a fine progetto.
- TextMeshPro, PhysicsMaterial2D, Particle System, 2D Pixel Perfect, IL2CPP

## Tre regole assolute
1. Niente magic numbers (ScriptableObject o costanti)
2. Un MonoBehaviour fa UNA cosa (~150 righe obiettivo)
3. Commit piccoli e descrittivi (refactor SEPARATI dalle feature)

## Principio guida (emerso in Fase 3)
**Confini puliti, non infrastruttura anticipata.** Costruire la cosa piccola che funziona,
con i layer separati, cosi il pezzo grosso si aggiunge AL BORDO senza riscrivere il cuore.
Online/pagamenti/evoluzione personaggi = lunghissimo periodo, NON si architettano ora.

## Stile di lavoro con Claude (preferenze confermate)
- Risposte in italiano, commenti nel codice in inglese.
- Claude da raccomandazioni architetturali DIRETTE (non liste neutre), ma conferma i bivi
  grossi prima di implementare. Micro-passi, ognuno con obiettivo verificabile, chiuso da
  commit + aggiornamento progresso.
- Claude fornisce sempre file INTERI quando li modifica (non frammenti).
- Commit mess": prima riga concisa; corpo opzionale.

---

# STATO ATTUALE: FASI 0-3 ✅ + RICOGNIZIONE MOBILE ✅ — in corso: Fase 4 (Audio)

## Fase 4 — Audio (in corso)
Ordine deciso: mixer (prerequisito) -> pass SFX -> musica+volume.
Volume regolabile per ULTIMO: governa il mix completo (SFX + musica), non è "roba della musica".

- [x] Passo 0: AudioMixer GameAudio (Master -> Music, SFX). Output dell'AudioSource di
  SfxPlayer (scena Gameplay, GameObject Audios) instradato sul gruppo SFX. Zero codice:
  PlayOneShot rispetta l'Output dell'AudioSource. Parametri volume NON esposti (-> Troncone B).

### Troncone A — pass SFX (quasi chiuso)
Già sonorizzati: colpo player (hitSound), punto a terra (pointSound).
- [x] Rete: ImpactFeedback -> Ball.OnNetTouched -> netSound.
- [x] Salto: PlayerController.OnJumped -> PlayerFeedback -> jumpSound (uno per player).
- [x] Fine match: MatchFeedback (scene-scoped) -> GameManager.OnMatchEnded -> matchEndSound.
  NEUTRO per scelta (fischio finale). Vittoria/sconfitta = estensione futura al bordo
  (GameplayBootstrap inietta il lato umano), NON riscrittura.
Famiglia feedback completa: palla->ImpactFeedback, player->PlayerFeedback, match->MatchFeedback.
Resta (opz.): tap UI/menu -> probabile Fase 5 (polish).  
  

### Troncone B — musica chiptune + volume (in corso)
Roadmap: B1 MusicPlayer -> B2 parametri mixer+VolumeController (mappatura log, 0=-80dB)
-> B3 SaveData volumi + applicazione all'avvio -> B4 slider UI.
- [x] B1: MusicPlayer meccanismo persistente (root + DontDestroyOnLoad), traccia in loop
  sul gruppo Music. Meccanismo puro (COSA suona, non QUANTO forte). Dedup singleton ->
  doppione in più scene sicuro, musica ininterrotta tra menu e gameplay. Placeholder
  bgm_loop 8s sintetizzato. Una traccia per tutte le scene in v1 (per-stage = futuro al bordo).
- [x] B2: parametri Music/SFX esposti sul mixer come MusicVol/SfxVol. VolumeController
  (autorità unica del volume, singleton di scena) mappa lineare 0..1 -> dB con log10*20,
  0 forzato a -80 dB (silenzio). Applica ai param esposti; NON decide i valori (solo il COME).
  Hook OnValidate temporaneo (solo editor) per provare la curva live. Prossimo: B3 (SaveData).

## Piano per fasi
- Fase 0 Setup ✅ | Fase 1 Prototipo ✅ | Fase 2 Game feel+arte+AI+input ✅
- **Fase 3 Contenuti ✅ (APPENA CHIUSA)**
- Fase 4 Audio (musica chiptune, pass SFX completo)
- Fase 5 Mobile e polish (build su device, performance, multi-schermo)
- Fase 6 Pubblicazione

# DECISIONE PRESA AL CONFINE: ANTICIPATO GIRO DI BUILD ANDROID (pezzo di Fase 5)

Prima della Fase 4 (audio) si fa una RICOGNIZIONE su device Android, timeboxed e finibile.
Obiettivo: SCOPRIRE le sorprese mobile (touch, performance IL2CPP/ARM, schermo), non
rifinirle. Ogni sorpresa si ANNOTA nel backlog di Fase 5, non si risolve adesso.
- Dentro scope: build IL2CPP che parte sul device, partita 1vsCPU giocabile col touch,
  framerate osservato, check schermo (aspect ratio, bande nere, UI tagliata).
- Fuori scope: polish menu, multi-schermo esaustivo, ottimizzazione fine, iOS, 2P touch.
Dopo la ricognizione -> Fase 4 (audio) con la conoscenza del comportamento reale del device.

## Avanzamento ricognizione
- [x] Passo 1: selezione input umano per piattaforma. Player1 ospita Keyboard + Touch;
  GameplayBootstrap sceglie via InputPlatformMode (Auto/ForceTouch/ForceKeyboard) e mostra
  il contenitore TouchControls solo quando il touch e attivo. Testabile in editor col mouse.
- [ ] Passo 2: prima build IL2CPP su device.

- [x] Passo 2: prima build IL2CPP su device. Switch piattaforma Android, package
  com.winfrost.beachvolley, orientamento Landscape, dev build firma debug. Il gioco
  parte sul telefono e si gioca 1vsCPU col touch.

## Sorprese mobile emerse (backlog Fase 5)
- (da compilare giocando: touch / performance / schermo / UI)

- Nessuna sorpresa BLOCCANTE sul device di test: touch reattivo, performance ok,
  schermo/UI corretti. I rischi temuti (camera su aspect non-16:9, perf IL2CPP) non
  si sono presentati.
- Resta Fase 5 (non bloccante): validazione multi-device, safe-area/notch, iOS,
  visibilita bottoni touch gia gestita, polish menu.


---

# ARCHITETTURA — modello mentale (leggere prima di tutto)

**Il cuore: un match e configurato da DATI che arrivano da fuori la scena.**
- **MatchConfig** (data class, Content): la FORMA di un match (chi vs chi, modalita,
  difficolta CPU, stadio). Stato di SESSIONE, non tuning -> plain class, non SO.
- **MatchSession** (statico, Content): TRASPORTA un MatchConfig pendente tra scene. Unico
  seam per passare dati attraverso un cambio scena. Auto-reset a ogni Play.
- **GameplayBootstrap** (Core): COMPOSITION ROOT. L'unico autorizzato a dipendere da tutti
  i layer (il suo mestiere e assemblare). In Start legge MatchSession.Pending (o i campi
  fallback serializzati per il test "boot dritto in Gameplay") e applica: personaggi, input
  di P2 (umano/CPU), difficolta, stadio. Poi avvia il match.
- **Forma vs trasporto vs applicazione** separati: cambiare CHI riempie il config (menu,
  torneo) non tocca ne la forma ne il codice che la legge/applica.

**Iniezione per RIFERIMENTO, non per valore.** Si passa QUALE asset usare (PlayerStats,
AIStats, sprite), non si copiano i valori. I consumer leggono DAL VIVO -> lo swap vale subito.
Vale per PlayerController.SetStats, .SetInput, AIPlayerInput.SetStats, StageDresser.

**Layer e dipendenze (una direzione sola):** Content -> Gameplay/AI -> Core. Presentation
e UI stanno sopra e possono dipendere dai layer sotto. Unica eccezione: GameplayBootstrap.

**Due assi indipendenti del personaggio:**
- FORMA FISICA = PlayerStats (come si muove il corpo). Del personaggio, mai toccata dalla difficolta.
- BRAVURA = AIStats (quanto bene la CPU guida quel corpo). E la difficolta. Solo in 1P.

**Stato di sessione vs dati salvati:** Match/TournamentSession si AZZERANO a ogni Play
(RuntimeInitializeOnLoadMethod). I salvataggi PERSISTONO su disco. Due memorie diverse,
di proposito. Si salva solo cio che e del giocatore e cambia; il contenuto e nel build.

**Un'informazione = un canale:** il tint del personaggio = IDENTITA; il marcatore sopra la
testa = DISTINZIONE P1/P2. Tenuti separati cosi stesso personaggio in 2P resta distinguibile.

**Cio che dipende dal config passa dal Bootstrap; cio che e fisso dello slot sta sul player**
(es. PlayerMarker: Player1 e sempre P1 -> settato in inspector, applicato in Awake).

---

# STRUTTURA DEL PROGETTO

```
Assets/_Project/
  Scenes/   MainMenu.unity, Gameplay.unity   (entrambe in Build Settings, MainMenu indice 0)
  Settings/ *_Default.asset, PixelArtSprite.preset, BallBouncy.physicsMaterial2D
    Characters/  Character_X.asset + PlayerStats_X.asset (uno per personaggio)
    Difficulty/  AIStats_Easy/Medium/Hard.asset
    Stages/      Stage_Spiaggia.asset, Stage_Palazzetto.asset
  Sprites/  Ball/ Characters/ Environment/ UI/   (bg_*, sand_strip, parquet_strip, marker, frame)
  Scripts/
    AI/           AIPlayerInput, AIStats
    Content/      CharacterDefinition, PlayerCharacter, MatchConfig, MatchSession,
                  StageDefinition, StageDresser, ISelectableContent,
                  TournamentRun, TournamentSession
    Core/         GameManager, GameplayBootstrap, GameState, GameStateDebugListener,
                  MatchController, SceneNames, SaveData, SaveSystem
    Gameplay/     Ball, BallStats, PlayerController, PlayerStats, IPlayerInput,
                  KeyboardPlayerInput, TouchButton, TouchPlayerInput
    Presentation/ BallSquashStretch, CameraShake, HitStop, ImpactFeedback, SfxPlayer,
                  SpriteAnimator, PlayerMarker
    UI/           HUDController, WinScreenController, MainMenuController, TournamentLauncher,
                  TournamentFlow, ContentSwatch, ContentSelector<T>, CharacterSelector,
                  StageSelector, RecordDisplay
```
Namespace allineati alle cartelle: BeachVolley.Core/.Gameplay/.AI/.Content/.Presentation/.UI
Simmetria contenuto: Scripts/* (codice) <-> Settings/* (dati) <-> Sprites/* (arte).

**Gerarchia Gameplay:** _Bootstrap, Main Camera, Environment (Background, LeftWall,
RightWall, NetPlaceholder, Ground[sprite sabbia+collider sullo STESSO GameObject]),
Gameplay (Player1[+Visual+Marker], Player2[+Visual+Marker], Ball+Visual), UI (Canvas:
ScoreText, WinPanel, TournamentPanel, Btn touch), EventSystem, FX, Audios, StageDresser.

**Gerarchia MainMenu (Canvas):** MenuColumn (Vertical LG) > ModeRow, DifficultyRow,
SelectorsRow (SelectorP1/P2 con SwatchContainer), StageRow, Play/BtnTorneo. MenuManager
ha MainMenuController + TournamentLauncher + RecordDisplay. GameManager a RADICE.

---

# COMPONENTI CHIAVE FASE 3 (cosa fa cosa)

**Contenuto iniettabile**
- CharacterDefinition (SO): identita (displayName, portrait, tint) + PlayerStats + AIStats.
  CONTIENE le stat per riferimento, non le duplica. Implementa ISelectableContent.
- PlayerCharacter (su ogni player): Apply(def) -> spinge PlayerStats sul controller + tint
  sullo SpriteRenderer figlio Visual.
- StageDefinition (SO): identita + estetica (backgroundSprite, sandSprite). Sezione gameplay
  COMMENTATA (geometria/rete/musica) = porta aperta al futuro funzionale. Implementa ISelectableContent.
- StageDresser (scena): Apply(stage) -> scambia SOLO gli sprite di Background e Ground.
  MAI il BoxCollider2D del Ground (geometria). Sprite pavimento devono essere 512x64 (Ground scala X20).

**Menu e selezione (UI generata dai dati)**
- ISelectableContent: interfaccia (DisplayName, SwatchColor, Preview) implementata da
  CharacterDefinition e StageDefinition.
- ContentSwatch (prefab): una cella toccabile. Mostra Preview se c'e, altrimenti SwatchColor.
- ContentSelector<T> (base generica astratta, where T: ScriptableObject, ISelectableContent):
  genera uno swatch per ogni elemento del roster, traccia/evidenzia Selected. UNA logica.
  Sottoclassi di 2 righe: CharacterSelector, StageSelector. (Roster tipizzato serializzabile.)
- MainMenuController: raccoglie personaggi (selector P1/P2), modalita, difficolta, stadio ->
  scrive MatchConfig via MatchSession -> carica Gameplay. Riga difficolta visibile solo in 1P.

**Difficolta CPU**
- AIStats_Easy/Medium/Hard: stessi campi, tarati. Leva forte = usePrediction (off su Easy).
- La difficolta SOSTITUISCE l'aiStats del personaggio (config-or-fallback: cpuStats null ->
  usa quello del personaggio). Applicata in Bootstrap via AIPlayerInput.SetStats.
- Player2 ospita ENTRAMBI KeyboardPlayerInput + AIPlayerInput; il Bootstrap ne attiva uno
  via PlayerController.SetInput in base a mode.

**Distinzione giocatori**
- PlayerMarker (su ogni player): colora lo SpriteRenderer del figlio Marker per slot (P1
  rosso / P2 blu). Sprite 16x16 corpo bianco + contorno scuro (tinting moltiplica: corpo
  prende il colore, contorno resta scuro). Sorting Layer Foreground.

**Torneo (gauntlet)**
- TournamentRun (Content): stato corsa (player + coda avversari shuffle + CurrentIndex).
  BuildCurrentMatchConfig() (difficolta crescente: ultimo match il piu duro; stadio ciclato).
  Advance(). Pura logica.
- TournamentSession (statico): trasporta la run tra scene. Fratello di MatchSession.
- TournamentLauncher (UI): bottone Torneo -> avversari = cast UNIVOCO meno il giocatore
  (servono >=3 personaggi per avanzamento) -> costruisce la run, lancia match 1.
- TournamentFlow (UI, scena Gameplay): ascolta OnMatchEnded; se torneo attivo ramifica
  (vinto->avanza/CAMPIONE, perso->ELIMINATO). Continua = ri-arma GameManager a MainMenu +
  ResetMatch + ricarica Gameplay col prossimo config. WinScreenController fa la GUARDIA
  (in torneo non interviene). Registra il risultato nel salvataggio.

**Salvataggi**
- SaveData (Core): schema esplicito (tournamentsWon, longestStreak; commenti per settings/unlock futuri).
- SaveSystem (Core, statico): JSON in Application.persistentDataPath. Load (cache, mai null) / Save.
- RecordDisplay (UI): mostra il record nel menu all'avvio (prova della persistenza).

---

# RIFINITURE / DEBITI RIMANDATI

**Fase 2 (gameplay/arte)**
- Rete vestita: oggi barra bianca con collider; manca sprite (pali+maglia) sul collider.
- Mira CPU: rimanda la palla ma non sceglie DOVE (no mira al campo scoperto).
- Predizione CPU coi rimbalzi sui muri: ignora le carambole laterali (si corregge dopo).

**Fase 3 (menu/UI) — quasi tutto da fare nel polish (Fase 5)**
- Estetica menu grezza: bottoni di sistema, niente titolo/tema/centratura.
- Layout menu: ordine righe da sistemare, spaziature/centratura, catena Content Size Fitter
  SelectorsRow->Play da raffinare.
- Cornice selezione (SelectedMark): impostare Border=6 nello Sprite Editor + Image Type=Sliced
  (oggi la 48x48 e stirata, bordo sgranato).
- Controlli irrilevanti in modalita torneo (colonna P2, difficolta/stadio manuali) visibili
  ma ignorati premendo Torneo -> nasconderli in modalita torneo.
- Rinominare StageDefinition.sandSprite -> floorSprite (ora e parquet/sabbia: nome generico).

**Risolti in Fase 3** (per memoria): reset punteggio tra match (ResetMatch in TournamentFlow.
OnContinue); selettore input automatico (Bootstrap sceglie Keyboard/AI); nodo tint (marcatore).

- [RISOLTO passo 1 ricognizione] Selettore input automatico per piattaforma + visibilita
  bottoni touch (era: P1 sempre tastiera, bottoni sempre visibili). Ora P1 = touch su mobile
  / tastiera su desktop, e i bottoni compaiono solo in touch. Override per test in editor.

---

# DECISIONI DI ARCHITETTURA — FASI 1-2 (riferimento)

**Fase 1**
- GameManager: Singleton, DontDestroyOnLoad, root GameObject, Script Execution Order -100.
- State Machine con transizioni validate esplicitamente. Stati: Boot, MainMenu, ModeSelect,
  Loading, Playing, Paused, PointScored, MatchEnd.
- Comunicazione via event Action<T> (OnEnable iscrive / OnDisable disiscrive, sempre).
- Player NON Singleton, NESSUN riferimento al GameManager (accoppiamento minimo).
- Movimento: velocity diretta (arcade); salto: impulso + variable jump height + gravity
  multipliers; ground check via Physics2D.OverlapCircle.
- Config in ScriptableObject (PlayerStats, BallStats). Ball usa eventi, no logica punteggio.
  Lato campo dalla X rispetto alla rete (X=0). Punteggio nel GameManager; MatchController = ponte.
- Active Input Handling = "Both".

**Fase 2 — Game feel**
- Colpo direzionale ("combinato"): contatto (normalizzato su collider.bounds.extents.x) +
  velocita del player (letta da collision.rigidbody). aim[-1,+1] -> angolo = aim*maxHitAngle,
  palla sempre verso l'alto. Il colpo SOSTITUISCE il rimbalzo fisico; PhysicsMaterial2D solo
  per muri/terreno/rete. Parametri in BallStats.
- Pattern "fisica sul root, grafica su figlio Visual" (palla e player).
- Meccanismo/policy: CameraShake, HitStop (Time.timeScale=0 reale; WaitForSecondsRealtime;
  OnDestroy ripristina), SfxPlayer = meccanismi; ImpactFeedback = policy unica che si iscrive
  agli eventi Ball e smista. Particelle: il ParticleSystem E il meccanismo (World, burst).
  Questi singleton sono SCOPED SULLA SCENA (no DontDestroyOnLoad).

**Fase 2 — Pixel art**
- PPU 25; interna 480x270 (x4 a 1080p); ortho size 5.4. Pixel Perfect Camera (PPU 25, ref 480x270).
- Import: Single, Point, No Compression, no MipMaps, PPU 25, Full Rect. Preset PixelArtSprite.
- Sorting Layers: Default, Background, Midground, Gameplay, Ball, Foreground, FX, UI.
  Particle Sorting Layer nel modulo Renderer (NON SpriteRenderer).
- Pivot giocatore Center. Sprite: palla 16x16; player 24x48 tintabile; sfondo 480x270; sabbia 512x64.
  pixel = unita_mondo x PPU.

**Fase 2 — Input/AI/animazione**
- IPlayerInput (Tick, Horizontal, JumpHeld, ConsumeJumpPressed). PlayerController lo ottiene
  via GetComponent. Implementazioni: Keyboard / AI / Touch.
- AIPlayerInput: insegue/prevede la palla; salta entro hitReachX e jumpTriggerHeight;
  jumpCooldown. Predizione balistica risolve y(t)=altezza_intercetto (g effettiva =
  Physics2D.gravity.y * ballRb.gravityScale) poi X(t); ricalcolata ogni frame (si autocorregge).
- Touch: TouchButton (IPointerDown/Up) letto da TouchPlayerInput. Testabile col mouse.
- SpriteAnimator: cicla frame singoli per stato (idle/walk/air) dalla velocita del Rigidbody2D.

---

# LEZIONI DAGLI INTOPPI (da non ripetere)

**Codice**
- Nome file = nome classe per i MonoBehaviour. Un errore di compilazione in QUALSIASI script
  blocca l'aggiunta di TUTTI i componenti custom e li fa sparire dai menu OnClick (Console!).
- Asset SO condiviso = stesso oggetto in memoria (non un template): condividerlo tra due
  entita le rende identiche -> un asset per entita se servono valori diversi.

**Pixel art**
- Normalizzare la scala a (1,1,1) PRIMA della pixel art; dimensione dai PIXEL a PPU 25, non
  dalla scala. Collider/offset in unita reali (Size x Scale).
- Preset salva TUTTI i campi (incl. Sprite Mode): crearlo da uno sprite Single pulito.
- Reimport puo far perdere i riferimenti SpriteRenderer: verificarli dopo reimport massivi.
- Bordo superiore collider terreno = superficie VISIBILE della sabbia.

**UI Unity (Fase 3, sudata)**
- Catena layout annidati: Layout Element ("io figlio sono grande X") -> Layout Group ("io
  dispongo i figli") -> Content Size Fitter ("io mi adatto ai figli"). Ogni livello deve
  LEGGERE l'altezza dei figli (Control Child Size Height) E DICHIARARLA al genitore (Content
  Size Fitter), o la catena si spezza e gli elementi si sovrappongono.
- "Contenitore" = GameObject vuoto + RectTransform + Layout Group (NON un Panel: il Panel
  porta un'Image che ruba i click e mette uno sfondo).
- La UI non ha profondita: l'ordine di disegno segue l'ordine nella Hierarchy.
- Selezione "uno tra N": Button + tint manuale richiede Transition = None (o il tint
  automatico di Unity sovrascrive). In alternativa Toggle + ToggleGroup.
- Cambiare il tipo di un componente referenziato rompe i riferimenti serializzati (vanno
  ri-cablati); ricreare un prefab/script da zero idem.

---

# COSTANTI TECNICHE CHIAVE
- Pixel art: PPU 25; interna 480x270 (x4 a 1080p); ortho size 5.4. Pixel Perfect Camera (PPU 25, ref 480x270).
- Player: Position Y -3.55, Scale (1,1,1), CapsuleCollider2D Size (0.9,1.9) Offset (0,0). groundCheckOffsetY -0.95.
- Ground: Position Y -5, Scale (20,1,1), BoxCollider2D Size (1,1) -> bordo superiore a -4.5.
- Sabbia/pavimento: sprite 512x64, Position Y -5.78 (top a -4.5), Scale (1,1,1), layer Midground.
- Sfondo: sprite 480x270, Position (0,0,0), Scale (1,1,1), layer Background.
- Canvas UI: Scale With Screen Size, ref 1920x1080.
- Build: IL2CPP, ARMv7 + ARM64, Minimum API Android 7.0. GameManager Script Execution Order -100.
- Salvataggio: JSON in Application.persistentDataPath/save.json (cancellabile per reset record).

# BACKLOG IDEE FUTURE (NON implementare ora)
- Variazione potenza del colpo in base alla velocita in ingresso (smash).
- Mira deliberata della CPU verso il campo scoperto.
- Animazioni piu ricche (Aseprite, sprite di profilo con flip).
- Stadi FUNZIONALI (geometria campo, altezza rete, physics material) - sezione gia prevista
  e commentata in StageDefinition.
- Evoluzione personaggi: CharacterDefinition resta TEMPLATE immutabile; la progressione vive
  in un layer profilo. Stat effettive = base + modificatori composte a RUNTIME -> NON mutare
  mai l'asset condiviso (lo cambierebbe per tutti).
- Musica chiptune + volume regolabile -> a quel punto SaveData guadagna i campi settings.

# NOTE
- Vecchio progetto (>2 anni fa, solo APK) NON recuperato: solo ispirazione visiva.
- Prototipo HTML5 beach-volley.html come riferimento di game design.
