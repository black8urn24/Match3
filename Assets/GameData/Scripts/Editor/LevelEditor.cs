using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using UnityEditorInternal;
using FullSerializer;
using Match3.Enums;
using Match3.Core;

namespace Match3.Editor
{
    public class LevelEditor : EditorWindow
    {
        #region Variables
        private int prevWidth = -1;
        private int prevHeight = -1;
        private PieceColor currentPieceColor;
        private Vector2 scrollPos;
        private Level currentLevel;
        private readonly Dictionary<string, Texture> tileTextures = new Dictionary<string, Texture>();
        private Texture buttonTexture;
        private GUIContent buttonTextureContent;
        #endregion

        #region Default Methods
        [MenuItem("Tools/Match 3/Editor", false, 0)]
        private static void Init()
        {
            var window = GetWindow(typeof(LevelEditor));
            window.titleContent = new GUIContent("Match 3 Editor");
        }

        private void OnEnable()
        {
            // get images into dictionary from resources folder
            var editorImagesPath = new DirectoryInfo(Application.dataPath + "/GameData/Editor/Resources");
            var fileInfo = editorImagesPath.GetFiles("*.png", SearchOption.TopDirectoryOnly);
            foreach (var file in fileInfo)
            {
                var filename = Path.GetFileNameWithoutExtension(file.Name);
                tileTextures[filename] = Resources.Load(filename) as Texture;
            }
        }

        private void OnGUI()
        {
            Draw();
        }
        #endregion

        #region Protected Methods
        protected void SaveJsonFile<T>(string path, T data) where T : class
        {
            fsData serializedData;
            var serializer = new fsSerializer();
            serializer.TrySerialize(data, out serializedData).AssertSuccessWithoutWarnings();
            var file = new StreamWriter(path);
            var json = fsJsonPrinter.PrettyJson(serializedData);
            file.WriteLine(json);
            file.Close();
        }

        protected T LoadJsonFile<T>(string path) where T : class
        {
            if (!File.Exists(path))
            {
                return null;
            }

            var file = new StreamReader(path);
            var fileContents = file.ReadToEnd();
            var data = fsJsonParser.Parse(fileContents);
            object deserialized = null;
            var serializer = new fsSerializer();
            serializer.TryDeserialize(data, typeof(T), ref deserialized).AssertSuccessWithoutWarnings();
            file.Close();
            return deserialized as T;
        }
        #endregion

