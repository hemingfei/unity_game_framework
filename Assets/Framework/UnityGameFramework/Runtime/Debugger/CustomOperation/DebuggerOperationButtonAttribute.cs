/****************************************************
*	文件：DebuggerOperationButtonAttribute.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/01/05 19:12:32
*	功能：暂无
*****************************************************/

using System;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// Button Attribute
    /// </summary>
    public class DebuggerOperationButtonAttribute : Attribute
    {
        #region  Attributes and Properties
        public string Description { get; private set; }

        public int Priority { get; private set; }
        #endregion

        #region Public Methods
        public DebuggerOperationButtonAttribute(string description, int priority = 0)
        {
            this.Description = description;
            this.Priority = priority;
        }
        #endregion
    }
}


