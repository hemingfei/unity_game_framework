/****************************************************
*	文件：AutoBuildSetting.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/06/20 10:36:34
*	功能：暂无
*****************************************************/

using UnityEditor;
using UnityEngine;

namespace CustomGameFramework.Editor
{
    [CreateAssetMenu(fileName = "EasyBuildSetting", menuName = "Framework/Create Easy Build Settings")]
    public class EasyBuildSetting : ScriptableObject
    {
        [Header("输出的文件名")]
        public string OutputNameWithoutExtension = "myGame";
        [Header("Android相关配置")]
        public string AndroidKeyStoreName = "Keystore/mygame.keystore";
        public string AndroidKeyStorePass = "mygame";
        public string AndroidKeyAliasName = "mygame";
        public string AndroidKeyAliasPass = "mygame";
    }
}

