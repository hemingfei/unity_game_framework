/****************************************************
*	文件：HttpWebRequestComponent.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/05/17 15:11:47
*	功能：暂无
*****************************************************/

using GameFramework;
using GameFramework.Event;
using UnityGameFramework.Runtime;

namespace CustomGameFramework.Runtime
{
    public sealed class HttpWebRequestComponent : GameFrameworkComponent
    {
        private const string DataNode_Root_CustomWebRequest = "CustomWebRequest";
        private const string DataNode_Root_CustomMsgReturnData = "CustomMsgReturnData";
        private DataNodeComponent m_DataNodeComponent;
        private EventComponent m_EventComponent;

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

            m_EventComponent.Subscribe(WebRequestStartEventArgs.EventId, OnWebRequestStartEvent);
            m_EventComponent.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccessEvent);
            m_EventComponent.Subscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailureEvent);
        }

        private void OnWebRequestStartEvent(object sender, GameEventArgs e)
        {
            var userData = ((WebRequestStartEventArgs)e).UserData;
            if (userData == null || userData.GetType() != typeof(HttpWebRequestInfo)) return;
            var args = HttpWebRequestStartEventArgs.Create((WebRequestStartEventArgs)e);

            Log.Debug($"CustomWebRequest [SerialId] {args.SerialId} Start \n" +
                      "【URL】" + args.WebRequestUri +
                      "【Headers】" + Utility.Json.ToJson(((HttpWebRequestInfo)args.UserData).Headers) +
                      "【Body】" + ((HttpWebRequestInfo)args.UserData).BodyJson +
                      "【UID】" + args.UID);

            m_EventComponent.Fire(this, args);
        }

        private void OnWebRequestSuccessEvent(object sender, GameEventArgs e)
        {
            var userData = ((WebRequestSuccessEventArgs)e).UserData;
            if (userData == null || userData.GetType() != typeof(HttpWebRequestInfo)) return;
            var args = HttpWebRequestSuccessEventArgs.Create((WebRequestSuccessEventArgs)e);

            var returnDataJson = Utility.Converter.GetString(args.GetWebResponseBytes());

            SetReturnData(args.UID, returnDataJson);

            Log.Debug($"CustomWebRequest [SerialId] {args.SerialId} Success \n" +
                      "【ResponseData】" + returnDataJson +
                      "【UID】" + args.UID);

            m_EventComponent.Fire(this, args);
        }

        private void OnWebRequestFailureEvent(object sender, GameEventArgs e)
        {
            var userData = ((WebRequestFailureEventArgs)e).UserData;
            if (userData == null || userData.GetType() != typeof(HttpWebRequestInfo)) return;
            var args = HttpWebRequestFailureEventArgs.Create((WebRequestFailureEventArgs)e);

            Log.Debug($"CustomWebRequest [SerialId] {args.SerialId} Failure \n" +
                      "【ErrorMessage】" + args.ErrorMessage +
                      "【UID】" + args.UID);

            m_EventComponent.Fire(this, args);
        }

        // 储存返回数据
        private void SetReturnData(string uid, string returnDataJson)
        {
            var nodePath = Utility.Text.Format("{0}.{1}.{2}", DataNode_Root_CustomWebRequest,
                DataNode_Root_CustomMsgReturnData, uid);
            m_DataNodeComponent.SetData<VarString>(nodePath, returnDataJson);
        }

        /// <summary>
        ///     获取 return data
        /// </summary>
        /// <param name="uid">uid</param>
        /// <typeparam name="T">return data 的类型</typeparam>
        /// <returns>若存在则返回，不存在则为null</returns>
        public T GetReturnData<T>(string uid)
        {
            var nodePath = Utility.Text.Format("{0}.{1}.{2}", DataNode_Root_CustomWebRequest,
                DataNode_Root_CustomMsgReturnData, uid);
            if (m_DataNodeComponent.GetNode(nodePath) != null)
            {
                string returnDataJson = m_DataNodeComponent.GetData<VarString>(nodePath);
                return Utility.Json.ToObject<T>(returnDataJson);
            }

            return default;
        }

        // 删除某个数据
        public void DeleteReturnData(int uid)
        {
            var nodePath = Utility.Text.Format("{0}.{1}.{2}", DataNode_Root_CustomWebRequest,
                DataNode_Root_CustomMsgReturnData, uid);
            if (m_DataNodeComponent.GetNode(nodePath) != null) m_DataNodeComponent.RemoveNode(nodePath);
        }

        // 删除所有数据
        public void DeleteAllReturnData()
        {
            var nodePath = Utility.Text.Format("{0}.{1}", DataNode_Root_CustomWebRequest,
                DataNode_Root_CustomMsgReturnData);
            if (m_DataNodeComponent.GetNode(nodePath) != null) m_DataNodeComponent.RemoveNode(nodePath);
        }
    }
}