        #region Public Methods
        public static ReorderableList SetupReorderableList<T>(
           string headerText,
           List<T> elements,
           ref T currentElement,
           Action<Rect, T> drawElement,
           Action<T> selectElement,
           Action createElement,
           Action<T> removeElement)
        {
            var list = new ReorderableList(elements, typeof(T), true, true, true, true)
            {
                drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, headerText); },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = elements[index];
                    drawElement(rect, element);
                }
            };
            list.onSelectCallback = l =>
            {
                var selectedElement = elements[list.index];
                selectElement(selectedElement);
            };
            if (createElement != null)
            {
                list.onAddDropdownCallback = (buttonRect, l) =>
                {
                    createElement();
                };
            }
            list.onRemoveCallback = l =>
            {
                if (!EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete this item?", "Yes", "No")
                )
                {
                    return;
                }
                var element = elements[l.index];
                removeElement(element);
                ReorderableList.defaultBehaviours.DoRemoveButton(l);
            };
            return list;
        }

        public void SaveLevel(string path)
        {
#if UNITY_EDITOR
            var lvlFileName = GlobalConstants.Match3LevelPrefix + currentLevel.id + ".json";
            SaveJsonFile(path + lvlFileName, currentLevel);
            AssetDatabase.Refresh();
#endif
        }
        #endregion

        #region Private Methods
        private void Draw()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            DrawMainMenu();
            if (currentLevel != null)
            {
                DrawData();
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawMainMenu()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("New", GUILayout.Width(100), GUILayout.Height(50)))
            {
                currentLevel = new Level();
            }
            // open Button
            if (GUILayout.Button("Open", GUILayout.Width(100), GUILayout.Height(50)))
            {
                var path = EditorUtility.OpenFilePanel("Open level",
                    Application.dataPath + "/GameData/Resources/Levels",
                    "json");
                if (!string.IsNullOrEmpty(path))
                {
                    currentLevel = LoadJsonFile<Level>(path);
                }
            }
            // save Button
            if (GUILayout.Button("Save", GUILayout.Width(100), GUILayout.Height(50)))
            {
                SaveLevel(Application.dataPath + "/GameData/Resources/Levels/");
            }
            GUILayout.EndHorizontal();
        }

        private void DrawData()
        {
            var level = currentLevel;
            prevWidth = level.boardWidth;
            GUILayout.Space(15);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUILayout.BeginVertical();
            DrawGeneralSettings();
            GUILayout.Space(50);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUILayout.BeginVertical();
            DrawLevelEditor();
            GUILayout.EndVertical();
            GUILayout.Space(50);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DrawGeneralSettings()
        {
            GUILayout.BeginVertical();
            EditorGUILayout.LabelField("General", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal(GUILayout.Width(300));
            EditorGUILayout.HelpBox("The general settings of this level.", MessageType.Info);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Level number", "The number of this level."), GUILayout.Width(EditorGUIUtility.labelWidth));
            currentLevel.id = EditorGUILayout.IntField(currentLevel.id, GUILayout.Width(30));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Moves", "The maximum number of moves of this level."), GUILayout.Width(EditorGUIUtility.labelWidth));
            currentLevel.moves = EditorGUILayout.IntField(currentLevel.moves, GUILayout.Width(30));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Target Score", "Total score earned on completing this level"), GUILayout.Width(EditorGUIUtility.labelWidth));
            currentLevel.targetScore = EditorGUILayout.IntField(currentLevel.targetScore, GUILayout.Width(70));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DrawLevelEditor()
        {
            EditorGUILayout.LabelField("Level", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal(GUILayout.Width(300));
            EditorGUILayout.HelpBox("The layout settings of this level.", MessageType.Info);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Width", "The width of this level."), GUILayout.Width(EditorGUIUtility.labelWidth));
            currentLevel.boardWidth = EditorGUILayout.IntField(currentLevel.boardWidth, GUILayout.Width(30));
            GUILayout.EndHorizontal();
            prevHeight = currentLevel.boardHeight;
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Height", "The height of this level."), GUILayout.Width(EditorGUIUtility.labelWidth));
            currentLevel.boardHeight = EditorGUILayout.IntField(currentLevel.boardHeight, GUILayout.Width(30));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Game Pieces", "The current type of game Pieces."), GUILayout.Width(EditorGUIUtility.labelWidth));
            currentPieceColor = (PieceColor)EditorGUILayout.EnumPopup(currentPieceColor, GUILayout.Width(100));
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            if (prevWidth != currentLevel.boardWidth || prevHeight != currentLevel.boardHeight)
            {
                currentLevel.levelPieces = new List<LevelPiece>(currentLevel.boardWidth * currentLevel.boardHeight);
                for (var i = 0; i < currentLevel.boardWidth; i++)
                {
                    for (var j = 0; j < currentLevel.boardHeight; j++)
                    {
                        currentLevel.levelPieces.Add(new LevelPiece());
                    }
                }
            }
            for (var i = 0; i < currentLevel.boardHeight; i++)
            {
                GUILayout.BeginHorizontal();
                for (var j = 0; j < currentLevel.boardWidth; j++)
                {
                    var pieceIndex = (currentLevel.boardWidth * i) + j;
                    CreateButton(pieceIndex);
                }
                GUILayout.EndHorizontal();
            }
        }

        private void CreateButton(int pieceIndex)
        {
            string tileName = string.Empty;
            if (currentLevel.levelPieces[pieceIndex] != null)
            {
                tileName = currentLevel.levelPieces[pieceIndex].pieceColor.ToString();
                buttonTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/GameData/Editor/Resources/" + tileName + ".png", typeof(Texture));
                buttonTextureContent = new GUIContent(buttonTexture);
            }
            if (tileName != string.Empty)
            {
                if (GUILayout.Button(buttonTextureContent, GUILayout.Width(60), GUILayout.Height(60)))
                {
                    DrawTile(pieceIndex);
                }
            }
            else
            {
                if (GUILayout.Button("", GUILayout.Width(60), GUILayout.Height(60)))
                {
                    DrawTile(pieceIndex);
                }
            }
        }

        private void DrawTile(int pieceIndex)
        {
            var x = pieceIndex % currentLevel.boardWidth;
            var y = pieceIndex / currentLevel.boardWidth;
            LevelPiece levelPiece = new LevelPiece();
            switch (currentPieceColor)
            {
                case PieceColor.Blue:
                    levelPiece.pieceValue = (int)PieceColor.Blue;
                    levelPiece.pieceColor = currentPieceColor;
                    currentLevel.levelPieces[pieceIndex] = levelPiece;
                    levelPiece = new LevelPiece();
                    break;
                case PieceColor.Cyan:
                    levelPiece.pieceValue = (int)PieceColor.Cyan;
                    levelPiece.pieceColor = currentPieceColor;
                    currentLevel.levelPieces[pieceIndex] = levelPiece;
                    levelPiece = new LevelPiece();
                    break;
                case PieceColor.Green:
                    levelPiece.pieceValue = (int)PieceColor.Green;
                    levelPiece.pieceColor = currentPieceColor;
                    currentLevel.levelPieces[pieceIndex] = levelPiece;
                    levelPiece = new LevelPiece();
                    break;
                case PieceColor.Indigo:
                    levelPiece.pieceValue = (int)PieceColor.Indigo;
                    levelPiece.pieceColor = currentPieceColor;
                    currentLevel.levelPieces[pieceIndex] = levelPiece;
                    levelPiece = new LevelPiece();
                    break;
                case PieceColor.Magenta:
                    levelPiece.pieceValue = (int)PieceColor.Magenta;
                    levelPiece.pieceColor = currentPieceColor;
                    currentLevel.levelPieces[pieceIndex] = levelPiece;
                    levelPiece = new LevelPiece();
                    break;
                case PieceColor.Red:
                    levelPiece.pieceValue = (int)PieceColor.Red;
                    levelPiece.pieceColor = currentPieceColor;
                    currentLevel.levelPieces[pieceIndex] = levelPiece;
                    levelPiece = new LevelPiece();
                    break;
                case PieceColor.Teal:
                    levelPiece.pieceValue = (int)PieceColor.Teal;
                    levelPiece.pieceColor = currentPieceColor;
                    currentLevel.levelPieces[pieceIndex] = levelPiece;
                    levelPiece = new LevelPiece();
                    break;
                case PieceColor.Yellow:
                    levelPiece.pieceValue = (int)PieceColor.Yellow;
                    levelPiece.pieceColor = currentPieceColor;
                    currentLevel.levelPieces[pieceIndex] = levelPiece;
                    levelPiece = new LevelPiece();
                    break;
                case PieceColor.None:
                    break;
                default:
                    break;
            }

        }
        #endregion
    }
}