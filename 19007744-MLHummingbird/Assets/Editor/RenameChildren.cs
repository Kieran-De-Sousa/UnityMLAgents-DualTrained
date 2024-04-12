using UnityEngine;
using UnityEditor;

namespace ZipZap
{
    /// <summary>
    /// Mass rename children of selected parent object, with name and index starting number.
    /// </summary>
    /// <reference>
    /// https://discussions.unity.com/t/is-there-anyway-to-batch-renaming-via-editor-not-on-runtime-multiple-game-objects-in-the-hierarchy/238420/2
    /// </reference>
    /// <Author>Kieran</Author>
    public class RenameChildren : EditorWindow
    {
        private static readonly Vector2Int _size = new Vector2Int(250, 100);
        private string _childrenPrefix;
        private int _startIndex;

        [MenuItem("Tools/Gameplay/Rename Children")]
        public static void ShowWindow()
        {
            EditorWindow window = GetWindow<RenameChildren>();
            window.minSize = _size;
            window.maxSize = _size;
        }
        private void OnGUI()
        {
            _childrenPrefix = EditorGUILayout.TextField("Children prefix", _childrenPrefix);
            _startIndex = EditorGUILayout.IntField("Start index", _startIndex);
            if (GUILayout.Button("Rename children"))
            {
                GameObject[] selectedObjects = Selection.gameObjects;
                foreach (var gameObject in selectedObjects)
                {
                    Transform selectedObjectT = gameObject.transform;
                    for (int childI = 0, i = _startIndex; childI < selectedObjectT.childCount; childI++)
                    {
                        selectedObjectT.GetChild(childI).name = $"{_childrenPrefix}{i++}";
                    }
                }
            }
        }
    }
}