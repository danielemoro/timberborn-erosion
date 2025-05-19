# Timberborn Modding: A Comprehensive Guide

This guide provides a comprehensive overview of modding for Timberborn, covering game architecture, setup, core concepts, and various modding techniques.

## Table of Contents

1.  [Introduction to Timberborn Modding](#1-introduction-to-timberborn-modding)
2.  [Understanding Timberborn's Architecture](#2-understanding-timberborns-architecture)
    *   [Key Game Systems](#key-game-systems)
    *   [Important Assemblies & Namespaces](#important-assemblies--namespaces)
3.  [Setting Up Your Modding Environment](#3-setting-up-your-modding-environment)
    *   [Prerequisites](#prerequisites)
    *   [Unity Quick Start](#unity-quick-start)
    *   [Installing Modding Tools Only (Advanced)](#installing-modding-tools-only-advanced)
4.  [Core Mod Structure and Concepts](#4-core-mod-structure-and-concepts)
    *   [Mod Directory Structure](#mod-directory-structure)
    *   [The Mod Manifest (`manifest.json`)](#the-mod-manifest-manifestjson)
    *   [Assembly Definition Files (`.asmdef`)](#assembly-definition-files-asmdef)
    *   [Compatibility Versions](#compatibility-versions)
5.  [Modding Techniques](#5-modding-techniques)
    *   [C# Scripting (`IModStarter`, DLL Loading)](#c-scripting-imodstarter-dll-loading)
    *   [Harmony Patching](#harmony-patching)
    *   [Dependency Injection with Bindito.Core](#dependency-injection-with-binditocore)
    *   [Working with Game Object Templates (Decorators)](#working-with-game-object-templates-decorators)
    *   [Data-Driven Modding (Blueprints)](#data-driven-modding-blueprints)
    *   [Asset Modding (Images, Models, AssetBundles)](#asset-modding-images-models-assetbundles)
    *   [Localization](#localization)
6.  [Building and Managing Mods](#6-building-and-managing-mods)
    *   [Using the Mod Builder](#using-the-mod-builder)
    *   [The In-Game Mod Manager](#the-in-game-mod-manager)
7.  [Debugging and Logging](#7-debugging-and-logging)
8.  [Community Resources](#8-community-resources)
9.  [Case Study: Extending the Hello World Mod](#9-case-study-extending-the-hello-world-mod)

## 1. Introduction to Timberborn Modding

Timberborn, built in Unity, offers a rich platform for modding. Mods can extend gameplay, add new content, alter existing mechanics, or enhance the user interface. This guide aims to equip you with the knowledge to create your own mods.

Timberborn's modding capabilities include:
*   Adding new buildings, goods, and beaver needs.
*   Modifying game logic through C# scripting.
*   Overriding game data like blueprints and recipes using JSON.
*   Introducing custom assets like textures, models, and UI elements.

## 2. Understanding Timberborn's Architecture

Timberborn is a tile-based city-building game with deterministic simulation systems, primarily written in C#.

### Key Game Systems
*   **World & Block System**: The game world is a grid of tiles (`TileCoord`). The `WorldBlockManager` handles block data (type, health, etc.) on these tiles.
*   **Water Simulation**: Managed by `WaterFlowManager`, this system calculates water depth and flow rates across tiles in discrete ticks.
*   **Data-Driven Design**: Many game elements (buildings, needs, goods) are defined by "Blueprints" (JSON files), which mods can easily override or extend.
*   **Entity System**: Beavers, buildings, and other dynamic objects are managed as entities with components.

### Important Assemblies & Namespaces
*   **Game Assemblies (found in `Timberborn/<version>/Timberborn_Data/Managed/`)**:
    *   `Assembly-CSharp.dll`: Contains the core game code (e.g., `Timberborn.*` namespaces).
    *   `Timberborn.ModdingTools.dll`: Provides official mod entry points like `IModStarter`.
    *   Unity Engine DLLs (e.g., `UnityEngine.Core.dll`): Timberborn is built on Unity.
*   **Key Namespaces**:
    *   `Timberborn.World`: For world grid, block management.
    *   `Timberborn.Water`: For fluid simulation.
    *   `Timberborn.DataStructures`: For `TileCoord`, utility collections.
    *   `Timberborn.UI`, `Timberborn.CoreUI`, `Timberborn.UILayoutSystem`: For UI elements, windows, and layout.
    *   `Timberborn.Input`: For block selection, camera controls.
    *   `Timberborn.SingletonSystem`: For game-wide singleton services.
    *   `Timberborn.TemplateSystem`: For working with prefabs/templates.
    *   `Bindito.Core`: A dependency injection framework used extensively.

## 3. Setting Up Your Modding Environment

### Prerequisites
*   Unity Hub and the Unity Editor version matching Timberborn (check `ProjectSettings/ProjectVersion.txt` in the official modding repository).
*   Windows/macOS Build Support modules for Unity (install via Unity Hub).
*   Git for cloning repositories.
*   A code editor like Visual Studio or Rider.

### Unity Quick Start
1.  Download the correct Unity version. Ensure "Windows Build Support" and "Mac Build Support" (if applicable) are installed.
2.  Clone the official `mechanistry/timberborn-modding` repository: `git clone https://github.com/mechanistry/timberborn-modding.git`
3.  Add the cloned project to Unity Hub. **Do not open it yet.**
4.  In Unity Hub, for the project, add the command line argument: `-disable-assembly-updater` (prevents unwanted script updates).
5.  Open the project in Unity. If prompted about Safe Mode, select "Ignore."
6.  A prompt should appear to import Timberborn DLLs. Select your Timberborn game installation folder.
    *   This can be manually triggered via "Timberborn" > "Import Timberborn dlls" in the Unity menu.
7.  Wait for import completion.
8.  Access the Mod Builder via "Timberborn" > "Show Mod Builder" to compile and package mods.

### Installing Modding Tools Only (Advanced)
Alternatively, to add modding tools to an existing Unity project:
1.  Open Unity Package Manager.
2.  Select "Add package from git URL..."
3.  Use: `https://github.com/mechanistry/timberborn-modding.git?path=/Assets/Tools`
    *   Update this package periodically.

## 4. Core Mod Structure and Concepts

### Mod Directory Structure
Mods are typically stored in `Documents/Timberborn/Mods/`, each within its own subfolder.
Example:
```
Documents/
└── Timberborn/
    └── Mods/
        └── YourModName/
            ├── AssetBundles/
            │   └── YourModAssets.assets  (Platform-specific: _win, _mac)
            ├── Blueprints/
            │   ├── Goods/
            │   │   └── Good.YourCustomGood.json
            │   └── Buildings/
            │       └── YourBuilding.json
            ├── Localizations/
            │   └── enUS_YourModName.csv
            ├── Sprites/
            │   └── YourIcon.png
            ├── YourModCode.dll
            └── manifest.json
```

### The Mod Manifest (`manifest.json`)
This JSON file is mandatory for every mod, located in its root folder. It provides metadata for the game.

**Example:**
```json
{
  "Name": "My Awesome Mod",
  "Version": "1.0.0",
  "Id": "MyName.MyAwesomeMod",
  "MinimumGameVersion": "0.5.0.0",
  "Description": "This mod adds awesome new features.",
  "RequiredMods": [
    {
      "Id": "SomeOtherMod.CoreLibrary",
      "MinimumVersion": "1.2.0"
    }
  ],
  "OptionalMods": [
    {
      "Id": "AnotherMod.ExtraFeatureSupport"
    }
  ]
}
```
*   **`Name`**: Human-readable name.
*   **`Version`**: Your mod's version (e.g., "0.1", "1.0.0").
*   **`Id`**: A unique identifier (e.g., `YourName.ModName`). Crucial for dependencies.
*   **`MinimumGameVersion`**: The oldest Timberborn version this mod supports.
*   **`Description`**: A short description of the mod.
*   **`RequiredMods`**: Array of mods that *must* be present. Missing required mods will show a warning.
    *   `Id`: The unique ID of the required mod.
    *   `MinimumVersion` (optional): The minimum version of the required mod.
*   **`OptionalMods`**: Array of mods that this mod can integrate with but are not essential.

### Assembly Definition Files (`.asmdef`)
For C# mods, `.asmdef` files (Unity feature) define how scripts compile into a .NET assembly (`.dll`). They are JSON files typically placed in your mod's `Scripts` folder.

**Example (`Timberborn.ModExamples.HelloWorld.asmdef`):**
```json
{
  "name": "Timberborn.ModExamples.HelloWorld",
  "rootNamespace": "Mods.HelloWorld.Scripts",
  "references": [
    "GUID:...", // Reference to Bindito.Core.dll.asmdef if it exists
    "GUID:..."  // Reference to Timberborn.GameShared.dll.asmdef etc.
    // Or by assembly name if they are precompiled and not in project with .asmdef
    // "Timberborn.Core", "UnityEngine.CoreModule" 
  ],
  "includePlatforms": [],
  "excludePlatforms": [],
  "allowUnsafeCode": true,
  "overrideReferences": false, // Usually false for mods
  "precompiledReferences": [ // For DLLs not part of Unity project with .asmdef
      "Bindito.Core.dll",
      "Timberborn.GameCommandSystem.dll",
      // Add other Timberborn.*.dlls your mod directly uses
  ],
  "autoReferenced": false, // **CRITICAL for mods: set to false**
  "defineConstraints": [],
  "versionDefines": [],
  "noEngineReferences": false
}
```
*   **`name`**: The output name of the assembly (e.g., `MyName.MyAwesomeMod`).
*   **`rootNamespace`**: The default namespace for scripts in this assembly.
*   **`references`**: An array of assembly names or GUIDs this assembly depends on. This is how you link to game code (e.g., `Assembly-CSharp`, `Timberborn.Core`) or other libraries.
*   **`precompiledReferences`**: For referencing DLLs directly if they don't have their own `.asmdef` within the Unity project (often the case for game DLLs you import).
*   **`autoReferenced`: `false`**: **Essential for mods.** Prevents the mod's assembly from being automatically referenced by all other assemblies, avoiding conflicts.
*   **`allowUnsafeCode`: `true`**: Allows C# `unsafe` keyword.
*   **GUIDs vs Names**: When referencing other `.asmdef` files within the same Unity project, Unity often uses GUIDs. For precompiled DLLs (like the game's), you might list their file names (without `.dll`) in `precompiledReferences`.

### Compatibility Versions
To support multiple game versions (e.g., Stable and Experimental) with different mod builds:
Place version-specific mod files in subfolders named `version-X.Y.Z.W` (e.g., `version-0.6.8.4`).
The game loads the mod from the subfolder closest to (but not exceeding) the current game version.
If any `version-X` folder exists, the root mod folder's content (for that mod type - e.g. DLLs, asset bundles) is ignored.
```
Documents/Timberborn/Mods/YourModName/
├── version-0.6.0.0/
│   ├── YourModCode.dll
│   └── manifest.json (can be specific if needed)
└── version-0.7.0.0/
    ├── YourModCode.dll
    └── manifest.json
```

## 5. Modding Techniques

### C# Scripting (`IModStarter`, DLL Loading)
The game loads all `.dll` files from a mod's folder and its subfolders.
To execute code when the mod loads, implement the `IModStarter` interface (from `Timberborn.ModdingTools.dll`).

**Example:**
```csharp
// In YourModCode.dll
using Timberborn.ModdingTools;
using UnityEngine; // For Debug.Log, if needed for basic logging

public class MyModStarter : IModStarter {
    public void StartMod(IModEnvironment modEnvironment) {
        // Your mod's initialization code here.
        // For example, apply Harmony patches, register services.
        Debug.Log($"[{modEnvironment.ModManifest.Name}] has started!");
        // Use modEnvironment.Logger for dedicated mod logging:
        // modEnvironment.Logger.Info("This is an info message from MyModStarter.");
    }
}
```
*   The class implementing `IModStarter` needs a parameterless public constructor.
*   `IModEnvironment` provides access to the mod's directory, manifest, and a dedicated logger.

### Harmony Patching
Harmony is a library for runtime method patching (modifying C# methods at runtime without altering original DLLs). It's widely used in Timberborn modding.

**Core Concepts:**
*   **Patch Types**:
    *   `Prefix`: Runs *before* the original method. Can alter arguments or skip the original.
    *   `Postfix`: Runs *after* the original method. Can alter the result.
    *   `Transpiler`: Modifies the IL (Intermediate Language) of the original method. More complex.
*   **Process**:
    1.  Create a `Harmony` instance with a unique ID: `var harmony = new HarmonyLib.Harmony("your.mod.id");`
    2.  Find the target method using `AccessTools.Method(typeof(TargetClass), "MethodName", parameters_if_overloaded)`.
    3.  Apply patches: `harmony.Patch(originalMethod, prefix: new HarmonyMethod(typeof(MyPatchClass), nameof(MyPrefixMethod)));`

**Example (from Erosion Mod Tutorial - conceptual):**
```csharp
using HarmonyLib;
using Timberborn.Water; // For WaterFlowManager
using Timberborn.DataStructures; // For TileCoord
using Timberborn.ModdingTools; // For IModStarter, IModEnvironment

public class ErosionModPatcher : IModStarter {
    public void StartMod(IModEnvironment env) {
        var harmony = new Harmony("com.myname.erosionmod");
        env.Logger.LogInfo("ErosionMod: Patching WaterFlowManager.UpdateFlow...");
        harmony.Patch(
            original: AccessTools.Method(
                typeof(WaterFlowManager),
                "UpdateFlow", // Ensure this method exists and parameters match if any
                new System.Type[] { typeof(TileCoord) } // Example: if UpdateFlow takes a TileCoord
            ),
            postfix: new HarmonyMethod(
                typeof(ErosionModPatcher), // Class containing the patch method
                nameof(UpdateFlowPostfix)   // Name of the patch method
            )
        );
    }

    // This method will run after every call to WaterFlowManager.UpdateFlow(TileCoord coord)
    public static void UpdateFlowPostfix(TileCoord coord, WaterFlowManager __instance) {
        // __instance refers to the instance of WaterFlowManager if UpdateFlow is not static
        // coord is the argument passed to the original UpdateFlow method.
        // Your custom logic here, e.g., ErosionManager.Instance.ProcessTile(coord);
        // UnityEngine.Debug.Log($"Water updated for tile: {coord}");
    }
}
```
*   Patch methods (`UpdateFlowPostfix`) can access original method arguments by name, or special names like `__result` (for postfixes, to get/set the return value) and `__instance` (to get the instance the original method was called on, if not static).

### Dependency Injection with Bindito.Core
Timberborn uses `Bindito.Core` for dependency injection, promoting decoupled code.

**Key Components:**
*   **`IConfigurator` / `Configurator`**: Implement these to define bindings. `Configurator` is a base class providing `Configure()` to override, while `IConfigurator` has `Configure(IContainerDefinition definition)`.
*   **`[Context("...")]`**: Attribute specifying when the configurator runs (e.g., `"Game"`, `"MainMenu"`, `"Global"`).
*   **`IContainerDefinition`**: Used in `IConfigurator` to define bindings.
*   **Bindings**: `containerDefinition.Bind<InterfaceType>().To<ImplementationType>().AsSingleton();`
*   **Injection**: Dependencies are injected via constructors or `[Inject]`-annotated methods/properties.

**Example: Binding and Constructor Injection (HelloWorld mod)**
```csharp
// HelloWorldConfigurator.cs
using Bindito.Core;
using Mods.HelloWorld.Scripts; // Assuming HelloWorldInitializer is in this namespace

namespace Mods.HelloWorld.Configuration { // Example namespace
  [Context("Game")]
  public class HelloWorldConfigurator : IConfigurator {
    public void Configure(IContainerDefinition containerDefinition) {
      containerDefinition.Bind<HelloWorldInitializer>().AsSingleton();
    }
  }
}

// HelloWorldInitializer.cs
using Timberborn.CoreUI;
using Timberborn.UILayoutSystem;
using Timberborn.SingletonSystem;

namespace Mods.HelloWorld.Scripts {
  public class HelloWorldInitializer : ILoadableSingleton {
    private readonly UILayout _uiLayout;
    private readonly VisualElementLoader _visualElementLoader;

    // Dependencies injected by Bindito
    public HelloWorldInitializer(UILayout uiLayout, VisualElementLoader visualElementLoader) {
      _uiLayout = uiLayout;
      _visualElementLoader = visualElementLoader;
    }
    public void Load() {
      var visualElement = _visualElementLoader.LoadVisualElement("HelloWorld"); // Assumes a UI asset
      _uiLayout.AddBottomRight(visualElement, 0);
    }
  }
}
```

### Working with Game Object Templates (Decorators)
The `TemplateModule` system allows modifying game prefabs (templates) by adding "decorators." This is useful for attaching custom components to existing or new types of game objects.

**Example (ShantySpeaker mod):**
```csharp
// ShantySpeakerConfigurator.cs
using Bindito.Core;
using Timberborn.TemplateSystem;
using Mods.ShantySpeaker.Scripts; // Assuming your components are here

namespace Mods.ShantySpeaker.Configuration { // Example namespace
  [Context("Game")]
  public class ShantySpeakerConfigurator : Configurator { // Note: extends Configurator
    protected override void Configure() {
      MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
    }

    private static TemplateModule ProvideTemplateModule() {
      var builder = new TemplateModule.Builder();
      // If a prefab has FinishableBuildingSoundPlayerSpec, also add FinishableBuildingSoundPlayer
      builder.AddDecorator<FinishableBuildingSoundPlayerSpec, FinishableBuildingSoundPlayer>();
      return builder.Build();
    }
  }
}
```

### Data-Driven Modding (Blueprints)
Modify or add game elements like buildings, goods, needs, and recipes using JSON files called Blueprints. Place them in your mod's `Blueprints/` subfolder (e.g., `Blueprints/Goods/`).

*   **New Blueprints**: If a JSON file path doesn't match an existing game blueprint, it's treated as new content.
*   **Modifying Existing Blueprints**: If the path matches an existing blueprint:
    *   Fields in your JSON replace original values.
    *   Omitted fields retain original values.
    *   List fields:
        *   Default: Overwritten.
        *   `"FieldName#append": [...]`: Elements are added to the original list.
        *   `"FieldName#remove": [...]`: Specified elements are removed.
*   **Optional Blueprints**: Filename ending with `.optional.json` (e.g., `Need.Beaver.Fun.optional.json`). Modifies an existing blueprint *only if* it's already present (from base game or another mod).

**Example: Modifying Carrots**
Place in `YourMod/Blueprints/Goods/Good.Carrot.json`:
```json
{
  "GoodSpec": {
    "ConsumptionEffects#append": [ // Add a new effect
      {
        "NeedId": "Thirst",
        "Points": 0.3
      }
    ],
    "Weight": 2 // Change existing weight
  }
}
```

### Asset Modding (Images, Models, AssetBundles)
*   **Images (`.png`, `.jpg`)**:
    *   Place in mod directory (e.g., `Sprites/MyIcon.png`).
    *   Overrides game images if path matches.
    *   Control import settings with a `.meta.json` file (e.g., `Sprites/MyIcon.png.meta.json`).
*   **3D Models**: Timberborn uses a custom `.timbermesh` format. See official docs for creation.
*   **AssetBundles**:
    *   Package custom assets (prefabs, materials, sounds, models, etc.) built in Unity.
    *   Place in `AssetBundles/` subfolder of your mod.
    *   Platform-specific bundles: `MyBundle_win.assets`, `MyBundle_mac.assets`.
    *   Assets in bundles can also include Blueprints (JSON files) or other mod data.

### Localization
Add or modify in-game text using CSV files.
*   Store in `Localizations/` subfolder (e.g., `MyMod/Localizations/enUS_MyModTexts.csv`).
*   Format: `ID,Text,Comment`
    ```csv
    MyMod.NewBuilding.Name,"My Awesome Building","Building name"
    MyMod.NewBuilding.Description,"Constructs awesome things.","Tooltip text"
    ```
*   Use these loc keys in Blueprints or code.
*   To add new languages, translate official game files and follow naming conventions (e.g., `noNO_MyMod.csv`).

## 6. Building and Managing Mods

### Using the Mod Builder
In Unity (with the Timberborn Modding Project setup):
1.  Go to "Timberborn" > "Show Mod Builder" from the toolbar.
2.  Select mods to build (or all).
3.  The builder compiles code, creates AssetBundles, and places the packaged mod into `Documents/Timberborn/Mods/`.
    *   Requires Mac/Windows Build Support in Unity for cross-platform AssetBundles.

### The In-Game Mod Manager
*   Accessible from the main menu or shown on startup if mods are detected.
*   Allows enabling/disabling mods and changing load order.
*   Dependencies from `manifest.json` influence default load order.

## 7. Debugging and Logging
*   **`IModEnvironment.Logger`**: Use this in your `IModStarter` or other C# classes to write to a dedicated mod log file (usually `Timberborn/Logs/Mods.log` or similar).
    *   `env.Logger.Info("Message")`, `env.Logger.Warning("Message")`, `env.Logger.Error("Message")`.
*   **Unity `Debug.Log()`**: Messages appear in Timberborn's `Player.log` and the Unity Editor console (if attached).
*   **Harmony Debugging**: Can be complex. Use logging extensively. Consider `AccessTools.GetMethodDelegate` for easier calling of private methods if needed for inspection.
*   **Visual Studio/Rider Debugger**: Attach to the Timberborn process.
    1.  In Unity Editor, ensure Script Debugging is enabled.
    2.  Launch Timberborn.
    3.  In your IDE, use "Attach to Process" and select Timberborn.exe. Set breakpoints in your mod's C# code. (This works best for code that runs after initial loading, like UI interactions or game event handlers. Patching code itself might be harder to debug this way.)

## 8. Community Resources
*   **Official Timberborn Discord**: ([https://discord.gg/timberborn](https://discord.gg/timberborn)) - #modding channel is key.
*   **Timberborn Wiki**: ([https://timberborn.wiki.gg/wiki/Creating_Mods](https://timberborn.wiki.gg/wiki/Creating_Mods))
*   **GitHub `mechanistry/timberborn-modding`**: Official examples and tools.

## 9. Case Study: Extending the Hello World Mod

This section details practical lessons learned while extending the basic Hello World mod to add keyboard input handling and dynamic UI updates.

### Key Challenges and Solutions

#### 1. Input System in Timberborn

Timberborn uses Unity's new Input System package instead of the legacy Input class. Attempts to use the old system will result in runtime errors:

```
InvalidOperationException: You are trying to read Input using the UnityEngine.Input class, but you have switched active Input handling to Input System package in Player Settings.
```

**Solution**: Import the `UnityEngine.InputSystem` namespace and use the new API:

```csharp
// Add reference to Unity.InputSystem in your .asmdef file

// Old approach (will fail)
if (Input.GetKeyDown(KeyCode.R)) { /* ... */ }

// New approach
private Keyboard _keyboard;

private void Awake() {
    _keyboard = Keyboard.current;
}

private void Update() {
    // Ensure keyboard is available
    if (_keyboard == null) {
        _keyboard = Keyboard.current;
        if (_keyboard == null) return;
    }
    
    if (_keyboard.rKey.wasPressedThisFrame) {
        // Handle key press
    }
}
```

#### 2. Dependency Injection and Lifecycle

Timberborn uses Bindito.Core for dependency injection. To properly integrate with the game:

1. Create a Configurator class annotated with `[Context("Game")]`
2. Implement lifecycle interfaces like `ILoadableSingleton` and `IUpdatableSingleton` for initialization and update calls
3. Use constructor injection to get required services

**Example**:
```csharp
[Context("Game")]
public class MyModConfigurator : IConfigurator {
    public void Configure(IContainerDefinition containerDefinition) {
        containerDefinition.Bind<MyModService>().AsSingleton();
    }
}

public class MyModService : ILoadableSingleton, IUpdatableSingleton {
    // Constructor injection
    public MyModService(UILayout uiLayout, VisualElementLoader visualElementLoader) {
        // Store dependencies
    }
    
    // Called after instantiation
    public void Load() {
        // Initialize your mod
    }
    
    // Called every frame
    public void UpdateSingleton() {
        // Handle updates
    }
}
```

#### 3. UI Management

Timberborn uses Unity's UIElements system for its UI. To create and update UI elements:

1. Use the `VisualElementLoader` to load UI templates defined in XML (UXML)
2. Create elements procedurally with proper styling
3. Add elements to the layout with `UILayout.AddBottomRight()` or similar methods

**Example for dynamic UI updates**:
```csharp
private readonly UILayout _uiLayout;
private readonly VisualElementLoader _visualElementLoader;
private VisualElement _rootElement;
private Label _infoLabel;

public void Load() {
    // Load template
    _rootElement = _visualElementLoader.LoadVisualElement("HelloWorld");
    
    // Create new element
    _infoLabel = new Label("Initial text");
    
    // Style the element
    _infoLabel.style.color = new StyleColor(new Color(1f, 1f, 0.8f));
    _infoLabel.style.fontSize = 14;
    _infoLabel.style.paddingTop = 5;
    // ... other styling ...
    
    // Add to UI tree and layout
    _rootElement.Add(_infoLabel);
    _uiLayout.AddBottomRight(_rootElement, 0);
}

// Later, to update:
_infoLabel.text = "Updated text";
_infoLabel.style.backgroundColor = new StyleColor(new Color(0.8f, 0.2f, 0.2f, 0.9f));
```

#### 4. Assembly Definition (.asmdef) Structure

A properly configured .asmdef file is crucial for mod development:

```json
{
  "name": "YourMod.Name",
  "rootNamespace": "YourMod.Namespace",
  "references": [
    "Timberborn.CoreUI",
    "Timberborn.SingletonSystem",
    "Timberborn.UILayoutSystem",
    "Unity.InputSystem",
    // Other necessary references
  ],
  "precompiledReferences": [
    "Bindito.Core.dll",
    "Timberborn.ModdingTools.dll"
  ],
  "autoReferenced": false
}
```

Key points:
- `autoReferenced: false` prevents namespace collisions
- Include Unity.InputSystem for input handling
- Include Timberborn namespaces for services you need
- List Bindito.Core.dll in precompiledReferences

#### 5. Debugging and Logging

For effective debugging:

- Use `Debug.Log()` with clear prefixes for filtering: `[YourMod] Message`
- Create a simple helper logger class for consistent formatting
- When troubleshooting UI, verify that configurator and load methods are being called
- Use multiple approaches simultaneously when troubleshooting input issues

### Summary of Best Practices

1. **Follow Timberborn's architecture**: Use its lifecycle interfaces, dependency injection, and UI system.
2. **Use the correct input system**: Always use the new Unity Input System package.
3. **Keep direct references**: Store references to UI elements you'll need to update later.
4. **Add proper debugging**: Use appropriate logging to diagnose issues.
5. **Build incrementally**: Start with minimal functionality and add features progressively.
6. **Understand the UI hierarchy**: Elements must be properly added to the visual tree.
7. **Use simple approaches first**: A direct text update is more reliable than complex nested UI.

The Hello World mod is an excellent starting point for understanding how Timberborn's modding system works. Build on this foundation by carefully following the patterns established in the examples. Happy modding! 