# Timberborn: Game Mechanics & Architecture Overview

This document provides a high-level overview of Timberborn's core game mechanics and software architecture, relevant for context when developing mods. For detailed modding instructions, setup, and API usage, please refer to the [Timberborn Modding: A Comprehensive Guide](modding.md).

## Table of Contents

1.  [Introduction to Timberborn](#1-introduction-to-timberborn)
2.  [Core Game Architecture](#2-core-game-architecture)
    *   [Unity Engine Base](#unity-engine-base)
    *   [Key Game Assemblies](#key-game-assemblies)
    *   [Primary Namespaces](#primary-namespaces)
3.  [Fundamental Game Systems](#3-fundamental-game-systems)
    *   [World and Block System](#world-and-block-system)
    *   [Water Simulation](#water-simulation)
    *   [Beaver Needs and Behavior](#beaver-needs-and-behavior)
    *   [Construction and Production](#construction-and-production)
    *   [Pathfinding](#pathfinding)
    *   [UI System](#ui-system)
4.  [Data-Driven Design: Blueprints](#4-data-driven-design-blueprints)

## 1. Introduction to Timberborn

Timberborn is a city-building game featuring beaver colonies, resource management, and dynamic water physics. Players build and manage settlements in a post-human world where water is paramount.

Key characteristics relevant to modding:
*   **Tile-Based World**: The game operates on a 3D grid of tiles.
*   **Deterministic Simulation**: Game logic updates in discrete, predictable ticks, crucial for systems like water flow and beaver AI.
*   **C# and Unity**: Built with the Unity game engine, with core game logic written in C#.
*   **Extensible Systems**: Many systems are designed to be extendable or overridable by mods.

## 2. Core Game Architecture

### Unity Engine Base
Timberborn leverages the Unity engine for rendering, physics (in a broad sense, though water is custom), asset management, and its component-based GameObject system.

### Key Game Assemblies
Located in `Timberborn/<version>/Timberborn_Data/Managed/`:
*   `Assembly-CSharp.dll`: Contains the majority of the game's unique C# code, organized into `Timberborn.*` namespaces.
*   `Timberborn.ModdingTools.dll`: Official interfaces and tools provided for modders (e.g., `IModStarter`).
*   Various `UnityEngine.*.dll` files: Core Unity engine libraries.
*   Other `Timberborn.*.dll` files: The game is split into multiple assemblies for better organization (e.g., `Timberborn.Core`, `Timberborn.Buildings`, `Timberborn.Characters`, `Timberborn.WaterSystem`).

### Primary Namespaces
Familiarity with these namespaces is beneficial:
*   `Timberborn.World`: Grid system, `TileCoord`, `IBlockService`, terrain data.
*   `Timberborn.Water`: `WaterFlowManager`, water depth, flow calculations.
*   `Timberborn.Characters`: Beaver behavior, needs, pathfinding components.
*   `Timberborn.Buildings`: Building components, construction, production chains.
*   `Timberborn.Goods`: Definitions and management of in-game resources.
*   `Timberborn.Needs`: Beaver needs system.
*   `Timberborn.UI`, `Timberborn.CoreUI`, `Timberborn.UILayoutSystem`: UI windows, elements, and layout management.
*   `Timberborn.Input`: Player input handling, tool interactions.
*   `Timberborn.SingletonSystem`: Access to global game services.
*   `Timberborn.TemplateSystem`: For working with prefabs and applying decorators.
*   `Bindito.Core`: Dependency injection framework.

## 3. Fundamental Game Systems

### World and Block System
*   The world is a grid of `TileCoord` (X, Y coordinates, with Z implied by terrain height).
*   `IBlockService` (often accessed via `WorldBlockManager` or similar injected services) provides methods to get, set, and remove blocks on tiles.
*   `BlockData` stores information about each block, including its type, health, and other metadata.
*   `BlockType` is an enum representing different block types (e.g., Dirt, Water, Log, specific buildings).

### Water Simulation
*   Managed primarily by `WaterFlowManager`.
*   Operates in discrete ticks, updating flow rates and water depths for all relevant tiles.
*   Key properties per tile: flow rate (velocity/volume) and depth (static volume).
*   Simulation considers terrain, water sources, drains, and obstacles.
*   Mods often interact with this system by patching methods on `WaterFlowManager` or related classes.

### Beaver Needs and Behavior
*   Beavers have a set of needs (Hunger, Thirst, Sleep, etc.) that drive their behavior.
*   AI systems manage beaver task selection, resource gathering, construction, and leisure activities.
*   The `NeedManager` and individual need components play a significant role.

### Construction and Production
*   Buildings are constructed by beavers delivering resources.
*   Production buildings transform input goods into output goods based on recipes.
*   These systems involve components on building game objects, inventory management, and work assignments.

### Pathfinding
*   Beavers and (to some extent) resource distribution rely on a pathfinding system to navigate the world map, considering terrain, bridges, and obstacles.

### UI System
*   Timberborn uses a UI system built on Unity's UI Toolkit (formerly UIElements).
*   Mods can add new UI elements, panels, or modify existing ones using services like `UILayout` and `VisualElementLoader`.

## 4. Data-Driven Design: Blueprints

Many game elements, such as buildings, goods, needs, and recipes, are defined using JSON files called "Blueprints."
*   The base game contains default blueprints for all its content.
*   Mods can easily:
    *   **Override** existing blueprints to change their properties.
    *   **Add** new blueprints to introduce new content.
*   This system allows for significant game modification without necessarily writing C# code.
*   Details on how to use blueprints for modding are covered in the main [Timberborn Modding: A Comprehensive Guide](modding.md).

This overview should provide a conceptual map of Timberborn's internals. For practical mod development, consult the detailed modding guide.
