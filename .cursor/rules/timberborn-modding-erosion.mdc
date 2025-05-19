---
description: How to build the erosion mod for timberborn
globs: 
alwaysApply: false
---
Table of Contents

Project Overview

Prerequisites

Cloning & Initial Setup

Project Structure

Configuring the Mod

Starter Project: Inspecting Erosion on Click

Implementing Erosion & Deposition

Building the Mod

Running & Testing

Debugging Tips

Further Reading

1. Project Overview

...

2. Prerequisites

Unity Hub and the Unity Editor version matching Timberborn (6.x.x).

Windows/macOS Build Support modules via Unity Hub.

Git (or GitHub Desktop) to clone repositories.

Timberborn installed on your machine (version ≥ 6.0).

Basic knowledge of C#, Unity, and Harmony/Cecil patching.

Modding Background: Harmony & Cecil Patching

Timberborn mods hook into the game's compiled C# assemblies using two main approaches:

Harmony: A runtime method-patching library that intercepts calls to existing methods and allows you to prepend, append, or replace functionality without modifying the original DLLs. You create a Harmony instance with a unique ID, then use AccessTools.Method to locate the target (e.g., WaterFlowManager.UpdateFlow) and supply Prefix or Postfix methods in your own code. Harmony handles IL injection at runtime, making it simple to extend game behavior dynamically.

Cecil: A compile-time library for reading and rewriting .NET assemblies. Some advanced mod workflows use Cecil scripts to permanently modify the Timberborn DLLs before launching, embedding new methods or altering existing ones. These patches are applied once and saved to a copy of the game assembly, reducing runtime overhead but requiring maintenance whenever the game updates.

Why choose Harmony?

No need to alter game files on disk

Easy to roll back or disable conflicts between mods

Widely used in the modding community with extensive docs and examples

Why use Cecil?

Better performance for large-scale changes

Useful when you need to inject new classes or assets directly into the assembly

Most community mods—including our Erosion Mod—use Harmony for quick iteration and compatibility. Harmony’s postfix patches, as shown in ErosionModStarter.cs, let you hook into the water simulation loop without touching the original source.

3. Cloning & Initial Setup

Unity Hub and the Unity Editor version matching Timberborn (6.x.x).

Windows/macOS Build Support modules via Unity Hub.

Git (or GitHub Desktop) to clone repositories.

Timberborn installed on your machine (version ≥ 6.0).

Basic knowledge of C#, Unity, and Harmony/Cecil patching.

3. Cloning & Initial Setup

Open a terminal and run:

git clone https://github.com/mechanistry/timberborn-modding.git

Launch Unity Hub, click Add, and select the cloned timberborn-modding folder.

In Advanced Options, add -disable-assembly-updater to avoid unwanted auto-updates.

Open the project. When prompted, locate your Timberborn install to import its DLLs.

4. Project Structure

timberborn-modding/
├── Assets/
│   ├── Mods/
│   │   ├── ExampleMod1/
│   │   └── HelloWorld/  ← duplicate this for `ErosionMod`
│   └── ... (modding tools & samples)
├── ProjectSettings/
└── Packages/

Assets/Mods/HelloWorld: A template mod demonstrating setup & patching.

Mod Builder: Unity window for compiling & packaging mods.

$1

6. Starter Project: Inspecting Erosion on Click

Before diving into full erosion mechanics, start with a simple “click-to-inspect” tool:

Goal: When you click on a dirt block in-game, calculate and display the sum of water flow rates around it.

Outcome: You’ll gain familiarity with Harmony patching, tile coords, and basic UI/log output.

Files to Create

ClickInspectModStarter.csLocation: Assets/Mods/ErosionMod/Scripts/ClickInspectModStarter.cs

using Timberborn.ModdingTools;
using HarmonyLib;
using Timberborn.DataStructures;
using Timberborn.Water;
using Timberborn.World;
using Timberborn.UI;

namespace ErosionMod {
    public class ClickInspectModStarter : IModStarter {
        public void StartMod(IModEnvironment env) {
            var harmony = new Harmony("com.yourname.clickinspect");
            harmony.Patch(
                original: AccessTools.Method(
                    typeof(Timberborn.Input.BlockSelectionTool),
                    "OnBlockClick"
                ),
                postfix: new HarmonyMethod(
                    typeof(ClickInspectModStarter),
                    nameof(OnBlockClickPostfix)
                )
            );
            env.Logger.Info("[ClickInspect] Patch applied");
        }

        public static void OnBlockClickPostfix(TileCoord coord) {
            var block = WorldBlockManager.Instance.GetBlock(coord);
            if (block.Type != BlockType.Dirt) return;
            float totalFlow = 0f;
            foreach (var dir in TileCoordExtensions.Adjacent8) {
                var neighbor = new TileCoord(coord.X + dir.x, coord.Y + dir.y);
                totalFlow += WaterFlowManager.Instance.GetFlowRate(neighbor);
            }
            UIPopupManager.Instance.ShowMessage($"Erosion Score: {totalFlow:F2}");
        }
    }
}

ClickInspectMod.asmdefLocation: Assets/Mods/ErosionMod/Scripts/ClickInspectMod.asmdef

{
  "name": "ClickInspectMod",
  "references": [
    "Assembly-CSharp",
    "Timberborn.ModdingTools",
    "Timberborn.UI"
  ],
  "includePlatforms": ["Windows", "OSX"],
  "autoReferenced": true
}

6. Setup & Test

Build via Mod Builder (as in Building the Mod).

Enable Erosion Simulation in-game (the same mod ID handles both tools).

Click on any dirt block—look for the popup showing its “Erosion Score.”

$2

6. Implementing Erosion & Deposition

