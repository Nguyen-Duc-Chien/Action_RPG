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
        CinemachineConfiner2D confiner = GetComponent<CinemachineConfiner2D>();
        confiner.m_BoundingShape2D = GameObject.FindWithTag("Confiner").GetComponent<PolygonCollider2D>();
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
