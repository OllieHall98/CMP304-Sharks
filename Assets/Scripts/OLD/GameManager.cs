using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private enum CameraType{Static, Follow};
    CameraType cameraType;
    [SerializeField] private Camera followCamera = null;
    [SerializeField] private Camera staticCamera = null;

    // Start is called before the first frame update
    void Start()
    {
        cameraType = CameraType.Follow;
        disableCamera(staticCamera);
        enableCamera(followCamera);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void switchCameraType()
    {
        if(cameraType == CameraType.Follow)
        {
            disableCamera(followCamera);
            enableCamera(staticCamera);

            cameraType = CameraType.Static;
        }

        else if (cameraType == CameraType.Static)
        {
            disableCamera(staticCamera);
            enableCamera(followCamera);

            cameraType = CameraType.Follow;
        }
    }

    public void disableCamera(Camera cam)
    {
        if (cam.enabled)
        {
            cam.enabled = false;
        }
    }

    public void enableCamera(Camera cam)
    {
        if (!cam.enabled)
        {
            cam.enabled = true;
        }
    }

    public void changeTimeScale(string timeScale)
    {
        bool result;
        int value;

        result = int.TryParse(timeScale, out value);

        if (result)
        {
            Time.timeScale = value;
        }

    }


}
