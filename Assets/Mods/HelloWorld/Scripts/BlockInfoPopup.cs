using UnityEngine;
using UnityEngine.UI;

namespace Mods.HelloWorld.Scripts
{
    public class BlockInfoPopup : MonoBehaviour
    {
        private static BlockInfoPopup _instance;
        private Text _infoText;
        private Image _background;
        private RectTransform _rectTransform;
        private GameObject _canvas;

        private float _displayTimer = 0f;
        private float _displayDuration = 5f;

        public static void ShowInfo(string info, Vector3 worldPosition)
        {
            Debug.Log("[HelloWorld] BlockInfoPopup.ShowInfo called");

            try
            {
                if (_instance == null)
                {
                    Debug.Log("[HelloWorld] Creating new popup instance");
                    _instance = CreatePopupInstance();
                }

                if (_instance != null)
                {
                    _instance.DisplayInfo(info, worldPosition);
                }
                else
                {
                    Debug.LogError("[HelloWorld] Failed to create popup instance");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[HelloWorld] Error in ShowInfo: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private static BlockInfoPopup CreatePopupInstance()
        {
            try
            {
                // Create a canvas for our UI if there isn't one already
                GameObject canvasObject = new GameObject("BlockInfoCanvas");
                DontDestroyOnLoad(canvasObject);
                Debug.Log("[HelloWorld] Created canvas object");

                Canvas canvas = canvasObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 100; // Ensure it appears on top

                canvasObject.AddComponent<CanvasScaler>();
                canvasObject.AddComponent<GraphicRaycaster>();

                // Create the popup object
                GameObject popupObject = new GameObject("BlockInfoPopup");
                popupObject.transform.SetParent(canvasObject.transform, false);
                Debug.Log("[HelloWorld] Created popup object");

                // Add background image
                Image background = popupObject.AddComponent<Image>();
                background.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

                // Add text component
                GameObject textObject = new GameObject("InfoText");
                textObject.transform.SetParent(popupObject.transform, false);

                Text infoText = textObject.AddComponent<Text>();
                infoText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                infoText.fontSize = 14;
                infoText.color = Color.white;
                infoText.alignment = TextAnchor.UpperLeft;

                // Set up RectTransform for text
                RectTransform textRectTransform = infoText.GetComponent<RectTransform>();
                textRectTransform.anchorMin = new Vector2(0.05f, 0.05f);
                textRectTransform.anchorMax = new Vector2(0.95f, 0.95f);
                textRectTransform.offsetMin = Vector2.zero;
                textRectTransform.offsetMax = Vector2.zero;

                // Set up popup size
                RectTransform popupRectTransform = popupObject.GetComponent<RectTransform>();
                popupRectTransform.sizeDelta = new Vector2(300, 200);

                // Initialize the popup component
                BlockInfoPopup popup = popupObject.AddComponent<BlockInfoPopup>();
                popup._infoText = infoText;
                popup._background = background;
                popup._rectTransform = popupRectTransform;
                popup._canvas = canvasObject;

                // Hide it initially
                popupObject.SetActive(false);

                Debug.Log("[HelloWorld] Popup creation completed successfully");
                return popup;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[HelloWorld] Error creating popup: {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }

        private void DisplayInfo(string info, Vector3 worldPos)
        {
            try
            {
                Debug.Log(
                    $"[HelloWorld] Displaying info in popup: {info.Substring(0, Mathf.Min(50, info.Length))}..."
                );
                _infoText.text = info;
                gameObject.SetActive(true);

                // Position near the cursor but ensure it's visible
                _rectTransform.position = Input.mousePosition + new Vector3(10, 10, 0);
                Debug.Log($"[HelloWorld] Popup position set to {_rectTransform.position}");

                // Ensure it's on screen
                Vector2 screenBounds = new Vector2(Screen.width, Screen.height);
                Vector3 position = _rectTransform.position;
                Vector2 size = _rectTransform.sizeDelta;

                if (position.x + size.x > screenBounds.x)
                    position.x = screenBounds.x - size.x;
                if (position.y + size.y > screenBounds.y)
                    position.y = screenBounds.y - size.y;

                _rectTransform.position = position;

                // Set timer
                _displayTimer = _displayDuration;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(
                    $"[HelloWorld] Error displaying info: {ex.Message}\n{ex.StackTrace}"
                );
            }
        }

        private void Update()
        {
            if (_displayTimer > 0)
            {
                _displayTimer -= Time.deltaTime;
                if (_displayTimer <= 0)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
