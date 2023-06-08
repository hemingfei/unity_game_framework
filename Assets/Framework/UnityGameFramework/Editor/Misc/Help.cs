//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace UnityGameFramework.Editor
{
    /// <summary>
    /// 帮助相关的实用函数。
    /// </summary>
    public static class Help
    {
        [MenuItem("Tools/Game Framework/Documentation")]
        public static void ShowDocumentation()
        {
            ShowHelp("https://gameframework.cn/document/");
        }

        [MenuItem("Tools/Game Framework/API Reference")]
        public static void ShowApiReference()
        {
            ShowHelp("https://gameframework.cn/api/");
        }

        private static void ShowHelp(string uri)
        {
            Application.OpenURL(uri);
        }
    }
}
