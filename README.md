# Square Spin 🎮

Una versione modern-arcade del classico Frogger, con un cubo che evita ostacoli in tempo reale!

## 🎯 Features
- ✅ **Obstacle Avoidance AI** - Rilevamento automatico degli ostacoli in 3 corsie
- ✅ **Real-time Evasion** - Movimento fluido tra le corsie
- ✅ **3D Graphics** - Built con Unity + URP
- ✅ **WebGL Support** - Gioca direttamente dal browser

## 🚀 Play Online
[**Gioca ora su Vercel →**](https://square-spin.vercel.app)

## 🛠️ Setup Locale

### Requirements
- Unity 2022.3 LTS o superiore
- Git

### Installazione
```bash
git clone https://github.com/AmbrosioBogdan/SquareSpin-Test.git
cd "Square Spin"
# Apri il progetto in Unity Editor
```

### Build WebGL
1. `File → Build Settings`
2. Seleziona **WebGL**
3. Click **Build**
4. Salva nella cartella `Build/WebGL`

## 📁 Struttura del Progetto

```
├── Assets/
│   ├── Scripts/
│   │   ├── PlayerCubeController.cs      # Logica del player
│   │   ├── FallingObstacleController.cs # Gestione ostacoli
│   │   ├── ObstacleSpawner.cs           # Generazione ostacoli
│   │   ├── LaneConfiguration.cs         # Config corsie
│   │   ├── GameManager.cs               # Game loop
│   │   └── GameStateManager.cs          # Stato di gioco
│   ├── Prefabs/                         # Prefab riutilizzabili
│   ├── Materials/                       # Materiali e shader
│   └── Scenes/                          # Scene di gioco
├── ProjectSettings/                     # Config Unity
├── Build/                               # WebGL build (generato)
├── package.json                         # Config Vercel
└── vercel.json                          # Deploy config
```

## 🎮 Come Giocare
- **Arrow Keys / WASD**: Sposta il cubo tra le corsie
- **ESC**: Pausa
- **Evita gli ostacoli rossi** che scendono dalla pista!

## 🔧 Tech Stack
- **Engine**: Unity 6 (2022.3 LTS)
- **Rendering**: Universal Render Pipeline (URP)
- **Platform**: WebGL
- **Hosting**: Vercel
- **VCS**: GitHub

## 📊 Performance
- **Target**: 60 FPS
- **Build Size**: ~15-20 MB (WebGL)
- **Memory**: 100-200 MB runtime

## 🚀 Deploy automatico
Ogni push su `main` su GitHub triggeran un auto-deploy su Vercel!

```bash
git add .
git commit -m "feat: nuova feature"
git push origin main
# → Vercel fa il deploy automaticamente
```

## 📝 License
MIT License - Vedi LICENSE file

## 👨‍💻 Autore
**AmbrosioBogdan** - [GitHub](https://github.com/AmbrosioBogdan)

---

Made with ❤️ in 2026
