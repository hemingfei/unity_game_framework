//
//  EditorUtil.cs 
//
//  Author: HGT 
//
//  Copyright (c) 2021 hegametech.com 
//

using UnityEngine;
using System.Text;
using System.IO;
using System;
using System.Reflection;
using UnityEditor;

namespace CustomGameFramework.Editor
{
    public class EditorUtil
    {
        #region read write string file

        public static void WriteStringByFile(string path, string content)
        {
            byte[] dataByte = Encoding.GetEncoding("UTF-8").GetBytes(content);
            CreateFile(path, dataByte);
        }

        public static string ReadStringByFile(string path)
        {
            StringBuilder line = new StringBuilder();
            try
            {
                if (!File.Exists(path))
                {
                    Debug.Log("path dont exists ! : " + path);
                    return "";
                }

                StreamReader sr = File.OpenText(path);
                line.Append(sr.ReadToEnd());
                sr.Close();
                sr.Dispose();
            }
            catch (Exception e)
            {
                Debug.Log("Load text fail ! message:" + e.Message);
            }

            return line.ToString();
        }

        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else
            {
                Debug.Log("File:[" + path + "] dont exists");
            }
        }


        public static void CreateFile(string path, byte[] byt)
        {
            try
            {
                CreatFilePath(path);
                File.WriteAllBytes(path, byt);
            }
            catch (Exception e)
            {
                Debug.LogError("File Create Fail! \n" + e.Message);
            }
        }

        public static void CreatFilePath(string filepath)
        {
            string newPathDir = Path.GetDirectoryName(filepath);
            CreatPath(newPathDir);
        }

        public static void CreatPath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        #endregion

        #region sorting layer

        public static void AddSortLayerIfNotExist(string name)
        {
            if (!isExistSortingLayer(name))
            {
                int index = GetSortingLayerCount();
                AddSortingLayer();
                SetSortingLayerName(index, name);
                Debug.Log($"创建 {name} 引导层 sorting layer");
            }
        }

        public static bool isExistSortingLayer(string name)
        {
            bool isExist = false;
            string[] layers = get_sortingLayerNames();

            for (int i = 0; i < layers.Length; i++)
            {
                if (name == layers[i])
                {
                    isExist = true;
                }
            }

            return isExist;
        }

        public static string GetSortingLayer(int index)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                 BindingFlags.Static;
            Assembly ab = Assembly.Load("UnityEditor");
            Type type = ab.GetType("UnityEditorInternal.InternalEditorUtility");
            MethodInfo mi = type.GetMethod("GetSortingLayerName", flags);
            object[] objs = new object[1];
            objs[0] = index;
            return (string)mi.Invoke(null, objs);
        }

        public static int GetSortingLayerCount()
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                 BindingFlags.Static;
            Assembly ab = Assembly.Load("UnityEditor");
            Type type = ab.GetType("UnityEditorInternal.InternalEditorUtility");
            MethodInfo mi = type.GetMethod("GetSortingLayerCount", flags);
            return (int)mi.Invoke(null, null);
        }

        public static void SetSortingLayerName(int index, string name)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                 BindingFlags.Static;
            Assembly ab = Assembly.Load("UnityEditor");
            Type type = ab.GetType("UnityEditorInternal.InternalEditorUtility");
            MethodInfo mi = type.GetMethod("SetSortingLayerName", flags);
            object[] objs = new object[2];
            objs[0] = index;
            objs[1] = name;
            mi.Invoke(null, objs);
        }

        public static void SetSortingLayerLocked(int index, bool locked)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                 BindingFlags.Static;
            Assembly ab = Assembly.Load("UnityEditor");
            Type type = ab.GetType("UnityEditorInternal.InternalEditorUtility");
            MethodInfo mi = type.GetMethod("SetSortingLayerLocked", flags);
            object[] objs = new object[2];
            objs[0] = index;
            objs[1] = locked;
            mi.Invoke(null, objs);
        }

        public static bool GetSortingLayerLocked(int index)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                 BindingFlags.Static;
            Assembly ab = Assembly.Load("UnityEditor");
            Type type = ab.GetType("UnityEditorInternal.InternalEditorUtility");
            MethodInfo mi = type.GetMethod("GetSortingLayerLocked", flags);
            object[] objs = new object[1];
            objs[0] = index;
            return (bool)mi.Invoke(null, objs);
        }

        public static void AddSortingLayer()
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                 BindingFlags.Static;
            Assembly ab = Assembly.Load("UnityEditor");
            Type type = ab.GetType("UnityEditorInternal.InternalEditorUtility");
            MethodInfo mi = type.GetMethod("AddSortingLayer", flags);
            mi.Invoke(null, null);
        }

        public static string[] get_sortingLayerNames()
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                 BindingFlags.Static;
            Assembly ab = Assembly.Load("UnityEditor");
            Type type = ab.GetType("UnityEditorInternal.InternalEditorUtility");
            MethodInfo mi = type.GetMethod("get_sortingLayerNames", flags);
            return (string[])mi.Invoke(null, null);
        }

        #endregion

        #region draw

        public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        /// <summary>
        /// Draw a distinctly different looking header label
        /// </summary>
        public static bool DrawHeader(string text, string key, bool forceOn, bool minimalistic)
        {
            bool state = EditorPrefs.GetBool(key, true);

            if (!minimalistic)
            {
                GUILayout.Space(3f);
            }

            if (!forceOn && !state)
            {
                GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
            }

            GUILayout.BeginHorizontal();
            GUI.changed = false;

            if (minimalistic)
            {
                if (state)
                {
                    text = "\u25BC" + (char)0x200a + text;
                }
                else
                {
                    text = "\u25BA" + (char)0x200a + text;
                }

                GUILayout.BeginHorizontal();
                GUI.contentColor = EditorGUIUtility.isProSkin
                    ? new Color(1f, 1f, 1f, 0.7f)
                    : new Color(0f, 0f, 0f, 0.7f);

                if (!GUILayout.Toggle(true, text, "PreToolbar2", GUILayout.MinWidth(20f)))
                {
                    state = !state;
                }

                GUI.contentColor = Color.white;
                GUILayout.EndHorizontal();
            }
            else
            {
                text = "<b><size=11>" + text + "</size></b>";

                if (state)
                {
                    text = "\u25BC " + text;
                }
                else
                {
                    text = "\u25BA " + text;
                }

                if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f)))
                {
                    state = !state;
                }
            }

            if (GUI.changed)
            {
                EditorPrefs.SetBool(key, state);
            }

            if (!minimalistic)
            {
                GUILayout.Space(2f);
            }

            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;

            if (!forceOn && !state)
            {
                GUILayout.Space(3f);
            }

            return state;
        }

        #endregion
    }
}

