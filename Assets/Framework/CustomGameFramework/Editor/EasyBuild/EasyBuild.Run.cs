/****************************************************
*	文件：EasyBuild.Run.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/06/09 16:22:04
*	功能：暂无
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace CustomGameFramework.Editor
{
    public partial class EasyBuild
    {
        public static class Run
        {
            public static void SetPlayerSettings(string buildVersion, int versionCode, BuildMode buildMode)
            {
                Debug.Log("Version:" + buildVersion);
                Debug.Log("Code:" + versionCode);
                Debug.Log("Mode:" + buildMode);
                PlayerSettings.bundleVersion = buildVersion;
                PlayerSettings.SplashScreen.show = false;
                PlayerSettings.SplashScreen.showUnityLogo = false;
                // BuildOptions buildOption = BuildOptions.None;
                // if (buildMode == BuildMode.DEVELOP)
                // {
                //     buildOption = BuildOptions.Development;
                // }
                PlayerSettings.Android.bundleVersionCode = versionCode;
                PlayerSettings.macOS.buildNumber = versionCode.ToString();
                PlayerSettings.iOS.buildNumber = versionCode.ToString();
            }

            public static void SetAndroidKeyStore(string keystoreName, string keystorePass, string keyaliasName, string keyaliasPass)
            {
                Debug.Log("keystoreName:" + keystoreName);
                Debug.Log("keystorePass:" + keystorePass);
                Debug.Log("keyaliasName:" + keyaliasName);
                Debug.Log("keyaliasPass:" + keyaliasPass);
                PlayerSettings.Android.useCustomKeystore = true;
                PlayerSettings.Android.keystoreName = keystoreName;
                PlayerSettings.Android.keystorePass = keystorePass; // 密钥密码
                PlayerSettings.Android.keyaliasName = keyaliasName; // 密钥别名
                PlayerSettings.Android.keyaliasPass = keyaliasPass;
            }

            public static void SwitchBuildTarget(BuildTarget buildTarget)
            {
                Debug.Log("SwitchBuildTarget: " + buildTarget);
                switch (buildTarget)
                {
                    case BuildTarget.StandaloneWindows64:
                        EasyBuild_RunWindows.SwitchBuildTarget();
                        break;
                    case BuildTarget.Android:
                        EasyBuild_RunAndroid.SwitchBuildTarget();
                        break;
                    default:
                        Debug.LogError("SwitchBuildTarget Error: " + buildTarget);
                        break;
                }
            }

            public static async Task StartBuild(BuildTarget buildTarget, string outputDirPath, string outputFileName, BuildOptions buildOption)
            {
                // delete Build folder
                EasyBuild_Utility.DeleteFolder("Assets/../../Build");
                await EasyBuild_Utility.WaitCompile();

                string finalOutputFilePath = "";
                switch (buildTarget)
                {
                    case BuildTarget.StandaloneWindows64:
                        if (!Directory.Exists(outputDirPath))
                        {
                            Directory.CreateDirectory(outputDirPath);
                        }
                        finalOutputFilePath = Path.Combine(outputDirPath, outputFileName) + ".exe";
                        EasyBuild_RunWindows.StartBuild(finalOutputFilePath, buildOption);
                        break;
                    case BuildTarget.Android:
                        finalOutputFilePath = outputDirPath + ".apk";
                        EasyBuild_RunAndroid.StartBuild(finalOutputFilePath, buildOption);
                        break;
                    default:
                        Debug.LogError("StartBuild Error: " + buildTarget);
                        break;
                }
                Debug.Log("Build Finish, Output: " + finalOutputFilePath);
                await EasyBuild_Utility.WaitCompile();
            }

            public static void CompressOutput(BuildTarget buildTarget, string outputFolderPath)
            {
                string zipFilePath = outputFolderPath + ".zip";
                if (File.Exists(zipFilePath))
                {
                    File.Delete(zipFilePath);
                }
                switch (buildTarget)
                {
                    case BuildTarget.StandaloneWindows64:
                        EasyBuild_Utility.CompressFolder(outputFolderPath, zipFilePath);
                        break;
                    case BuildTarget.Android:
                        EasyBuild_Utility.CompressFile(outputFolderPath + ".apk", zipFilePath);
                        break;
                    default:
                        Debug.LogError("StartBuild Error: " + buildTarget);
                        break;
                }
                Debug.Log("Compress Finish, Output: " + zipFilePath);
            }
        }
    }
}

