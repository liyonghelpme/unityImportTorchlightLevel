using UnityEngine;
using System.Collections;
using UnityEditor;
using SimpleJSON;
using System.IO;
using System.Collections.Generic;


public class LevelConfig {
    public string room;
    public int x;
    public int y;

    public LevelConfig(string r, int x1, int y1) {
        room = r;
        x = x1;
        y = y1;
    }
}

[CustomEditor(typeof(MakeScene))]
public class MakeSceneEditor : Editor
{
    string dir = "";
    string layoutStr = "";
    string modelStr = "";
    string lightStr = "";

    public override void OnInspectorGUI()
    {
        MakeScene.makeScene = target as MakeScene;
        dir = GUILayout.TextField(dir);
        if (GUILayout.Button("组合模型Props"))
        {
            var md = Resources.LoadAssetAtPath("Assets/Config/Props.dat.json", typeof(TextAsset)) as TextAsset;
            var jobj = JSON.Parse(md.text).AsObject;
            int count = 0;
            foreach (KeyValuePair<string, JSONNode> N in jobj)
            {
                count++;
                var content = N.Value.AsObject;
                var fileName = content ["filename"];
                var colName = content ["collisionfile"];
                Debug.Log("fileName:" + fileName + " " + colName.ToString());
                if (colName.ToString() != "\"null\"")
                {
                    string fn = null;
                    fn = Path.GetFileName(fileName);
                    
                    
                    var fbx = fn.Replace(".mesh", ".fbx");
                    var prefab = fn.Replace(".mesh", ".prefab");
                    var oldFile = Path.Combine("Assets/levelsets/props", fbx);
                    Debug.Log("oldFile:" + oldFile);
                    var g = Resources.LoadAssetAtPath<GameObject>(oldFile);
                    if (g == null)
                    {
                        Debug.Log("load fbx error:" + fbx);
                    } else
                    {
                        var tar = Path.Combine("Assets/prefabs/props", prefab);
                        Debug.Log("Prefab is " + tar + " " + fbx + " GameObje: " + g);
                        var tg = PrefabUtility.CreatePrefab(tar, g);
                        var meshCollider = tg.AddComponent<MeshCollider>();
                    
                        var colN = Path.GetFileName(colName);
                        colN = Path.Combine("Assets/levelsets/props", colN.Replace(".mesh", ".fbx"));
                        var gcol = Resources.LoadAssetAtPath<GameObject>(colN);
                        meshCollider.sharedMesh = gcol.GetComponent<MeshFilter>().sharedMesh;
                    }
                    //break;
                } else
                {
                    string fn = Path.GetFileName(fileName);
                    var fbx = fn.Replace(".mesh", ".fbx");
                    var prefab = fn.Replace(".mesh", ".prefab");
                    Debug.Log("Create Collision null:" + fn);
                    var oldFile = Path.Combine("Assets/levelsets/props", fbx);
                    var tar = Path.Combine("Assets/prefabs/props", prefab);
                    var g = Resources.LoadAssetAtPath<GameObject>(oldFile);
                    if (g == null)
                    {
                        Debug.Log("load fbx error:"+fbx);   
                    } else
                    {
                        var tg = PrefabUtility.CreatePrefab(tar, g);
                    }
                }
            }
            Debug.Log("Export Count:" + count);
        }

        if (GUILayout.Button("组合模型Mine"))
        {
            var md = Resources.LoadAssetAtPath("Assets/Config/Mine.dat.json", typeof(TextAsset)) as TextAsset;
            var jobj = JSON.Parse(md.text).AsObject;
            int count = 0;
            foreach (KeyValuePair<string, JSONNode> N in jobj)
            {
                count++;
                var content = N.Value.AsObject;
                var fileName = content ["filename"];
                var colName = content ["collisionfile"];
                Debug.Log("fileName:" + fileName + " " + colName.ToString());
                if (colName.ToString() != "\"null\"")
                {
                    string fn = null;
                    fn = Path.GetFileName(fileName);
                 

                    var fbx = fn.Replace(".mesh", ".fbx");
                    var prefab = fn.Replace(".mesh", ".prefab");
                    var oldFile = Path.Combine("Assets/levelsets/mine", fbx);
                    Debug.Log("oldFile:" + oldFile);
                    var g = Resources.LoadAssetAtPath<GameObject>(oldFile);
                    var tar = Path.Combine("Assets/prefabs", prefab);
                    Debug.Log("Prefab is " + tar + " " + fbx + " GameObje: " + g);
                    var tg = PrefabUtility.CreatePrefab(tar, g);
                    var meshCollider = tg.AddComponent<MeshCollider>();

                    var colN = Path.GetFileName(colName);
                    colN = Path.Combine("Assets/levelsets/mine", colN.Replace(".mesh", ".fbx"));
                    var gcol = Resources.LoadAssetAtPath<GameObject>(colN);
                    meshCollider.sharedMesh = gcol.GetComponent<MeshFilter>().sharedMesh;
                    //break;
                } else
                {
                    string fn = Path.GetFileName(fileName);
                    var fbx = fn.Replace(".mesh", ".fbx");
                    var prefab = fn.Replace(".mesh", ".prefab");
                    Debug.Log("Create Collision null:" + fn);
                    var oldFile = Path.Combine("Assets/levelsets/mine", fbx);
                    var tar = Path.Combine("Assets/prefabs", prefab);
                    var g = Resources.LoadAssetAtPath<GameObject>(oldFile);
                    var tg = PrefabUtility.CreatePrefab(tar, g);
                    //tg.AddComponent<BoxCollider>();
                }
            }
            Debug.Log("Export Count:" + count);
        }
        layoutStr = GUILayout.TextField(layoutStr);

        if (GUILayout.Button("组合场景"))
        {

            var md = Resources.LoadAssetAtPath("Assets/Config/Mine.dat.json", typeof(TextAsset)) as TextAsset;
            var jobj = JSON.Parse(md.text).AsObject;
            /*
            foreach(KeyValuePair<string, JSONNode> N in jobj) {
                if(N.Key == "-3920187160188284450") {
                    Debug.Log("key is:"+N.Key);
                    //Debug.Log("value is "+ jobj[N.Key].ToString());
                    Debug.Log("is null "+(jobj[N.Key]==null));
                }
            }
            */
            //return;


            var layout = Resources.LoadAssetAtPath<TextAsset>("Assets/Config/"+layoutStr+".json");
            var jlist = JSON.Parse(layout.text).AsArray;
            Debug.Log("ArrayLength " + jlist.Count);
            int notFindCount = 0;
            int allPieces = 0;
            var r = GameObject.Find("root");
            if (r != null)
            {
                GameObject.DestroyImmediate(r);
            }

            var root = new GameObject("root");
            root.transform.localPosition = Vector3.zero;
            root.transform.localRotation = Quaternion.identity;
            root.transform.localScale = Vector3.one;

            ///
            /// ogre unity 
            /// 坐标系差别
            /// x 轴方向相反
            /// ogre为 右手坐标系
            /// unity为左手坐标系
            /// 因此旋转direction不同
            ///
            foreach (JSONNode n in jlist)
            {
                var obj = n.AsObject;
                var pieces = obj ["pieces"].AsArray;
                Debug.Log("pieces :" + pieces.Count);
                allPieces = pieces.Count;

                Dictionary<string, int> pieceId = new Dictionary<string, int>();
                foreach (JSONNode p in pieces)
                {
                    var pobj = p.AsObject;
                    var gid = pobj ["guid"].Value;

                    var meshFile = jobj [gid];
                    if (meshFile == null)
                    {
                        //Debug.Log("Not Find gid:"+gid+" "+gid.GetType());
                        notFindCount++;
                    } else
                    {
                        var fileName = meshFile ["filename"];
                        float px = -pobj ["posx"].AsFloat;
                        float py = pobj ["posy"].AsFloat;
                        float pz = pobj ["posz"].AsFloat;

                        float fx = pobj ["forx"].AsFloat;
                        float fy = pobj ["fory"].AsFloat;
                        float fz = pobj ["forz"].AsFloat;

                        float rx = pobj ["rix"].AsFloat;
                        float ry = pobj ["riy"].AsFloat;
                        float rz = pobj ["riz"].AsFloat;

                        var fn = Path.GetFileName(fileName.Value);
                        var prefab = fn.Replace(".mesh", ".prefab");
                        var oldFile = Path.Combine("Assets/prefabs", prefab);
                        Debug.Log("instantiate :" + oldFile);
                        var g = GameObject.Instantiate(Resources.LoadAssetAtPath<GameObject>(oldFile)) as GameObject;
                        int co = 0;
                        pieceId.TryGetValue(gid, out co);

                        g.name = g.name + "_" + co;
                        pieceId [gid] = co + 1;
                        g.transform.parent = root.transform;
                        g.transform.localPosition = new Vector3(px, py, pz);
                        //g.transform.localRotation = Quaternion.FromToRotation(Vector3.forward, new Vector3(fx, fy, fz))*Quaternion.Euler(new Vector3(-90, 0, 0));
                        //-90 0 0 
                        var rot = Quaternion.LookRotation(new Vector3(fx, fy, fz), Vector3.up);
                        var rot2 = Quaternion.Euler(new Vector3(rot.eulerAngles.x, -rot.eulerAngles.y, rot.eulerAngles.z));
                        g.transform.localRotation = rot2 * Quaternion.Euler(new Vector3(-90, 0, 0));// g.transform.localRotation;
                        g.transform.localScale = Vector3.one;
                    }
                }
            }
            Debug.Log("notFind:" + notFindCount);
            Debug.Log("allPieces:" + allPieces);
        }

        if (GUILayout.Button("Test"))
        {
            /*
            var q1 = Quaternion.Euler(-90, 0, 0);
            Debug.Log(q1.eulerAngles);
            var q2 = Quaternion.FromToRotation(Vector3.forward, new Vector3(0, 0, -1));
            Debug.Log(q2.eulerAngles);
            var nq = Quaternion.Euler(new Vector3(0, q2.eulerAngles.y, 0));
            Debug.Log(nq.eulerAngles);
            var q3 = nq*q1;
            Debug.Log(q3.eulerAngles);
            */

            var md = Resources.LoadAssetAtPath("Assets/Config/Mine.dat.json", typeof(TextAsset)) as TextAsset;
            var jobj = JSON.Parse(md.text).AsObject;
            var layout = Resources.LoadAssetAtPath<TextAsset>("Assets/Config/1X1ENTRANCE_E.json");
            var jlist = JSON.Parse(layout.text).AsArray;
            Debug.Log("ArrayLength " + jlist.Count);

            int notFindCount = 0;
            int allPieces = 0;

            foreach (JSONNode n in jlist)
            {
                var obj = n.AsObject;
                var pieces = obj ["pieces"].AsArray;
                Debug.Log("pieces :" + pieces.Count);
                allPieces = pieces.Count;
                
                Dictionary<string, int> pieceId = new Dictionary<string, int>();
                foreach (JSONNode p in pieces)
                {
                    var pobj = p.AsObject;
                    var gid = pobj ["guid"].Value;
                    
                    var meshFile = jobj [gid];
                    if (meshFile == null)
                    {
                        Debug.Log("Not Find gid:" + gid);
                        notFindCount++;
                    } else
                    {

                    }
                }
            }
            Debug.Log("notFind:" + notFindCount);
            Debug.Log("allPieces:" + allPieces);
        }
        if(GUILayout.Button("导出Scene中的root为room Prefab")) {
            var root = GameObject.Find("root");
            if(root != null) {
                var path = EditorApplication.currentScene.Split(char.Parse("/"));
                var sceneName = path[path.Length-1];
                PrefabUtility.CreatePrefab(Path.Combine("Assets/room", sceneName.Replace(".unity", ".prefab")), root);

            }
        }

        if(GUILayout.Button("根据配置文件构建关卡")) {
            var r = GameObject.Find("root");
            if (r != null)
            {
                GameObject.DestroyImmediate(r);
            }
            
            var root = new GameObject("root");
            root.transform.localPosition = Vector3.zero;
            root.transform.localRotation = Quaternion.identity;
            root.transform.localScale = Vector3.one;


            var config = new List<LevelConfig>(){
                new LevelConfig("ENTRANCE_S", -1, 3),
                new LevelConfig("NS", -1, 2),
                new LevelConfig("NS", -1, 1),
                new LevelConfig("NE", -1, 0),
                new LevelConfig("EW", 0, 0),
                new LevelConfig("NW", 1, 0),
                new LevelConfig("NS", 1, 1),
                new LevelConfig("Exit_S", 1, 2),
            };

            //var load = Resources.LoadAssetAtPath("Assets/scenes/1X1_NS.unity", typeof(UnityEngine.SceneAsset));
            //Debug.Log("load scene is "+load);
            var roomPath = Path.Combine(Application.dataPath, "room");
            var resDir = new DirectoryInfo(roomPath);
            FileInfo[] fileInfo = resDir.GetFiles("*.prefab", SearchOption.AllDirectories); 
            List<GameObject> nameToGameObject = new List<GameObject>();

            foreach(FileInfo f in fileInfo) {
                Debug.Log("fileName "+f.FullName);
                Debug.Log("DataPath "+Application.dataPath);
                var pa = f.FullName.Replace(Application.dataPath, "Assets");

                var pre = Resources.LoadAssetAtPath<GameObject>(pa);

                nameToGameObject.Add(pre);
            }
            Debug.Log("prefab num "+nameToGameObject.Count);
            foreach(LevelConfig lc in config) {
                var namePart = lc.room.ToLower().Split(char.Parse("_"));
                bool gotPrefab = false;
                GameObject insPrefab = null;
                foreach(GameObject g in nameToGameObject) {
                    var prefabElement = g.name.ToLower().Split(char.Parse("_"));
                    bool find = true;
                    foreach(string n in namePart) {
                        if(!checkIn(n, prefabElement)) {
                            find = false;
                            break;
                        }
                    }
                    if(find) {
                        insPrefab = g;
                        gotPrefab = true;
                        Debug.Log("find part "+lc.room+" "+g.name);
                        break;
                    }
                }
                if(!gotPrefab) {
                    Debug.Log("not find "+lc.room);
                }else {
                    var newG = GameObject.Instantiate(insPrefab) as GameObject;
                    newG.transform.parent = root.transform;
                    newG.transform.localPosition = new Vector3(lc.x*96, 0, lc.y*96+48);
                    newG.transform.localScale = Vector3.one;
                    newG.transform.localRotation = Quaternion.identity;
                }
            }
        }


        modelStr = GUILayout.TextField(modelStr);
        if (GUILayout.Button("导入无动画模型"))
        {
            
            
            Debug.Log(Application.dataPath);
            
            var allModel = Path.Combine(Application.dataPath, modelStr);
            var resDir = new DirectoryInfo(allModel);
            FileInfo[] fileInfo = resDir.GetFiles("*.fbx", SearchOption.AllDirectories);
            AssetDatabase.StartAssetEditing();
            foreach(FileInfo file in fileInfo) {
                Debug.Log("file is "+file.Name+" "+file.Name);
                
                var ass = Path.Combine("Assets/"+modelStr, file.Name);
                var import = ModelImporter.GetAtPath(ass) as ModelImporter;
                Debug.Log("import is " + import);
                import.globalScale = 1;
                import.importAnimation = false;
                import.animationType = ModelImporterAnimationType.None;
                
                Debug.Log("import change state "+import);
                AssetDatabase.WriteImportSettingsIfDirty(ass);
            }
            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();
        }

        if(GUILayout.Button("设置shader 为Custom/light")) {
            Debug.Log(Application.dataPath);
            
            var allModel = Path.Combine(Application.dataPath, modelStr);
            var resDir = new DirectoryInfo(allModel);
            FileInfo[] fileInfo = resDir.GetFiles("*.fbx", SearchOption.AllDirectories);
            AssetDatabase.StartAssetEditing();
            foreach(FileInfo file in fileInfo) {
                Debug.Log("file is "+file.Name+" "+file.Name);
                
                var ass = Path.Combine("Assets/"+modelStr, file.Name);
                var res = Resources.LoadAssetAtPath<GameObject>(ass);
                res.renderer.sharedMaterial.shader = Shader.Find("Custom/light");
                EditorUtility.SetDirty(res.renderer.sharedMaterial);

                Debug.Log("import change state ");
                AssetDatabase.WriteImportSettingsIfDirty(ass);
            }
            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();

        }
        if(GUILayout.Button("调整prefab的layer属性")) {
            var allModel = Path.Combine(Application.dataPath, modelStr);
            var resDir = new DirectoryInfo(allModel);
       
            FileInfo[] fileInfo = resDir.GetFiles("*.fbx", SearchOption.AllDirectories);
            //AssetDatabase.StartAssetEditing();
            foreach(FileInfo file in fileInfo) {
                var ass = Path.Combine("Assets/"+modelStr, file.Name);
                var res = Resources.LoadAssetAtPath<GameObject>(ass);
                var tar = Path.Combine("Assets/lightPrefab", file.Name.Replace(".fbx", ".prefab"));
                var tg = PrefabUtility.CreatePrefab(tar, res);
                tg.layer =  8;


            }
            //AssetDatabase.StopAssetEditing();
            //AssetDatabase.Refresh();
        }

        lightStr = GUILayout.TextField(lightStr);
        if(GUILayout.Button("读取生成所有的light位置")) {
            GameObject lightObj = GameObject.Find("light");
            if(lightObj != null) {
                GameObject.DestroyImmediate(lightObj);
            }
            lightObj = new GameObject("light");

            var light = Resources.LoadAssetAtPath("Assets/Config/"+lightStr+".json", typeof(TextAsset)) as TextAsset;
            var larr = JSON.Parse(light.text) as JSONArray;
            foreach(JSONNode n in larr) {
                var ln = Path.GetFileName(n["FILE"].Value);
                Debug.Log("light file name "+ln);
                var pb = "Assets/lightPrefab/"+ln.Replace(".MESH", ".prefab");
                var lobj = Resources.LoadAssetAtPath(pb, typeof(GameObject)) as GameObject;
                var copyobj = GameObject.Instantiate(lobj) as GameObject;
                copyobj.transform.parent = lightObj.transform;
                copyobj.transform.localPosition = new Vector3(-n["POSITIONX"].AsFloat, n["POSITIONY"].AsFloat, n["POSITIONZ"].AsFloat);
                copyobj.transform.localScale = new Vector3(n["SCALE X"].AsFloat, n["SCALE Z"].AsFloat, 1);
                var rot = Quaternion.Euler(new Vector3(0, n["ANGLE"].AsFloat, 0));
                copyobj.transform.localRotation = rot*Quaternion.Euler(new Vector3(-90, 0, 0));

            }
        }

    }
    bool checkIn(string s, string[] group) {
        foreach(string s1 in group) {
            if(s == s1) 
                return true;
        }
        return false;
    }
}
