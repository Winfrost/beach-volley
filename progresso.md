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

## Stato attuale: FASE 1 COMPLETATA ✅
Beach volley giocabile end-to-end: 2 player, palla fisica, punteggio,
HUD, fine partita, rivincita. Prossima: Fase 2 (game feel + arte + audio + AI + mobile).

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
**Data:** [28/05/2026]

**Cosa ho fatto - Sessione 5 completa (CHIUSURA FASE 1):**
- GameManager esteso con punteggio: scorePlayer1/2, pointsToWin (7), NextServer
- Eventi: OnScoreChanged(int,int), OnMatchEnded(PlayerIndex)
- Metodi AwardPoint(scoringPlayer) e ResetMatch()
- Pattern Singleton auto-creating (FindFirstObjectByType + AddComponent se manca)
- GameplayBootstrap: forza esistenza GameManager + transizione a Playing
  - Usa coroutine StartMatchWhenReady (aspetta che stato esca da Boot)
- MatchController (ponte Ball <-> GameManager):
  - Ascolta Ball.OnGroundTouched, assegna punto all'avversario del lato colpito
  - Coroutine ResetBallAfterDelay (pausa 1.5s poi reset palla + ritorno a Playing)
  - Flag isResolvingPoint per evitare punti multipli sui rimbalzi
  - Ascolta OnStateChanged per resettare flag su rematch (MatchEnd->Playing)
- HUD: Canvas (Scale With Screen Size, ref 1920x1080) + TextMeshPro
  - HUDController ascolta OnScoreChanged, aggiorna ScoreText
