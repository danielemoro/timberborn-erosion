using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Mods.HelloWorld.Scripts
{
    // This is a simplified class to implement IModStarter
    // Later we can expand it with Harmony patching once we resolve dependency issues
    public class DirtBlockClickHandler : MonoBehaviour
    {
        private Camera _mainCamera;
        private static DirtBlockClickHandler _instance;
        private Keyboard _keyboard;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            // This gets called when the game starts
            Debug.Log("[HelloWorld] Dirt Block Click Handler initialized");

            // Create a GameObject to attach our handler to
            GameObject handlerObject = new GameObject("DirtBlockClickHandler");
            _instance = handlerObject.AddComponent<DirtBlockClickHandler>();
            DontDestroyOnLoad(handlerObject);

            HelloWorldLogger.LogBlockInfo(
                "Use this mod to click on dirt blocks and see information about them"
            );
        }

        private void Awake()
        {
            Debug.Log("[HelloWorld] DirtBlockClickHandler Awake called");
            FindMainCamera();

            // Get the keyboard
            _keyboard = Keyboard.current;
            if (_keyboard == null)
            {
                Debug.LogWarning("[HelloWorld] Keyboard not found in Awake");
            }
        }

        private void OnEnable()
        {
            Debug.Log("[HelloWorld] DirtBlockClickHandler enabled");
        }

        private void FindMainCamera()
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null)
            {
                Debug.LogWarning("[HelloWorld] Main camera not found in Awake");
            }
            else
            {
                Debug.Log($"[HelloWorld] Found main camera: {_mainCamera.name}");
            }
        }

        private void Update()
        {
            // Make sure we have the keyboard
            if (_keyboard == null)
            {
                _keyboard = Keyboard.current;
                if (_keyboard == null)
                    return;
            }

            // Test key: Press R to log information
            if (_keyboard.rKey.wasPressedThisFrame)
            {
                Debug.Log("[HelloWorld] R key pressed - TEST FUNCTION");
                HelloWorldLogger.LogBlockInfo("R KEY PRESSED - Testing mod functionality");

                // Log camera info
                if (_mainCamera != null)
                {
                    Debug.Log(
                        $"[HelloWorld] Camera: {_mainCamera.name}, position: {_mainCamera.transform.position}"
                    );
                }
                else
                {
                    Debug.Log("[HelloWorld] No camera found!");
                    FindMainCamera();
                }

                // Try test functionality
                TestPopupFunctionality();
            }
        }

        private void TestPopupFunctionality()
        {
            Debug.Log("[HelloWorld] TEST: Creating test message for output");

            // Build a detailed information string
            StringBuilder info = new StringBuilder();
            info.AppendLine("TEST MESSAGE");
            info.AppendLine("------------");
            info.AppendLine("If you see this message in the logs, the test function is working");
            info.AppendLine($"Current time: {System.DateTime.Now}");

            // Log to console
            HelloWorldLogger.LogBlockInfo(info.ToString());
            Debug.Log($"[HelloWorld] Test message sent: {info.ToString()}");

            // Let's log important environment info - don't worry about the deprecated method warnings
            // They're just warnings, not errors, so the code will still work
            Debug.Log(
                $"[HelloWorld] Game objects in scene: {GameObject.FindObjectsOfType<GameObject>().Length}"
            );
            Debug.Log(
                $"[HelloWorld] Cameras in scene: {GameObject.FindObjectsOfType<Camera>().Length}"
            );
            Debug.Log($"[HelloWorld] Current Time.frameCount: {Time.frameCount}");
            Debug.Log($"[HelloWorld] Application.isPlaying: {Application.isPlaying}");

            // Using Debug.Break() to pause the game in the editor if in development
            // This won't affect the actual game
#if UNITY_EDITOR
            Debug.Log("[HelloWorld] Triggering Debug.Break() in editor...");
            Debug.Break();
#endif
        }

        private void HandleMouseClick()
        {
            Debug.Log("[HelloWorld] Handling mouse click");

            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            Debug.Log($"[HelloWorld] Ray origin: {ray.origin}, direction: {ray.direction}");

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Attempt to identify what was clicked
                GameObject clickedObject = hit.collider.gameObject;

                // Log information about the clicked object
                Debug.Log(
                    $"[HelloWorld] Raycast hit: {clickedObject.name} at position: {hit.point}"
                );
                HelloWorldLogger.LogBlockInfo(
                    $"Clicked on: {clickedObject.name} at position: {hit.point}"
                );

                // Try to determine if it's a dirt block based on the object name or tag
                if (IsDirtBlock(clickedObject))
                {
                    HelloWorldLogger.LogBlockInfo("This appears to be a dirt block!");
                    DisplayDirtBlockInfo(clickedObject, hit.point);
                }
                else
                {
                    Debug.Log(
                        $"[HelloWorld] Object {clickedObject.name} not recognized as dirt block"
                    );
                }
            }
            else
            {
                Debug.Log("[HelloWorld] Raycast did not hit anything");
            }
        }

        private bool IsDirtBlock(GameObject obj)
        {
            // This is a simplified check - we'd need to use the game's
            // actual system to properly identify dirt blocks
            string lowercaseName = obj.name.ToLowerInvariant();
            bool isDirt =
                lowercaseName.Contains("dirt")
                || lowercaseName.Contains("ground")
                || lowercaseName.Contains("soil")
                || lowercaseName.Contains("terrain")
                || lowercaseName.Contains("land")
                || lowercaseName.Contains("earth");

            Debug.Log($"[HelloWorld] Checking if object '{obj.name}' is dirt: {isDirt}");

            return isDirt;
        }

        private void DisplayDirtBlockInfo(GameObject dirtBlock, Vector3 position)
        {
            Debug.Log("[HelloWorld] Displaying dirt block info");

            // Build a detailed information string
            StringBuilder info = new StringBuilder();
            info.AppendLine("DIRT BLOCK INFORMATION");
            info.AppendLine("----------------------");
            info.AppendLine($"Position: {position}");
            info.AppendLine($"Block Size: {dirtBlock.transform.localScale}");

            // Try to get any relevant components
            var components = dirtBlock.GetComponents<Component>();
            info.AppendLine($"Components: {components.Length}");

            // Only show first 5 components to avoid cluttering the popup
            int componentCount = Mathf.Min(components.Length, 5);
            for (int i = 0; i < componentCount; i++)
            {
                info.AppendLine($"- {components[i].GetType().Name}");
            }

            if (components.Length > 5)
            {
                info.AppendLine($"...and {components.Length - 5} more components");
            }

            // Log to console
            HelloWorldLogger.LogBlockInfo(info.ToString());

            // Show in popup
            BlockInfoPopup.ShowInfo(info.ToString(), position);
        }
    }
}
