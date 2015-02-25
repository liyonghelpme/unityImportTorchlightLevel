using UnityEngine;
using System.Collections;
using UnityEditor;
using SimpleJSON;
using System.IO;
using System.Collections.Generic;

[CustomEditor(typeof(MakeScene))]
public class MakeSceneEditor : Editor
{
    string dir = "";
    string layoutStr = "";
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
    }
}
