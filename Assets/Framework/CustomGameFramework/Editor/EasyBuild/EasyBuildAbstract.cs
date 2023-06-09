/****************************************************
*	文件：EasyBuild.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/06/09 16:26:04
*	功能：暂无
*****************************************************/

using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using YooAsset;

namespace CustomGameFramework.Editor
{
    public abstract class EasyBuildAbstract
    {
        public abstract void GetCommandLineArgs(ref string buildVersion, ref int versionCode, ref EasyBuild.BuildMode buildMode);
        
        public async Task StartBuild(BuildTarget buildTarget, 
            string buildVersion, int versionCode, EasyBuild.BuildMode buildMode,
            bool isRunDataTable, bool isRunSpriteAtlas, bool isRunAssetBundle, 
            string packageVersion, IEncryptionServices encryptionServices,
            bool isOutputFileAddTimestamp, bool isCompressOutputFile)
        {
            // 开始 BUILD
            ShowLog("BUILD START");
            // 切换平台
            ShowLog($"切至打包平台");
            EasyBuild.Run.SwitchBuildTarget(buildTarget);
            await EasyBuild.EasyBuild_Utility.WaitCompile();
            // 尝试获取命令行参数
            ShowLog("获取输入参数");
            GetCommandLineArgs(ref buildVersion, ref versionCode, ref buildMode);
            await EasyBuild.EasyBuild_Utility.WaitCompile();
            // 设置PlayerSettings
            ShowLog($"设置打包参数");
            EasyBuild.Run.SetPlayerSettings(buildVersion, versionCode, buildMode);
            await EasyBuild.EasyBuild_Utility.WaitCompile();
            // 设置安卓keystore
            if (buildTarget == BuildTarget.Android)
            {
                ShowLog("设置安卓密钥");
                EasyBuild.Run.SetAndroidKeyStore(EasyBuildSettingData.Setting.AndroidKeyStoreName, EasyBuildSettingData.Setting.AndroidKeyStorePass, EasyBuildSettingData.Setting.AndroidKeyAliasName, EasyBuildSettingData.Setting.AndroidKeyAliasPass);
                await EasyBuild.EasyBuild_Utility.WaitCompile();
            }
            // 打表
            if (isRunDataTable)
            {
                ShowLog("开始打数据表");
                await EasyBuild.EasyBuild_DataTable.RunDataTable();
            }
            // 打图集
            if (isRunSpriteAtlas)
            {
                ShowLog("开始构建图集");
                await EasyBuild.EasyBuild_SpriteAtlas.RunSpriteAtlas();
            }
            // 打资源包
            if (isRunAssetBundle)
            {
                ShowLog("开始打资源包");
                bool isAssetBundleSuccess = await EasyBuild.EasyBuild_Resource.RunAssetBundle(buildTarget, packageVersion, encryptionServices);
                if(!isAssetBundleSuccess)
                {
                    Debug.LogError("资源包构建失败");
                    return;
                }
            }
            // 设置BuildOptions
            BuildOptions buildOption = BuildOptions.None;
            if (buildMode == EasyBuild.BuildMode.DEVELOP)
            {
                buildOption = BuildOptions.Development;
            }
            // 设置输出文件路径
            string dirPath = EasyBuild.EasyBuild_Utility.GetOutputDir();
            string outputFileName = EasyBuildSettingData.Setting.OutputNameWithoutExtension;
            if (isOutputFileAddTimestamp)
            {
                outputFileName = $"{EasyBuildSettingData.Setting.OutputNameWithoutExtension}-";
                outputFileName += (buildMode == EasyBuild.BuildMode.DEVELOP? "develop-": "release-");
                outputFileName += $"{buildVersion}-";
                outputFileName += $"{versionCode}-";
                outputFileName += DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            string outputDir = Path.Combine(dirPath, outputFileName);
            // 开始打包
            ShowLog("开始打包构建");
            await EasyBuild.Run.StartBuild(buildTarget, outputDir, outputFileName, buildOption);
            if (isCompressOutputFile)
            {
                await EasyBuild.EasyBuild_Utility.WaitCompile();
                // 压缩输出文件夹
                ShowLog("开始压缩文件");
                await EasyBuild.Run.CompressOutput(buildTarget, outputDir);
            }
            // 结束 BUILD
            await EasyBuild.EasyBuild_Utility.WaitCompile();
            ShowLog("BUILD FINISH");
        }

        public static void ShowLog(string msg)
        {
            Debug.Log($"-------------------------------------------><color=cyan>{msg}</color><------------------------------------------");
        }
    }
}

