using UnityEngine;
using System.Collections;
using UnityEditor;



[CustomEditor(typeof(CSFillSprite))]
public class CSFillSpriteEditor : Editor
{
    CSFillSprite sprite; 
    void OnEnable()
    {
        sprite = target as CSFillSprite;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        sprite.Picture = EditorGUILayout.ObjectField("Pic", sprite.Picture, typeof(Texture2D)) as Texture2D;
        sprite.fillAmount = EditorGUILayout.Slider("FillCount", sprite.fillAmount, 0, 1);
        sprite.Color = EditorGUILayout.ColorField("Color", sprite.Color);
    }
}
