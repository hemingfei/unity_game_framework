/****************************************************
*	文件：LubanMenu.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/01/04 16:09:39
*	功能：暂无
*****************************************************/

using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CustomGameFramework.Editor.DataTable
{
    public class LubanMenu
    {
        public const string GenBatFile = "gen.bat";
        public const string GenShFile = "gen.sh";

        [MenuItem("Tools/Generate DataTable \t 生成数据表", false, 50)]
        public static void RunDataTableGenerate()
        {
            if (Application.platform is RuntimePlatform.OSXEditor or RuntimePlatform.LinuxEditor)
                RunDataTableGenerate(GenShFile);
            else
                RunDataTableGenerate(GenBatFile);
        }
        
        [MenuItem("Tools/Open Excel Folder \t 打开Excel目录", false, 51)]
        public static void OpenDataTableExcel()
        {
            var path = RunBatchTool.FormatPath(Path.GetFullPath(Application.dataPath + "/../Luban/DataTable/Datas/"));
            if (Directory.Exists(path))
            {
                Process.Start(path);
                Debug.Log("Opened: " + path);
            }
            else
            {
                Debug.LogError("不存在目录: " + path);
            }
        }

        public static void RunDataTableGenerate(string fileName)
        {
            RunBatchTool.RunBat(fileName, Path.GetFullPath(Application.dataPath + "/../Luban/DataTable/"));
        }
    }
}