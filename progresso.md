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

## Stato attuale: Fase 1 - Ricostruzione prototipo in Unity (in corso)
Player giocabili in 2 con tastiera. Manca: rete con collider, palla, punteggio.

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
**Data:** [data di oggi]

**Cosa ho fatto - Sessione 3 completa:**
- Creata scena Gameplay separata (Build Profiles aggiornato: MainMenu idx 0, Gameplay idx 1)
- Sprite placeholder via Unity 2D Object: Player1 (rosso), Player2 (blu), Ground (sabbia), NetPlaceholder (bianco)
- Fisica Player: Rigidbody2D dinamico, GravityScale=3, FreezeRotationZ, Continuous Collision
- CapsuleCollider2D dimensionato considerando Transform.Scale (Size 0.9 x 0.95 dato Scale Y=2)
- Layer "Ground" creato (User Layer 6) e assegnato al Ground GameObject
- ScriptableObject PlayerStats (movement, jump, ground check) con [Header], [Tooltip], [Range], [CreateAssetMenu]
- Asset PlayerStats_Default in Settings/, tuning a mano (7/14/2/2.5)
- Script PlayerController.cs in Gameplay/:
  - [RequireComponent(Rigidbody2D)]
  - Input vecchio Input Manager (Player1: frecce+Spazio, Player2: WASD)
  - Update legge input, FixedUpdate gestisce fisica
  - Pattern flag jumpPressedThisFrame per non perdere input tra frame
  - Ground check via Physics2D.OverlapCircle + LayerMask
  - Velocity diretta per movimento orizzontale (no momentum, feel arcade)
  - Variable jump height (low jump multiplier se rilascio il tasto in salita)
  - Fall gravity multiplier per atterraggi snappy
  - Gizmo verde nel OnDrawGizmosSelected per visualizzare ground check
  - Enum PlayerIndex per mappare input ai due giocatori
- Active Input Handling cambiato a "Both" in Player Settings (Unity 6 default era "Input System Package Only")
- Player2 creato per duplicazione: 0 righe di codice, solo Inspector

**Pattern imparati:**
- ScriptableObject per configurazioni (no magic numbers nel codice)
- [CreateAssetMenu] per generare asset da menu Unity
- [Header]/[Tooltip]/[Range] per Inspector user-friendly
- [RequireComponent] per garantire dipendenze tra componenti
- Update vs FixedUpdate (input vs physics)
- Velocity diretta vs Forze (arcade feel vs realismo)
- Variable jump height (Mario-style)
- Physics2D.OverlapCircle per ground check geometrico
- Layer (fisica) vs Sorting Layer (rendering) - sono cose diverse
- Gizmos.DrawWireSphere per debug visivo in Scene view
- Riusabilità: 1 script + ScriptableObject + enum = N personaggi

**Lezioni dagli intoppi:**
- CapsuleCollider2D NON scala 1:1 visivamente se Transform.Scale != 1 — Size va aggiustata
- Unity 6 Universal 2D template usa il nuovo Input System di default → InvalidOperationException
- Verificare sempre i collider con Gizmos in Scene view
- Leggere e fidarsi dei messaggi di errore: spesso indicano già la soluzione

**Dove sono bloccato:**
- Niente

**Prossimo passo - Sessione 4:**
- Rete vera con BoxCollider2D che blocca i Player a metà campo
- Limiti laterali al campo (no uscire dai bordi)
- PALLA: GameObject sprite circolare + Rigidbody2D + CircleCollider2D + PhysicsMaterial2D
- Script Ball.cs per gestire collisioni e lanciare eventi
- Setup di reset palla dopo un punto (futuro)

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

## Decisioni di architettura prese (aggiunte Sessione 3)
- Active Input Handling = "Both" (vecchio + nuovo Input System contemporaneamente)
- Vecchio Input Manager usato per il primo prototipo, migrazione al nuovo Input System nella Sessione 5 (per multi-touch e gestione gamepad)
- Player NON è un Singleton, è oggetto di gioco normale
- Player NON ha riferimenti al GameManager (accoppiamento minimo)
- Movimento orizzontale: velocity diretta (no AddForce) per feel arcade
- Salto: impulso verticale + variable jump height + gravity multipliers
- Ground check: Physics2D.OverlapCircle con LayerMask "Ground"
- Configurazione gameplay: tutta in ScriptableObject (PlayerStats), nessun magic number negli script
- Convention naming asset: PlayerStats_Default, PlayerStats_Speedy (futuri), ecc. (suffisso = variante)
  
## Backlog idee future
*(vuoto per ora, raccoglierà idee che emergono lungo il percorso
ma che NON vanno implementate subito)*

## Note utili
- Il vecchio progetto di beach volley (più di 2 anni fa, solo APK rimasto)
  non viene recuperato. Solo "memoria visiva" per ispirazione.
- Setup HTML5 prototipo: c'è un file `beach-volley.html` di prototipo
  creato all'inizio del percorso, utile come riferimento di game design
  (meccaniche base già funzionanti).