using UnityEngine;
using System.Collections;

public class TestLightMap : MonoBehaviour {
	public Texture2D lightMap;
	public Vector4 camPos;
	public float camSize;
	void Awake() {
	}
	// Use this for initialization
	void Start () {
		lightMap = new Texture2D(256, 256);
		var cols = new Color[lightMap.width*lightMap.height];
		for(int i = 0; i < cols.Length; i++) {
			cols[i] = Color.white;
		}
		lightMap.SetPixels(cols);
		lightMap.Apply();

		Shader.SetGlobalTexture("_LightMap", lightMap);
		Shader.SetGlobalVector("_CamPos", camPos);
		Shader.SetGlobalFloat("_CameraSize", camSize);

	}

	
	// Update is called once per frame
	void Update () {
	
	}
}
