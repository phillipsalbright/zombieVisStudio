using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviorSetup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int resx = Screen.resolutions[0].width;
        int resy = Screen.resolutions[0].height;
        Screen.SetResolution(resx, Mathf.RoundToInt(1080 * resx / 15360), true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
