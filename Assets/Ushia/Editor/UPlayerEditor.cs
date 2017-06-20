using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UPlayer))]
[CanEditMultipleObjects]
public class UPlayerEditor : Editor
{
    private static string googleParseError = "Invalid URL - use google maps url";

    bool foldoutWorldGeneration = true;
    SerializedProperty genTerrain;
    SerializedProperty genOSM;

    bool foldoutChunkOptions = true;
    SerializedProperty chunkSize;
    SerializedProperty chunkAdjacentLayers;
    SerializedProperty maxResidualChunks;

    bool foldoutWorldLocation = true;
    bool useGoogleURL = true;
    string url;
    SerializedProperty startLon;
    SerializedProperty startLat;
    SerializedProperty zoom;

    SerializedProperty _clearHashMap;

    bool foldoutDebugGizmos = false;
    SerializedProperty debugChunks;
    SerializedProperty debugDistances;
    SerializedProperty debugPositions;
    SerializedProperty debugOSM;

    void OnEnable()
    {
        genTerrain = serializedObject.FindProperty("genTerrain");
        genOSM = serializedObject.FindProperty("genOSM");

        chunkSize = serializedObject.FindProperty("chunkSize");
        chunkAdjacentLayers = serializedObject.FindProperty("chunkAdjacentLayers");
        maxResidualChunks = serializedObject.FindProperty("maxResidualChunks");

        startLon = serializedObject.FindProperty("startLon");
        startLat = serializedObject.FindProperty("startLat");
        zoom = serializedObject.FindProperty("zoom");

        _clearHashMap = serializedObject.FindProperty("_clearHashMap");

        debugChunks = serializedObject.FindProperty("debugChunks");
        debugDistances = serializedObject.FindProperty("debugDistances");
        debugPositions = serializedObject.FindProperty("debugPositions");
        debugOSM = serializedObject.FindProperty("debugOSM");
    }

    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();

        /// generation
        if (foldoutWorldGeneration = EditorGUILayout.Foldout(foldoutWorldGeneration, "Generation", EditorStyles.foldout))
        {
            /// API key
            if (UVariables.mapzenAPIKey == "")
            {
                Color old = GUI.color;
                GUI.color = new Color(0.8f, 0.2f, 0.2f);
                EditorGUILayout.HelpBox("You don\'t have a Mapzen API key.\nYou can change the Mapzen API key in UVariables.cs.\nGet one in: \nhttps://mapzen.com/developers/sign_in", MessageType.Info);
                GUI.color = old;
            }
            else
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField(new GUIContent("Current Mapzen API key is \"" + UVariables.mapzenAPIKey + "\"", "You can change the \"Mapzen API key\" in UVariables.cs.\nIf you don't have one, get it in: \nhttps://mapzen.com/developers/sign_in"));
                EditorGUILayout.EndVertical();
            }

            GUILayout.BeginHorizontal();
            genTerrain.boolValue = UEditor.UToggleButton(genTerrain.boolValue, "Terrain");
            genOSM.boolValue = UEditor.UToggleButton(genOSM.boolValue, "OSM Data");
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

        /// chunks options
        if (foldoutChunkOptions = EditorGUILayout.Foldout(foldoutChunkOptions, "Chunk options", EditorStyles.foldout))
        {
            chunkSize.floatValue = EditorGUILayout.FloatField("Chunk size", chunkSize.floatValue);
            chunkAdjacentLayers.intValue = EditorGUILayout.IntSlider("Chunk adjacent layers", chunkAdjacentLayers.intValue, 0, 10);
            maxResidualChunks.intValue = EditorGUILayout.IntField("Max residual chunks", maxResidualChunks.intValue);
        }

        EditorGUILayout.Space();

        /// world location
        if (foldoutWorldLocation = EditorGUILayout.Foldout(foldoutWorldLocation, "World location", EditorStyles.foldout))
        {
            GUILayout.BeginHorizontal();
            url = EditorGUILayout.TextField("Google maps URL", url);
            if(UEditor.UToggleButton("Clear", options: GUILayout.Width(50)))
            {
                url = "";
            }
            GUILayout.EndHorizontal();
            string parsedUrl = parserGoogleUrl(url);
            if (parsedUrl != googleParseError)
            {
                startLat.doubleValue = EditorGUILayout.DoubleField("Latitude", double.Parse(parsedUrl.Split(',')[0]));
                startLon.doubleValue = EditorGUILayout.DoubleField("Longitude", double.Parse(parsedUrl.Split(',')[1]));
            }
            else
            {
                startLat.doubleValue = EditorGUILayout.DoubleField("Latitude", startLat.doubleValue);
                startLon.doubleValue = EditorGUILayout.DoubleField("Longitude", startLon.doubleValue);
            }
            zoom.intValue = EditorGUILayout.IntSlider("Zoom", zoom.intValue, 0, 15);
        }
        EditorGUILayout.Space();

        /// clear map
        _clearHashMap.boolValue = UEditor.UToggleButton(_clearHashMap.boolValue, "Clear Map", options : GUILayout.Height(30));

        EditorGUILayout.Space();

        /// debug
        if (foldoutDebugGizmos = EditorGUILayout.Foldout(foldoutDebugGizmos, "Debug Gizmos", EditorStyles.foldout))
        {
            GUILayout.BeginHorizontal();
            debugChunks.boolValue = UEditor.UToggleButton(debugChunks.boolValue, "Chunks");
            debugDistances.boolValue = UEditor.UToggleButton(debugDistances.boolValue, "Distances");
            debugPositions.boolValue = UEditor.UToggleButton(debugPositions.boolValue, "Positions");
            debugOSM.boolValue = UEditor.UToggleButton(debugOSM.boolValue, "OSM Data");
            GUILayout.EndHorizontal();
        }

        this.serializedObject.ApplyModifiedProperties();
    }
    
    private string parserGoogleUrl(string url)
    {
        string[] coord = null;
        try { coord = url.Split('@')[1].Split(','); } catch { }
        if(coord != null)
            return coord[0] + ',' + coord[1];
        return googleParseError;
    }
}