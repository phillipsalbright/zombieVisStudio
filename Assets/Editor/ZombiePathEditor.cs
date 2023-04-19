using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(ZombiePathGenerator))]
public class ZombiePathEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ZombiePathGenerator zpg = (ZombiePathGenerator)target;
        if (GUILayout.Button("Generate"))
        {
            if (zpg.IsSceneBound())
            {
                zpg.Generate();
            }
        }
        if (GUILayout.Button("Display Paths"))
        {
            if (zpg.IsSceneBound())
            {
                zpg.DebugLines();
            }
        }
    }
}