The core logic lives in two C# scripts plus an ASM definition. Here’s exactly what you need:

6.1 File: ErosionModStarter.cs

Location: Assets/Mods/ErosionMod/Scripts/ErosionModStarter.cs

using Timberborn.ModdingTools;
using HarmonyLib;

namespace ErosionMod {
    public class ErosionModStarter : IModStarter {
        public void StartMod(IModEnvironment env) {
            // Create a Harmony instance with a unique ID
            var harmony = new Harmony("com.yourname.erosionmod");

            // Patch the UpdateFlow method on WaterFlowManager
            harmony.Patch(
                original: AccessTools.Method(
                    typeof(Timberborn.Water.WaterFlowManager),
                    "UpdateFlow"
                ),
                postfix: new HarmonyMethod(
                    typeof(ErosionModStarter),
                    nameof(UpdateFlowPostfix)
                )
            );

            env.Logger.Info("[ErosionMod] Patched WaterFlowManager.UpdateFlow");
        }

        // Runs immediately after each tile's flow update
        public static void UpdateFlowPostfix(Timberborn.DataStructures.TileCoord coord) {
            ErosionManager.Instance.ProcessTile(coord);
        }
    }
}

What this does:

Hooks into the water‐simulation loop (UpdateFlow).

After each tile’s flow is computed, calls our ProcessTile method.

6.2 File: ErosionManager.cs

Location: Assets/Mods/ErosionMod/Scripts/ErosionManager.cs

using Timberborn.DataStructures;
using Timberborn.Water;
using Timberborn.World;
using UnityEngine;
using System.Collections.Generic;

namespace ErosionMod {
    public class ErosionManager {
        // Singleton
        private static ErosionManager _instance;
        public static ErosionManager Instance => _instance ??= new ErosionManager();

        // Track per‐tile state
        private readonly Dictionary<TileCoord, float> _dirtErosion = new();
        private readonly Dictionary<TileCoord, float> _waterSilt   = new();

        // Tweakable parameters
        private const float ErosionRate           = 0.1f;    // silt % per flow unit
        private const float DepositionFlowThresh  = 0.05f;   // below this, deposition triggers
        private const float MaxPercent            = 100f;

        // Called each tick for every tile
        public void ProcessTile(TileCoord coord) {
            float flowStrength = WaterFlowManager.Instance.GetFlowRate(coord);
            var block = WorldBlockManager.Instance.GetBlock(coord);

            // 1) Erosion: water over dirt
            if (block.Type == BlockType.Dirt) {
                float silt = flowStrength * ErosionRate;
                AddSilt(coord, silt);
                ErodeDirt(coord, silt);
            }

            // 2) Deposition: slow water in empty (water) tile
            if (block.Type == BlockType.Water && flowStrength < DepositionFlowThresh) {
                if (_waterSilt.TryGetValue(coord, out float storedSilt) && storedSilt > 0) {
                    if (storedSilt >= MaxPercent) {
                        WorldBlockManager.Instance.SetBlock(coord, BlockType.Dirt);
                        _waterSilt[coord] = 0;
                    }
                }
            }
        }

        private void AddSilt(TileCoord c, float amount) {
            _waterSilt.TryGetValue(c, out float cur);
            _waterSilt[c] = Mathf.Clamp(cur + amount, 0, MaxPercent);
        }

        private void ErodeDirt(TileCoord waterTile, float amount) {
            // Dirt lies beneath water: y-1
            var dirtTile = new TileCoord(waterTile.X, waterTile.Y - 1);
            _dirtErosion.TryGetValue(dirtTile, out float erosion);
            erosion += amount;

            if (erosion >= MaxPercent) {
                WorldBlockManager.Instance.RemoveBlock(dirtTile);
                _dirtErosion.Remove(dirtTile);
            } else {
                _dirtErosion[dirtTile] = erosion;
            }
        }
    }
}

Key points:

We maintain two dictionaries for dirt‐erosion % and water‐silt %.

On each flow tick, if water covers dirt, we erode and move silt into the water.

When water is slow enough, we check stored silt—if ≥100%, we spawn a new dirt block.

6.3 File: ErosionMod.asmdef

Location: Assets/Mods/ErosionMod/Scripts/ErosionMod.asmdef

Create an ASM definition so Unity compiles your scripts into their own assembly and references the correct game DLLs:

{
  "name": "ErosionMod",
  "references": [
    "Assembly-CSharp",          
    "Timberborn.ModdingTools"
  ],
  "includePlatforms": ["Windows", "OSX"],
  "allowUnsafeCode": true,
  "autoReferenced": true
}

Directory snapshot after adding these files:

Assets/Mods/ErosionMod/
├── manifest.json
└── Scripts/
    ├── ErosionModStarter.cs
    ├── ErosionManager.cs
    └── ErosionMod.asmdef

7. Building the Mod

. Building the Mod

In Unity’s menu: Timberborn → Show Mod Builder.

Tick ErosionMod and click Build Selected.

The output folder Documents/Timberborn/Mods/ErosionMod will contain:

Code.dll

manifest.json

Any AssetBundles

8. Running & Testing

Start Timberborn and open Mod Manager.

Enable Erosion Simulation.

Begin or load a map with flowing water.

Verify log outputs or visual cues (e.g., debug gizmos) to confirm silt/erosion updates.

9. Debugging Tips

Log generously: Use env.Logger.Info(...) in your patch methods.

Test small areas: Focus on a minimal map to isolate behavior.

Breakpoints: Attach Visual Studio to Unity to step through Harmony patches.

10. Further Reading

Timberborn Official Modding Docs: In-game → Help → Mods

mechanistry/timberborn-modding GitHub repo

TimberAPI on Thunderstore for higher-level patching utilities


