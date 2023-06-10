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
            public static async Task<bool> RunAssetBundle(BuildTarget buildTarget, string packageVersion,
                IEncryptionServices encryptionServices, IShareAssetPackRule shareAssetPackRule, string packageName = "DefaultPackage")
            {
                // delete StreamingAssets
                EasyBuild_Utility.DeleteFolder("Assets/StreamingAssets/BuildinFiles");
                await EasyBuild_Utility.WaitCompile();
                // delete Bundles
                EasyBuild_Utility.DeleteFolder("Assets/../Bundles");
                await EasyBuild_Utility.WaitCompile();
                // build
                bool isSuccess = BuildInternal(buildTarget, packageVersion, encryptionServices, shareAssetPackRule, packageName);
                await EasyBuild_Utility.WaitCompile();
                return isSuccess;
            }

            private static bool BuildInternal(BuildTarget buildTarget, string packageVersion,
                IEncryptionServices encryptionServices, IShareAssetPackRule shareAssetPackRule, string packageName = "DefaultPackage")
            {
                // 构建参数
                string defaultOutputRoot = AssetBundleBuilderHelper.GetDefaultOutputRoot();
                BuildParameters buildParameters = new BuildParameters();
                buildParameters.OutputRoot = defaultOutputRoot;
                buildParameters.BuildTarget = buildTarget;
                buildParameters.BuildPipeline = EBuildPipeline.BuiltinBuildPipeline;
                buildParameters.BuildMode = EBuildMode.ForceRebuild;
                buildParameters.PackageName = packageName;
                buildParameters.PackageVersion = packageVersion;
                buildParameters.VerifyBuildingResult = true;
                buildParameters.CompressOption = ECompressOption.LZ4;
                buildParameters.OutputNameStyle = EOutputNameStyle.HashName;
                buildParameters.CopyBuildinFileOption = ECopyBuildinFileOption.ClearAndCopyAll;
                buildParameters.EncryptionServices = encryptionServices;
                buildParameters.ShareAssetPackRule = shareAssetPackRule;
                // 执行构建
                AssetBundleBuilder builder = new AssetBundleBuilder();
                var buildResult = builder.Run(buildParameters);
                return buildResult.Success;
            }
        }
    }
}


