using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Mods.HelloWorld.Scripts
{
    public class HelloWorldInitializer : ILoadableSingleton, IUpdatableSingleton
    {
        private readonly UILayout _uiLayout;
        private readonly VisualElementLoader _visualElementLoader;
        private Keyboard _keyboard;
        private VisualElement _rootElement;
        private Label _infoLabel;
        private int _clickCount = 0;

        public HelloWorldInitializer(UILayout uiLayout, VisualElementLoader visualElementLoader)
        {
            _uiLayout = uiLayout;
            _visualElementLoader = visualElementLoader;
            _keyboard = Keyboard.current;
        }

        public void Load()
        {
            Debug.Log("[HelloWorld] Hello World 2 mod is running - now with dirt block detection!");

            // Load the HelloWorld visual element (UI template)
            _rootElement = _visualElementLoader.LoadVisualElement("HelloWorld");

            // Try to find any text element in the UI to update
            var textElement = _rootElement.Q<TextElement>();
            if (textElement != null)
            {
                textElement.text = "Hello World 2";
            }

            // Add simple label that we can update later
            _infoLabel = new Label("Press R to see dirt block info");
            _infoLabel.style.color = new StyleColor(new Color(1f, 1f, 0.8f));
            _infoLabel.style.fontSize = 14;
            _infoLabel.style.paddingTop = 5;
            _infoLabel.style.paddingBottom = 5;
            _infoLabel.style.paddingLeft = 10;
            _infoLabel.style.paddingRight = 10;
            _infoLabel.style.backgroundColor = new StyleColor(new Color(0.2f, 0.2f, 0.2f, 0.7f));
            _infoLabel.style.borderTopLeftRadius = 3;
            _infoLabel.style.borderTopRightRadius = 3;
            _infoLabel.style.borderBottomLeftRadius = 3;
            _infoLabel.style.borderBottomRightRadius = 3;
            _infoLabel.style.marginTop = 10;

            // Add to UI
            _rootElement.Add(_infoLabel);
            _uiLayout.AddBottomRight(_rootElement, 0);

            Debug.Log("[HelloWorld] UI loaded from HelloWorldInitializer");
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
            if (_keyboard.rKey.wasPressedThisFrame && _infoLabel != null)
            {
                _clickCount++;
                Debug.Log("[HelloWorld] R key pressed - updating label text directly");

                // Update the text
                _infoLabel.text =
                    $"R KEY PRESSED {_clickCount} TIMES!\nDirt Block Info\nTime: {System.DateTime.Now.ToString("HH:mm:ss")}";

                // Change background color to make it obvious
                _infoLabel.style.backgroundColor = new StyleColor(
                    new Color(0.8f, 0.2f, 0.2f, 0.9f)
                );

                // Make it larger
                _infoLabel.style.fontSize = 16;

                Debug.Log("[HelloWorld] Updated the label text directly");
            }
        }
    }
}
