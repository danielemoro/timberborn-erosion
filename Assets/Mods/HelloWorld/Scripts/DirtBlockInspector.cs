using Bindito.Core;
using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Mods.HelloWorld.Scripts
{
    // Context annotation tells the dependency system to register this configurator
    // for the Game context (when the game is running)
    [Context("Game")]
    public class DirtBlockInspectorConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            Debug.Log("[HelloWorld] DirtBlockInspectorConfigurator.Configure called");

            // Register our singleton service
            containerDefinition.Bind<DirtBlockInspector>().AsSingleton();
        }
    }

    // This class will be created by the game's dependency system
    // ILoadableSingleton interface ensures Load() is called after creation
    // IUpdatableSingleton would be better if it exists in Timberborn
    public class DirtBlockInspector : ILoadableSingleton
    {
        private readonly UILayout _uiLayout;
        private readonly VisualElementLoader _visualElementLoader;
        private bool _updateListenerCreated = false;

        // Constructor receives injected dependencies
        public DirtBlockInspector(UILayout uiLayout, VisualElementLoader visualElementLoader)
        {
            Debug.Log("[HelloWorld] DirtBlockInspector constructor called");
            _uiLayout = uiLayout;
            _visualElementLoader = visualElementLoader;
        }

        // Called by the game after instantiation
        public void Load()
        {
            Debug.Log("[HelloWorld] DirtBlockInspector.Load called");

            // Log that we're listening for the R key
            HelloWorldLogger.LogBlockInfo("Dirt Block Inspector loaded - press R key to test");

            // Create a GameObject with MonoBehaviour to handle Update
            if (!_updateListenerCreated)
            {
                GameObject updateObj = new GameObject("HelloWorldUpdateListener");
                var updateListener = updateObj.AddComponent<UpdateListener>();
                updateListener.OnKeyRPressed += ShowDebugInfo;
                GameObject.DontDestroyOnLoad(updateObj);
                _updateListenerCreated = true;

                Debug.Log("[HelloWorld] Created UpdateListener GameObject");
            }
        }

        private void ShowDebugInfo()
        {
            Debug.Log("[HelloWorld] R key pressed - showing debug info");

            // Instead of creating our own UI, we can try to use the game's UI system
            try
            {
                var gameInfoElement = _visualElementLoader.LoadVisualElement("HelloWorld");

                // This might need to be adjusted based on the actual UI template
                var messagePanel = new VisualElement();
                messagePanel.name = "HelloWorldDebugPanel";

                var messageLabel = new Label("Dirt Block Inspector Debug Info (R key pressed)");
                messagePanel.Add(messageLabel);

                // Add some additional debug text
                var debugInfo = new Label($"Debug info generated at: {System.DateTime.Now}");
                messagePanel.Add(debugInfo);

                // Style the panel properly
                messagePanel.style.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
                messagePanel.style.color = Color.white;

                // Setting padding with individual properties
                messagePanel.style.paddingTop = 10;
                messagePanel.style.paddingBottom = 10;
                messagePanel.style.paddingLeft = 10;
                messagePanel.style.paddingRight = 10;

                messagePanel.style.marginTop = 10;

                // Add to the main element
                gameInfoElement.Add(messagePanel);

                // No need to add to _uiLayout as the HelloWorld element is already there

                Debug.Log("[HelloWorld] Added debug info to UI");
            }
            catch (System.Exception ex)
            {
                Debug.LogError(
                    $"[HelloWorld] Error showing debug info: {ex.Message}\n{ex.StackTrace}"
                );
            }
        }
    }

    // Simple MonoBehaviour to listen for key presses
    public class UpdateListener : MonoBehaviour
    {
        public event System.Action OnKeyRPressed;
        private Keyboard _keyboard;

        private void Awake()
        {
            // Get reference to the keyboard
            _keyboard = Keyboard.current;
            if (_keyboard == null)
            {
                Debug.LogError("[HelloWorld] UpdateListener - Failed to get keyboard in Awake!");
            }
        }

        private void Update()
        {
            // Ensure we have a keyboard reference
            if (_keyboard == null)
            {
                _keyboard = Keyboard.current;
                if (_keyboard == null)
                    return;
            }

            // Check if R key was pressed this frame using the new Input System
            if (_keyboard.rKey.wasPressedThisFrame)
            {
                Debug.Log("[HelloWorld] R key detected in UpdateListener");
                HelloWorldLogger.LogBlockInfo("R key pressed in UpdateListener");
                OnKeyRPressed?.Invoke();
            }
        }
    }
}
