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
    public class SimpleBlockInfoConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            Debug.Log("[HelloWorld] SimpleBlockInfoConfigurator.Configure called");
            containerDefinition.Bind<SimpleBlockInfo>().AsSingleton();
        }
    }

    public class SimpleBlockInfo : ILoadableSingleton, IUpdatableSingleton
    {
        private readonly UILayout _uiLayout;
        private readonly VisualElementLoader _visualElementLoader;
        private Keyboard _keyboard;
        private VisualElement _visualElement;
        private Label _infoLabel;

        public SimpleBlockInfo(UILayout uiLayout, VisualElementLoader visualElementLoader)
        {
            _uiLayout = uiLayout;
            _visualElementLoader = visualElementLoader;
            _keyboard = Keyboard.current;
            Debug.Log("[HelloWorld] SimpleBlockInfo constructor called");
        }

        public void Load()
        {
            Debug.Log("[HelloWorld] SimpleBlockInfo.Load called");

            try
            {
                // Get the HelloWorld visual element that's already working
                _visualElement = _visualElementLoader.LoadVisualElement("HelloWorld");

                // Create an info label just like the instruction label that works
                _infoLabel = new Label("Press R to see block info");

                // Style it exactly like the working label
                _infoLabel.style.color = new StyleColor(new Color(1f, 1f, 0.8f));
                _infoLabel.style.fontSize = 12;
                _infoLabel.style.paddingTop = 5;
                _infoLabel.style.paddingBottom = 5;
                _infoLabel.style.paddingLeft = 10;
                _infoLabel.style.paddingRight = 10;
                _infoLabel.style.backgroundColor = new StyleColor(
                    new Color(0.2f, 0.2f, 0.2f, 0.7f)
                );
                _infoLabel.style.borderTopLeftRadius = 3;
                _infoLabel.style.borderTopRightRadius = 3;
                _infoLabel.style.borderBottomLeftRadius = 3;
                _infoLabel.style.borderBottomRightRadius = 3;
                _infoLabel.style.marginTop = 10;

                // Add it to the visual element
                _visualElement.Add(_infoLabel);

                Debug.Log("[HelloWorld] Added info label to HelloWorld UI");
            }
            catch (System.Exception ex)
            {
                Debug.LogError(
                    $"[HelloWorld] Error in SimpleBlockInfo.Load: {ex.Message}\n{ex.StackTrace}"
                );
            }
        }

        public void UpdateSingleton()
        {
            // Check for keyboard
            if (_keyboard == null)
            {
                _keyboard = Keyboard.current;
                if (_keyboard == null)
                    return;
            }

            // Check for R key
            if (_keyboard.rKey.wasPressedThisFrame)
            {
                Debug.Log("[HelloWorld] R key detected in SimpleBlockInfo");
                UpdateInfoText();
            }
        }

        private void UpdateInfoText()
        {
            if (_infoLabel == null)
            {
                Debug.LogError("[HelloWorld] InfoLabel is null, can't update");
                return;
            }

            // Build info text
            StringBuilder info = new StringBuilder();
            info.AppendLine("R KEY PRESSED!");
            info.AppendLine("---------------");
            info.AppendLine($"Time: {System.DateTime.Now.ToString("HH:mm:ss")}");
            info.AppendLine("This is dirt block info");

            // Update the label text - this is the simple approach that should visibly work
            _infoLabel.text = info.ToString();
            _infoLabel.style.backgroundColor = new StyleColor(new Color(0.5f, 0.2f, 0.2f, 0.9f));

            Debug.Log("[HelloWorld] Updated info label text");
        }
    }
}
