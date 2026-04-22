# 🎮 SQUARE SPIN - Setup Guida Veloce

## ⚡ Setup in 1 Click

Apri Unity e vai al menu:

**Square Spin → 🎮 SETUP GIOCO COMPLETO**

✅ Fine! Tutto è pronto.

---

## 🎮 Come Giocare

1. **Premi PLAY** nel editor
2. **Premi SPACE** per iniziare
3. **A/D o Frecce** per muoverti tra le corsie
4. **R** per resettare

---

## 📂 Struttura File

### File Principali
- **SquareSpinSetup.cs** ← **USA QUESTO** per il setup

### File di Utility (nella cartella Tools/)
- TrackGenerator.cs - Genera materiali e prefab della pista
- PlayerCubeGenerator.cs - Genera materiali e prefab del player
- MaterialCustomizer.cs - Modifica materiali rapidamente
- PrefabCustomizer.cs - Modifica prefab rapidamente
- EnvironmentSetup.cs - Setup camera e particelle
- GameSetupHelper.cs - Helper per il setup
- ViewAdjuster.cs - Aggiusta telecamera
- GroundPlaneSetup.cs - Crea il ground plane
- SceneResetAndSetup.cs - Resetta e riconfigura la scena

---

## 🔧 Customizzazione Avanzata

Se vuoi modificare il gioco, tutti i tool sono nel menu **Square Spin**:

```
Square Spin/
├── 🎮 SETUP GIOCO COMPLETO (MAIN)
├── Customizer/
│   ├── Modify Track Materials
│   ├── Modify Player Materials
│   ├── Make Floor Brighter
│   ├── Make Lane Glow Stronger
│   └── ...
├── Generator/
│   ├── Create Track Materials & Prefab
│   └── Create Player Cube
└── Setup/
    ├── Configure Camera & Environment
    ├── Add Ground Plane
    └── ...
```

---

## 📝 Configurazione Camera

Se vuoi modificare la posizione della camera, modifica in:
**Assets/Editor/SquareSpinSetup.cs** linea ~115

Valori attuali:
- **Posizione**: (0, 12, -0.3)
- **Rotazione**: (41°, 0°, 0°)
- **FOV**: 50°

---

## 🎨 Configurazione Player

Se vuoi modificare la posizione di spawn del player:
**Assets/Scripts/GameManager.cs** linea ~10

Valore attuale:
- **Spawn Position**: (0, 2, 5.8)

---

## ⚙️ Script Componenti

- **GameManager.cs** - Manage il gioco, istanzia player e track
- **GameStateManager.cs** - Gestisce stati (Idle/Playing/GameOver)
- **PlayerCubeController.cs** - Controllo player (movimento, glow)
- **TrackSegmentController.cs** - Anima il glow della pista

---

## 🐛 Se Qualcosa Non Funziona

1. Esegui **Square Spin → 🎮 SETUP GIOCO COMPLETO** di nuovo
2. Ricaricare la scena (Ctrl+R)
3. Controllare la Console per errori

---

## 📦 Cos'è già Pronto

✅ Pista spaziale con 3 corsie e divisori luminosi  
✅ Cubo player con glow animato  
✅ Particelle luminescenti blu  
✅ Sfondo nero completo  
✅ Camera posizionata correttamente  
✅ Sistema di movimento con controlli  
✅ Stato di gioco (Idle / Playing)  
✅ Collider e fisiche già configurate  

---

## 🚀 Prossimi Step

Per aggiungere funzionalità:
1. **Sistema di punti**: Aggiungi un UIManager
2. **Ostacoli**: Crea prefab di ostacoli nella pista
3. **Velocità progressiva**: Incrementa playerSpeed nel tempo
4. **Audio**: Aggiungi AudioManager con suoni sci-fi
5. **Effetti**: Aggiungi particle effects di collisione

---

Made with ❤️ for Square Spin
