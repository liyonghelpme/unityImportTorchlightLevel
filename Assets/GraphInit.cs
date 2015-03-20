using UnityEngine;
using System.Collections;

public class GraphInit : MonoBehaviour {
    public Texture lightMap;
    public Vector3 camPos = Vector3.zero;
    public float camSize = 10;
    public Vector3 ambient = Vector3.one;
    void Awake() {
        Shader.SetGlobalTexture("_LightMap", lightMap);
        Shader.SetGlobalVector("_CamPos", camPos);
        Shader.SetGlobalFloat("_CameraSize", camSize);
        Shader.SetGlobalVector("_AmbientCol", ambient);
    }
	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
