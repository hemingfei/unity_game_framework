/****************************************************
*	文件：EasyBuild.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/06/09 14:10:29
*	功能：暂无
*****************************************************/

using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace CustomGameFramework.Editor
{
    public partial class EasyBuild
    {
        [Serializable]
        public enum BuildMode
        {
            DEVELOP,
            RELEASE,
        }
        public static class EasyBuild_Utility
        {
            /// <summary>
            /// 等待编译结束
            /// </summary>
            public static async Task WaitCompile()
            {
                await Task.Delay(100);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                while (EditorApplication.isCompiling)
                {
                    await Task.Delay(1000);
                }
                await Task.Delay(100);
            }

            /// <summary>
            /// 删除文件夹
            /// </summary>
            public static void DeleteFolder(string folderDelete)
            {
                if (Directory.Exists(folderDelete))
                {
                    foreach (string file in Directory.GetFiles(folderDelete))
                    {
                        File.Delete(file);
                    }

                    foreach (string subDir in Directory.GetDirectories(folderDelete))
                    {
                        Directory.Delete(subDir, true);
                    }

                    Directory.Delete(folderDelete, true);
                }

                if (File.Exists(folderDelete + ".meta"))
                {
                    File.Delete(folderDelete + ".meta");
                }
            }

            /// <summary>
            /// 压缩文件
            /// </summary>
            public static void CompressFile(string sourceFilePath, string zipFilePath)
            {
                if(!File.Exists(sourceFilePath))
                {
                    Debug.LogError($"压缩文件不存在, 请检查路径是否正确: {sourceFilePath}");
                    return;
                }
                // 获取要压缩的文件大小
                long fileSize = new FileInfo(sourceFilePath).Length;
                string fileSizeMB = Mathf.Clamp(fileSize / 1048576f, 0.1f, float.MaxValue).ToString("f1");

                // 创建一个新的ZIP文件并打开它以写入数据
                using (var archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                {
                    // 将要压缩的文件添加到ZIP归档中，并在每次写入操作时更新进度
                    var entry = archive.CreateEntry(Path.GetFileName(sourceFilePath));
                    using (var fileStream = new FileStream(sourceFilePath, FileMode.Open))
                    using (var entryStream = entry.Open())
                    {
                        byte[] buffer = new byte[4096]; // 缓冲区大小
                        long bytesProcessed = 0; // 已处理的字节数
                        int bytesRead;
                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            entryStream.Write(buffer, 0, bytesRead);
                            bytesProcessed += bytesRead;
                            double progress = (double)bytesProcessed / (double)fileSize * 100.0; // 计算当前进度
                            string processSizeMB = Mathf.Clamp(bytesProcessed / 1048576f, 0.1f, float.MaxValue)
                                .ToString("f1");
                            // 在Unity Editor中显示进度条
                            EditorUtility.DisplayProgressBar($"正在压缩",
                                $"已压缩{processSizeMB}MB/{fileSizeMB}MB,  {progress:0.00}%",
                                (float)(bytesProcessed / (double)fileSize));
                        }
                    }
                }

                // 关闭进度条
                EditorUtility.ClearProgressBar();
            }

            /// <summary>
            /// 压缩文件夹
            /// </summary>
            public static void CompressFolder(string sourceFolderPath, string zipFilePath)
            {
                if (!Directory.Exists(sourceFolderPath))
                {
                    Debug.LogError("压缩文件夹不存在, 请检查路径是否正确: " + sourceFolderPath);
                    return;
                }
                long totalBytesToCompress = Directory.GetFiles(sourceFolderPath, "*", SearchOption.AllDirectories)
                    .Sum(f => new FileInfo(f).Length);
                long compressedBytes = 0;

                using (var archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                {
                    foreach (string filePath in Directory.GetFiles(sourceFolderPath, "*", SearchOption.AllDirectories))
                    {
                        var relativePath = Path.GetRelativePath(sourceFolderPath, filePath);

                        var entry = archive.CreateEntry(relativePath);
                        using (var fileStream = new FileStream(filePath, FileMode.Open))
                        using (var entryStream = entry.Open())
                        {
                            fileStream.CopyTo(entryStream);
                            compressedBytes += fileStream.Length;
                            double percentComplete = (double)compressedBytes / totalBytesToCompress * 100.0;
                            // 在Unity Editor中显示进度条
                            EditorUtility.DisplayProgressBar($"正在压缩",
                                $"已压缩{compressedBytes / 1048576f}MB/{totalBytesToCompress / 1048576f}MB,  {percentComplete:0.00}%",
                                (float)(compressedBytes / (double)totalBytesToCompress));
                        }
                    }
                }

                // 关闭进度条
                EditorUtility.ClearProgressBar();
            }
            
            public static string GetOutputDir()
            {
                string dirPath = Directory.GetCurrentDirectory() + "/../Build";
#if UNITY_ANDROID
                dirPath += "/Android";
#elif UNITY_IOS
                dirPath += "/iOS";
#elif UNITY_STANDALONE_WIN
                dirPath += "/PC";
#elif UNITY_STANDALONE_OSX
                dirPath += "/Mac";
#endif
                return dirPath;
            }
        }
    }
}

