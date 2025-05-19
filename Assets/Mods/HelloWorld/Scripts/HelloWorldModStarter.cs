using Timberborn.ModManagerScene;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mods.HelloWorld.Scripts
{
    // This class follows the Timberborn mod startup pattern from the example in examples.md
    public class HelloWorldModStarter : IModStarter
    {
        // Called when the mod starts
        public void StartMod(IModEnvironment modEnvironment)
        {
            // Log that the mod is starting
            var playerLogPath = Application.persistentDataPath + "/Player.log";
            Debug.Log($"[HelloWorld] Hello World 2 is starting! Log file is at: {playerLogPath}");
            Debug.Log($"[HelloWorld] Press the R key while playing to test the mod");

            // We can't use modEnvironment.Logger as it doesn't exist
            // Just use our own logger
            HelloWorldLogger.LogImportantEvent("Hello World mod initialized through IModStarter");
            HelloWorldLogger.LogImportantEvent("Press R key to test functionality");

            // Set up input monitoring
            var inputObject = new GameObject("HelloWorldInputMonitor");
            var inputMonitor = inputObject.AddComponent<HelloWorldInputMonitor>();
            Object.DontDestroyOnLoad(inputObject);

            Debug.Log("[HelloWorld] Input monitor created");
        }
    }

    // Simple MonoBehaviour to detect key presses using the new Input System
    public class HelloWorldInputMonitor : MonoBehaviour
    {
        private Keyboard _keyboard;

        private void Awake()
        {
            Debug.Log("[HelloWorld] InputMonitor Awake called");
            _keyboard = Keyboard.current;
            if (_keyboard == null)
            {
                Debug.LogError("[HelloWorld] Failed to get keyboard device!");
            }
        }

        private void Update()
        {
            // Make sure keyboard is available
            if (_keyboard == null)
            {
                _keyboard = Keyboard.current;
                if (_keyboard == null)
                    return;
            }

            // Check for R key using the new Input System
            if (_keyboard.rKey.wasPressedThisFrame)
            {
                Debug.Log("[HelloWorld] R KEY DETECTED - Global monitor");
                HelloWorldLogger.LogBlockInfo("R key pressed - from global monitor");

                // Create a temporary visual effect to confirm key was detected
                CreateVisualConfirmation();
            }
        }

        private void CreateVisualConfirmation()
        {
            // Create a temporary object with a renderer to show the key was pressed
            GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            indicator.transform.position = new Vector3(0, 100, 0); // Position it high up so it's visible
            indicator.transform.localScale = new Vector3(5, 5, 5); // Make it large

            // Set a bright color
            Renderer renderer = indicator.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.yellow;
            }

            // Destroy after 2 seconds
            Destroy(indicator, 2f);

            Debug.Log("[HelloWorld] Created visual confirmation indicator");
        }
    }
}
