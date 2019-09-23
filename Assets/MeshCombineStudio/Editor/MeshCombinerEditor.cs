using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;

namespace MeshCombineStudio
{
    [CustomEditor(typeof(MeshCombiner))]
    public class MeshCombinerEditor : Editor
    {
        enum VSyncCountMode { DontSync, EveryVBlank, EverySecondVBlank };
        VSyncCountMode vSyncCountMode;

        GameObject meshCombine; 
        MeshCombiner meshCombiner;

        // Search Options
        SerializedProperty drawGizmos, drawMeshBounds;
        SerializedProperty parent, searchOptions, objectCenter, useSearchBox, searchBoxSquare, searchBoxPivot, searchBoxSize;
        SerializedProperty useMaxBoundsFactor, maxBoundsFactor, useVertexInputLimit, vertexInputLimit, vertexInputLimitLod, useVertexInputLimitLod;
        SerializedProperty useLayerMask, layerMask, useTag, tag, useNameContains, nameContainList, onlyActive, onlyStatic;
        SerializedProperty useComponentsFilter, componentCondition, componentNameList;

        // Output Settings
        SerializedProperty useCells, cellSize, cellOffset, useVertexOutputLimit, vertexOutputLimit, addMeshColliders, makeMeshesUnreadable;
        SerializedProperty removeTrianglesBelowSurface, surfaceLayerMask, maxSurfaceHeight;
        SerializedProperty removeBackFaceTriangles, backFaceTriangleMode, backFaceDirection, backFaceBounds, twoSidedShadows;
        SerializedProperty scaleInLightmap, outputLayer;
        SerializedProperty copyBakedLighting, validCopyBakedLighting, rebakeLighting, rebakeLightingMode;
        
        // Runtime
        SerializedProperty combineInRuntime, combineOnStart, useCombineSwapKey, originalMeshRenderers, originalLODGroups; // combineSwapKey

        // JoSettings
        SerializedProperty jobSettings, combineJobMode, combineMeshesPerFrame, threadAmountMode, customThreadAmount;
        SerializedProperty useMultiThreading, useMainThread, showStats;

        SerializedProperty activeOriginal;
        
        SerializedObject jobManagerSerializedObject;

        float editorSkinMulti;
        
        private void OnEnable()
        {
            editorSkinMulti = EditorGUIUtility.isProSkin ? 1 : 0.35f;

            meshCombiner = (MeshCombiner)target;
            Transform t = meshCombiner.transform;
            t.hideFlags = HideFlags.HideInInspector;
            
            drawGizmos = serializedObject.FindProperty("drawGizmos");
            drawMeshBounds = serializedObject.FindProperty("drawMeshBounds");

            // SearchParent
            searchOptions = serializedObject.FindProperty("searchOptions");
            objectCenter = searchOptions.FindPropertyRelative("objectCenter");
            useSearchBox = searchOptions.FindPropertyRelative("useSearchBox");
            searchBoxPivot = searchOptions.FindPropertyRelative("searchBoxPivot");
            searchBoxSize = searchOptions.FindPropertyRelative("searchBoxSize");
            searchBoxSquare = searchOptions.FindPropertyRelative("searchBoxSquare");
            parent = searchOptions.FindPropertyRelative("parent");

            useMaxBoundsFactor = searchOptions.FindPropertyRelative("useMaxBoundsFactor");
            maxBoundsFactor = searchOptions.FindPropertyRelative("maxBoundsFactor");
            useVertexInputLimit = searchOptions.FindPropertyRelative("useVertexInputLimit");
            vertexInputLimit = searchOptions.FindPropertyRelative("vertexInputLimit");
            useVertexInputLimitLod = searchOptions.FindPropertyRelative("useVertexInputLimitLod");
            vertexInputLimitLod = searchOptions.FindPropertyRelative("vertexInputLimitLod");
            useLayerMask = searchOptions.FindPropertyRelative("useLayerMask");
            layerMask = searchOptions.FindPropertyRelative("layerMask");
            useTag = searchOptions.FindPropertyRelative("useTag");
            tag = searchOptions.FindPropertyRelative("tag");
            onlyActive = searchOptions.FindPropertyRelative("onlyActive");
            onlyStatic = searchOptions.FindPropertyRelative("onlyStatic");
            useComponentsFilter = searchOptions.FindPropertyRelative("useComponentsFilter");
            componentCondition = searchOptions.FindPropertyRelative("componentCondition");
            componentNameList = searchOptions.FindPropertyRelative("componentNameList");
            useNameContains = searchOptions.FindPropertyRelative("useNameContains");
            nameContainList = searchOptions.FindPropertyRelative("nameContainList");

            // Output Settings
            useCells = serializedObject.FindProperty("useCells");
            cellSize = serializedObject.FindProperty("cellSize");
            cellOffset = serializedObject.FindProperty("cellOffset");
            addMeshColliders = serializedObject.FindProperty("addMeshColliders");
            makeMeshesUnreadable = serializedObject.FindProperty("makeMeshesUnreadable");
            useVertexOutputLimit = serializedObject.FindProperty("useVertexOutputLimit");
            vertexOutputLimit = serializedObject.FindProperty("vertexOutputLimit");
            copyBakedLighting = serializedObject.FindProperty("copyBakedLighting");
            validCopyBakedLighting = serializedObject.FindProperty("validCopyBakedLighting");
            rebakeLighting = serializedObject.FindProperty("rebakeLighting");
            rebakeLightingMode = serializedObject.FindProperty("rebakeLightingMode");
            scaleInLightmap = serializedObject.FindProperty("scaleInLightmap");
            outputLayer = serializedObject.FindProperty("outputLayer");

            removeTrianglesBelowSurface = serializedObject.FindProperty("removeTrianglesBelowSurface");
            surfaceLayerMask = serializedObject.FindProperty("surfaceLayerMask");
            maxSurfaceHeight = serializedObject.FindProperty("maxSurfaceHeight");

            removeBackFaceTriangles = serializedObject.FindProperty("removeBackFaceTriangles");
            backFaceTriangleMode = serializedObject.FindProperty("backFaceTriangleMode");
            backFaceDirection = serializedObject.FindProperty("backFaceDirection");
            backFaceBounds = serializedObject.FindProperty("backFaceBounds");
            twoSidedShadows = serializedObject.FindProperty("twoSidedShadows");

            // Runtime
            combineInRuntime = serializedObject.FindProperty("combineInRuntime");
            combineOnStart = serializedObject.FindProperty("combineOnStart");
            useCombineSwapKey = serializedObject.FindProperty("useCombineSwapKey");
            // combineSwapKey = serializedObject.FindProperty("combineSwapKey");
            originalMeshRenderers = serializedObject.FindProperty("originalMeshRenderers");
            originalLODGroups = serializedObject.FindProperty("originalLODGroups");

            activeOriginal = serializedObject.FindProperty("activeOriginal");

            jobSettings = serializedObject.FindProperty("jobSettings");

            combineJobMode = jobSettings.FindPropertyRelative("combineJobMode");
            combineMeshesPerFrame = jobSettings.FindPropertyRelative("combineMeshesPerFrame");
            threadAmountMode = jobSettings.FindPropertyRelative("threadAmountMode");
            customThreadAmount = jobSettings.FindPropertyRelative("customThreadAmount");
            useMultiThreading = jobSettings.FindPropertyRelative("useMultiThreading");
            useMainThread = jobSettings.FindPropertyRelative("useMainThread");
            showStats = jobSettings.FindPropertyRelative("showStats");

            meshCombiner.GetOriginalRenderersAndLODGroups();

            if (meshCombiner.instantiatePrefab == null) SetInstantiatePrefabReference();
        }

