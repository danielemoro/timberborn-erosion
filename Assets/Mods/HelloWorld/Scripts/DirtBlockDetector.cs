using System.Text;
using Bindito.Core;
using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Mods.HelloWorld.Scripts
{
    [Context("Game")]
    public class DirtBlockDetectorConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            Debug.Log("[HelloWorld] DirtBlockDetectorConfigurator.Configure called");

            // Register our service
            containerDefinition.Bind<DirtBlockDetector>().AsSingleton();
        }
    }

    // Simple service to detect and display dirt block info
    public class DirtBlockDetector : ILoadableSingleton, IUpdatableSingleton
    {
        private readonly UILayout _uiLayout;
        private readonly VisualElementLoader _visualElementLoader;
        private Keyboard _keyboard;

        public DirtBlockDetector(UILayout uiLayout, VisualElementLoader visualElementLoader)
        {
            _uiLayout = uiLayout;
            _visualElementLoader = visualElementLoader;

            // Try to get keyboard right away
            _keyboard = Keyboard.current;

            Debug.Log("[HelloWorld] DirtBlockDetector constructor called");
        }

        public void Load()
        {
            Debug.Log("[HelloWorld] DirtBlockDetector.Load called");

            // Make sure we have a keyboard reference
            if (_keyboard == null)
            {
                _keyboard = Keyboard.current;
                if (_keyboard == null)
                {
                    Debug.LogWarning("[HelloWorld] Keyboard not available in Load!");
                }
                else
                {
                    Debug.Log("[HelloWorld] Keyboard found in Load");
                }
            }
        }

        // This will be called every frame by Timberborn
        public void UpdateSingleton()
        {
            // Ensure we have a keyboard reference
            if (_keyboard == null)
            {
                _keyboard = Keyboard.current;
                return;
            }

            // Check for R key using the new Input System
            if (_keyboard.rKey.wasPressedThisFrame)
            {
                Debug.Log("[HelloWorld] R key detected in DirtBlockDetector.UpdateSingleton");
                ShowDirtBlockInfo();
            }
        }

        private void ShowDirtBlockInfo()
        {
            Debug.Log("[HelloWorld] Showing dirt block info from DirtBlockDetector");

            // Create a simple info panel in the existing HelloWorld UI
            try
            {
                var visualElement = _visualElementLoader.LoadVisualElement("HelloWorld");

                // Build information text
                StringBuilder info = new StringBuilder();
                info.AppendLine("DIRT BLOCK INFO (R KEY TEST)");
                info.AppendLine("---------------------------");
                info.AppendLine($"Current time: {System.DateTime.Now}");
                info.AppendLine(
                    "This is test data - normally we would show actual block data here"
                );
                info.AppendLine("If you can see this, the R key was successfully detected!");

                // Create a panel
                var panel = new VisualElement();
                panel.name = "DirtInfoPanel";

                // Add a text element
                var text = new Label(info.ToString());
                panel.Add(text);

                // Style the panel - properly setting UIElements styles
                panel.style.backgroundColor = new Color(0.2f, 0.1f, 0.1f, 0.9f);
                panel.style.color = Color.white;

                // Setting padding with individual properties
                panel.style.paddingTop = 10;
                panel.style.paddingBottom = 10;
                panel.style.paddingLeft = 10;
                panel.style.paddingRight = 10;

                panel.style.marginTop = 20;

                // Remove existing info panels to avoid stacking
                // Use the correct Query method for UIElements
                var existingPanel = visualElement.Query<VisualElement>("DirtInfoPanel").First();
                if (existingPanel != null)
                {
                    visualElement.Remove(existingPanel);
                }

                // Add the panel
                visualElement.Add(panel);

                Debug.Log("[HelloWorld] Added dirt block info panel to UI");
            }
            catch (System.Exception ex)
            {
                Debug.LogError(
                    $"[HelloWorld] Error showing dirt block info: {ex.Message}\n{ex.StackTrace}"
                );
            }
        }
    }
}
