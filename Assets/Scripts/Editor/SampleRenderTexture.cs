using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SampleRenderTexture : EditorWindow {
    RenderTexture renderTexture;
    Texture2D texture;
    string path = "SampledTexture.png";
    
    [MenuItem("Tools/Sample Render Texture")]
    public static void ShowWindow() {
        GetWindow<SampleRenderTexture>("Sample Render Texture");
    }

    void OnGUI() {
        renderTexture = (RenderTexture) EditorGUILayout.ObjectField("Render Texture", renderTexture,  typeof(RenderTexture), true);
        path = EditorGUILayout.TextField("Path", path);
        if (GUILayout.Button("Render to Texture2D")){
            ConvertToTexture2D();
        }
    }

    void ConvertToTexture2D() {
        if (renderTexture) {
            RenderTexture.active = renderTexture;
            texture = new Texture2D(renderTexture.width, renderTexture.height);
            texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            texture.Apply();
            
            Byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/" + path, bytes);
            
            DestroyImmediate(texture);
            RenderTexture.active = null;
            Debug.Log("Texture saved");
        }
    }
}
