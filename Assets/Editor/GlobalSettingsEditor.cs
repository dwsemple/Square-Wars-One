using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GlobalSettings))]
public class GlobalSettingsEditor : Editor
{
	GlobalSettings spawnLocationList;
	SerializedObject targetSpawnLocationList;
	SerializedProperty thisSpawnLocationList;
	SerializedProperty numberOfPlayers;

	//bool spawnLocationsFolded;
	int listSize;

	void OnEnable()
	{
		spawnLocationList = (GlobalSettings)target;
		targetSpawnLocationList = new SerializedObject(spawnLocationList);
		thisSpawnLocationList = targetSpawnLocationList.FindProperty("spawnLocations");//.FindPropertyRelative("spawnLocations");
		numberOfPlayers = targetSpawnLocationList.FindProperty("numberOfPlayers");

		//spawnLocationsFolded = true;
	}

	public override void OnInspectorGUI()
	{
		targetSpawnLocationList.Update();
		//bool spawnLocationsSelected = EditorGUILayout.Foldout(spawnLocationsFolded, "Spawn Locations", true);
		EditorGUILayout.LabelField("Spawn Locations");

		//if (spawnLocationsSelected) {
		listSize = thisSpawnLocationList.arraySize;
		listSize = EditorGUILayout.IntField ("Number of Spawn Locations", listSize);

		if (listSize != thisSpawnLocationList.arraySize) {
			while (listSize > thisSpawnLocationList.arraySize) {
				//spawnLocationList.AddNew();
				thisSpawnLocationList.InsertArrayElementAtIndex (thisSpawnLocationList.arraySize);
			}
			while (listSize < thisSpawnLocationList.arraySize) {
				thisSpawnLocationList.DeleteArrayElementAtIndex (thisSpawnLocationList.arraySize - 1);
			}
		}

		for (int i = 0; i < thisSpawnLocationList.arraySize; i++) {
			SerializedProperty currentSpawnLocation = thisSpawnLocationList.GetArrayElementAtIndex (i);
			SerializedProperty currentPlayerId = currentSpawnLocation.FindPropertyRelative ("playerId");
			SerializedProperty currentX = currentSpawnLocation.FindPropertyRelative ("x");
			SerializedProperty currentY = currentSpawnLocation.FindPropertyRelative ("y");

			EditorGUILayout.PropertyField (currentPlayerId);
			EditorGUILayout.PropertyField (currentX);
			EditorGUILayout.PropertyField (currentY);

			EditorGUILayout.LabelField ("Delete Spawn Location");
			if (GUILayout.Button ("Delete")) {
				thisSpawnLocationList.DeleteArrayElementAtIndex (i);
			}
		}

			/*
		SpawnLocationList spawnLocation = (SpawnLocationList)target;

		spawnLocation.playerId = EditorGUILayout.IntField("PlayerID", spawnLocation.playerId);
		spawnLocation.x = EditorGUILayout.IntField("x", spawnLocation.x);
		spawnLocation.y = EditorGUILayout.IntField("y", spawnLocation.y);*/
		//}
		EditorGUILayout.PropertyField(numberOfPlayers);

		targetSpawnLocationList.ApplyModifiedProperties();
	}
}