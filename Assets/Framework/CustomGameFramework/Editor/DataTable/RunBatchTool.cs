/****************************************************
*	文件：RunBatchTool.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/01/04 16:09:18
*	功能：暂无
*****************************************************/

using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CustomGameFramework.Editor.DataTable
{
    public class RunBatchTool
    {
        /// <summary>
        /// 运行脚本
        /// </summary>
        /// <param name="fileName">脚本文件名</param>
        /// <param name="workingDir">脚本所在路径</param>
        /// <param name="args">传入参数</param>
        public static void RunBat(string fileName, string workingDir, string args = "")
        {
            var path = FormatPath(workingDir);
            if (!System.IO.File.Exists(path + fileName))
            {
                Debug.LogError("bat文件不存在：" + path + fileName);
            }
            else
            {
                CreateShellExProcess(fileName, args, path);
            }
        }
        
        private static void CreateShellExProcess(string fileName, string args, string workingDir = "")
        {
            if (Application.platform is RuntimePlatform.OSXEditor or RuntimePlatform.LinuxEditor)
            {
                CreateShellExProcessLinux(fileName, args, workingDir);
            }
            else
            {
                CreateShellExProcessWin(fileName, args, workingDir);
            }
            AssetDatabase.Refresh();
        }

        private static void CreateShellExProcessWin(string fileName, string args, string workingDir = "")
        {
            using (Process process = new Process())
            {
                var pStartInfo = new System.Diagnostics.ProcessStartInfo(fileName);
                pStartInfo.Arguments = args;
                pStartInfo.CreateNoWindow = false;
                pStartInfo.UseShellExecute = true;
                pStartInfo.RedirectStandardError = false;
                pStartInfo.RedirectStandardInput = false;
                pStartInfo.RedirectStandardOutput = false;
                if (!string.IsNullOrEmpty(workingDir))
                {
                    pStartInfo.WorkingDirectory = workingDir;
                }

                process.StartInfo = pStartInfo;
                process.Start();
                //process.WaitForExit();
                process.Close();
            }
        }
        
        private static void CreateShellExProcessLinux(string fileName, string args, string workingDir = "")
        {
            using (Process process = new Process())
            {
                var pStartInfo = new System.Diagnostics.ProcessStartInfo(fileName);
                pStartInfo.FileName = "/bin/sh";
                pStartInfo.Arguments = fileName + " " +args;
                pStartInfo.ErrorDialog = true;
                pStartInfo.CreateNoWindow = false;
                pStartInfo.UseShellExecute = false;
                pStartInfo.RedirectStandardError = true;
                pStartInfo.RedirectStandardInput = true;
                pStartInfo.RedirectStandardOutput = true;
                if (!string.IsNullOrEmpty(workingDir))
                {
                    pStartInfo.WorkingDirectory = workingDir;
                }

                process.StartInfo = pStartInfo;
                process.Start();
                process.WaitForExit();
                process.Close();
            }
        }
    
        /// <summary>
        /// 格式化路径 斜杠，反斜杠
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static string FormatPath(string path)
        {
            path = path.Replace("/", "\\");
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                path = path.Replace("\\", "/");
            }
            return path;
        }
    }
}