- Schermata vittoria: WinPanel (disattivo all'avvio) + WinnerText + RematchButton
  - WinScreenController ascolta OnMatchEnded, mostra pannello + nome vincitore
  - Pulsante RIVINCITA: ResetMatch + ChangeState(Playing) + ResetBall
- Rimosso debug R/T dalla Ball

**Pattern imparati:**
- Coroutine (IEnumerator + yield return null / WaitForSeconds) - timer e attese
- Programmazione robusta: "aspetta condizione" invece di "assumi ordine esecuzione"
- Singleton lazy auto-creating + bootstrapper per inizializzazione esplicita
- Pattern "ponte" (MatchController) per disaccoppiare oggetti di gioco e logica
- UI Unity: Canvas, RectTransform (box) vs Alignment (testo nel box) vs Wrapping
- Button.onClick.AddListener (observer integrato Unity)
- SetActive per mostrare/nascondere GameObject

**Lezioni dagli intoppi:**
- Singleton lazy non si avvia se nessuno lo invoca -> serve bootstrapper
- Ordine Awake/Start: leggere stato altrui solo nel proprio Start (non Awake)
- Flag di stato vanno resettati su TUTTI i percorsi di uscita (bug rematch:
  isResolvingPoint restava true se il punto finiva la partita -> coroutine non partiva)
- UI testo: box stretto = testo va a capo. Stretch RectTransform + wrapping off
- RectTransform posiziona il box, Alignment posiziona il testo dentro il box

**Dove sono bloccato:**
- Niente. Fase 1 completa e funzionante.

**FASE 1 COMPLETATA - Riepilogo del gioco attuale:**
- Movimento e salto 2 player (tastiera: frecce+Spazio / WASD)
- Palla con fisica bouncy, collisioni, eventi
- Campo: rete + muri laterali + terreno
- Punteggio fino a 7, reset palla dopo ogni punto
- HUD punteggio a schermo
- Schermata vittoria + rivincita funzionante
- Tutto via eventi, architettura disaccoppiata

**Prossimo passo - FASE 2 (il gioco da "funzionale" a "bello"):**
Da decidere l'ordine, opzioni:
- Game feel: controllo direzionale del colpo, screen shake, squash/stretch, hit-stop
- Arte: pixel art per player, palla, sfondo (sostituire i placeholder)
- Audio: musica + SFX (rimbalzo, salto, punto, vittoria)
- AI: CPU controller per modalità 1 giocatore
- Mobile: migrazione al nuovo Input System + controlli touch

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

## Decisioni di architettura prese (aggiunte Sessione 5)
- Punteggio dentro GameManager (non ScoreManager separato) - scope semplice
- Singleton auto-creating (lazy) + GameplayBootstrap per init esplicita nelle scene
- MatchController come "ponte" tra Ball (gameplay) e GameManager (logica match)
- Regola punteggio MVP: palla a terra -> punto all'avversario del lato colpito
- Chi perde il punto serve dopo (NextServer)
- Pausa 1.5s dopo ogni punto prima del reset palla (game feel/leggibilità)
- Canvas Scale With Screen Size + ref 1920x1080 per adattamento mobile
- Win screen via SetActive del pannello, non scene separata
- Flag di stato critici (isResolvingPoint) resettati su tutti i percorsi di uscita

## Decisioni di architettura prese (aggiunte Sessione 6)
- Colpo direzionale: modello "combinato" (punto di contatto + velocità player)
- Punto di contatto normalizzato su collider.bounds.extents.x (indipendente da sprite/scala)
- Ball legge la velocità del player da collision.rigidbody (no dipendenza da PlayerStats)
- aim in [-1,+1] -> angolo da verticale = aim * maxHitAngle; dir = (sin, cos) -> palla sempre verso l'alto
- Colpo sostituisce il rimbalzo fisico (rb.linearVelocity diretto); PhysicsMaterial2D solo per muri/terreno/rete
- Parametri colpo in BallStats: hitForce, maxHitAngle, playerVelocityInfluence (niente magic numbers)
- ApplyHitDirection tenuto in Ball (coesione: fisica della palla); estraibile in BallHitResolver se serve
- Rimosso debug R/T Update da Ball (residuo da Sessione 5)

## Decisioni di architettura prese (aggiunte Sessione 6 - game feel)
- Pattern "fisica sulla radice, grafica su figlio": Ball root tiene Rigidbody2D +
  CircleCollider2D + Ball.cs; figlio "Visual" tiene SpriteRenderer (+ feedback visivo)
- Motivo: la scala del visual va animata liberamente senza mai toccare il collider
- BallSquashStretch: componente puramente reattivo, si iscrive a OnGroundTouched/
  OnHitByPlayer/OnNetTouched, anima localScale del figlio (snap + recover via coroutine)
- Squash ad assi fissi (no rotazioni): terra = appiattisce, colpo = allunga, rete = stringe
- Parametri squash come [SerializeField] sul componente (promuovibili a BallStats se servono varianti)
- Recover con AnimationCurve (overshoot tarabile a mano nell'Inspector)
- OnDisable ferma la coroutine e ripristina la scala (no stato visivo bloccato)
- Zero modifiche a Ball.cs: primo "juicer" che conferma l'architettura a eventi

## Decisioni di architettura prese (aggiunte Sessione 6 - screen shake)
- Separazione meccanismo/policy per il feedback di camera:
  - CameraShake = meccanismo (HOW): Shake(intensity), offset decrescente in LateUpdate, game-agnostic
  - GameplayShakeTrigger = policy (WHEN/HOW-HARD): ascolta eventi Ball, sceglie le intensità
- CameraShake è singleton SCOPED SULLA SCENA (no DontDestroyOnLoad, diverso da GameManager)
- Intensità per tipo di evento (colpo leggero, palla a terra forte); scaling per potenza reale rimandato a quando esisterà la schiacciata
- Zero modifiche a Ball.cs (secondo juicer event-driven)
- Nota: shake su localPosition assume camera statica; con camera-follow andrà su un transform figlio

## Decisioni di architettura prese (aggiunte Sessione 6 - hit-stop)
- HitStop = meccanismo (HOW): Time.timeScale=0 per N secondi REALI, poi ripristino
  - WaitForSecondsRealtime perché WaitForSeconds (scaled) si bloccherebbe a timeScale 0
  - OnDestroy ripristina timeScale a 1 (safety: stato globale resettato su ogni uscita)
  - Nota: con un futuro menu pausa, la pausa possiede timeScale e l'hit-stop va soppresso
- HitStopTrigger = policy (WHEN): freeze solo sulla palla a terra; hitFreeze esposto ma default 0
- Conferma del design temporale: squash e shake usano Time.deltaTime apposta, così si
  congelano/persistono in modo coerente durante il freeze (zero modifiche a Ball/CameraShake/squash)
  
## Backlog idee future
*(vuoto per ora, raccoglierà idee che emergono lungo il percorso
ma che NON vanno implementate subito)*

## Note utili
- Il vecchio progetto di beach volley (più di 2 anni fa, solo APK rimasto)
  non viene recuperato. Solo "memoria visiva" per ispirazione.
- Setup HTML5 prototipo: c'è un file `beach-volley.html` di prototipo
  creato all'inizio del percorso, utile come riferimento di game design
  (meccaniche base già funzionanti).