        void SetInstantiatePrefabReference()
        {
            string path = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(meshCombiner));
            path = path.Replace("/Scripts/Mesh/MeshCombiner.cs", "/Sources/InstantiatePrefab.prefab");

            meshCombiner.instantiatePrefab = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
        }

        private void OnDisable()
        {
            Tools.hidden = false;
        }
        
        void OnSceneGUI()
        {
            ApplyTransformLock();

            if (Tools.current == Tool.Rotate || Tools.current == Tool.Move || Tools.current == Tool.Scale) Tools.hidden = true;
            else { Tools.hidden = false; return; }

            serializedObject.Update();

            if (removeBackFaceTriangles.boolValue)
            {
                Bounds bounds = backFaceBounds.boundsValue;

                if (backFaceTriangleMode.enumValueIndex == (int)MeshCombiner.BackFaceTriangleMode.Direction)
                {
                    Quaternion rot = Handles.RotationHandle(Quaternion.Euler(backFaceDirection.vector3Value), bounds.center);
                    backFaceDirection.vector3Value = rot.eulerAngles;
                    Handles.color = new Color(0.19f, 0.4f, 898f, 1);
                    #if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4
                    Handles.ArrowCap(0, bounds.center, rot, HandleUtility.GetHandleSize(bounds.center));
                    #else
                    Handles.ArrowHandleCap(0, bounds.center, rot, HandleUtility.GetHandleSize(bounds.center), EventType.Repaint);
                    #endif

                    Handles.color = Color.white;
                }
                else
                {
                    if (Tools.current == Tool.Move)
                    {
                        bounds.center = Handles.PositionHandle(bounds.center, Quaternion.identity);
                    }
                    else if (Tools.current == Tool.Scale)
                    {
                        bounds.size = Handles.ScaleHandle(bounds.size, bounds.center, Quaternion.identity, HandleUtility.GetHandleSize(bounds.center));
                        bounds.size = Methods.SetMin(bounds.size, 0.001f);
                    }
                    backFaceBounds.boundsValue = bounds;
                }
                Handles.color = new Color(0, 0, 1, 0.15f);
                Handles.DrawSolidDisc(bounds.center, Vector3.up, HandleUtility.GetHandleSize(bounds.center) * 0.33f);
                Handles.color = Color.white;
            }

            if (useSearchBox.boolValue)
            {
                if (Tools.current == Tool.Move)
                {
                    searchBoxPivot.vector3Value = Handles.PositionHandle(searchBoxPivot.vector3Value, Quaternion.identity);
                }
                else if (Tools.current == Tool.Scale)
                {
                    searchBoxSize.vector3Value = Handles.ScaleHandle(searchBoxSize.vector3Value, searchBoxPivot.vector3Value, Quaternion.identity, HandleUtility.GetHandleSize(searchBoxPivot.vector3Value));
                }
                Handles.color = new Color(1, 0, 0, 0.15f);
                Handles.DrawSolidDisc(searchBoxPivot.vector3Value, Vector3.up, HandleUtility.GetHandleSize(searchBoxPivot.vector3Value) * 0.33f);
                Handles.color = Color.white;

                ApplyScaleLimit();
            }
            serializedObject.ApplyModifiedProperties();
        }
        
