/****************************************************
*	文件：SpriteAtlasNewCreater.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/06/07 17:09:48
*	功能：暂无
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace CustomGameFramework.Editor.SpriteAtlas
{
    public class SpriteAtlasBuilder : EditorWindow
    {
        // 打包品质
        public enum SpriteQualityLevel
        {
            High, // 最高品质
            Mid,
            Low, // 最低品质
        }
        
        // refresh all sprite atlas in project
        public static void RefreshAllSpriteAtlas()
        {
            SpriteAtlasUtility.PackAllAtlases(EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        // delete all sprite atlas files in specific folders and delete all cache
        public static void DeleteSpriteAtlasInSpecificFolders(string[] specificFolders)
        {
            // delete .spriteatlas files
            foreach (var specificAtlasFolder in specificFolders)
            {
                if (Directory.Exists(specificAtlasFolder))
                {
                    foreach (string filePath in Directory.GetFiles(specificAtlasFolder, "*.spriteatlas",
                                 SearchOption.AllDirectories))
                    {
                        if (Path.GetExtension(filePath) != ".spriteatlas")
                        {
                            continue;
                        }

                        File.Delete(filePath);
                    }

                    foreach (string filePath in Directory.GetFiles(specificAtlasFolder, "*.spriteatlas.meta",
                                 SearchOption.AllDirectories))
                    {
                        if (Path.GetExtension(filePath) != ".meta")
                        {
                            continue;
                        }

                        File.Delete(filePath);
                    }
                }
            }

            // delete cache
            string cache_dir = $"{Application.dataPath}/../Library/AtlasCache";
            if (Directory.Exists(cache_dir))
            {
                Directory.Delete(cache_dir, true);
            }

            // refresh
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        // pacakge folders' png files into one sprite atlas, each folders array package its self
        public static void PackageAllFoldersSprite(string[] folders, string nickName, string outputPath, SpriteQualityLevel qualityLevel)
        {
            List<string> pngFiles = new List<string>();
            List<Sprite> spriteFiles = new List<Sprite>();
            foreach (string folder in folders)
            {
                if (Directory.Exists(folder))
                {
                    string[] files = Directory.GetFiles(folder, "*.png", SearchOption.AllDirectories);
                    foreach (string file in files)
                    {
                        pngFiles.Add(file);
                        string pngAssetPath = file.Substring(file.IndexOf("Assets")).Replace("\\", "/");
                        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(pngAssetPath);

                        if (IsPackable(sprite))
                        {
                            spriteFiles.Add(sprite);
                        }
                    }
                }
            }

            if (pngFiles.Count > 0)
            {
                string atlasName = "[SpriteAtlas]" + nickName + ".spriteatlas";

                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                string filePath = outputPath + "/" + atlasName;
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                string fileAssetPath = filePath.Substring(filePath.IndexOf("Assets")).Replace("\\", "/");
                Debug.LogWarning("生成图集：" + fileAssetPath);
                AssetDatabase.CreateAsset(new UnityEngine.U2D.SpriteAtlas(), fileAssetPath);
                var sptAtlas = AssetDatabase.LoadAssetAtPath<UnityEngine.U2D.SpriteAtlas>(fileAssetPath);
                sptAtlas.Add(spriteFiles.ToArray());
                EditorUtility.SetDirty(sptAtlas);
                PackAtlas(sptAtlas, qualityLevel);
            }
        }
        
        private static bool IsPackable(Object o)
        {
            return o != null && (o.GetType() == typeof(Sprite) || o.GetType() == typeof(Texture2D) ||
                                 (o.GetType() == typeof(DefaultAsset) &&
                                  ProjectWindowUtil.IsFolder(o.GetInstanceID())));
        }
        
        private static void PackAtlas(UnityEngine.U2D.SpriteAtlas atlas, SpriteQualityLevel qualityLevel)
        {
            TextureImporterFormat QualityLevel_Mobile = TextureImporterFormat.ASTC_5x5;
            TextureImporterFormat QualityLevel_PC = TextureImporterFormat.DXT5;
            switch (qualityLevel)
            {
                case SpriteQualityLevel.High:
                    QualityLevel_Mobile = TextureImporterFormat.ASTC_4x4;
                    QualityLevel_PC = TextureImporterFormat.BC6H;
                    break;
                case SpriteQualityLevel.Mid:
                    QualityLevel_Mobile = TextureImporterFormat.ASTC_6x6;
                    QualityLevel_PC = TextureImporterFormat.DXT5;
                    break;
                case SpriteQualityLevel.Low:
                    QualityLevel_Mobile = TextureImporterFormat.ASTC_10x10;
                    QualityLevel_PC = TextureImporterFormat.DXT5Crunched;
                    break;
            }

            SpriteAtlasPackingSettings packSetting = new SpriteAtlasPackingSettings()
            {
                blockOffset = 1,
                enableRotation = false,
                enableTightPacking = false,
                padding = 4,
            };
            atlas.SetPackingSettings(packSetting);
            SpriteAtlasTextureSettings textureSetting = new SpriteAtlasTextureSettings()
            {
                readable = false,
                generateMipMaps = false,
                sRGB = true,
                filterMode = FilterMode.Bilinear,
            };
            atlas.SetTextureSettings(textureSetting);
            TextureImporterPlatformSettings normalSetting = new TextureImporterPlatformSettings()
            {
                maxTextureSize = 2048,
                format = TextureImporterFormat.Automatic,
                crunchedCompression = true,
                textureCompression = TextureImporterCompression.Compressed,
                compressionQuality = 50,
            };
            // 安卓设置
            TextureImporterPlatformSettings androidSetting = new TextureImporterPlatformSettings()
            {
                name = "Android",
                overridden = true,
                maxTextureSize = 2048,
                format = QualityLevel_Mobile,
                textureCompression = TextureImporterCompression.CompressedHQ,
            };
            // ios设置
            TextureImporterPlatformSettings iosSetting = new TextureImporterPlatformSettings()
            {
                name = "iPhone",
                overridden = true,
                maxTextureSize = 2048,
                format = QualityLevel_Mobile,
                textureCompression = TextureImporterCompression.CompressedHQ,
            };
            // PC设置
            TextureImporterPlatformSettings pcSetting = new TextureImporterPlatformSettings()
            {
                name = "Standalone",
                overridden = true,
                maxTextureSize = 2048,
                format = QualityLevel_PC,
                textureCompression = TextureImporterCompression.CompressedHQ,
            };
            // set
            atlas.SetPlatformSettings(normalSetting);
            atlas.SetPlatformSettings(androidSetting);
            atlas.SetPlatformSettings(iosSetting);
            atlas.SetPlatformSettings(pcSetting);

            atlas.SetIncludeInBuild(true);
            atlas.SetIsVariant(false);
            SpriteAtlasUtility.PackAtlases(new[] { atlas }, EditorUserBuildSettings.activeBuildTarget);
            EditorUtility.SetDirty(atlas);
            AssetDatabase.SaveAssets();
        }
    }
}

