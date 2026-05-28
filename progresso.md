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
Mini-gameplay fisico: 2 Player + palla che rimbalza + campo con rete e muri.
Manca: punteggio, reset automatico, HUD, fine partita.

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

**Cosa ho fatto - Sessione 4 completa:**
- Muri laterali invisibili (LeftWall X=-10, RightWall X=10, GameObject vuoti + BoxCollider2D 1x20)
- Rete resa solida: BoxCollider2D su NetPlaceholder (Player separati nei due lati)
- Tag "Net" assegnato a NetPlaceholder
- PALLA creata:
  - Sprite Circle (Scale 0.6), Sorting Layer Gameplay, Order 1, Tag "Ball"
  - Rigidbody2D: Dynamic, Mass ~0.35, GravityScale ~1.6, Continuous, no freeze rotation
  - CircleCollider2D
  - PhysicsMaterial2D "BallBouncy" (Friction 0.2, Bounciness 0.7) assegnato al Rigidbody
- ScriptableObject BallStats (serve position, serve velocity, rest velocity) + asset BallStats_Default
- Script Ball.cs in Gameplay/:
  - [RequireComponent] Rigidbody2D + CircleCollider2D
  - OnCollisionEnter2D con CompareTag per Player1/Player2/Ground/Net
  - 3 eventi: OnHitByPlayer, OnGroundTouched, OnNetTouched
  - lastTouchedBy + hasBeenTouched (stato per regole volley future)
  - HandleGroundTouch determina lato campo via transform.position.x < 0
  - ResetBall(PlayerIndex) azzera velocity lineare+angolare e riposiziona
  - Debug temporaneo: R resetta serve Player1, T serve Player2 (#if UNITY_EDITOR)
- Tuning fisica palla fatto giocando in 2

**Pattern imparati:**
- PhysicsMaterial2D (Friction + Bounciness) per superfici/oggetti
- GameObject vuoto + Collider per geometria di collisione invisibile (muri)
- OnCollisionEnter2D per rilevare impatti (richiede Rigidbody2D su almeno un oggetto)
- CompareTag invece di == per performance (no allocazioni stringhe)
- Ball annuncia eventi, non decide cosa fare (separazione responsabilità)
- Azzerare velocity lineare+angolare quando si riposiziona un Rigidbody (errore classico se dimenticato)
- Massa relativa tra oggetti influenza il rinculo nelle collisioni

**Note sul game feel:**
- Gameplay ancora grezzo (palla imprevedibile, colpi non precisi) - NORMALE a questo stadio
- Controllo direzionale del colpo, zone di colpo, assistenza traiettoria: da fare in Fase 2 (game feel)
- Obiettivo Sessione 4 era "palla fisica funzionante" - raggiunto

**Dove sono bloccato:**
- Niente

**Prossimo passo - Sessione 5 (completamento MVP giocabile):**
- ScoreManager (o estensione GameManager) con punteggio per i 2 lati
- Nuovo evento OnScoreChanged
- Ball.OnGroundTouched collegato al punteggio (chi segna quando la palla tocca terra)
- Reset automatico della palla dopo un punto (chi ha subito serve, o regole volley)
- HUD primitivo: punteggio a schermo (Canvas + Text)
- Schermata fine partita (primo a N punti vince)
- Rimuovere debug R/T da Ball.cs
- Collegare lo stato Playing/Paused/MatchEnd al gameplay

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

## Decisioni di architettura prese (aggiunte Sessione 4)
- Geometria di collisione invisibile = GameObject vuoto + Collider (no sprite inutili)
- Ball usa eventi (OnHitByPlayer, OnGroundTouched, OnNetTouched) - non contiene logica punteggio
- Lato del campo determinato dalla posizione X rispetto alla rete (X=0)
- Ball.ResetBall riposiziona e azzera SEMPRE velocity lineare e angolare
- BallStats in ScriptableObject (coerente con PlayerStats)
- PhysicsMaterial2D sul Rigidbody (non sul Collider) per fisica consistente della palla
  
## Backlog idee future
*(vuoto per ora, raccoglierà idee che emergono lungo il percorso
ma che NON vanno implementate subito)*

## Note utili
- Il vecchio progetto di beach volley (più di 2 anni fa, solo APK rimasto)
  non viene recuperato. Solo "memoria visiva" per ispirazione.
- Setup HTML5 prototipo: c'è un file `beach-volley.html` di prototipo
  creato all'inizio del percorso, utile come riferimento di game design
  (meccaniche base già funzionanti).