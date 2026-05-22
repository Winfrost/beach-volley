Beach Volley - Struttura Assets

_Project/             Tutto il contenuto del NOSTRO progetto
├── Scenes/           Scene Unity (MainMenu, Gameplay, ecc.)
├── Scripts/
│   ├── Core/         GameManager, sistemi fondamentali
│   ├── Gameplay/     Player, Ball, Net, fisica
│   ├── AI/           Logica CPU
│   ├── UI/           Menu, HUD, schermate
│   └── Utils/        Helper ed estensioni
├── Prefabs/          Prefab di gioco (Player, Ball, ecc.)
├── Sprites/          Immagini 2D
├── Animations/       Animator e clip
├── Audio/            Musica e SFX
├── Materials/        Materials di Unity
├── Fonts/            Font del gioco
└── Settings/         ScriptableObject di configurazione

Regola: tutto il contenuto del gioco va dentro _Project/.
Cartelle fuori da _Project/ (es. TextMesh Pro, Settings) sono asset
di terze parti, NON modificarle.