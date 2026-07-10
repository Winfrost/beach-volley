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
- TextMeshPro, PhysicsMaterial2D, Particle System, 2D Pixel Perfect, IL2CPP, AudioMixer

## Tre regole assolute
1. Niente magic numbers (ScriptableObject o costanti)
2. Un MonoBehaviour fa UNA cosa (~150 righe obiettivo)
3. Commit piccoli e descrittivi (refactor SEPARATI dalle feature)

## Principio guida
**Confini puliti, non infrastruttura anticipata.** Costruire la cosa piccola che funziona,
con i layer separati, cosi il pezzo grosso si aggiunge AL BORDO senza riscrivere il cuore.
Online/pagamenti/evoluzione personaggi/musica-per-stage = lunghissimo periodo, NON si architettano ora.

## Stile di lavoro con Claude (preferenze confermate)
- Risposte in italiano, commenti nel codice in inglese.
- Claude da raccomandazioni architetturali DIRETTE (non liste neutre), ma conferma i bivi
  grossi prima di implementare. Micro-passi, ognuno con obiettivo verificabile, chiuso da
  commit + aggiornamento progresso.
- Claude fornisce sempre file INTERI quando li modifica (non frammenti), come file da scaricare.
- I messaggi di commit e gli aggiornamenti di progresso.md stanno in chat (non nei file).
- Commit mess: prima riga concisa; corpo opzionale.
- Placeholder audio generati proceduralmente (Python/numpy WAV) per sbloccare l'implementazione
  prima di avere gli asset veri.

---

# STATO ATTUALE: FASI 0-3 + RICOGNIZIONE MOBILE + FASE 4 AUDIO — prossima: Fase 5 (Mobile e polish)

Il gioco e contenutisticamente completo end-to-end e ora anche SONORO:
- 2 giocatori locali (tastiera) e 1 vs CPU (predizione balistica). Touch predisposto.
- Game feel completo (colpo direzionale, squash&stretch, shake, hit-stop, particelle, SFX).
- Pixel art pixel-perfect (PPU 25, 480x270 upscale x4).
- Menu: scegli personaggi (P1/P2), modalita (1P/2P), difficolta (3 livelli), stadio, VOLUMI.
- Torneo gauntlet: scala a difficolta crescente, stadi variabili, campione/eliminato.
- Salvataggi: record torneo + volumi (JSON su disco).
- AUDIO: pass SFX di gioco completo + musica chiptune in loop + volume musica/SFX regolabile e persistente.
- Gira su device Android reale (build IL2CPP), 1vsCPU col touch. Selezione input per piattaforma.

## Piano per fasi
- Fase 0 Setup | Fase 1 Prototipo | Fase 2 Game feel+arte+AI+input
- Fase 3 Contenuti
- **Fase 4 Audio (APPENA CHIUSA)**
- Fase 5 Mobile e polish (2P touch, tap UI, safe-area/notch, multi-schermo, performance, estetica menu)
- Fase 6 Pubblicazione (incl. pass iOS)

## Ricognizione mobile (fatta, pezzo anticipato di Fase 5)
- Selezione input umano per piattaforma: Player1 ospita Keyboard + Touch; GameplayBootstrap
  sceglie via InputPlatformMode (Auto/ForceTouch/ForceKeyboard), TouchControls visibili solo in touch.
- Prima build IL2CPP su device: package com.winfrost.beachvolley, Landscape, dev build.
  Parte e si gioca 1vsCPU col touch.
- Nessuna sorpresa BLOCCANTE: touch reattivo, performance ok, schermo/UI corretti.
  I rischi temuti (camera su aspect non-16:9, perf IL2CPP) non si sono presentati.
- Resta a Fase 5 (non bloccante): multi-device, safe-area/notch, iOS, polish menu.

