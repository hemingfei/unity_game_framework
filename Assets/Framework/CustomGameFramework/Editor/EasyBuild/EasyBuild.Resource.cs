/****************************************************
*	文件：EasyBuild.Resource.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/06/09 14:13:32
*	功能：暂无
*****************************************************/

using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using YooAsset;
using YooAsset.Editor;

namespace CustomGameFramework.Editor
{
    public partial class EasyBuild
    {
        public static class EasyBuild_Resource
        {
            public static async Task<bool> RunAssetBundle(BuildTarget buildTarget, string packageVersion, IEncryptionServices encryptionServices)
            {
                // delete StreamingAssets
                EasyBuild_Utility.DeleteFolder("Assets/StreamingAssets/BuildinFiles");
                await EasyBuild_Utility.WaitCompile();
                // delete Bundles
                EasyBuild_Utility.DeleteFolder("Assets/../Bundles");
                await EasyBuild_Utility.WaitCompile();
                // build
                bool isSuccess = BuildInternal(buildTarget, packageVersion, encryptionServices);
                await EasyBuild_Utility.WaitCompile();
                return isSuccess;
            }

            private static bool BuildInternal(BuildTarget buildTarget, string packageVersion, IEncryptionServices encryptionServices)
            {
                // 构建参数
                string defaultOutputRoot = AssetBundleBuilderHelper.GetDefaultOutputRoot();
                BuildParameters buildParameters = new BuildParameters();
                buildParameters.OutputRoot = defaultOutputRoot;
                buildParameters.BuildTarget = buildTarget;
                buildParameters.BuildPipeline = AssetBundleBuilderSettingData.Setting.BuildPipeline;
                buildParameters.BuildMode = AssetBundleBuilderSettingData.Setting.BuildMode;
                buildParameters.PackageName = AssetBundleBuilderSettingData.Setting.BuildPackage;
                buildParameters.PackageVersion = packageVersion;
                buildParameters.VerifyBuildingResult = true;
                buildParameters.AutoAnalyzeRedundancy = true;
                buildParameters.ShareAssetPackRule = new DefaultShareAssetPackRule();
                buildParameters.EncryptionServices = encryptionServices;
                buildParameters.CompressOption = AssetBundleBuilderSettingData.Setting.CompressOption;
                buildParameters.OutputNameStyle = AssetBundleBuilderSettingData.Setting.OutputNameStyle;
                buildParameters.CopyBuildinFileOption = AssetBundleBuilderSettingData.Setting.CopyBuildinFileOption;
                buildParameters.CopyBuildinFileTags = AssetBundleBuilderSettingData.Setting.CopyBuildinFileTags;

                if (AssetBundleBuilderSettingData.Setting.BuildPipeline == EBuildPipeline.ScriptableBuildPipeline)
                {
                    buildParameters.SBPParameters = new BuildParameters.SBPBuildParameters();
                    buildParameters.SBPParameters.WriteLinkXML = true;
                }
                
                // 执行构建
                AssetBundleBuilder builder = new AssetBundleBuilder();
                var buildResult = builder.Run(buildParameters);
                return buildResult.Success;
            }
        }
    }
}


