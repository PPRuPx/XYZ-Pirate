using UnityEngine;

namespace Utils
{
    public static class WindowUtils
    {
        public static void CreateWindow(string resourcePath)
        {
            var window = Resources.Load<GameObject>(resourcePath);
            var canvas = GameObject.Find("HUDCanvas").GetComponent<Canvas>();
            Object.Instantiate(window, canvas.transform);
        }
    }
}