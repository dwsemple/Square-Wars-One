using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpawnLocation))]
public class SpawnLocationEditor : Editor
{
	public override void OnInspectorGUI()
	{
		SpawnLocation spawnLocation = (SpawnLocation)target;

		spawnLocation.playerId = EditorGUILayout.IntField("PlayerID", spawnLocation.playerId);
		spawnLocation.x = EditorGUILayout.IntField("x", spawnLocation.x);
		spawnLocation.y = EditorGUILayout.IntField("y", spawnLocation.y);
	}
}
