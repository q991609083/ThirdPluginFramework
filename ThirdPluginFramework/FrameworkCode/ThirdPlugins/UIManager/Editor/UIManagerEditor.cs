using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.ProjectWindowCallback;
using System.Text.RegularExpressions;
using System.Text;

public class UIManagerEditor : Editor
{
    /// <summary>
    /// 一键创建UI系统
    /// </summary>
    [MenuItem("GameObject/UISystem", false, 4)]
    private static void CreateUISystem()
    {
        GameObject uiSystem = new GameObject();
        uiSystem.name = "UISystem";
        uiSystem.layer = LayerMask.NameToLayer("UI");
        CreateEventSystem(uiSystem.transform);
        CreateUINode(uiSystem.transform, CreateUICamera(uiSystem.transform));
    }
    /// <summary>
    /// 创建EventSystem
    /// </summary>
    /// <param name="uiSystem"></param>
    private static void CreateEventSystem(Transform uiSystem)
    {
        GameObject eventSystem = new GameObject();
        eventSystem.name = "EventSystem";
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        eventSystem.layer = LayerMask.NameToLayer("UI");
        eventSystem.transform.SetParent(uiSystem, false);
    }
    /// <summary>
    /// 创建UI相机
    /// </summary>
    /// <param name="uiSystem"></param>
    /// <returns></returns>
    private static Camera CreateUICamera(Transform uiSystem)
    {
        GameObject uiCamera = new GameObject();
        uiCamera.name = "UICamera";
        uiCamera.layer = LayerMask.NameToLayer("UI");
        Camera camera = uiCamera.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.Depth;
        camera.cullingMask = LayerMask.GetMask(new string[] { "UI" });
        camera.farClipPlane = 11;
        camera.depth = 99;
        uiCamera.transform.SetParent(uiSystem, false);
        return camera;
    }
    /// <summary>
    /// 创建UICanvas根节点
    /// </summary>
    /// <param name="uiSystem"></param>
    /// <param name="uiCamera"></param>
    private static void CreateUINode(Transform uiSystem, Camera uiCamera)
    {
        GameObject uiNode = new GameObject();
        uiNode.name = "UINode";
        uiNode.layer = LayerMask.NameToLayer("UI");
        uiNode.AddComponent<RectTransform>();
        Canvas canvas = uiNode.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = uiCamera;
        canvas.planeDistance = 10;

        CanvasScaler scaler = uiNode.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(750, 1334);
        scaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.Shrink;

        GraphicRaycaster raycaster = uiNode.AddComponent<GraphicRaycaster>();
        uiNode.AddComponent<UIManager>();
        uiNode.transform.SetParent(uiSystem.transform, false);
    }
    /// <summary>
    /// UI代码的模板路径
    /// </summary>
    private static string templateUIPath = "Assets/ThirdPlugins/UIManager/Editor/UIScriptTemplate.cs.txt";
    /// <summary>
    /// 根据模板创建UI脚本
    /// </summary>
    [MenuItem("Assets/Create/JQ Plugins/C# UIScript", false, 1)]
    private static void CreateUIScript()
    {
        string path = GetSelectionPath();
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateScriptAsset>(), path + "/NewUIScript.cs", null,templateUIPath);
    }
    /// <summary>
    /// 获取文件选择路径
    /// </summary>
    /// <returns></returns>
    private static string GetSelectionPath()
    {
        string path = "Assets";
        foreach (UnityEngine.Object item in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(item);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }

    class CreateScriptAsset : EndNameEditAction
    {
        public override void Action(int instanceId, string newScriptPath, string templatePath)
        {
            UnityEngine.Object obj = CreateTemplateScriptAsset(newScriptPath, templatePath);
            ProjectWindowUtil.ShowCreatedAsset(obj);
        }

        public static UnityEngine.Object CreateTemplateScriptAsset(string newScriptPath, string templatePath)
        {
            string fullPath = Path.GetFullPath(newScriptPath);
            StreamReader streamReader = new StreamReader(templatePath);
            string text = streamReader.ReadToEnd();
            streamReader.Close();
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(newScriptPath);
            //替换模板的文件名
            text = Regex.Replace(text, "UIScriptTemplate", fileNameWithoutExtension);
            bool encoderShouldEmitUTF8Identifier = true;
            bool throwOnInvalidBytes = false;
            UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
            bool append = false;
            StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
            streamWriter.Write(text);
            streamWriter.Close();
            AssetDatabase.ImportAsset(newScriptPath);
            return AssetDatabase.LoadAssetAtPath(newScriptPath, typeof(UnityEngine.Object));
        }

    }
}
