# [Game Name]

A stylized 3D fantasy RPG built in Unity.

This project is an exploration of building a scalable open-world RPG as a small-team/independent development project. The game's visual style is intentionally low-poly and stylized, allowing environments, creatures, items, and other assets to be produced at a sustainable pace while maintaining a cohesive artistic direction.

The goal is to create a world that feels large and inhabited through **asset variety, environmental detail, exploration, and systemic gameplay**, rather than relying on highly detailed individual assets.

> **Project Status:** Early Development / Prototype

---

## 🎮 About the Game

The game is a first-person fantasy RPG set in a stylized open world.

The current prototype features a village surrounded by forests and mountains, with paths leading into the wilderness and toward a dungeon. Players will be able to explore the world, discover items, fight enemies, interact with NPCs, and eventually develop their character through equipment, progression, quests, and other RPG systems.

The game is being designed around the idea that a small development team can create a large-feeling RPG by building a **reusable library of stylized assets and data-driven systems**.

---

## 🎨 Art Direction

The game uses a stylized low-poly aesthetic as a deliberate design and production philosophy.

Rather than pursuing photorealistic assets, the project emphasizes:

* Strong silhouettes
* Simple, readable geometry
* Cohesive stylization
* Environmental composition
* Large quantities of reusable assets
* Variety and environmental density
* Handmade world-building

This approach allows assets to be created at a reasonable pace while still supporting a world with a large amount of visual variety.

Buildings, furniture, weapons, creatures, vegetation, props, and environmental elements are designed to work together as a consistent visual language.

---

## 🛠️ Current Systems

The project currently contains several foundational systems, including:

* First-person player controller
* Inventory system
* Collectible/interactable objects
* Scriptable item system
* Item tooltips
* Markup support for item names and descriptions
* 3D environment and test level
* Village environment
* Forest and mountain environments
* Dungeon environment
* Basic combat system
* Basic enemy AI
* Boar enemy prototype
* Enemy chasing/leashing behavior
* Enemy idle/wandering behavior
* Blender-created custom models and assets

The project is currently transitioning from primarily environment/system prototyping into **gameplay development**.

---

# 🗺️ Roadmap

The roadmap is intentionally flexible. Features may be redesigned, postponed, combined, or removed as development progresses.

### 🚧 In Progress

* [ ] **Combat System**

  * Player attacks
  * Damage and health
  * Enemy attacks
  * Combat feedback
  * Expanding the combat foundation

* [ ] **AI Enemies**

  * Enemy detection
  * Chasing
  * Combat behavior
  * Leashing
  * Idle/wandering behavior
  * Additional enemy behaviors

### 📋 Planned

* [ ] **Scriptable Enemies**

  * Data-driven enemy definitions
  * Reusable enemy configurations
  * Stats, abilities, loot, and other enemy properties

* [ ] **AI NPCs**

  * NPC behavior
  * World interaction
  * Basic schedules and/or routines

* [ ] **Equipment Inventory Revamp**

  * Equipment slots
  * Weapons and armor
  * Stat integration
  * Improved item management

* [ ] **Dialogue System**

  * NPC dialogue
  * Dialogue choices
  * Data-driven dialogue

* [ ] **Leveling System**

  * Experience
  * Character levels
  * Stat progression

* [ ] **Quest System**

  * Objectives
  * Rewards
  * Quest tracking
  * Quest progression

* [ ] **Scriptable Quests**

  * Data-driven quest definitions
  * Reusable quest objectives
  * Flexible quest configuration

### 🔮 Possible / Far Future

These features are ideas rather than commitments and will depend heavily on how the core game develops.

* [ ] **Magic System**

  * Spells
  * Magical abilities
  * Magic progression

* [ ] **Classes**

  * Character archetypes
  * Class-specific abilities
  * Potential specialization

* [ ] **Races**

  * Playable races
  * Racial traits
  * Potential racial abilities

* [ ] **Professions**

  * Crafting
  * Gathering
  * Trade skills
  * Specialized progression

* [ ] **Multiplayer**

  * Cooperative gameplay
  * Networking
  * Multiplayer progression

Multiplayer is considered a particularly long-term possibility and is **not currently part of the core development plan**.

---

# 🏗️ Development Philosophy

The project is being developed with a strong emphasis on **data-driven and reusable systems**.

Where practical, game content should be represented as configurable data rather than requiring unique code for every individual object.

For example, items and enemies can be defined through reusable ScriptableObject-based data, allowing new content to be created without repeatedly rebuilding the underlying systems.

The long-term goal is to establish systems where creating a new:

* Item
* Enemy
* Quest
* NPC
* Ability
* Weapon
* Equipment piece

requires primarily **content creation and configuration rather than new systems programming**.

This should allow the game to grow in scope without the complexity of the codebase growing at the same rate.

---

# 🌲 World Design

The world is intended to emphasize exploration and environmental continuity.

The current prototype demonstrates a basic structure:

**Village → Wilderness → Dungeon**

The eventual world may expand this concept into multiple settlements, wilderness regions, dungeons, and other locations connected through a larger open world.

Environmental storytelling and world detail will be important components of the game's design.

---

# 💻 Technology

* **Engine:** Unity
* **3D Modeling:** Blender
* **Programming:** C#
* **Version Control:** Git / GitHub
* **Asset Architecture:** ScriptableObject-based systems where appropriate

---

# 📁 Project Structure

The Unity project is organized around the game's major systems and content.

As development continues, the project will prioritize modularity and separation between:

* Gameplay systems
* Data definitions
* UI
* AI
* World/environment
* Art assets
* Audio
* Player systems

The exact structure will evolve as the project grows.

---

# 🚧 Development Status

This is an active development project and should be considered **pre-alpha/prototype software**.

Systems are likely to change substantially during development. Existing mechanics, assets, interfaces, and architecture may be replaced or redesigned as the project evolves.

The roadmap represents the current direction rather than a fixed list of promised features.

---

## 📜 License

License information will be added as the project develops.

All original assets and game content are the property of the project unless otherwise noted.
