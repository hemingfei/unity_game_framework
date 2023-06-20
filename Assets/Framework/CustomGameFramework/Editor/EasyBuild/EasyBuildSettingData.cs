using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CustomGameFramework.Editor
{
    public class EasyBuildSettingData
    {
        private static EasyBuildSetting _setting = null;
        public static EasyBuildSetting Setting
        {
            get
            {
                if (_setting == null)
                    LoadSettingData();
                return _setting;
            }
        }

        /// <summary>
        /// 配置数据是否被修改
        /// </summary>
        public static bool IsDirty { set; get; } = false;

        /// <summary>
        /// 加载配置文件
        /// </summary>
        private static void LoadSettingData()
        {
            _setting = SettingLoader.LoadSettingData<EasyBuildSetting>();
        }

        /// <summary>
        /// 存储文件
        /// </summary>
        public static void SaveFile()
        {
            if (Setting != null)
            {
                IsDirty = false;
                EditorUtility.SetDirty(Setting);
                AssetDatabase.SaveAssets();
                Debug.Log($"{nameof(EasyBuildSetting)}.asset is saved!");
            }
        }
    }
}