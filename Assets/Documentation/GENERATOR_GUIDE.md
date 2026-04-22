# Square Spin - Track & Player Generator

## 📋 Cosa è stato creato

Ho generato uno **script automatico** che crea tutti i materiali e i prefab per il tema **spaziale/dark/futuristico/metallico**.

---

## 🚀 Come Usare

### Step 1: Genera Track e Materiali
1. Nel menu Unity Editor → **Square Spin / Generator / Create Track Materials & Prefab**
2. Verranno creati automaticamente:
   - Cartella `Assets/Materials/Track/`
   - Cartella `Assets/Prefabs/Track/`
   - **5 materiali**
   - **Prefab `TrackSegment_01`** completo con gerarchia

### Step 2: Genera Cubo Player
1. Nel menu Unity Editor → **Square Spin / Generator / Create Player Cube**
2. Verranno creati:
   - Cartella `Assets/Materials/Player/`
   - Cartella `Assets/Prefabs/Player/`
   - **3 materiali player**
   - **Prefab `PlayerCube`** con dettagli tech

---

## 🎨 Materiali Creati

### Track (5 materiali)
| Nome | Descrizione | Colore |
|------|-------------|--------|
| `MAT_FloorMetal_Dark` | Pavimento principale, grigio scuro metallico | Grigio (#262626) |
| `MAT_Border_BlackSteel` | Bordi laterali neri, contrasto | Nero (#0D0D0D) |
| `MAT_LaneGlow_Blue` | Divisori corsie, neon blu emissivo | Blu (#1A4CCC) |
| `MAT_SideGlow_Purple` | Luci laterali viola/blu sci-fi | Viola (#8033CC) |
| `MAT_Panel_Gray` | Dettagli decorativi grigio | Grigio (#404040) |

### Player (3 materiali)
| Nome | Descrizione | Effetto |
|------|-------------|--------|
| `MAT_Player_CoreMetal` | Corpo principale scuro | Base metallica |
| `MAT_Player_Accent` | Accenti luminosi blu | Strisce laterali |
| `MAT_Player_Glow` | Parti illuminate cyan | Corner glow animato |

---

## 🏗️ Gerarchia TrackSegment_01

```
TrackSegment_01 (con TrackSegmentController)
├── Floor_Base           (Plane 9x10, MAT_FloorMetal_Dark)
├── Border_Left          (Cube, MAT_Border_BlackSteel)
├── Border_Right         (Cube, MAT_Border_BlackSteel)
├── LaneDivider_1        (Cube, MAT_LaneGlow_Blue)
├── LaneDivider_2        (Cube, MAT_LaneGlow_Blue)
├── SideLight_Left       (Cube, MAT_SideGlow_Purple)
├── SideLight_Right      (Cube, MAT_SideGlow_Purple)
├── Detail_Panels        (Container)
│   ├── Panel_0 ... Panel_9    (Piccoli cubi, MAT_Panel_Gray)
└── GlowParticles        (Empty, per effetti futuri)
```

---

## 👾 Gerarchia PlayerCube

```
PlayerCube (con Rigidbody + PlayerCubeController)
├── Details
│   ├── Stripes              (Strisce luminose, MAT_Player_Accent)
│   │   ├── Stripe_0
│   │   ├── Stripe_1
│   │   ├── Stripe_2
│   │   └── Stripe_3
│   └── Corner_Glow_0...7    (Sfere sui vertici, MAT_Player_Glow)
└── (Mesh cubo principale, MAT_Player_CoreMetal)
```

---

## ⚙️ Componenti Script

### TrackSegmentController
- ✅ Anima il glow dei materiali emissivi
- ✅ Pulsazione lenta per atmosfera sci-fi
- ✅ Velocità configurabile (glowSpeed)
- ✅ Intensità min/max

**Utilizzo**: Aggiunto automaticamente al prefab track

### PlayerCubeController
- ✅ Movimento avanti continuo
- ✅ Sistema corsie (3 colonne)
- ✅ Controlli: A/D o Frecce per cambiare corsie
- ✅ Cambio corsia smooth
- ✅ Animazione glow sui materiali
- ✅ Rigidbody con velocità controllata

**Utilizzo**: Aggiunto automaticamente al prefab player

---

## 🎮 Controlli Player

| Tasto | Azione |
|-------|--------|
| **A** / **← Freccia** | Sposta a sinistra |
| **D** / **→ Freccia** | Sposta a destra |

---

## 📐 Dimensioni

| Elemento | Valore |
|----------|--------|
| Larghezza totale pista | 9 unità |
| Larghezza corsie | 3 unità cad. |
| Lunghezza segmento | 10 unità |
| Altezza bordi | 0.5 unità |
| Player cubo | 1x1x1 unità |

---

## 🌟 Effetti Visivi

### Track
- **Floor**: Metallico scuro con riflessi, sensazione di nave spaziale
- **Lane Dividers**: Emissione blu neon, pulsazione lenta
- **Side Lights**: Emissione viola/blu, pulsazione indipendente
- **Bordi**: Nero lucido per contrasto

### Player
- **Core**: Metallo scuro base
- **Corner Glow**: Sfere luminose cyan animate
- **Stripes**: Accenti blu luminosi
- **Pulsazione**: Effetto sci-fi continuo

---

## 🔧 Come Personalizzare

### Cambiare colori materiali
1. Seleziona il materiale in `Assets/Materials/`
2. Modifica `_BaseColor` per il colore base
3. Modifica `_EmissionColor` per il glow
4. Personalizza Metallic/Smoothness

### Cambiare velocità glow
- **Track**: TrackSegmentController → `glowSpeed`
- **Player**: PlayerCubeController → `glowPulseSpeed`

### Cambiare velocità movimento
- PlayerCubeController → `moveSpeed`

### Aggiungere effetti particellari
- Nel prefab track, in `GlowParticles`, aggiungi:
  - Particle System con emissione blu/viola
  - Trail renderer sul player

---

## 📦 File Generati

```
Assets/
├── Editor/
│   ├── TrackGenerator.cs
│   └── PlayerCubeGenerator.cs
├── Materials/
│   ├── Track/
│   │   ├── MAT_FloorMetal_Dark.mat
│   │   ├── MAT_Border_BlackSteel.mat
│   │   ├── MAT_LaneGlow_Blue.mat
│   │   ├── MAT_SideGlow_Purple.mat
│   │   └── MAT_Panel_Gray.mat
│   └── Player/
│       ├── MAT_Player_CoreMetal.mat
│       ├── MAT_Player_Accent.mat
│       └── MAT_Player_Glow.mat
├── Prefabs/
│   ├── Track/
│   │   └── TrackSegment_01.prefab
│   └── Player/
│       └── PlayerCube.prefab
└── Scripts/
    ├── TrackSegmentController.cs
    └── PlayerCubeController.cs
```

---

## 💡 Suggerimenti Prossimi

1. **Ambiente Spaziale**
   - Skybox nero con stelle
   - Fog minimo per profondità

2. **Effetti Particellari**
   - Emissioni dal track durante il gioco
   - Trail sul player mentre corre

3. **Audio**
   - Suoni sci-fi low-fi per l'ambiente
   - Feedback audio per cambio corsie

4. **Level Progression**
   - Duplica TrackSegment_01 per creare livelli
   - Aumenta moveSpeed man mano

5. **Polishing**
   - Aggiungi bloom post-processing
   - Color grading ciano/blu

---

## 🎯 Pronto a Usare!

Tutto è **completamente modularizzato** e **configurabile in Inspector**.

Non è necessario fare niente in più per avere una track completamente renderizzabile con tema spaziale futuristico!
