/****************************************************
*	文件：EasyBuild.SpriteAtlas.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/06/09 14:13:32
*	功能：暂无
*****************************************************/

using System.Threading.Tasks;
using UnityEngine;

namespace CustomGameFramework.Editor
{
    public partial class EasyBuild
    {
        public static class EasyBuild_SpriteAtlas
        {
            public static async Task RunSpriteAtlas()
            {
                SpriteAtlas.SpriteAtlasConfigEditor.DeleteAllSpriteAtlas();
                await EasyBuild_Utility.WaitCompile();
                SpriteAtlas.SpriteAtlasConfigEditor.RepackAllSpriteAtlas();
                await EasyBuild_Utility.WaitCompile();
            }
        }
    }
}


