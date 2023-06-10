/****************************************************
*	文件：EasyBuild.DataTable.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/06/09 14:13:32
*	功能：暂无
*****************************************************/

using System.Threading.Tasks;

namespace CustomGameFramework.Editor
{
    public partial class EasyBuild
    {
        public static class EasyBuild_DataTable
        {
            public static async Task RunDataTable()
            {
                DataTable.LubanMenu.RunDataTableGenerate(DataTable.LubanMenu.GenShFile);
                await EasyBuild_Utility.WaitCompile();
            }
        }
    }
}


