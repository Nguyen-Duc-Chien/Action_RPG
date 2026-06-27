using UnityEngine;

namespace Minimap
{
    public class MinimapUI : MonoBehaviour
    {
        public CanvasGroup minimapCanvas;
        private bool isMinimapVisible = false;

        private void Start()
        {
            if (minimapCanvas == null)
            {
                minimapCanvas = GetComponent<CanvasGroup>();
            }
            minimapCanvas.alpha = 0;
        }

        private void Update()
        {
            if (Input.GetButtonDown("ToggleMap"))
            {
                if (!isMinimapVisible)
                {
                    // Prevent opening if the game is already paused by another panel
                    if (Time.timeScale == 0f) return;

                    Debug.Log("ToggleMap button pressed!");
                    Show();
                }
                else
                {
                    Debug.Log("ToggleMap button pressed again!");
                    Hide();
                }
            }
        }


        public void Show()
        {
            minimapCanvas.alpha = 1;
            minimapCanvas.interactable = true;
            minimapCanvas.blocksRaycasts = true;
            Time.timeScale = 0;
            isMinimapVisible = true;
        }

        public void Hide()
        {
            minimapCanvas.alpha = 0;
            minimapCanvas.interactable = false;
            minimapCanvas.blocksRaycasts = false;
            Time.timeScale = 1;
            isMinimapVisible = false;
        }
    }
}