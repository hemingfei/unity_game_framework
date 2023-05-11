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
        
        [MenuItem("Tools/DataTable \t\t\t 数据表格/Generate \t\t 生成配置", false, 10)]
        public static void RunDataTableGenerate()
        {
            if (Application.platform is RuntimePlatform.OSXEditor or RuntimePlatform.LinuxEditor)
            {
                RunDataTableGenerate(GenShFile);
            }
            else
            {
                RunDataTableGenerate(GenBatFile);
            }
        }
        
        public static void RunDataTableGenerate(string fileName)
        {
            RunBatchTool.RunBat(fileName, Path.GetFullPath(Application.dataPath + "/../Luban/DataTable/"));
        }

        [MenuItem("Tools/DataTable \t\t\t 数据表格/Go To Excel \t\t 打开Excel文件夹", false, 11)]
        public static void OpenDataTableExcel()
        {
            string path = RunBatchTool.FormatPath(Path.GetFullPath(Application.dataPath + "/../Luban/DataTable/Datas/"));
            System.Diagnostics.Process.Start(path);
            Debug.Log("Opened: " + path);
        }
        
        [MenuItem("Tools/DataTable \t\t\t 数据表格/Gen Tool Q&A \t 工具运行问题 /Visual Studio \t\t VS2022自带.NET6", false, 12)]
        public static void OpenURL_VisualStudio()
        {
            Application.OpenURL("https://visualstudio.microsoft.com/zh-hans/");
        }
        [MenuItem("Tools/DataTable \t\t\t 数据表格/Gen Tool Q&A \t 工具运行问题 /.NET \t\t\t 单独安装.NET6", false, 13)]
        public static void OpenURL_Dotnet()
        {
            Application.OpenURL("https://dotnet.microsoft.com/zh-cn/download/dotnet");
        }
        [MenuItem("Tools/DataTable \t\t\t 数据表格/Gen Tool Q&A \t 工具运行问题 /Mac Permission \t Mac机权限", false, 14)]
        public static void Problem_MacRunSHell()
        {
            Application.OpenURL("https://juejin.cn/post/6998927165655875592");
            Debug.LogWarning("需要打开Excel文件夹 运行 chmod +x ../gen.sh");
        }
    }
}

