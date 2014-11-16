using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AbstractBlock), true)]
public class AbstractBlockInspector : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        AbstractBlock thisBlock = target as AbstractBlock;
        thisBlock.leftSpiked = EditorGUILayout.Toggle("Spiked Left", thisBlock.leftSpiked);
        thisBlock.rightSpiked = EditorGUILayout.Toggle("Spiked Right", thisBlock.rightSpiked);
        thisBlock.upSpiked = EditorGUILayout.Toggle("Spiked Up", thisBlock.upSpiked);
        thisBlock.downSpiked = EditorGUILayout.Toggle("Spiked Down", thisBlock.downSpiked);
    }
}
