using UnityEngine;

namespace Mods.HelloWorld.Scripts
{
    /// <summary>
    /// Simple logger for HelloWorld mod
    /// </summary>
    public static class HelloWorldLogger
    {
        // Log block info to the console
        public static void LogBlockInfo(string message)
        {
            // Log with a distinctive prefix and in a way that's easy to find in logs
            Debug.Log($"[HELLO_WORLD_MOD] >>> {message}");

            // Try to display a notification in-game (we'll use standard Debug.Log for now)
            // In a real mod, we might use Timberborn's notification system if available
            Debug.Log($"<color=yellow>[HelloWorld Notification]</color> {message}");
        }

        // Log an important event that should be noticed
        public static void LogImportantEvent(string message)
        {
            // Make it even more visible in logs
            Debug.Log($"***********************************************************");
            Debug.Log($"[HELLO_WORLD_MOD] IMPORTANT: {message}");
            Debug.Log($"***********************************************************");

            // Try to make an on-screen notification
            // This Debug.Log with color might show up specially in Timberborn's UI
            Debug.Log($"<color=lime><b>[HelloWorld Important]</b> {message}</color>");
        }
    }
}
