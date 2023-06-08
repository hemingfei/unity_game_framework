/****************************************************
*	文件：SpriteAtlasConfig.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2022/12/12 10:03:25
*	功能：图集工具的配置文件
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomGameFramework.Editor.SpriteAtlas
{
    public class SpriteAtlasUserConfig : ScriptableObject
    {
        public Object[] SpriteFolders;
    }

}


