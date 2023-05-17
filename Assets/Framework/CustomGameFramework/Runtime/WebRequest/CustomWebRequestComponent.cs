/****************************************************
*	文件：CustomWebRequestComponent.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/05/17 15:11:47
*	功能：暂无
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace CustomGameFramework.Runtime
{
    public sealed class CustomWebRequestComponent : GameFrameworkComponent
    {
        private EventComponent m_EventComponent = null;
        private DataNodeComponent m_DataNodeComponent = null;
        
        private const string DataNode_Root_CustomWebRequest = "CustomWebRequest";
        private const string DataNode_Root_CustomMsgReturnData = "CustomMsgReturnData";

        private void Start()
        {
            m_EventComponent = GameEntry.GetComponent<EventComponent>();
            if (m_EventComponent == null)
            {
                Log.Fatal("Event component is invalid.");
                return;
            }
            
            m_DataNodeComponent = GameEntry.GetComponent<DataNodeComponent>();
            if (m_DataNodeComponent == null)
            {
                Log.Fatal("Data node component is invalid.");
                return;
            }
            
            m_EventComponent.Subscribe(UnityGameFramework.Runtime.WebRequestStartEventArgs.EventId, OnWebRequestStartEvent);
            m_EventComponent.Subscribe(UnityGameFramework.Runtime.WebRequestSuccessEventArgs.EventId, OnWebRequestSuccessEvent);
            m_EventComponent.Subscribe(UnityGameFramework.Runtime.WebRequestFailureEventArgs.EventId, OnWebRequestFailureEvent);
        }
        
        private void OnWebRequestStartEvent(object sender, GameEventArgs e)
        {
            var userData = ((WebRequestStartEventArgs)e).UserData;
            if (userData == null || userData.GetType() != typeof(CustomWebRequestInfo))
            {
                return;
            }
            var args = CustomWebRequestStartEventArgs.Create((UnityGameFramework.Runtime.WebRequestStartEventArgs)e);

            Log.Debug($"CustomWebRequest [SerialId] {args.SerialId} Start \n" +
                      "【URL】" + args.WebRequestUri +
                      "【Headers】" + GameFramework.Utility.Json.ToJson(((CustomWebRequestInfo)args.UserData).Headers) +
                      "【Body】" + ((CustomWebRequestInfo)args.UserData).BodyJson +
                      "【UID】" + args.UID);
            
            m_EventComponent.Fire(this, args);
        }

        private void OnWebRequestSuccessEvent(object sender, GameEventArgs e)
        {
            var userData = ((WebRequestSuccessEventArgs)e).UserData;
            if (userData == null || userData.GetType() != typeof(CustomWebRequestInfo))
            {
                return;
            }
            var args = CustomWebRequestSuccessEventArgs.Create((UnityGameFramework.Runtime.WebRequestSuccessEventArgs)e);
            
            var returnDataJson = GameFramework.Utility.Converter.GetString(args.GetWebResponseBytes());
            
            SetReturnData(args.UID, returnDataJson);
            
            Log.Debug($"CustomWebRequest [SerialId] {args.SerialId} Success \n" +
                      "【ResponseData】" + returnDataJson +
                      "【UID】" + args.UID);
            
            m_EventComponent.Fire(this, args);
        }

        private void OnWebRequestFailureEvent(object sender, GameEventArgs e)
        {
            var userData = ((WebRequestFailureEventArgs)e).UserData;
            if (userData == null || userData.GetType() != typeof(CustomWebRequestInfo))
            {
                return;
            }
            var args = CustomWebRequestFailureEventArgs.Create((UnityGameFramework.Runtime.WebRequestFailureEventArgs)e);
            
            Log.Debug($"CustomWebRequest [SerialId] {args.SerialId} Failure \n" +
                      "【ErrorMessage】" + args.ErrorMessage +
                      "【UID】" + args.UID);
            
            m_EventComponent.Fire(this, args);
        }

        // 储存返回数据
        private void SetReturnData(string uid, string returnDataJson)
        {
            string nodePath = GameFramework.Utility.Text.Format("{0}.{1}.{2}", DataNode_Root_CustomWebRequest, DataNode_Root_CustomMsgReturnData, uid);
            m_DataNodeComponent.SetData<VarString>(nodePath, returnDataJson);
        }

        /// <summary>
        /// 获取 return data
        /// </summary>
        /// <param name="uid">uid</param>
        /// <typeparam name="T">return data 的类型</typeparam>
        /// <returns>若存在则返回，不存在则为null</returns>
        public T GetReturnData<T>(string uid)
        {
            string nodePath = GameFramework.Utility.Text.Format("{0}.{1}.{2}", DataNode_Root_CustomWebRequest, DataNode_Root_CustomMsgReturnData, uid);
            if (m_DataNodeComponent.GetNode(nodePath) != null)
            {
                string returnDataJson = m_DataNodeComponent.GetData<VarString>(nodePath);
                return GameFramework.Utility.Json.ToObject<T>(returnDataJson);
            }
            else
            {
                return default;
            }
        }

        // 删除某个数据
        public void DeleteReturnData(int uid)
        {
            string nodePath = GameFramework.Utility.Text.Format("{0}.{1}.{2}", DataNode_Root_CustomWebRequest, DataNode_Root_CustomMsgReturnData, uid);
            if (m_DataNodeComponent.GetNode(nodePath) != null)
            {
                m_DataNodeComponent.RemoveNode(nodePath);
            }
        }

        // 删除所有数据
        public void DeleteAllReturnData()
        {
            string nodePath = GameFramework.Utility.Text.Format("{0}.{1}", DataNode_Root_CustomWebRequest, DataNode_Root_CustomMsgReturnData);
            if (m_DataNodeComponent.GetNode(nodePath) != null)
            {
                m_DataNodeComponent.RemoveNode(nodePath);
            }
        }
    }
}

