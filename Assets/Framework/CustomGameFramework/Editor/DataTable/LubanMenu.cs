/****************************************************
*	文件：LubanMenu.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/01/04 16:09:39
*	功能：暂无
*****************************************************/

using System.IO;
using UnityEditor;
using UnityEngine;

namespace CustomGameFramework.Editor.DataTable
{
    public class LubanMenu
    {
        public const string GenBatFile = "gen.bat";
        public const string GenShFile = "gen.sh";

        [MenuItem("Tools/DataTable Generate \t 生成数据表", false, 50)]
        public static void RunDataTableGenerate()
        {
            if (Application.platform is RuntimePlatform.OSXEditor or RuntimePlatform.LinuxEditor)
                RunDataTableGenerate(GenShFile);
            else
                RunDataTableGenerate(GenBatFile);
        }

        public static void RunDataTableGenerate(string fileName)
        {
            RunBatchTool.RunBat(fileName, Path.GetFullPath(Application.dataPath + "/../Luban/DataTable/"));
        }
    }
}