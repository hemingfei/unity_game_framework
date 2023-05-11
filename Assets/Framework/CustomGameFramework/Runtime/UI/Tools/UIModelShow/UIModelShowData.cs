/****************************************************
*	文件：UIModelShowData.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/01/08 02:29:08
*	功能：暂无
*****************************************************/

using UnityEngine;
//using UnityEngine.Rendering.Universal;

namespace CustomGameFramework.Runtime
{
    public class UIModelShowData
    {
        public GameObject top;
        public GameObject root;
        public GameObject model;
        public Camera camera;
        public RenderTexture renderTexture;
        public Camera baseCamerainURP;
        public Light light;

        public void Dispose()
        {
            // if (baseCamerainURP != null)
            // {
            //     var cameraStack = baseCamerainURP.GetUniversalAdditionalCameraData().cameraStack;
            //     if (cameraStack.Contains(camera))
            //     {
            //         cameraStack.Remove(camera);
            //     }
            // }
            UIModelShowTool.DestroyModelGameObject(model);
            GameObject.Destroy(top);
        }

        public void ChangeModel(string modelName)
        {
            int layer = model.layer;
            UIModelShowTool.DestroyModelGameObject(model);

            var pos = model.transform.localPosition;
            var angle = model.transform.localEulerAngles;
            var scale = model.transform.localScale;
            
            model = UIModelShowTool.CreateModelGameObject(modelName);
            model.transform.SetParent(root.transform);
            model.transform.localPosition = pos;
            model.transform.localEulerAngles = angle;
            model.transform.localScale = scale;
            model.SetLayerRecursively(layer);
        }
    }
}