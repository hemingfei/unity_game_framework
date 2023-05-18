/****************************************************
*	文件：UIModelShowRotate.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/01/08 00:19:28
*	功能：暂无
*****************************************************/

using UnityEngine;
using UnityEngine.EventSystems;

namespace CustomGameFramework.Runtime
{
    public class UIModelShowRotate : MonoBehaviour
    {
        public float rotateSpeed = 1;

        private Vector2 mousePos = Vector2.zero;

        private void Rotate(Vector2 delta)
        {
            transform.localRotation =
                Quaternion.Euler(transform.localRotation.eulerAngles + new Vector3(0, -delta.x * rotateSpeed, 0));
        }

        public void OnDrag(BaseEventData arg0)
        {
            if (mousePos == Vector2.zero)
            {
                mousePos = arg0.currentInputModule.input.mousePosition;
                return;
            }

            var delta = arg0.currentInputModule.input.mousePosition - mousePos;
            if (delta != Vector2.zero) mousePos = arg0.currentInputModule.input.mousePosition;
            Rotate(delta);
        }

        public void OnPointerUp(BaseEventData arg0)
        {
            mousePos = Vector2.zero;
        }
    }
}