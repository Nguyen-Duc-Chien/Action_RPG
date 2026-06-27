using UnityEditor;
using UnityEngine;

namespace Minimap
{
    public class MinimapCamera : MonoBehaviour
    {
        private const float ZOOM_CHANGE_AMOUNT = 10f;
        private const float ZOOM_MIN = 10f;
        private const float ZOOM_MAX = 50f;
        
        private Camera minimapCamera;
        private float zoom;

        private void Awake()
        {
            minimapCamera = transform.GetComponent<Camera>();
            if (minimapCamera != null)
            {
                zoom = minimapCamera.orthographicSize;
            }
        }

        public void SetZoom(float orthographicSize)
        {           
            if (minimapCamera != null)
            {
                minimapCamera.orthographicSize = orthographicSize;
            }
        }

        public float GetZoom()
        {
            return minimapCamera != null ? minimapCamera.orthographicSize : zoom;
        }

        public void ZoomIn()
        {
            zoom -= ZOOM_CHANGE_AMOUNT;
            if (zoom < ZOOM_MIN)
            {
                zoom = ZOOM_MIN;
            }
            SetZoom(zoom);
        }

        public void ZoomOut()
        {
            zoom += ZOOM_CHANGE_AMOUNT;
            if (zoom > ZOOM_MAX)
            {
                zoom = ZOOM_MAX;
            }
            SetZoom(zoom);
        }

        public void TriggerZoomIn()
        {
            ZoomIn(); 
        }

        public void TriggerZoomOut()
        {
            ZoomOut(); 
        }
    }
}