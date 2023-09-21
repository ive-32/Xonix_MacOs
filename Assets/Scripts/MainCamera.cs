using UnityEngine;

public class IcwCamera : MonoBehaviour
{
    private Camera _mainCamera;

    public void Awake()
    {
        _mainCamera = this.GetComponent<Camera>();
        SetUpCameraAngle();
        SetUpCameraPosition();
    }
    
    private void SetUpCameraAngle()
    {
        var cameraSize = 20.0f;
        var minScreenSize = Mathf.Min(_mainCamera.pixelWidth, _mainCamera.pixelHeight);
        var maxScreenSize = Mathf.Max(_mainCamera.pixelWidth, _mainCamera.pixelHeight);
        var cameraAspect = minScreenSize / maxScreenSize;
        var minSize = Mathf.Min(IcwGame.SizeX, IcwGame.SizeY);
        var maxSize = Mathf.Max(IcwGame.SizeX, IcwGame.SizeY);
        
        if (_mainCamera.pixelWidth > _mainCamera.pixelHeight)
            cameraSize = cameraAspect < 0.5 ? minSize + 4 : (maxSize + 6) * cameraAspect; 
        else
            cameraSize = cameraAspect < 0.5 ? (minSize + 6) / cameraAspect : maxSize + 4;
        
        _mainCamera.orthographicSize = cameraSize / 2;
    }
    
    private void SetUpCameraPosition()
    {
        var maxSize = Mathf.Max(IcwGame.SizeX, IcwGame.SizeY);
        var minSize = Mathf.Min(IcwGame.SizeX, IcwGame.SizeY);
        
        transform.localPosition = _mainCamera.pixelWidth > _mainCamera.pixelHeight 
            ? new Vector3(maxSize / 2.0f - 0.5f, minSize / 2.0f - 0.5f, -50) 
            : new Vector3(minSize / 2.0f - 0.5f, maxSize / 2.0f - 0.5f, -50);
    }
}

