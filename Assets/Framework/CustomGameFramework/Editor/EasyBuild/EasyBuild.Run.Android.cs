/****************************************************
*	文件：EasyBuild.Run.Android.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/06/09 15:24:49
*	功能：暂无
*****************************************************/

using UnityEditor;

namespace CustomGameFramework.Editor
{
    public partial class EasyBuild
    {
        public static class EasyBuild_RunAndroid
        {
            public static void SwitchBuildTarget()
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            }
            
            public static void StartBuild(string apkPath, BuildOptions buildOption)
            {
                BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, apkPath, BuildTarget.Android, buildOption);
            }
        }
    }
}

