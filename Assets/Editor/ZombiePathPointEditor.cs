using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(ZombiePathPoint))]
public class ZombiePathPointEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ZombiePathPoint zpg = (ZombiePathPoint)target;
        if (GUILayout.Button("Display Paths"))
        {
            if (zpg.IsSceneBound())
            {
                zpg.DebugDisplay();
            }
        }
        if (GUILayout.Button("Display Paths to this object"))
        {
            if (zpg.IsSceneBound())
            {
                zpg.GetDebugsToThisPoint();
            }
        }
    }
}
