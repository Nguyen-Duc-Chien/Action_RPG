using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConfinerFinder : MonoBehaviour
{
    private CinemachineConfiner2D confiner;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CinemachineConfiner2D confinerComponent = GetComponent<CinemachineConfiner2D>();
        if (confinerComponent != null)
        {
            GameObject confinerObj = GameObject.FindWithTag("Confiner");
            if (confinerObj != null)
            {
                confinerComponent.m_BoundingShape2D = confinerObj.GetComponent<PolygonCollider2D>();
            }
            else
            {
                confinerComponent.m_BoundingShape2D = null;
            }
            
            // Xóa cache cũ để Cinemachine nhận diện ranh giới mới ngay lập tức
            confinerComponent.InvalidateCache();
        }
    }
    void Awake()
    {
        confiner = GetComponent<CinemachineConfiner2D>();
    }

    public void UpdateCameraBounds(Collider2D newBounds)
    {
        if (confiner != null && newBounds != null)
        {
            confiner.m_BoundingShape2D = newBounds;

            confiner.InvalidateCache();
        }
    }
}
