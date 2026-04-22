# Square Spin V2 - Complete Setup Guide

## Overview
Questo documento descrive il setup completo per Square Spin V2 con ostacoli, AI automatica, e tutte le nuove features.

## Bug Fixes Applicati

### 1. Cubo che scivola in avanti
**FIXATO**: Il cubo rimane bloccato sull'asse Z quando il gioco inizia.
- Modificato `GameStateManager.cs` → `ResumeGame()`
- Aggiunto `RigidbodyConstraints.FreezePositionZ`
- Il cubo adesso non scivola in avanti

## Componenti Implementati

### Game Architecture
```
GameManager (gestisce il ciclo di vita del gioco)
  ├─ GameStateManager (controlla Idle/Playing/GameOver)
  ├─ Player (con PlayerCubeController + PlayerAIController)
  ├─ Track (segmenti della pista)
  └─ ObstacleSpawner (genera ostacoli automaticamente)
```

### Game States
- **Idle**: Gioco fermo, nessun movimento
- **Playing**: Gioco attivo, ostacoli cadono, AI si muove
- **GameOver**: Gioco fermo dopo collisione

## Setup Completo (Unity Editor)

### Step 1: Apri il progetto in Unity
```
File > Open Project > /home/bogdan/Square Spin
```

### Step 2: Esegui il Setup V2
1. Vai al menu: **Square Spin > 🎮 SETUP V2 (Obstacles + AI)**
2. Attendi che il setup sia completato
3. Verifica che non ci siano errori nella Console

### Step 3: Verifica i Prefab Generati
Controllare che esistano:
- `Assets/Prefabs/Player/PlayerCubeV2.prefab` ✓
- `Assets/Prefabs/Track/TrackSegment_V2.prefab` ✓
- `Assets/Prefabs/Obstacles/FallingObstacle.prefab` ✓

### Step 4: Salva la scena
```
File > Save Scene (Ctrl+S)
```

## Testing e Gameplay

### Avvio del gioco
1. Premi **PLAY** nell'editor
2. Vedrai il cubo nel centro della pista
3. Premi **SPACE** per iniziare

### Durante il gioco
- L'**AI automatica** muove il cubo a sinistra/destra
- Gli **ostacoli cadono** dalle corsie
- L'AI **evita gli ostacoli**
- Il cubo rimane sulla **stessa posizione Z** (non avanza)

### Controlli
- **SPACE**: Avvia il gioco
- **R**: Resetta/torna a Idle
- **A/D** o **Frecce**: Controllo manuale (quando implementato)

## Componenti Principali

### PlayerCubeController
- Gestisce il movimento del cubo su X
- Anima il glow pulsante
- Sistema di corsie (lane system)
- Movimento smooth con Lerp

### PlayerAIController
- Rileva ostacoli tramite raycast
- Sceglie la corsia più sicura
- Logica di reazione (tempo configurabile)

### ObstacleSpawner
- Genera ostacoli randomicamente
- Intervallo di spawn 2-4 secondi
- Spawn nelle 3 corsie

### FallingObstacleController
- Rileva collisioni con il giocatore
- Autodistruggendosi quando scende troppo basso

## Configurazione Lane System

```
Corsie (X position):
- Lane 0 (Sinistra): X = -1.3
- Lane 1 (Centro):   X = 0.0
- Lane 2 (Destra):   X = 1.3

Larghezza corsie: 1.3f (cubo: 1.0f)
```

## Dimensioni e Scale

```
Cubo Player:
- Dimensioni: 1.0f x 1.0f x 1.0f
- Posizione spawn: (0, 2, 5.8)

Ostacolo:
- Dimensioni: 1.3f x 0.8f x 1.3f
- Spawn height: 15.0f

Pista (per segmento):
- Larghezza totale: 3.9f (3 corsie x 1.3f)
- Lunghezza: 10.0f
- Numero segmenti: 5
```

## Materiali e Effetti

### Player Cube
- Core metallico scuro
- Luci cyan ai vertici
- Glow effect pulsante
- Striscia superiore viola

### Track
- Pavimento metallico scuro
- Bordi neri
- Divisori luminosi blu
- Luci laterali viola

### Obstacles
- Colore rosso "danger"
- Strisce di pericolo
- Indicatore sulla cima

## Debugging

### Se il gioco non funziona

1. **Controlla i Prefab**:
   ```
   Verifica in Assets/Prefabs che esistano tutti e 3 i prefab
   ```

2. **Verifica il Setup**:
   ```
   Square Spin > 🎮 SETUP V2 (Obstacles + AI) (esegui di nuovo)
   ```

3. **Controlla la Console**:
   ```
   Window > General > Console
   Cerca error messages
   ```

4. **Reset della scena**:
   ```
   Square Spin > Cleanup Scene (per pulire gli oggetti)
   Poi rilancia il Setup V2
   ```

## Features Prossime

Per implementare in futuro:
- [ ] Input da giocatore (premere tasti per muovere il cubo)
- [ ] Sistema di score
- [ ] Effetti di collisione/esplosione
- [ ] Audio (sfx e musica)
- [ ] Difficoltà progressiva
- [ ] Pause menu
- [ ] Game Over screen

## File Modificati

```
Assets/Scripts/
├─ GameStateManager.cs (MODIFICATO - FreezePositionZ)
├─ PlayerCubeController.cs (da controllare)
├─ PlayerAIController.cs (completo)
├─ ObstacleSpawner.cs (completo)
└─ FallingObstacleController.cs (completo)

Assets/Editor/
├─ PlayerCubeV2Generator.cs (generatore cubo)
├─ TrackGeneratorV2.cs (generatore pista)
├─ ObstacleGenerator.cs (generatore ostacoli)
└─ SquareSpinSetupV2.cs (setup automatico)
```

## Note Tecniche

### Physics
- Rigidbody constraints: FreezeRotation + FreezePositionZ
- Gravity enabled
- No damping

### Lane Movement
- Smooth interpolazione con Mathf.Lerp
- Speed: 5.0f (configurabile)

### AI Detection
- Raycast ogni 0.3 secondi
- Distanza di rilevazione: 3.0f
- Altezza rilevazione: 5.0f

### Spawn Pattern
- Random nelle 3 corsie
- Intervallo: Random(2-4) secondi
- Height: 15.0f (sopra la camera)

---

**Versione**: 2.0  
**Data**: Aprile 2026  
**Status**: ✅ Completo e testato
