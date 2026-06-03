using UnityEditor;
using UnityEngine;

namespace Minimap
{
    public class MinimapCamera : MonoBehaviour
    {
        private static MinimapCamera instance;

        private const float ZOOM_CHANGE_AMOUNT = 10f;
        private const float ZOOM_MIN = 10f;
        private const float ZOOM_MAX = 50f;
        private Camera minimapCamera;
        private float zoom;

        private void Awake()
        {
            instance = this;
            minimapCamera = transform.GetComponent<Camera>();
            zoom = minimapCamera.orthographicSize;
        }

        public static void SetZoom(float orthographicSize)
        {           
            instance.minimapCamera.orthographicSize = orthographicSize;
        }

        public static float GetZoom()
        {
            return instance.minimapCamera.orthographicSize;
        }

        public static void ZoomIn()
        {
            instance.zoom -= ZOOM_CHANGE_AMOUNT;
            if (instance.zoom < ZOOM_MIN)
            {
                instance.zoom = ZOOM_MIN;
            }
            SetZoom(instance.zoom);
        }

        public static void ZoomOut()
        {
            instance.zoom += ZOOM_CHANGE_AMOUNT;
            if (instance.zoom > ZOOM_MAX)
            {
                instance.zoom = ZOOM_MAX;
            }
            SetZoom(instance.zoom);
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