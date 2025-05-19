---
description: 
globs: 
alwaysApply: false
---
# Analysis of Timberborn Example Mods

This document analyzes four example mods for the game Timberborn: HelloWorld, ShantySpeaker, OverwritesExample, and YearOfTheSnakeBeaverTails.

## 1. HelloWorld Mod

*   **Source Files:**
    *   `Assets/Mods/HelloWorld/Scripts/HelloWorldConfigurator.cs`
    *   `Assets/Mods/HelloWorld/Scripts/HelloWorldInitializer.cs`
    *   `Assets/Mods/HelloWorld/Scripts/HelloWorldLogger.cs`
    *   `Assets/Mods/HelloWorld/Scripts/Timberborn.ModExamples.HelloWorld.asmdef`

*   **What it does:**
    *   Logs a "Hello World" message to the `Player.log` file when the mod starts. The log message includes the full path to the log file.
    *   Adds a simple UI element (loaded from a visual element template named "HelloWorld") to the bottom-right corner of the game's user interface.

*   **How it does it:**
    *   **`HelloWorldLogger.cs`**:
        *   Implements `IModStarter` from `Timberborn.ModManagerScene`.
        *   The `StartMod` method (called by the game's mod loader) uses `UnityEngine.Debug.Log()` to write to the log.
        *   `UnityEngine.Application.persistentDataPath` is used to get the directory of the `Player.log` file.
    *   **`HelloWorldConfigurator.cs`**:
        *   Implements `IConfigurator` from `Bindito.Core` (a dependency injection framework).
        *   Annotated with `[Context("Game")]`, so its `Configure` method is called during the game context setup.
        *   In `Configure`, it binds `HelloWorldInitializer` as a singleton in the dependency injection container: `containerDefinition.Bind<HelloWorldInitializer>().AsSingleton();`.
    *   **`HelloWorldInitializer.cs`**:
        *   Implements `ILoadableSingleton` from `Timberborn.SingletonSystem`.
        *   Its `Load` method is automatically called after instantiation because it's a loadable singleton.
        *   It receives `UILayout` and `VisualElementLoader` (from `Timberborn.CoreUI` and `Timberborn.UILayoutSystem`) via constructor injection.
        *   In `Load`, it uses `_visualElementLoader.LoadVisualElement("HelloWorld")` to load the UI and `_uiLayout.AddBottomRight(...)` to display it.
    *   **`Timberborn.ModExamples.HelloWorld.asmdef`**:
        *   Defines the assembly name as `"Timberborn.ModExamples.HelloWorld"`.
        *   `"autoReferenced": false` indicates it's a mod assembly not automatically referenced by other game code.
        *   `"allowUnsafeCode": true`.

*   **Imports (Key Namespaces/Libraries):**
    *   `Bindito.Core`: For dependency injection.
    *   `Timberborn.CoreUI`, `Timberborn.SingletonSystem`, `Timberborn.UILayoutSystem`, `Timberborn.ModManagerScene`: Game-specific APIs for UI, singletons, and mod lifecycle.
    *   `UnityEngine`: For core Unity engine functionalities.

## 2. ShantySpeaker Mod

*   **Source Files:**
    *   `Assets/Mods/ShantySpeaker/Scripts/ShantySpeakerConfigurator.cs`
    *   `Assets/Mods/ShantySpeaker/Scripts/FinishableBuildingSoundPlayer.cs`
    *   `Assets/Mods/ShantySpeaker/Scripts/FinishableBuildingSoundPlayerSpec.cs`
    *   `Assets/Mods/ShantySpeaker/Scripts/Timberborn.ModExamples.ShantySpeaker.asmdef`

*   **What it does:**
    *   Allows buildings to play a configurable, looping 3D sound when their construction is completed (they enter a "finished" state). The sound stops if they exit this state.

*   **How it does it:**
    *   **`FinishableBuildingSoundPlayerSpec.cs`**:
        *   A `BaseComponent` (from `Timberborn.BaseComponentSystem`).
        *   Contains a `[SerializeField] private string _soundName;` which allows the sound name to be set in the Unity Editor for building prefabs.
        *   Exposes `SoundName` via a public property.
    *   **`FinishableBuildingSoundPlayer.cs`**:
        *   A `BaseComponent` that implements `IFinishedStateListener` (likely from `Timberborn.BlockSystem` or a related system that manages building states).
        *   Gets `ISoundSystem` (from `Timberborn.SoundSystem`) injected via a method `InjectDependencies`.
        *   In its `Awake` method, it retrieves its associated `FinishableBuildingSoundPlayerSpec` component to get the `SoundName`.
        *   `OnEnterFinishedState()`: Called when the building is finished. It uses `_soundSystem.LoopSingle3DSound(...)` to play the specified sound on the building's `GameObject`. It also sets a custom audio mixer.
        *   `OnExitFinishedState()`: Called if the building leaves the finished state. It uses `_soundSystem.StopSound(...)` to stop the sound.
    *   **`ShantySpeakerConfigurator.cs`**:
        *   A `Configurator` (from `Bindito.Core`) for the `"Game"` context.
        *   Uses `MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();` to register a custom `TemplateModule` (from `Timberborn.TemplateSystem`).
        *   The `ProvideTemplateModule` method creates a `TemplateModule.Builder` and adds a decorator: `builder.AddDecorator<FinishableBuildingSoundPlayerSpec, FinishableBuildingSoundPlayer>();`. This is the core mechanism: any game object (template/prefab) that has the `FinishableBuildingSoundPlayerSpec` component will automatically get the `FinishableBuildingSoundPlayer` component added and managed by the system.
    *   **`Timberborn.ModExamples.ShantySpeaker.asmdef`**:
        *   Defines the assembly, similar to HelloWorld's `.asmdef`.

*   **Imports (Key Namespaces/Libraries):**
    *   `Bindito.Core`: For dependency injection.
    *   `Timberborn.TemplateSystem`: For interacting with game object templates and decorators.
    *   `Timberborn.BaseComponentSystem`, `Timberborn.BlockSystem` (implicitly for `IFinishedStateListener`), `Timberborn.CoreSound`, `Timberborn.SoundSystem`: Game-specific APIs for components, building states, and audio.
    *   `UnityEngine`: For `SerializeField` and `GameObject`.

## 3. OverwritesExample Mod

*   **Key Files/Structure:**
    *   `Assets/Mods/OverwritesExample/manifest.json`
    *   `Assets/Mods/OverwritesExample/Data/`
        *   `Assets/Mods/OverwritesExample/Data/Blueprints/Goods/Good.TreatedPlank.json`
        *   (Other JSON files for goods like Berries, Plank, etc.)
        *   `Assets/Mods/OverwritesExample/Data/Sprites/` (structure implies sprite overwrites are also possible)

*   **What it does:**
    *   Modifies existing game data by "overwriting" specific properties. For example, it changes the visual appearance of "TreatedPlank" in stockpiles and containers to look like regular "Plank".

*   **How it does it:**
    *   This mod is data-driven, not primarily C# script-driven for its main effect.
    *   **`manifest.json`**: Declares the mod's metadata (name, ID, version, description). It lists an optional dependency on `Timberborn.ModExamples.ShantySpeaker`.
    *   **JSON Data Files (e.g., `Good.TreatedPlank.json`):**
        *   These files are placed in a directory structure (`Data/Blueprints/Goods/`) that the game's mod loader presumably scans.
        *   The naming convention of the JSON file (e.g., `Good.TreatedPlank.json`) tells the game which original game asset/blueprint to target.
        *   The content of the JSON file is a partial representation of the game's data structure for that asset, only including the fields to be overridden. For `Good.TreatedPlank.json`:
            ```json
            {
              "GoodSpec": {
                "StockpileVisualization": "Plank",
                "VisibleContainer": {
                  "Value": "Plank"
                }
              }
            }
            ```
    *   The game's mod loading system reads these JSON files and merges/patches the specified values into the corresponding original game data.

*   **Imports/Dependencies:**
    *   No C# code imports in the traditional sense for its core functionality.
    *   It depends on the game's internal data structures (how goods, sprites, etc., are defined) and the mod loading system's ability to interpret these JSON overwrite files based on path and naming conventions.

## 4. YearOfTheSnakeBeaverTails Mod

*   **Key Files/Structure:**
    *   `Assets/Mods/YearOfTheSnakeBeaverTails/manifest.json`
    *   `Assets/Mods/YearOfTheSnakeBeaverTails/Data/`
        *   `Assets/Mods/YearOfTheSnakeBeaverTails/Data/Materials/Beavers/YearOfTheSnakeBeaverTail.png` (the texture file)
        *   `Assets/Mods/YearOfTheSnakeBeaverTails/Data/Blueprints/TailDecals/TailDecal.YearOfTheSnakeBeaverTail.json` (the definition file)

*   **What it does:**
    *   Adds a new custom tail pattern (a "Year of the Snake" design) for beavers in the game.

*   **How it does it:**
    *   This is primarily an asset-based mod.
    *   **`manifest.json`**: Declares the mod's metadata. Description: "The custom tail pattern, prepared by Mechanistry to celebrate the Year of the Snake."
    *   **`YearOfTheSnakeBeaverTail.png`**: The actual image file (texture) for the beaver tail pattern. Located under `Data/Materials/Beavers/`.
    *   **`TailDecal.YearOfTheSnakeBeaverTail.json`**: A JSON blueprint file that defines the new tail decal. Located under `Data/Blueprints/TailDecals/`.
        ```json
        {
            "TailDecalSpec": {
                "FactionId": "", // Empty likely means available to all/default faction
                "Texture": "Materials/Beavers/YearOfTheSnakeBeaverTail" // Path to the PNG texture (without extension)
            }
        }
        ```
    *   The game's mod loading system discovers these assets. The JSON file defines a new `TailDecalSpec` and links it to the provided texture. The game's character system then presumably makes this new tail decal available for beavers.

*   **Imports/Dependencies:**
    *   No C# code imports.
    *   Relies on the game's systems for character appearance customization, specifically how tail decals are defined (the structure of `TailDecalSpec` in JSON) and how textures are referenced and loaded.

## How Imports Work (General for C#-based Mods)

For the C#-based mods (`HelloWorld`, `ShantySpeaker`):

1.  **`using` Directives:**
    *   Standard C# feature. `using NamespaceName;` at the top of a `.cs` file allows unqualified use of types from that namespace (e.g., `UILayout` instead of `Timberborn.UILayoutSystem.UILayout`).

2.  **Assembly References:**
    *   Defined in the `.csproj` project files (though not fully shown in the snippets, this is standard). These link the mod's code to other compiled libraries (`.dll` files).
    *   **Game Assemblies:** Core Timberborn DLLs providing the modding API (e.g., `Timberborn.Core.dll`, `Timberborn.Buildings.dll` - actual names may vary).
    *   **Engine Assemblies:** Unity engine DLLs (e.g., `UnityEngine.dll`, `UnityEngine.CoreModule.dll` as seen in the `.csproj` snippets). Timberborn is built on Unity.
    *   **Modding Helper Assemblies:** Libraries like `Bindito.Core.dll` for dependency injection, often provided by the game or community.
    *   **.NET Standard/Core Libraries:** Base class libraries. The `<TargetFramework>netstandard2.1</TargetFramework>` in the `.csproj` indicates which .NET Standard version the mod targets.

3.  **`.asmdef` (Assembly Definition Files):**
    *   Unity-specific JSON files that define how a set of scripts compiles into an assembly (a `.dll` file).
    *   `"name"`: The output name of the assembly (e.g., `"Timberborn.ModExamples.HelloWorld"`).
    *   `"autoReferenced": false`: Crucial for mods. It prevents the mod's assembly from being automatically linked against by the main game code or other unrelated assemblies. This promotes isolation and helps avoid naming conflicts. Dependencies must be explicitly defined.
    *   `"allowUnsafeCode": true`: Permits the use of C#'s `unsafe` keyword within the assembly.
    *   `.asmdef` files can also contain explicit references to other assemblies (defined by their names from other `.asmdef` files or precompiled DLLs), which is how you establish a dependency graph within a Unity project.

4.  **Dependency Injection (DI):**
    *   Frameworks like `Bindito.Core` provide a way to manage dependencies.
    *   Instead of `new MyService()`, classes declare their dependencies via constructor parameters or `[Inject]` attributes on properties/methods.
    *   A `Configurator` class tells the DI container how to resolve these dependencies (e.g., `containerDefinition.Bind<InterfaceType>().To<ImplementationType>().AsSingleton();`).
    *   This "imports" dependencies in a structured way, promoting loose coupling. The component needing a service doesn't need to know how to create or locate it, only what contract (interface) it fulfills.

This analysis covers the functionality, implementation methods, and import/dependency mechanisms of the provided example mods.

## Key Code Examples and Concepts

This section highlights some crucial pieces of code and concepts that are important to understand when developing Timberborn mods.

### 1. Mod Manifest (`manifest.json`)

The `manifest.json` file is essential for every mod. It provides metadata that the game uses to identify, load, and manage the mod.

**Example (`OverwritesExample/manifest.json`):**
```json
{
  "Name": "Overwrites example",
  "Version": "0.1",
  "Id": "Timberborn.ModExamples.Overwrites",
  "MinimumGameVersion": "0.0.0.0",
  "Description": "Example mod that overwrites some game content",
  "RequiredMods": [],
  "OptionalMods": [
    {
      "Id": "Timberborn.ModExamples.ShantySpeaker"
    }
  ]
}
```

*   **`Name`**: Human-readable name of the mod.
*   **`Version`**: Version string for the mod (e.g., semantic versioning like "1.0.0").
*   **`Id`**: Unique identifier for the mod, typically in a reverse domain style (e.g., `YourName.ModName`). This is critical for dependency management.
*   **`MinimumGameVersion`**: The oldest version of Timberborn this mod is compatible with.
*   **`Description`**: A brief explanation of what the mod does.
*   **`RequiredMods`**: An array of mod IDs that this mod absolutely needs to function. The game will attempt to load these dependencies first.
*   **`OptionalMods`**: An array of mod IDs that this mod can integrate with if they are present, but are not strictly necessary for its core functionality.

### 2. Assembly Definition (`.asmdef`)

For C#-based mods, the `.asmdef` file defines how your scripts are compiled into a .NET assembly (`.dll`).

**Example (`HelloWorld/Scripts/Timberborn.ModExamples.HelloWorld.asmdef`):**
```json
{
  "name": "Timberborn.ModExamples.HelloWorld",
  "autoReferenced": false,
  "allowUnsafeCode": true
}
```

*   **`name`**: The name of the assembly that will be generated. This is often similar to the mod's `Id`.
*   **`autoReferenced`: `false`**: This is **critical for mods**. It ensures that your mod's assembly is not automatically referenced by all other assemblies in the game. This prevents namespace collisions and forces explicit dependency management, which is good practice for modding.
*   **`allowUnsafeCode`: `true`**: Allows the use of C#'s `unsafe` keyword if needed (e.g., for pointer manipulation or certain interop scenarios). It's often included by default in mod templates.
*   **Other common fields (not shown above but important):**
    *   `references`: An array of strings listing other assembly names (from other `.asmdef` files or precompiled DLLs like game assemblies) that this assembly depends on. This is how you link against game code or other libraries.
    *   `includePlatforms`, `excludePlatforms`: To specify for which platforms the assembly should be compiled.
    *   `defineConstraints`: To conditionally compile code based on symbols.

### 3. Using Namespaces and Importing Libraries (C#)

Standard C# `using` directives are used to make types from other namespaces accessible without full qualification.

**Example (from `HelloWorldInitializer.cs`):**
```csharp
using Timberborn.CoreUI; // For UILayout, VisualElementLoader
using Timberborn.SingletonSystem; // For ILoadableSingleton
using Timberborn.UILayoutSystem; // For UILayout (namespace can sometimes be redundant if types are split)

namespace Mods.HelloWorld.Scripts {
  public class HelloWorldInitializer : ILoadableSingleton {
    // ...
  }
}
```
These `using` statements allow the code to directly use `UILayout`, `VisualElementLoader`, and `ILoadableSingleton`. Without them, you'd have to write `Timberborn.CoreUI.UILayout`, etc.

### 4. Dependency Injection with `Bindito.Core`

Many Timberborn mods use `Bindito.Core` for dependency injection (DI), which helps in writing decoupled and testable code.

**Key Concepts:**

*   **`IConfigurator`**: An interface your class implements to configure bindings.
*   **`[Context("...")]`**: Attribute to specify when the configurator runs (e.g., `"Game"` for in-game, `"MainMenu"` for the main menu).
*   **`IContainerDefinition`**: Passed to the `Configure` method, used to define bindings.

**Example: Binding a Singleton (`HelloWorldConfigurator.cs`)**
```csharp
using Bindito.Core;

namespace Mods.HelloWorld.Scripts {
  [Context("Game")]
  public class HelloWorldConfigurator : IConfigurator {
    public void Configure(IContainerDefinition containerDefinition) {
      // Binds HelloWorldInitializer so it can be injected elsewhere.
      // AsSingleton() means only one instance will be created and reused.
      containerDefinition.Bind<HelloWorldInitializer>().AsSingleton();
    }
  }
}
```

**Example: Constructor Injection (`HelloWorldInitializer.cs`)**
```csharp
using Timberborn.CoreUI;
using Timberborn.UILayoutSystem;

public class HelloWorldInitializer : ILoadableSingleton {
  private readonly UILayout _uiLayout;
  private readonly VisualElementLoader _visualElementLoader;

  // Dependencies are "injected" by Bindito when HelloWorldInitializer is created.
  public HelloWorldInitializer(UILayout uiLayout, VisualElementLoader visualElementLoader) {
    _uiLayout = uiLayout;
    _visualElementLoader = visualElementLoader;
  }

  public void Load() {
    // Now _uiLayout and _visualElementLoader can be used.
    var visualElement = _visualElementLoader.LoadVisualElement("HelloWorld");
    _uiLayout.AddBottomRight(visualElement, 0);
  }
}
```

**Example: Method Injection (`FinishableBuildingSoundPlayer.cs`)**
```csharp
using Bindito.Core;
using Timberborn.SoundSystem;

internal class FinishableBuildingSoundPlayer : BaseComponent, IFinishedStateListener {
  private ISoundSystem _soundSystem;

  [Inject] // Attribute marks this method for injection.
  public void InjectDependencies(ISoundSystem soundSystem) {
    _soundSystem = soundSystem;
  }
  // ... _soundSystem can now be used by other methods.
}
```

### 5. Decorators with `TemplateModule` (ShantySpeaker Example)

The `TemplateModule` system allows modification of existing game prefabs (templates) by adding "decorators."

**Example (`ShantySpeakerConfigurator.cs`):**
```csharp
using Bindito.Core;
using Timberborn.TemplateSystem;

namespace Mods.ShantySpeaker.Scripts {
  [Context("Game")]
  public class ShantySpeakerConfigurator : Configurator { // Note: Configurator, not IConfigurator
    protected override void Configure() { // Override Configure for Configurator base class
      MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
    }

    private static TemplateModule ProvideTemplateModule() {
      var builder = new TemplateModule.Builder();
      // This tells the game: "For any prefab that has a FinishableBuildingSoundPlayerSpec component,
      // also add and manage a FinishableBuildingSoundPlayer component."
      builder.AddDecorator<FinishableBuildingSoundPlayerSpec, FinishableBuildingSoundPlayer>();
      return builder.Build();
    }
  }
}
```
This is a powerful way to attach custom C# components and logic to game objects based on their existing components, without directly modifying the original prefabs in many cases.

These examples provide a starting point for understanding common patterns and essential files in Timberborn modding.



