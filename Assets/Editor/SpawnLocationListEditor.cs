using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpawnLocationList))]
public class SpawnLocationListEditor : Editor
{
	SpawnLocationList spawnLocationList;
	SerializedObject targetSpawnLocationList;
	SerializedProperty thisSpawnLocationList;
	int listSize;

	void OnEnable()
	{
		spawnLocationList = (SpawnLocationList)target;
		targetSpawnLocationList = new SerializedObject(spawnLocationList);
		thisSpawnLocationList = targetSpawnLocationList.FindProperty("spawnLocations");
	}

	public override void OnInspectorGUI()
	{
		targetSpawnLocationList.Update();

		listSize = thisSpawnLocationList.arraySize;
		listSize = EditorGUILayout.IntField("Number of Spawn Locations", listSize);

		if(listSize != thisSpawnLocationList.arraySize)
		{
			while(listSize > thisSpawnLocationList.arraySize)
			{
				//spawnLocationList.AddNew();
				thisSpawnLocationList.InsertArrayElementAtIndex(thisSpawnLocationList.arraySize);
			}
			while(listSize < thisSpawnLocationList.arraySize)
			{
				thisSpawnLocationList.DeleteArrayElementAtIndex(thisSpawnLocationList.arraySize - 1);
			}
		}

		for(int i = 0;i < thisSpawnLocationList.arraySize;i++)
		{
			SerializedProperty currentSpawnLocation = thisSpawnLocationList.GetArrayElementAtIndex(i);
			SerializedProperty currentPlayerId = currentSpawnLocation.FindPropertyRelative("playerId");
			SerializedProperty currentX = currentSpawnLocation.FindPropertyRelative("x");
			SerializedProperty currentY = currentSpawnLocation.FindPropertyRelative("y");

			EditorGUILayout.PropertyField(currentPlayerId);
			EditorGUILayout.PropertyField(currentX);
			EditorGUILayout.PropertyField(currentY);

			EditorGUILayout.LabelField("Delete Spawn Location");
			if(GUILayout.Button("Delete"))
			{
				thisSpawnLocationList.DeleteArrayElementAtIndex(i);
			}
		}

		/*
		SpawnLocationList spawnLocation = (SpawnLocationList)target;

		spawnLocation.playerId = EditorGUILayout.IntField("PlayerID", spawnLocation.playerId);
		spawnLocation.x = EditorGUILayout.IntField("x", spawnLocation.x);
		spawnLocation.y = EditorGUILayout.IntField("y", spawnLocation.y);*/


		targetSpawnLocationList.ApplyModifiedProperties();
	}
}