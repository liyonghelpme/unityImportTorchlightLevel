using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.VersionControl;
using System.IO;

[CustomEditor(typeof(TestModelmport))]
public class ImportFbx : Editor
{
    void Awake()
    {

        //ModelImporter.globalScale = 1;
        //ModelImporter.importAnimation = false;

    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Set Import"))
        {


            Debug.Log(Application.dataPath);

            var allModel = Path.Combine(Application.dataPath, "levelsets/mine");
            var resDir = new DirectoryInfo(allModel);
            FileInfo[] fileInfo = resDir.GetFiles("*.fbx", SearchOption.AllDirectories);
            AssetDatabase.StartAssetEditing();
            foreach(FileInfo file in fileInfo) {
                Debug.Log("file is "+file.Name+" "+file.Name);
                    
                var ass = Path.Combine("Assets/levelsets/mine", file.Name);
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
    }


}
