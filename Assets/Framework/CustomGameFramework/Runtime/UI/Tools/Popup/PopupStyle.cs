/****************************************************
*	文件：PopupStyle.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/01/07 21:29:18
*	功能：暂无
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomGameFramework.Runtime
{
    /// <summary>
    /// 弹窗显示的样式
    /// </summary>
    public enum PopupStyle
    {
        ShowTitle = 1,
        ShowContent = 2,
        ShowOk = 4,
        ShowOkNo = 8,
        Default = (ShowTitle | ShowContent | ShowOkNo),
        Title_Content_Ok = (ShowTitle | ShowContent | ShowOk),
        Title_Ok = (ShowTitle | ShowOk),
        Title_OkNo = (ShowTitle | ShowOkNo),
        Content_Ok = (ShowContent | ShowOk),
        Content_OkNo = (ShowContent | ShowOkNo),
    }
}
