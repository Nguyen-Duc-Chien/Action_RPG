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
            // Clear bounds on scene load to prevent clamping to arbitrary confiners before MapGenerator is ready
            confinerComponent.m_BoundingShape2D = null;
            confinerComponent.InvalidateCache();
        }
    }
    void Awake()
    {
        confiner = GetComponent<CinemachineConfiner2D>();
    }

    public void UpdateCameraBounds(Collider2D newBounds)
    {
        if (confiner != null && newBounds != null && confiner.m_BoundingShape2D != newBounds)
        {
            // Set the bounding shape immediately so subsequent calls with the same bounds are ignored
            confiner.m_BoundingShape2D = newBounds;
            
            // Stop any existing coroutine to prevent race conditions
            StopAllCoroutines();
            StartCoroutine(UpdateBoundsCoroutine());
        }
    }

    private System.Collections.IEnumerator UpdateBoundsCoroutine()
    {
        // Đợi 1 frame để Physics và Cinemachine warp camera xong
        yield return new WaitForEndOfFrame();
        
        if (confiner != null && confiner.m_BoundingShape2D != null)
        {
            confiner.InvalidateCache();
        }
    }

    public void SnapCameraTo(Vector3 position)
    {
        // Di chuyển ngay lập tức transform của camera tới vị trí mới để tránh Cinemachine trôi từ level cũ sang
        transform.position = new Vector3(position.x, position.y, transform.position.z);
        
        // Báo cho Cinemachine biết vị trí đã bị warp (nếu sử dụng Cinemachine 2.x)
        var vcam = GetComponent<CinemachineVirtualCameraBase>();
        if (vcam != null)
        {
            vcam.PreviousStateIsValid = false;
        }
    }
}