Fase 5 · Safe-area: SafeAreaFitter (Screen.safeArea → anchor normalizzati, indipendente dal Canvas Scaler; Play-mode only). Applicato al MainMenu avvolgendo MenuColumn in un container SafeArea. Gameplay al passo successivo.
Fase 5 · Safe-area (Gameplay): container SafeArea avvolge ScoreText + TouchControls; WinPanel/TournamentPanel restano figli diretti del Canvas (sfondo fine-match edge-to-edge, contenuto centrato già sicuro). Safe-area chiusa su entrambe le scene.
Backlog / molto dopo — 2P touch: rimandato (caso d'uso raro su mobile; il layer input è già astratto, si aggiunge al bordo senza debito).
Debiti Fase 5 da non dimenticare — nascondere il bottone "2 Giocatori" sulle piattaforme touch (oggi P2 su mobile non ha input → match ingiocabile). Stesso pattern della riga difficoltà (visibile solo in 1P). Va chiuso prima della pubblicazione; naturale collocarlo nell'estetica/layout menu.
Fase 5 · Tap UI SFX: UIClickFeedback (Presentation) auto-aggancia il click a tutti i Button statici del Canvas via SfxPlayer → rispetta lo slider SFX. Aggiunto SfxPlayer al MainMenu. Swatch runtime e TouchButton esclusi per scelta. Placeholder ui_click.wav sintetizzato. Debito tap UI di Fase 4 chiuso.
Scope v1: SOLO single player. 2P locale (stesso telefono) e multiplayer online → entrambi rimandati e da valutare dopo il completamento del single-player. Nessun debito: input già astratto (IPlayerInput) per il 2P locale al bordo; l'online è un layer esterno, non si architetta ora.
Menu (in estetica/layout): togliere la riga modalità ModeRow_G (o nascondere il 2P) — in v1 c'è solo 1P. Regola di visibilità, si chiude nello step estetica.
Fase 5 · Menu (deterministico): rimossa ModeRow_G (1P/2P) — v1 solo single-player. MainMenuController costruisce sempre OnePlayerVsCPU, difficoltà sempre visibile. MatchMode.TwoPlayers resta in Core per il 2P locale futuro. Torneo invariato.
Appeso a step B (layout/flusso): nascondere P2/difficoltà/stadio in intento-torneo richiede uno stato di modalità nel menu (bivio UX: due bottoni-lancio vs "scegli modalità → configura → avvia"). Da decidere lì.
Fase 5 · Step B — font: adottato Pixel Operator (CC0) come famiglia unica (Bold=titolo, Regular=corpo). TMP asset in RASTER + Filter Point, taglie a multipli interi della nativa. Palette scelta: "caldo spiaggia" (da applicare allo skin).
---

# ARCHITETTURA — modello mentale (leggere prima di tutto)

**Il cuore: un match e configurato da DATI che arrivano da fuori la scena.**
- **MatchConfig** (data class, Content): la FORMA di un match (chi vs chi, modalita,
  difficolta CPU, stadio). Stato di SESSIONE, non tuning -> plain class, non SO.
- **MatchSession** (statico, Content): TRASPORTA un MatchConfig pendente tra scene. Auto-reset a ogni Play.
- **GameplayBootstrap** (Core): COMPOSITION ROOT. L'unico autorizzato a dipendere da tutti
  i layer. In Start legge MatchSession.Pending (o i fallback serializzati per il boot dritto in
  Gameplay) e applica: personaggi, input di P2 (umano/CPU), difficolta, stadio. Poi avvia il match.

**Iniezione per RIFERIMENTO, non per valore.** Si passa QUALE asset usare, non si copiano i
valori. I consumer leggono DAL VIVO -> lo swap vale subito.

**Layer e dipendenze (una direzione sola):** Content -> Gameplay/AI -> Core. Presentation
e UI stanno sopra e possono dipendere dai layer sotto. Unica eccezione: GameplayBootstrap.

**Due assi indipendenti del personaggio:**
- FORMA FISICA = PlayerStats (come si muove il corpo). Del personaggio, mai toccata dalla difficolta.
- BRAVURA = AIStats (quanto bene la CPU guida quel corpo). E la difficolta. Solo in 1P.

**Stato di sessione vs dati salvati:** Match/TournamentSession si AZZERANO a ogni Play.
I salvataggi PERSISTONO su disco. Due memorie diverse, di proposito.

**Un'informazione = un canale:** il tint del personaggio = IDENTITA; il marcatore sopra la
testa = DISTINZIONE P1/P2. Tenuti separati.

**Annuncia vs reagisce (pattern feedback, consolidato in Fase 2 e Fase 4):** il layer di
GAMEPLAY annuncia eventi e NON possiede feedback; il layer di PRESENTATION reagisce. Vedi la
famiglia *Feedback sotto. Ball/PlayerController/GameManager annunciano; Impact/Player/MatchFeedback reagiscono.

---

# STRUTTURA DEL PROGETTO

```
Assets/_Project/
  Scenes/   MainMenu.unity, Gameplay.unity   (entrambe in Build Settings, MainMenu indice 0)
  Settings/ *_Default.asset, PixelArtSprite.preset, BallBouncy.physicsMaterial2D
    Characters/  Character_X.asset + PlayerStats_X.asset (uno per personaggio)
    Difficulty/  AIStats_Easy/Medium/Hard.asset
    Stages/      Stage_Spiaggia.asset, Stage_Palazzetto.asset
    Audio/       GameAudio.mixer (Master -> Music, SFX)
  Audio/    SFX/ (net/jump/whistle...) Music/ (bgm_loop...)   [placeholder sintetizzati]
  Sprites/  Ball/ Characters/ Environment/ UI/
  Scripts/
    AI/           AIPlayerInput, AIStats
    Content/      CharacterDefinition, PlayerCharacter, MatchConfig, MatchSession,
                  StageDefinition, StageDresser, ISelectableContent,
                  TournamentRun, TournamentSession
    Core/         GameManager, GameplayBootstrap, GameState, GameStateDebugListener,
                  MatchController, SceneNames, SaveData, SaveSystem
    Gameplay/     Ball, BallStats, PlayerController, PlayerStats, IPlayerInput,
                  KeyboardPlayerInput, TouchButton, TouchPlayerInput
    Presentation/ BallSquashStretch, CameraShake, HitStop, SpriteAnimator, PlayerMarker,
                  ImpactFeedback, PlayerFeedback, MatchFeedback,          (famiglia feedback)
                  SfxPlayer, MusicPlayer, VolumeController, VolumeSettings (audio)
    UI/           HUDController, WinScreenController, MainMenuController, TournamentLauncher,
                  TournamentFlow, ContentSwatch, ContentSelector<T>, CharacterSelector,
                  StageSelector, RecordDisplay, AudioSettingsUI
```
Namespace allineati alle cartelle: BeachVolley.Core/.Gameplay/.AI/.Content/.Presentation/.UI

**Gerarchia Gameplay:** _Bootstrap, Main Camera, Environment (Background, LeftWall,
RightWall, NetPlaceholder[tag Net], Ground[sprite sabbia+collider stesso GameObject]),
Gameplay (Player1[+Visual+Marker+PlayerFeedback], Player2[idem], Ball+Visual), UI (Canvas),
EventSystem, FX, Audios (SfxPlayer+AudioSource output->SFX; ImpactFeedback; MatchFeedback),
StageDresser. (Opz.) MusicPlayer per il boot diretto.

**Gerarchia MainMenu (Canvas):** MenuColumn (Vertical LG) > ModeRow, DifficultyRow,
SelectorsRow, StageRow, MusicSlider, SfxSlider, Play/BtnTorneo. MenuManager ha
MainMenuController + TournamentLauncher + RecordDisplay + VolumeController + VolumeSettings +
AudioSettingsUI. GameManager a RADICE. MusicPlayer (root, persistente).

---

# COMPONENTI CHIAVE FASE 3 (cosa fa cosa)

**Contenuto iniettabile**
- CharacterDefinition (SO): identita + PlayerStats + AIStats per riferimento. ISelectableContent.
- PlayerCharacter (su ogni player): Apply(def) -> PlayerStats sul controller + tint sul Visual.
- StageDefinition (SO): identita + estetica (backgroundSprite, sandSprite). Sezione gameplay
  commentata (geometria/rete/musica) = porta aperta al futuro. ISelectableContent.
- StageDresser (scena): Apply(stage) -> scambia SOLO gli sprite di Background e Ground. MAI il collider.

**Menu e selezione (UI generata dai dati)**
- ISelectableContent: interfaccia (DisplayName, SwatchColor, Preview).
- ContentSwatch (prefab): una cella toccabile.
- ContentSelector<T> (base generica astratta): genera uno swatch per elemento, traccia Selected.
  Sottoclassi di 2 righe: CharacterSelector, StageSelector.
- MainMenuController: raccoglie personaggi/modalita/difficolta/stadio -> MatchConfig via MatchSession
  -> carica Gameplay. Riga difficolta visibile solo in 1P. NON gestisce audio (vedi AudioSettingsUI).

**Difficolta CPU**
- AIStats_Easy/Medium/Hard: leva forte = usePrediction (off su Easy). La difficolta SOSTITUISCE
  l'aiStats del personaggio (config-or-fallback). Applicata in Bootstrap via AIPlayerInput.SetStats.
- Player2 ospita Keyboard + AI; il Bootstrap ne attiva uno via PlayerController.SetInput.

**Distinzione giocatori**
- PlayerMarker (su ogni player): colora il Marker per slot (P1 rosso / P2 blu). Sorting Foreground.

**Torneo (gauntlet)**
- TournamentRun (Content): stato corsa; BuildCurrentMatchConfig() (difficolta crescente); Advance().
- TournamentSession (statico): trasporta la run tra scene.
- TournamentLauncher (UI): avversari = roster meno il giocatore -> costruisce la run.
- TournamentFlow (UI, Gameplay): ascolta OnMatchEnded; vinto->avanza/CAMPIONE, perso->ELIMINATO.
  Continua = ri-arma GameManager + ResetMatch + ricarica Gameplay. Registra il risultato nel salvataggio.

**Salvataggi**
- SaveData (Core): schema esplicito (tournamentsWon, longestStreak, musicVolume, sfxVolume).
- SaveSystem (Core, statico): JSON in persistentDataPath. Load usa FromJsonOverwrite (vedi lezioni).
- RecordDisplay (UI): mostra il record nel menu all'avvio.

---

# COMPONENTI CHIAVE FASE 4 (AUDIO)

**Fondamenta mixer**
- AudioMixer GameAudio: Master -> Music, SFX. Output dell'AudioSource di SfxPlayer -> gruppo SFX;
  output di MusicPlayer -> gruppo Music. PlayOneShot rispetta l'Output dell'AudioSource: instradare
  non richiede codice, solo il campo Output nell'inspector.
- Parametri esposti sul mixer: MusicVol, SfxVol (Volume dei gruppi Music/SFX). Nome LETTERALE:
  SetFloat fallisce in silenzio se non combacia (grafia esatta: SfxVol, non SFXVol/SfxVolume).

**Famiglia feedback (Presentation reagisce, Gameplay annuncia)**
- ImpactFeedback: coordinatore degli eventi della PALLA. Si iscrive a Ball.OnHitByPlayer /
  OnGroundTouched / OnNetTouched e smista a CameraShake/HitStop/particelle/SfxPlayer.
  Config raggruppata PER EVENTO. Suoni: colpo (hitSound), punto (pointSound), rete (netSound).
- PlayerFeedback: coordinatore degli eventi del PLAYER (uno per player). Si iscrive a
  PlayerController.OnJumped -> jumpSound. NB nome PlayerFeedback e NON PlayerSfx (vedi lezioni).
- MatchFeedback: coordinatore degli eventi del MATCH (scene-scoped). Si iscrive a
  GameManager.OnMatchEnded -> matchEndSound. NEUTRO per scelta (fischio finale): il winner e
  ignorato. Vittoria/sconfitta = estensione futura al bordo (GameplayBootstrap inietta il lato
  umano), non riscrittura.

**Meccanismi audio**
- SfxPlayer: MECCANISMO one-shot (PlayOneShot, layering, gira durante hit-stop). Scene-scoped singleton.
- MusicPlayer: MECCANISMO musica in loop. Singleton PERSISTENTE (root + DontDestroyOnLoad). Il dedup
  singleton rende sicuro metterlo in piu scene: musica ininterrotta tra menu e gameplay. Sa COSA
  suona, non QUANTO forte.

**Volume (catena SaveData -> VolumeSettings -> VolumeController -> mixer)**
- VolumeController: AUTORITA del COME. Mappa lineare 0..1 -> dB (log10*20); 0 forzato a -80 dB
  (silenzio). Applica ai param esposti. NON decide i valori. Diagnostica i param allo Start.
- VolumeSettings: PONTE. Possiede i VALORI. Load in Awake (senza dipendenze), apply in Start
  (quando VolumeController.Instance e pronto). I setter applicano + salvano. Scene-scoped singleton nel menu.
- AudioSettingsUI: lega due Slider a VolumeSettings. Init con SetValueWithoutNotify (no save spurio),
  poi forward ai setter. Tenuto FUORI da MainMenuController (il volume e un'altra cosa).

---

# RIFINITURE / DEBITI RIMANDATI

**Fase 2 (gameplay/arte)**
- Rete vestita: oggi barra bianca con collider; manca sprite (pali+maglia).
- Mira CPU: rimanda la palla ma non sceglie DOVE.
- Predizione CPU coi rimbalzi sui muri: ignora le carambole laterali.

**Fase 3 (menu/UI) — polish in Fase 5**
- Estetica menu grezza: bottoni di sistema, niente titolo/tema/centratura.
- Layout menu: ordine righe, spaziature/centratura, catena Content Size Fitter da raffinare.
- Cornice selezione (SelectedMark): Border=6 + Image Type=Sliced (oggi stirata).
- Controlli irrilevanti in torneo (colonna P2, difficolta/stadio) visibili ma ignorati -> nasconderli.
- Rinominare StageDefinition.sandSprite -> floorSprite.

**Fase 4 (audio) — debito unico**
- Tap UI/menu SFX: rimandato a Fase 5 (e polish, non audio di gioco). SfxPlayer gia pronto.
- Placeholder audio sintetizzati (net/jump/whistle/bgm): sostituibili con asset veri quando si vuole
  (Bfxr per SFX; Beepbox/FamiStudio per musica). L'import musica lunga: Vorbis + Load Type Streaming.

---

# DECISIONI DI ARCHITETTURA — FASI 1-2 (riferimento)

**Fase 1**
- GameManager: Singleton, DontDestroyOnLoad, root GameObject, Script Execution Order -100.
- State Machine con transizioni validate. Stati: Boot, MainMenu, ModeSelect, Loading, Playing,
  Paused, PointScored, MatchEnd.
- Comunicazione via event Action<T> (OnEnable iscrive / OnDisable disiscrive, sempre).
- Movimento: velocity diretta (arcade); salto: impulso + variable jump height + gravity multipliers;
  ground check via Physics2D.OverlapCircle.
- Config in ScriptableObject. Lato campo dalla X rispetto alla rete (X=0). Active Input Handling = "Both".

**Fase 2 — Game feel**
- Colpo direzionale: contatto + velocita del player -> aim[-1,+1] -> angolo, palla sempre verso l'alto.
  Il colpo SOSTITUISCE il rimbalzo fisico. Parametri in BallStats.
- Pattern "fisica sul root, grafica su figlio Visual".
- Meccanismo/policy: CameraShake, HitStop (Time.timeScale=0; WaitForSecondsRealtime; ripristino in
  OnDestroy), SfxPlayer = meccanismi; ImpactFeedback = policy. Singleton SCOPED SULLA SCENA.

**Fase 2 — Pixel art**
- PPU 25; interna 480x270 (x4 a 1080p); ortho size 5.4. Pixel Perfect Camera (PPU 25, ref 480x270).
- Import: Single, Point, No Compression, no MipMaps, PPU 25, Full Rect. Preset PixelArtSprite.
- Sorting Layers: Default, Background, Midground, Gameplay, Ball, Foreground, FX, UI.

**Fase 2 — Input/AI/animazione**
- IPlayerInput (Tick, Horizontal, JumpHeld, ConsumeJumpPressed). Implementazioni: Keyboard/AI/Touch.
- AIPlayerInput: predizione balistica (g effettiva = Physics2D.gravity.y * ballRb.gravityScale),
  ricalcolata ogni frame (si autocorregge).
- SpriteAnimator: cicla frame per stato dalla velocita del Rigidbody2D.

---

# LEZIONI DAGLI INTOPPI (da non ripetere)

**Codice**
- Nome file = nome classe per i MonoBehaviour. Un errore di compilazione in QUALSIASI script
  blocca l'aggiunta di TUTTI i componenti custom e li fa sparire dai menu OnClick (Console!).
- Asset SO condiviso = stesso oggetto in memoria: un asset per entita se servono valori diversi.
- Evitare coppie di nomi che sono le stesse parole invertite (es. SfxPlayer / PlayerSfx):
  sembrano lo stesso identificatore a colpo d'occhio. Meccanismo e policy distinti anche nel nome
  (percio il listener del salto si chiama PlayerFeedback, non PlayerSfx).

**Pixel art**
- Normalizzare la scala a (1,1,1) PRIMA della pixel art; dimensione dai PIXEL a PPU 25.
- Preset salva TUTTI i campi (incl. Sprite Mode): crearlo da uno sprite Single pulito.
- Reimport puo far perdere i riferimenti SpriteRenderer: verificarli dopo reimport massivi.

**UI Unity**
- Catena layout annidati: Layout Element -> Layout Group -> Content Size Fitter. Ogni livello deve
  LEGGERE l'altezza dei figli (Control Child Size Height) E DICHIARARLA al genitore, o si spezza.
  (Uno Slider in un Vertical LG con Control Child Height puo schiacciarsi: dargli un Layout Element
  con Preferred Height.)
- "Contenitore" = GameObject vuoto + RectTransform + Layout Group (NON un Panel: l'Image ruba i click).
- Selezione "uno tra N": Button + tint manuale richiede Transition = None.
- Slider: settare slider.value PRIMA di agganciare onValueChanged (o SetValueWithoutNotify), altrimenti
  l'init scatena un cambio/salvataggio spurio.

**Audio / settings (Fase 4)**
- PlayOneShot rispetta l'Output dell'AudioSource -> instradare al mixer = solo inspector, zero codice.
- Parametro esposto sul mixer: SetFloat/GetFloat fanno match LETTERALE sul nome. "Exposed name does not
  exist" = non esposto o nome/maiuscole non combaciano. Esporre il Volume del gruppo e rinominarlo esatto.
- Volume percepito e LOGARITMICO: mappare lineare 0..1 -> dB con log10*20; lo 0 va forzato a -80 dB
  (log10(0) = -infinito).
- SaveData: usare JsonUtility.FromJsonOverwrite su un'istanza fresca (coi default negli inizializzatori),
  NON FromJson<T>. Cosi i campi mancanti in un vecchio save restano ai default (niente muto/zero sui
  salvataggi gia esistenti quando lo schema cresce).
- OnValidate NON parte solo sui cambi Inspector: parte anche ALL'INGRESSO in Play mode, prima di Start,
  col valore serializzato nella SCENA (non quello su disco). Un hook di test che persiste in OnValidate
  sovrascrive il salvataggio col valore stantio. Guardare con un flag settato a fine Start (o non persistere).
- Chi legge un valore in Start deve poter contare che sia gia caricato: caricare in Awake (se non ha
  dipendenze) cosi l'ordine indefinito tra gli Start non morde. Applicare cio che dipende da altri
  singleton in Start (quando gli Instance sono pronti).

---

# COSTANTI TECNICHE CHIAVE
- Pixel art: PPU 25; interna 480x270 (x4 a 1080p); ortho size 5.4. Pixel Perfect Camera (PPU 25, ref 480x270).
- Player: Position Y -3.55, Scale (1,1,1), CapsuleCollider2D Size (0.9,1.9) Offset (0,0). groundCheckOffsetY -0.95.
- Ground: Position Y -5, Scale (20,1,1), BoxCollider2D Size (1,1) -> bordo superiore a -4.5.
- Sabbia/pavimento: sprite 512x64, Position Y -5.78, Scale (1,1,1), layer Midground.
- Sfondo: sprite 480x270, Position (0,0,0), layer Background.
- Canvas UI: Scale With Screen Size, ref 1920x1080.
- Build: IL2CPP, ARMv7 + ARM64, Minimum API Android 7.0. GameManager Script Execution Order -100.
- Salvataggio: JSON in persistentDataPath/save.json (cancellabile per reset).
- Audio: AudioMixer GameAudio (Master->Music,SFX); param esposti MusicVol, SfxVol; silenzio = -80 dB.

# BACKLOG IDEE FUTURE (NON implementare ora)
- Variazione potenza del colpo in base alla velocita in ingresso (smash).
- Mira deliberata della CPU verso il campo scoperto.
- Animazioni piu ricche (Aseprite, sprite di profilo con flip).
- Stadi FUNZIONALI (geometria campo, altezza rete, physics material) - sezione gia commentata in StageDefinition.
- Musica per-stage: StageDefinition ha gia lo slot commentato; MusicPlayer cambierebbe traccia al bordo.
- Suono fine match vittoria/sconfitta: MatchFeedback oggi e neutro; il lato umano si inietta dal Bootstrap.
- Evoluzione personaggi: CharacterDefinition resta TEMPLATE immutabile; progressione in un layer profilo;
  stat effettive = base + modificatori a RUNTIME -> NON mutare mai l'asset condiviso.

# NOTE
- Vecchio progetto (>2 anni fa, solo APK) NON recuperato: solo ispirazione visiva.
- Prototipo HTML5 beach-volley.html come riferimento di game design.
