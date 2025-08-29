# 🌍 Procedural World Generation System for Unity

A **comprehensive and performant system** for generating vast, dynamic, and endless 3D worlds in **Unity**.  
Built with a **modular, data-driven architecture**, the system features **procedural terrain**, **object spawning**, **dynamic NavMesh baking**, and **resource-aware UI**, all optimized with **multi-threading** and **Level of Detail (LOD)** systems.

---

## ✨ Core Features

- **Endless Terrain Generation**  
  Generates new terrain chunks as the player explores, giving the illusion of an infinite world.  
  Controlled by customizable `ScriptableObject` assets.

- **Level of Detail (LOD) System**  
  High-detail meshes for nearby terrain and low-polygon meshes for distant chunks ensure a smooth frame rate.

- **Procedural Object Spawning**  
  Intelligent placement of trees, houses, enemies, and more using **Poisson Disc Sampling**.  
  Rules include slope, height, and texture-based distribution.

- **Dynamic NavMesh Baking**  
  Automatic NavMesh generation per chunk after terrain and objects are spawned, enabling **seamless AI navigation**.

- **Data-Driven Architecture**  
  Full customization with `ScriptableObject` assets (`TerrainData`, `NoiseData`, `ObjectData`, etc.), allowing biome variations without changing code.

- **UI & Resource Management**  
  Animated UI popups for resources like wood, coins, and stone, managed by a flexible `UIManager`.

---

## ⚙️ How It Works

### 1. **Chunk Management** (`EndlessTerrain.cs`)
- Tracks player position and determines visible chunks.  
- Creates and recycles terrain chunks dynamically.  
- Assigns different LOD meshes based on player distance.

### 2. **Data Generation** (`MapGenerator.cs`)
- Multi-threaded heightmap, mesh, and object placement data generation.  
- Reads parameters from ScriptableObjects.  
- Processes generation queues on the main thread to prevent stutter.

### 3. **Object Spawning & NavMesh Baking**
- Uses `Physics.Raycast` for precise object placement.  
- Delays one frame for object registration, then calls `navMeshSurface.BuildNavMesh()`.  

---

## 🛠️ Setup & Usage

### 1. Scene Setup
1. Create a **Player** GameObject with a `Transform`.  
2. Create an empty **Map Generator** object and attach `MapGenerator.cs`.  
3. Attach `EndlessTerrain.cs` to the Player (or a manager). Assign the **Viewer** field.

### 2. Create Data Assets
- In the Project window:  
  `Right-click → Create → [TerrainData / NoiseData / TextureData / ObjectData]`  
- Configure assets for different **biomes** (e.g., Trees, Enemies).

### 3. Assign Assets
- Select **Map Generator** → Drag your ScriptableObject assets into slots.  
- Multiple ObjectData assets can be used for variety.

### 4. Configure LODs
- In **EndlessTerrain**, define detail levels with distance thresholds.  
- Ensure **LOD 0** has **Use For Collider** enabled.

### 5. UI Setup
- Create a **UIManager** object, attach `UIManager.cs`.  
- Add a UI panel with an `Animator` for popups (wood, coin, stone).  
- Assign the animator to `popupAnimator` in UIManager.

---

## 🎮 Key Outcomes

✔️ Stable, **high-performance** procedural world generation.  
✔️ Solved challenges like floating objects, NavMesh race conditions, and overlap issues.  
✔️ Robust system for **explorable, intelligent AI-driven worlds**.  

---

## 📸 Screenshots / Demo
*(Add images or GIFs here once you have in-game screenshots)*

---

