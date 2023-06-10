/****************************************************
*	文件：EasyBuild.Run.Windows.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/06/09 15:33:03
*	功能：暂无
*****************************************************/

using System.IO;
using UnityEditor;

namespace CustomGameFramework.Editor
{
    public partial class EasyBuild
    {
        public static class EasyBuild_RunWindows
        {
            public static void SwitchBuildTarget()
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
            }
            
            public static void StartBuild(string exePath, BuildOptions buildOption)
            {
                BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, exePath, BuildTarget.StandaloneWindows64, buildOption);
            }
        }
    }
}

