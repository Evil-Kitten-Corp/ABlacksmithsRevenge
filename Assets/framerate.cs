using UnityEngine;

public class framerate : MonoBehaviour 
{
    private int FPS = 60;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() 
    {
        Application.targetFrameRate = FPS;
    }

}
