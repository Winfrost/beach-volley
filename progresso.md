# Beach Volley - Diario di progetto

## Visione del progetto
Sviluppo di un videogioco di beach volley per Android e iOS.
Obiettivo finale: pubblicazione sugli store come prodotto serio
(possibile monetizzazione). Stile grafico: pixel art retro.
Modalità: 1 vs CPU e 2 giocatori sullo stesso schermo.

## Stack tecnologico
- **Motore:** Unity (versione LTS più recente)
- **Linguaggio:** C#
- **IDE:** Visual Studio Community con workload "Game development with Unity"
- **Versionamento:** Git + GitHub (repo privato `beach-volley`)
- **OS sviluppo:** Windows
- **Dispositivo test:** Android (fisico, via USB)
- **iOS:** affrontato a fine progetto (servirà Mac in cloud o fisico)

## Decisioni di design e strategia
- Si riparte da ZERO, non si recupera il vecchio progetto Unity
- Linguaggio: C# (non altre alternative)
- Architettura: si definirà nella Sessione 2 prima di scrivere gameplay
- Approccio: solide fondamenta tecniche prima del gameplay
- Scope: ridotto e finibile, non gioco gigantesco
  - Pochi personaggi (3 circa) con stat differenti
  - 2-3 stadi
  - Modalità singleplayer vs CPU + 2 giocatori locali
  - NIENTE multiplayer online (almeno per v1.0)
  - NIENTE monetizzazione complessa per v1.0

## Piano per fasi
- **Fase 0** — Setup ambiente (IN CORSO)
- **Fase 1** — Ricostruzione prototipo in Unity (base solida)
- **Fase 2** — Game feel (squash/stretch, screen shake, particelle, juice)
- **Fase 3** — Contenuto (personaggi, stadi, modalità torneo, salvataggi)
- **Fase 4** — Audio (musica chiptune, SFX)
- **Fase 5** — Mobile e polish (touch UI, performance, vari schermi)
- **Fase 6** — Pubblicazione (account store, asset, beta, submission)

Tempistica realistica: 8-12 mesi di calendario al ritmo di 2-4 ore/settimana
(con possibile aumento delle ore).

## Stato attuale: Fase 1 - Ricostruzione prototipo in Unity
Setup completato. Architettura di base in piedi. Pronti per il gameplay.

## Prossima Fase: 1 - Ricostruzione prototipo in Unity
Obiettivo: replicare il gameplay del prototipo HTML in Unity con codice
pulito, modulare, e fondamenta architetturali solide.

### Completato ✅
- [x] Visual Studio Community installato
- [x] Workload "Game development with Unity" installato e verificato
- [x] Test autocompletamento Unity (transform.) funzionante in Visual Studio

### Da fare ⬜
- [ ] Unity Hub installato
- [ ] Unity LTS installato con moduli:
  - Android Build Support (con Android SDK & NDK Tools, OpenJDK)
  - iOS Build Support
  - Documentation
- [ ] Git installato e configurato (user.name, user.email)
- [ ] Cartella progetto creata in posizione corretta
  (es. C:\Dev\BeachVolley, NON in OneDrive/Documenti/Dropbox)
- [ ] Repository GitHub `beach-volley` (privato) collegato alla cartella locale
- [ ] Primo commit + push fatto con successo
- [ ] `.gitignore` provvisorio creato (quello specifico per Unity sarà nella Sessione 2)
- [ ] Progetto Test creato in Unity per verifica finale

## Ultima sessione
**Data:** [22/05/2026]

**Cosa ho fatto - Sessione 2 completa:**
- Configurato .gitignore Unity ufficiale
- Project Settings per Android mobile (1920x1080 landscape, IL2CPP, ARMv7+ARM64, API 24+)
- Struttura cartelle in Assets/_Project/ (Scripts/Core, Gameplay, AI, UI, Utils; Sprites, Audio, Prefabs, ecc.)
- Scena MainMenu: camera ortografica size 5.4 a (0,0,-10), Sorting Layers (Background/Midground/Default/Gameplay/Foreground/UI), Tags (Player1/Player2/Ball/Net/Ground)
- Hierarchy organizzata con contenitori separatori
- DECISIONI ARCHITETTURA: Singleton + State Machine + Eventi + ScriptableObject + Input System nuovo
- 3 script creati in Scripts/Core/:
  - GameState.cs (enum con 8 stati)
  - GameManager.cs (Singleton + DontDestroyOnLoad + state machine con validazione transizioni)
  - GameStateDebugListener.cs (listener di esempio con subscription resiliente)
- Script Execution Order: GameManager = -100 (gira prima di tutti)
- BUG risolti:
  - DontDestroyOnLoad richiede root GameObject (spostato GameManager fuori da Managers)
  - Race condition OnEnable: risolta con Script Execution Order + fallback in Start del listener

**Pattern imparati:**
- Singleton (con auto-detach dal parent come safety)
- State Machine (transizioni esplicitamente validate)
- Observer/Events (Action<T1,T2> + sottoscrizione in OnEnable / disiscrizione in OnDisable)
- SerializeField per esporre private in Inspector
- Direttive preprocessore #if UNITY_EDITOR per codice solo-debug
- Script Execution Order per dipendenze tra script

**Dove sono bloccato:**
- Niente

**Prossimo passo - inizio Fase 1 vera:**
- Sessione 3: scena di gioco "Gameplay" + camera per il campo
- Creare il GameObject Player (sprite placeholder + Rigidbody2D + Collider)
- Primo script Player.cs: movimento orizzontale + salto con input keyboard
- (Touch input dopo, prima facciamo che funzioni su PC)

## Strategia chat con Claude
- Una chat per FASE del progetto, non per singola sessione
- Chat corrente: "Beach Volley - Fase 0 - Setup"
- Prossima chat sarà: "Beach Volley - Fase 1 - Ricostruzione prototipo Unity"
- Questo `progresso.md` è il ponte tra le chat
- All'inizio di ogni nuova chat: incollare questo file in cima al primo messaggio

## Decisioni di architettura prese
- GameManager come Singleton con DontDestroyOnLoad, root GameObject
- Namespace: BeachVolley.Core (futuro: .Gameplay, .AI, .UI)
- State Machine con transizioni esplicitamente validate (no transizioni "magiche")
- Comunicazione tra sistemi via event Action<T> (no riferimenti diretti tra script)
- Pattern sottoscrizione: OnEnable iscrive, OnDisable disiscrive, sempre
- Script Execution Order: i manager girano a -100 (prima del resto)
- Codice di debug protetto da #if UNITY_EDITOR (non finisce in build)
- Tre regole assolute:
  1. Niente magic numbers nel codice (ScriptableObject o costanti)
  2. MonoBehaviour fa UNA cosa (max ~150 righe come obiettivo)
  3. Commit frequenti, piccoli e descrittivi

## Backlog idee future
*(vuoto per ora, raccoglierà idee che emergono lungo il percorso
ma che NON vanno implementate subito)*

## Note utili
- Il vecchio progetto di beach volley (più di 2 anni fa, solo APK rimasto)
  non viene recuperato. Solo "memoria visiva" per ispirazione.
- Setup HTML5 prototipo: c'è un file `beach-volley.html` di prototipo
  creato all'inizio del percorso, utile come riferimento di game design
  (meccaniche base già funzionanti).