        public override void OnInspectorGUI()
        {
            meshCombiner = (MeshCombiner)target;
            serializedObject.Update();

            // DrawDefaultInspector();
            if (meshCombiner.lodGroupsSettings == null || meshCombiner.lodGroupsSettings.Length != 8) meshCombiner.CreateLodGroupsSettings();

            DrawInspectorGUI();
            
            if (meshCombiner.searchOptions.useSearchBox)
            {
                if (!meshCombiner.combined && (searchBoxPivot.vector3Value != meshCombiner.oldPosition || searchBoxSize.vector3Value != meshCombiner.oldScale))
                {
                    if (meshCombiner.octreeContainsObjects)
                    { 
                        // Debug.Log("Reset");
                        meshCombiner.ResetOctree();
                    }
                    meshCombiner.oldPosition = searchBoxPivot.vector3Value;
                    meshCombiner.oldScale = searchBoxSize.vector3Value;
                }
            }

            if (searchBoxSquare.boolValue)
            {
                float sizeX = searchBoxSize.vector3Value.x;
                searchBoxSize.vector3Value = new Vector3(sizeX, sizeX, sizeX);
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Search"))
            {
                meshCombiner.AddObjectsAutomatically();
                SceneView.RepaintAll();
            }
            
            if (!combineInRuntime.boolValue)
            { 
                GUILayout.Space(10);

                string buttonText;
                if (meshCombiner.jobCount == 0) buttonText = "Combine"; else buttonText = "Cancel";
                if (GUILayout.Button(buttonText))
                {
                    if (meshCombiner.jobCount == 0) meshCombiner.CombineAll();
                    else
                    {
                        MeshCombineJobManager.instance.AbortJobs();
                        meshCombiner.jobCount = 0;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            
            if (meshCombiner.unreadableMeshes.Count > 0)
            {
                float v = Mathf.Abs(Mathf.Cos(Time.realtimeSinceStartup * 5));
                GUI.backgroundColor = new Color(v, 0, 0, 1);
                if (GUILayout.Button("Make Meshes Readable"))
                {
                    meshCombiner.MakeMeshesReadableInImportSettings();
                    meshCombiner.AddObjectsAutomatically();
                }
                GUI.backgroundColor = Color.white;
                GUIDraw.DrawSpacer();
                Repaint();
            }

            bool hasFoundObjects = (meshCombiner.foundObjects.Count > 0 || meshCombiner.foundLodObjects.Count > 0);
            bool hasCombinedChildren = (meshCombiner.transform.childCount > 0);

            GUIDraw.DrawSpacer(2.5f, 5, 2.5f);

            EditorGUILayout.BeginHorizontal();
            if (!hasFoundObjects) GUI.color = Color.grey;
            if (GUILayout.Button("Select Original"))
            {
                if (hasFoundObjects) Methods.SelectChildrenWithMeshRenderer(((GameObject)parent.objectReferenceValue).transform);
            }
            if (!hasCombinedChildren) GUI.color = Color.grey; else GUI.color = Color.white;
            GUILayout.Space(10);
            if (GUILayout.Button("Select Combined"))
            {
                if (hasCombinedChildren) Methods.SelectChildrenWithMeshRenderer(meshCombiner.transform);
            }
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
            
            if (hasFoundObjects)
            {
                GUIDraw.DrawSpacer(2.5f, 5, 2.5f);
                string buttonText;
                if (activeOriginal.boolValue) { buttonText = "Disable Original Renderers and LODGroups"; GUI.backgroundColor = new Color(0.70f, 0.70f, 1); }
                else { buttonText = "Enable Original Renderers and LODGroups"; GUI.backgroundColor = new Color(0.70f, 1, 0.70f); }
                    
                if (GUILayout.Button(buttonText))
                {
                    activeOriginal.boolValue = !activeOriginal.boolValue;
                    meshCombiner.ExecuteHandleObjects(activeOriginal.boolValue, MeshCombiner.HandleComponent.Disable, MeshCombiner.HandleComponent.Disable);
                }
                GUI.backgroundColor = Color.white;
                GUIDraw.DrawSpacer(2.5f, 5, 2.5f);
                //GUILayout.Space(5);

            }
            
            if (hasCombinedChildren)
            {
                EditorGUILayout.BeginHorizontal();
                
                GUILayout.Space(3);

                if (GUILayout.Button("Save Combined"))
                {
                    if (Event.current.shift) meshCombiner.saveMeshesFolder = Application.dataPath;
                    SaveCombinedMeshes();
                }
                
                GUILayout.Space(3);

                GUI.backgroundColor = new Color(0.5f, 0.25f, 0.25f, 1);
                if (GUILayout.Button("Delete Combined"))
                {
                    if (meshCombiner.jobCount > 0)
                    {
                        MeshCombineJobManager.instance.AbortJobs();
                    }
                    meshCombiner.DestroyCombinedObjects();
                }
                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();
                GUIDraw.DrawSpacer(2.5f, 5, 5);
            }
            
            DisplayOctreeInfo();
            DisplayVertsAndTrisInfo();

            GUIDraw.DrawSpacer();

            serializedObject.ApplyModifiedProperties();
            // Debug.Log("Time " + stopwatch.ElapsedMilliseconds);
        }

        void SaveCombinedMeshes()
        {
            if (meshCombiner.saveMeshesFolder == "") meshCombiner.saveMeshesFolder = Application.dataPath;

            string filePath = EditorUtility.SaveFolderPanel("Save Combined Meshes", meshCombiner.saveMeshesFolder, "");
            if (filePath == "") return;
            else if (!filePath.Contains(Application.dataPath))
            {
                Debug.Log("Mesh Combine Studio -> Meshes need to be saved in one of this project folders.");
                return;
            }

            meshCombiner.saveMeshesFolder = filePath;

            MeshFilter[] mfs = meshCombiner.transform.GetComponentsInChildren<MeshFilter>();

            if (mfs == null || mfs.Length == 0)
            {
                Debug.Log("Mesh Combine Studio -> No meshes are found for saving");
                return;
            }

            string path = filePath.Replace(Application.dataPath, "Assets");
            
            for (int i = 0; i < mfs.Length; i++)
            {
                Mesh mesh = mfs[i].sharedMesh;

                GarbageCollectMesh garbageCollectMesh = mfs[i].GetComponent<GarbageCollectMesh>();
                if (garbageCollectMesh != null)
                {
                    garbageCollectMesh.mesh = null;
                    DestroyImmediate(garbageCollectMesh);
                }

                AssetDatabase.CreateAsset(mesh, path + "/" + mesh.name + i.ToString() + ".asset");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void ShowStaticBatchingMessage()
        {
            bool staticBatching;
            bool dynamicBatching;

            GetBatching(out staticBatching, out dynamicBatching);

            GUI.color = Color.red * editorSkinMulti;
            EditorGUILayout.BeginVertical("Box");
            GUI.color = Color.white;

            GUIDraw.LabelWidthUnderline(new GUIContent("Unity's settings", "Unity's batching settings from Player Settings.\nUnity's V sync Count settings from Quality Settings."), 14);

            GUI.changed = false;
            if (staticBatching)
            {
                if (combineInRuntime.boolValue)
                {
                    GUI.backgroundColor = new Color(Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup * 5)), 0, 0, 1);
                    EditorGUILayout.HelpBox("Make sure to disable Unity's static batching, as it will override MCS combining in runtime.", MessageType.Warning, true);
                    Repaint();
                }
                // else EditorGUILayout.HelpBox("Unity's static batching can be used for meshes that are not combined with MCS", MessageType.Warning, true);
            }

            staticBatching = EditorGUILayout.Toggle(new GUIContent("Static Batching"), staticBatching);
            GUI.backgroundColor = Color.white;
            dynamicBatching = EditorGUILayout.Toggle(new GUIContent("Dynamic Batching"), dynamicBatching);
            if (GUI.changed) SetBatchingActive(staticBatching, dynamicBatching);

            vSyncCountMode = (VSyncCountMode)QualitySettings.vSyncCount;

            if (vSyncCountMode != VSyncCountMode.DontSync)
            {
                EditorGUILayout.HelpBox("Put 'V Sync Count' to 'Don't Sync' to see true fps to measure performance difference. Otherwise FPS will be capped to 60 fps.", MessageType.Warning, true);
            }
             
            GUI.changed = false;
            vSyncCountMode = (VSyncCountMode)EditorGUILayout.EnumPopup(new GUIContent("V Sync Count"), vSyncCountMode);
            if (GUI.changed) QualitySettings.vSyncCount = (int)vSyncCountMode;

            EditorGUILayout.EndVertical();
        }
        
        void GetBatching(out bool staticBatching, out bool dynamicBatching)
        {
            MethodInfo getBatchingForPlatForm = typeof(PlayerSettings).GetMethod("GetBatchingForPlatform", BindingFlags.Static | BindingFlags.NonPublic);

            object tempStatic = null, tempDynamic = null;
            object[] args = new object[] { EditorUserBuildSettings.activeBuildTarget, tempStatic, tempDynamic };
            getBatchingForPlatForm.Invoke(typeof(PlayerSettings), args);
            staticBatching = ((int)args[1]) == 1 ? true : false;
            dynamicBatching = ((int)args[2]) == 1 ? true : false;
        }

        void SetBatchingActive(bool staticActive, bool dynamicActive)
        {
            MethodInfo setBatchingForPlatForm = typeof(PlayerSettings).GetMethod("SetBatchingForPlatform", BindingFlags.Static | BindingFlags.NonPublic);
            object[] args2 = new object[] { EditorUserBuildSettings.activeBuildTarget, staticActive ? 1 : 0, dynamicActive ? 1 : 0 };
            setBatchingForPlatForm.Invoke(typeof(PlayerSettings), args2);
        }

        bool GetBakedGI()
        {
            #if UNITY_5_1 || UNITY_5_2 || UNITY_5_3
            PropertyInfo bakedLightmapsEnabled = typeof(Lightmapping).GetProperty("bakedLightmapsEnabled", BindingFlags.Static | BindingFlags.NonPublic);
            return (bool)bakedLightmapsEnabled.GetValue(null, new Object[] { });
            #else
            return Lightmapping.bakedGI;
            #endif
        }

        bool GetRealtimeGI()
        {
            #if UNITY_5_1 || UNITY_5_2 || UNITY_5_3
            PropertyInfo realtimeLightmapsEnabled = typeof(Lightmapping).GetProperty("realtimeLightmapsEnabled", BindingFlags.Static | BindingFlags.NonPublic);
            return (bool)realtimeLightmapsEnabled.GetValue(null, new Object[] { });
            #else
            return Lightmapping.realtimeGI;
            #endif
        }

        void ApplyScaleLimit()
        {
            Vector3 size = searchBoxSize.vector3Value;
            Vector3 newSize = size;

            if (newSize.x < 0.01f) newSize.x = 0.01f;
            if (newSize.y < 0.01f) newSize.y = 0.01f;
            if (newSize.z < 0.01f) newSize.z = 0.01f;

            if (newSize != size) searchBoxSize.vector3Value = newSize;
        }

        void ApplyTransformLock()
        {
            meshCombiner = (MeshCombiner)target;
            Transform t = meshCombiner.transform;
            if (t.childCount == 0) meshCombiner.combined = false;

            t.position = Vector3.zero;
            t.localScale = Vector3.one;
        }

        void DrawInspectorGUI()
        {
            GUIDraw.DrawSpacer(5, 5, 0);

            GUI.backgroundColor = new Color(0.35f, 1, 0.35f);
            if (GUILayout.Button("Documentation"))
            {
                Application.OpenURL("http://www.terraincomposer.com/mcs-documentation/");
            }
            GUI.backgroundColor = Color.white;

            GUIDraw.DrawSpacer(1, 5, 2);
                ShowStaticBatchingMessage();
            GUIDraw.DrawSpacer(1, 5, 2);
                DrawSearchOptions(Color.red * editorSkinMulti);
            GUIDraw.DrawSpacer(4, 5, 3);
                DrawOutputSettings(Color.blue * editorSkinMulti);
            GUIDraw.DrawSpacer(1, 3, 1);

            meshCombiner.InitMeshCombineJobManager();
            if (MeshCombineJobManager.instance != null)
            {
                
                Color color;
                if (meshCombiner.jobCount > 0)
                {
                    color = Color.Lerp(Color.blue, Color.green, Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup * 3)));
                    Repaint();
                }
                else color = Color.blue;

                DrawJobSettings(color * editorSkinMulti);
            }
             
            // Debug.Log("Repaint");
            
            GUIDraw.DrawSpacer(3, 5, 3);

            DrawRuntime(Color.green * editorSkinMulti);
            GUIDraw.DrawSpacer();
        }
        
        void DrawSearchOptions(Color color)
        {
            GUI.color = color;
            EditorGUILayout.BeginVertical("Box");
            GUI.color = Color.white;

            GUIDraw.LabelWidthUnderline(new GUIContent("Search Options", "With search options you can filter the GameObjects (with meshes) that will be combined."), 14);

            EditorGUILayout.PropertyField(parent, new GUIContent("Parent", "The GameObject parent that holds all meshes (as children) that need to be combined."));

            EditorGUILayout.PropertyField(objectCenter, new GUIContent("Object Center", "Which position should be used to determine the cell location."));
            EditorGUILayout.PropertyField(drawGizmos);
            if (drawGizmos.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(drawMeshBounds);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(onlyActive, new GUIContent("Only Active", "Only combine active GameObjects."));
            EditorGUILayout.PropertyField(onlyStatic, new GUIContent("Only Static", "Only combine GameObjects that are marked as 'Static'."));
            // GUIDraw.DrawSpacer(0, 3, 0);
            EditorGUILayout.PropertyField(useSearchBox, new GUIContent("Use Search Box", "Only combine meshes that are within the bounds of the search box."));

            if (useSearchBox.boolValue)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(searchBoxPivot);
                    if (GUILayout.Button(new GUIContent("R", "Reset the Pivot position."), EditorStyles.miniButtonMid, GUILayout.Width(25))) searchBoxPivot.vector3Value = Vector3.zero;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(searchBoxSize);
                    if (GUILayout.Button(new GUIContent("R", "Reset the Size."), EditorStyles.miniButtonMid, GUILayout.Width(25))) searchBoxSize.vector3Value = Vector3.one;
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.PropertyField(searchBoxSquare, new GUIContent("Search Box Square", "Make the search box bounds square."));

                EditorGUI.indentLevel--;
            }

            if (useCells.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(new GUIContent("Use Max Bounds Factor", "Only combine meshes which bounds are not bigger than x times the cell size."));
                EditorGUILayout.PropertyField(useMaxBoundsFactor, new GUIContent(""), GUILayout.Width(25));
                if (useMaxBoundsFactor.boolValue)
                {
                    GUI.changed = false;
                    EditorGUILayout.PropertyField(maxBoundsFactor, new GUIContent(""));
                    if (GUI.changed)
                    {
                        if (maxBoundsFactor.floatValue < 1) maxBoundsFactor.floatValue = 1;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(new GUIContent("Use Vertex Input Limit", "Only combine meshes that don't exceed this vertex limit."));
                EditorGUILayout.PropertyField(useVertexInputLimit, new GUIContent(""), GUILayout.Width(25));
                if (useVertexInputLimit.boolValue)
                {
                    EditorGUILayout.PropertyField(vertexInputLimit, new GUIContent(""));
                }

                if (vertexInputLimit.intValue < 1) vertexInputLimit.intValue = 1;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(new GUIContent("Use Vertex Input Limit LOD", "Only combine lod meshes which LOD0 mesh doesn't exceed this vertex limit."));
                EditorGUILayout.PropertyField(useVertexInputLimitLod, new GUIContent(""), GUILayout.Width(25));
                if (useVertexInputLimitLod.boolValue)
                {
                    EditorGUILayout.PropertyField(vertexInputLimitLod, new GUIContent(""));
                }

                if (vertexInputLimitLod.intValue < 1) vertexInputLimitLod.intValue = 1;
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(new GUIContent("Use LayerMask", "Only combine GameObjects which Layer is in this LayerMask."));
                EditorGUILayout.PropertyField(useLayerMask, new GUIContent(""), GUILayout.Width(25));
                if (useLayerMask.boolValue)
                {
                    EditorGUILayout.PropertyField(layerMask, new GUIContent(""));
                }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(new GUIContent("Use Tag", "Only combine GameObjects which this tag."));
                EditorGUILayout.PropertyField(useTag, new GUIContent(""), GUILayout.Width(25)); 
                if (useTag.boolValue)
                {
                    tag.stringValue = EditorGUILayout.TagField("", tag.stringValue);
                }
            EditorGUILayout.EndHorizontal();
           
            EditorGUILayout.PropertyField(useComponentsFilter, new GUIContent("Use Components Filter", "Only combine GameObjects with a certain component."));
            if (useComponentsFilter.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(componentCondition, new GUIContent("Condition", "And: Only include GameObjects that have all components.\nOr: Include GameObjects that have one of the components."));
                GUIDraw.PropertyArray(componentNameList, new GUIContent("Component Names"));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(useNameContains, new GUIContent("Use Name Contains", "Only combine GameObjects that with a certain name."));
            if (useNameContains.boolValue)
            {
                EditorGUI.indentLevel++;
                GUIDraw.PropertyArray(nameContainList, new GUIContent("Names"));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }

        void DrawOutputSettings(Color color)
        {

            GUI.color = color;
            EditorGUILayout.BeginVertical("Box");
            GUI.color = Color.white;

            GUIDraw.LabelWidthUnderline(new GUIContent("Output Settings", "The settings for the combined meshes."), 14);

            EditorGUILayout.PropertyField(useCells, new GUIContent("Use Cells", "Combine cell based, only disable cells for combining parts for dynamic objects."));

            if (useCells.boolValue)
            {
                EditorGUI.indentLevel++;
                GUI.changed = false;
                int oldCellSize = cellSize.intValue;
                EditorGUILayout.PropertyField(cellSize, new GUIContent("Cell Size", "Meshes within a cell will be combined together."));
                if (GUI.changed)
                {
                    if (cellSize.intValue < 4) cellSize.intValue = 4;
                    if (oldCellSize != cellSize.intValue)
                    {
                        float ratio = (float)cellSize.intValue / oldCellSize;
                        cellOffset.vector3Value *= ratio;
                        
                        if (meshCombiner.octreeContainsObjects) meshCombiner.ResetOctree();
                    }
                }
                GUI.changed = false;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(cellOffset, new GUIContent("Cell Offset", "Offset position of the cells."));
                int halfCellSize = cellSize.intValue / 2;
                if (GUI.changed)
                {
                    Vector3 cellOffsetValue = cellOffset.vector3Value;
                    if (cellOffsetValue.x > halfCellSize) cellOffsetValue.x = halfCellSize;
                    else if (cellOffsetValue.x < 0) cellOffsetValue.x = 0;
                    if (cellOffsetValue.y > halfCellSize) cellOffsetValue.y = halfCellSize;
                    else if (cellOffsetValue.y < 0) cellOffsetValue.y = 0;
                    if (cellOffsetValue.z > halfCellSize) cellOffsetValue.z = halfCellSize;
                    else if (cellOffsetValue.z < 0) cellOffsetValue.z = 0;
                    cellOffset.vector3Value = cellOffsetValue;
                }
                if (GUILayout.Button(new GUIContent("H", "Offset by half the cell size"), EditorStyles.miniButtonMid, GUILayout.Width(25))) cellOffset.vector3Value = new Vector3(halfCellSize, halfCellSize, halfCellSize);
                if (GUILayout.Button(new GUIContent("R", "Reset the cell offset"), EditorStyles.miniButtonMid, GUILayout.Width(25))) cellOffset.vector3Value = Vector3.zero;
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.PropertyField(removeTrianglesBelowSurface, new GUIContent("Remove Tris Below Surface", "Remove triangles below any surface (terrain and or meshes)."));

            if (removeTrianglesBelowSurface.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(surfaceLayerMask);
                EditorGUILayout.PropertyField(maxSurfaceHeight, new GUIContent("Raycast Height", "This needs to be at least the maximum height of your surface."));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(removeBackFaceTriangles, new GUIContent("Remove Backface Tris", "This can be used if the camera position is limited within an area."));

            if (removeBackFaceTriangles.boolValue)
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.PropertyField(backFaceTriangleMode, new GUIContent("Backface Mode"));
                EditorGUILayout.BeginHorizontal();
                    
                    if (backFaceTriangleMode.enumValueIndex == (int)MeshCombiner.BackFaceTriangleMode.Direction)
                    {
                        EditorGUILayout.PrefixLabel("Direction");
                        EditorGUI.indentLevel--;

                        EditorGUILayout.PropertyField(backFaceDirection, new GUIContent(""));
                        if (GUILayout.Button("R", EditorStyles.miniButtonMid, GUILayout.Width(25))) backFaceDirection.vector3Value = Vector3.zero;
                        EditorGUILayout.EndHorizontal();
                    }
                    else
                    {
                        EditorGUILayout.PrefixLabel("Center");
                        EditorGUI.indentLevel--;

                        Bounds bounds = backFaceBounds.boundsValue;
                        bounds.center = EditorGUILayout.Vector3Field(new GUIContent(""), bounds.center);
                        if (GUILayout.Button("R", EditorStyles.miniButtonMid, GUILayout.Width(25))) bounds.center = Vector3.zero;
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PrefixLabel("Size");
                        EditorGUI.indentLevel--;
                        bounds.size = EditorGUILayout.Vector3Field(new GUIContent(""), bounds.size);
                        bounds.size = Methods.SetMin(bounds.size, 0.001f);
                        if (GUILayout.Button("R", EditorStyles.miniButtonMid, GUILayout.Width(25))) bounds.size = Vector3.one;
                        backFaceBounds.boundsValue = bounds;
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel++;
                

                EditorGUILayout.PropertyField(twoSidedShadows);

                EditorGUI.indentLevel--;
            }
            
            if (addMeshColliders.boolValue)
            {
                EditorGUILayout.HelpBox("Only use this option if you do not have primitive colliders on the original GameObjects, otherwise it will slow down physics.", MessageType.Warning, true);
            }
            EditorGUILayout.PropertyField(addMeshColliders, new GUIContent("Add Mesh Colliders", "Add mesh colliders to the combined meshes. Only use this option if you do not have primative colliders on the original GameObjects."));

            if (!makeMeshesUnreadable.boolValue)
            {
                EditorGUILayout.HelpBox("Only use this option if you want to read from the meshes at runtime. Otherwise making meshes unreadable removes the mesh copy from CPU memory, which saves memory.", MessageType.Warning, true);
            }
            EditorGUILayout.PropertyField(makeMeshesUnreadable);

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(new GUIContent("Use Vertex Output Limit", "Combined meshes won't exceed this vertex count."));
                EditorGUILayout.PropertyField(useVertexOutputLimit, new GUIContent(""), GUILayout.Width(25));
                if (useVertexOutputLimit.boolValue)
                {
                    EditorGUILayout.PropertyField(vertexOutputLimit, new GUIContent(""));
                }
            
                if (vertexOutputLimit.intValue < 1) vertexOutputLimit.intValue = 1;

            EditorGUILayout.EndHorizontal();

            if (GetBakedGI() && !GetRealtimeGI())
            {
                if (copyBakedLighting.boolValue)
                {
                    EditorGUILayout.HelpBox("Copy baked lighting will results in more combined meshes (more draw calls than with rebaking) as the source objects need to have the same lightmap index. The advantage is that the Scene file size doesn't increase when used with 'Combine In Runtime'.", MessageType.Info, true);
                }
                EditorGUILayout.PropertyField(copyBakedLighting, new GUIContent("Copy Baked Lighting", "The Lighting of the original meshes will be copied to the combined meshes."));
                if (copyBakedLighting.boolValue) rebakeLighting.boolValue = false;
                validCopyBakedLighting.boolValue = copyBakedLighting.boolValue;
            }
            if (!combineInRuntime.boolValue && !Application.isPlaying)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(new GUIContent("Rebake Lighting", "Rebake the lighting on the combines meshes."));
                EditorGUILayout.PropertyField(rebakeLighting, new GUIContent(""), GUILayout.Width(25));
                if (rebakeLighting.boolValue)
                {
                    copyBakedLighting.boolValue = false;
                    EditorGUILayout.PropertyField(rebakeLightingMode, new GUIContent(""));
                }
                EditorGUILayout.EndHorizontal();
                if (rebakeLighting.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(scaleInLightmap, new GUIContent("Scale In Lightmap", "The scale of the combined meshes in the Lightmap, default value is 1 and a smaller value will create Lightmaps with less file size."));
                    if (scaleInLightmap.floatValue < 0.001f) scaleInLightmap.floatValue = 0.001f;
                    EditorGUI.indentLevel--;
                }
            }
            
            outputLayer.intValue = EditorGUILayout.LayerField(new GUIContent("Output Layer", "The combined GameObjects will be on this layer."), outputLayer.intValue);
            
            //EditorGUILayout.PropertyField(removeGeometryBelowTerrain);
            //if (removeGeometryBelowTerrain.boolValue)
            //{
            //    DrawPropertyArray(terrains);
            //}

            EditorGUILayout.EndVertical();
        }

        void DrawRuntime(Color color)
        {
            GUI.color = color;
            EditorGUILayout.BeginVertical("Box");
            GUI.color = Color.white;
            GUIDraw.LabelWidthUnderline(new GUIContent("Runtime", ""), 14);

            GUI.changed = false;
            EditorGUILayout.PropertyField(combineInRuntime, new GUIContent("Combine In Runtime", "Combine meshes at runtime."));
            
            if (combineInRuntime.boolValue)
            {
                if (GUI.changed)
                {
                    meshCombiner.RestoreOriginalRenderersAndLODGroups();
                    activeOriginal.boolValue = true;
                }

                EditorGUILayout.PropertyField(combineOnStart, new GUIContent("Combine On Start", "Combine meshes on start up."));

                EditorGUILayout.PropertyField(originalMeshRenderers, new GUIContent("Original Mesh Renderers", "What to do with the origal MeshRenderer components."));
                EditorGUILayout.PropertyField(originalLODGroups, new GUIContent("Original LODGroups", "What to do with the original LODGroup components."));

                if ((MeshCombiner.HandleComponent)originalMeshRenderers.enumValueIndex == MeshCombiner.HandleComponent.Disable && (MeshCombiner.HandleComponent)originalLODGroups.enumValueIndex == MeshCombiner.HandleComponent.Disable)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(new GUIContent("On/Off MCS With 'Tab'", "Toggle beween MCS combined meshes rendering and original GameObjects rendering."));
                    EditorGUILayout.PropertyField(useCombineSwapKey, new GUIContent(""), GUILayout.Width(25));
                    //if (useCombineSwapKey.boolValue)
                    //{
                    //    EditorGUILayout.PropertyField(combineSwapKey, new GUIContent(""));
                    //}
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();
        }

        void DrawJobSettings(Color color)
        {
            MeshCombineJobManager meshCombineJobManager = MeshCombineJobManager.instance;

            GUI.color = color;
            EditorGUILayout.BeginVertical("Box");
            GUI.color = Color.white;

            GUIDraw.LabelWidthUnderline(new GUIContent("Job Settings", ""), 14);

            GUI.changed = false;

            EditorGUILayout.PropertyField(combineJobMode, new GUIContent("Combine Job Mode", "Should meshes be combined all at once or per frame."));
            if ((MeshCombineJobManager.CombineJobMode)combineJobMode.enumValueIndex == MeshCombineJobManager.CombineJobMode.CombinePerFrame)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(combineMeshesPerFrame, new GUIContent("Meshes Per Frame"));
                if (combineMeshesPerFrame.intValue < 1) combineMeshesPerFrame.intValue = 1;
                else if (combineMeshesPerFrame.intValue > 128) combineMeshesPerFrame.intValue = 128;
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Use Multi Threading", "Combine meshes using multi threading which results in higher fps and faster combining."));
            EditorGUILayout.PropertyField(useMultiThreading, new GUIContent(""), GUILayout.Width(25));
            if (useMultiThreading.boolValue)
            {
                EditorGUILayout.PropertyField(threadAmountMode, new GUIContent(""));
            }
            EditorGUILayout.EndHorizontal();

            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL && meshCombiner.combineInRuntime)
            {
                EditorGUILayout.HelpBox("WebGL doesn't support the use of multi threading.", MessageType.Info, true);
                useMultiThreading.boolValue = false;
            }

            if (useMultiThreading.boolValue)
            {
                if ((MeshCombineJobManager.ThreadAmountMode)threadAmountMode.enumValueIndex == MeshCombineJobManager.ThreadAmountMode.Custom)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(customThreadAmount);
                    if (customThreadAmount.intValue < 1) customThreadAmount.intValue = 1;
                    else if (customThreadAmount.intValue > meshCombineJobManager.cores) customThreadAmount.intValue = meshCombineJobManager.cores;
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.PropertyField(useMainThread);
            }

            if (meshCombiner != null && meshCombiner.jobCount > 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Jobs Pending");
                EditorGUILayout.LabelField(meshCombiner.jobCount.ToString());
                EditorGUILayout.EndHorizontal();
            }

            bool guiChanged = GUI.changed;

            EditorGUILayout.PropertyField(showStats);

            if (showStats.boolValue)
            {
                GUIDraw.DrawSpacer();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Meshes Cached");
                EditorGUILayout.LabelField(meshCombineJobManager.meshCacheDictionary.Count.ToString());
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Meshes Arrays Cached");
                EditorGUILayout.LabelField(meshCombineJobManager.newMeshObjectsPool.Count.ToString());
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Total New Mesh Objects");
                EditorGUILayout.LabelField(meshCombineJobManager.totalNewMeshObjects.ToString());
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Mesh Combine Jobs");
                EditorGUILayout.LabelField(meshCombineJobManager.meshCombineJobs.Count.ToString());
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Mesh Combine Jobs Thread");
                for (int i = 0; i < meshCombineJobManager.cores; i++)
                {
                    EditorGUILayout.LabelField(meshCombineJobManager.meshCombineJobsThreads[i].meshCombineJobs.Count.ToString() + " ", GUILayout.Width(35));
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("New Mesh Objects Jobs?");
                // EditorGUILayout.LabelField(meshCombineJobManager.newMeshObjectsJobs.Count.ToString());
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("New Mesh Objects Done Thread");
                EditorGUILayout.LabelField(meshCombineJobManager.newMeshObjectsDoneThread.Count.ToString());
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("New Mesh Objects Done");
                EditorGUILayout.LabelField(meshCombineJobManager.newMeshObjectsDone.Count.ToString());
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            
            if (guiChanged)
            {
                serializedObject.ApplyModifiedProperties();
                meshCombiner.CopyJobSettingsToAllInstances();
                meshCombineJobManager.SetJobMode(meshCombiner.jobSettings);
            }
        }

        void DisplayOctreeInfo()
        {
            if (!meshCombiner.octreeContainsObjects) return;

            EditorGUILayout.BeginVertical("Box");
            GUIDraw.LabelWidthUnderline(new GUIContent("Found Objects", ""), 14);
            EditorGUILayout.LabelField("Cells " + ObjectOctree.MaxCell.maxCellCount);
            GUIDraw.DrawUnderLine(2);

            MeshCombiner.LodParentHolder[] lodParentsCount = meshCombiner.lodParentHolders;

            if (lodParentsCount == null || lodParentsCount.Length == 0) return;

            for (int i = 0; i < lodParentsCount.Length; i++)
            {
                MeshCombiner.LodParentHolder lodParentCount = lodParentsCount[i];
                if (!lodParentCount.found) continue;

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("LOD Group " + (i + 1) + " |", GUILayout.Width(95));

                int[] lods = lodParentCount.lods;

                for (int j = 0; j < lods.Length; j++)
                {
                    if (lods[j] == lods[0]) GUI.color = Color.green; else GUI.color = Color.red;
                    // EditorGUILayout.LabelField("LOD" + j + " -> " + lods[j] + " Objects");
                    EditorGUILayout.LabelField(lods[j].ToString(), GUILayout.Width(38));
                    GUI.color = Color.white;
                    EditorGUILayout.LabelField("|", GUILayout.Width(7));
                }
                EditorGUILayout.EndHorizontal();
                GUIDraw.DrawUnderLine();
            }

            GUILayout.Space(10);
            EditorGUILayout.EndVertical();
        }

        void DisplayVertsAndTrisInfo()
        {
            if (!meshCombiner.combined) return;

            EditorGUILayout.BeginVertical("Box");
            GUIDraw.LabelWidthUnderline(new GUIContent("Vertex And Triangle Count"), 14);

            GUI.color = Color.blue * editorSkinMulti;
            EditorGUILayout.BeginVertical("Box");
            GUI.color = Color.white;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Original", GUILayout.Width(65));
            EditorGUILayout.LabelField("Batches " + meshCombiner.originalDrawCalls, GUILayout.Width(90));
            EditorGUILayout.LabelField("Verts " + meshCombiner.originalTotalVertices, GUILayout.Width(90));
            EditorGUILayout.LabelField("Tris " + meshCombiner.originalTotalTriangles);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            GUI.color = Color.green * editorSkinMulti;
            EditorGUILayout.BeginVertical("Box");
            GUI.color = Color.white;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Combined", GUILayout.Width(65));
            EditorGUILayout.LabelField("Batches " + meshCombiner.newDrawCalls, GUILayout.Width(90));
            EditorGUILayout.LabelField("Verts " + meshCombiner.totalVertices, GUILayout.Width(90));
            EditorGUILayout.LabelField("Tris " + meshCombiner.totalTriangles);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            GUI.color = Color.yellow * editorSkinMulti;
            EditorGUILayout.BeginVertical("Box");
            GUI.color = Color.white;

            if (meshCombiner.originalTotalVertices != 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Saved", GUILayout.Width(65));
                EditorGUILayout.LabelField("Batches " + (((meshCombiner.originalDrawCalls - meshCombiner.newDrawCalls) / (float)meshCombiner.originalDrawCalls) * 100).ToString("F1") + "%", GUILayout.Width(90));
                EditorGUILayout.LabelField("Verts " + (((meshCombiner.originalTotalVertices - meshCombiner.totalVertices) / (float)meshCombiner.originalTotalVertices) * 100).ToString("F2") + "%", GUILayout.Width(90));
                EditorGUILayout.LabelField("Tris " + (((meshCombiner.originalTotalTriangles - meshCombiner.totalTriangles) / (float)meshCombiner.originalTotalTriangles) * 100).ToString("F2") + "%");
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }
    }
}