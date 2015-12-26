﻿/****************************************************************************
Copyright (c) 2015 Lingjijian

Created by Lingjijian on 2015

342854406@qq.com
http://www.cocos2d-x.org

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
****************************************************************************/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

namespace Lui
{
    /// <summary>
    /// 摇杆
    /// </summary>
    public class LControlView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        protected const float MOVE_DELAY = 0.5f;
        protected const int PARAM_PRE = 10;

        public Vector2 centerPoint;
        public int radius;
        public bool relocateWithAnimation;
        public GameObject joyStick;
        private Vector2 lastPoint;
        private UnityAction<float, float> onControlHandler;
        private Rect joyStickBoundBox;

        public LControlView()
        {
            this.radius = 100;
            this.centerPoint = Vector2.zero;
            this.lastPoint = Vector2.zero;
            this.relocateWithAnimation = true;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            stopAnimateUpdate();
            Vector2 point = transform.InverseTransformPoint(eventData.position);
            if (joyStick)
            {
                if (eventData.pointerEnter == joyStick)
                {
                    onExecuteEventHandle();
                }
            }
            else
            {
                lastPoint = point;
                onExecuteEventHandle();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (joyStick)
            {
                Vector2 point = transform.InverseTransformPoint(eventData.position);
                float dis = Vector3.Distance(centerPoint, point);
                joyStick.transform.localPosition = dis < radius ? point : new Vector2(
                    ((point.x - centerPoint.x) / dis) * radius + centerPoint.x,
                    ((point.y - centerPoint.y) / dis) * radius + centerPoint.y);
            }
            else
            {
                Vector2 point = transform.InverseTransformPoint(eventData.position);
                float dis = Vector3.Distance(centerPoint, point);
                lastPoint = dis < radius ? point : new Vector2(
                    ((point.x - centerPoint.x) / dis) * radius + centerPoint.x,
                    ((point.y - centerPoint.y) / dis) * radius + centerPoint.y);
            }

            onExecuteEventHandle();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (joyStick)
            {
                if (!relocateWithAnimation)
                {
                    onExecuteEventHandle();
                }
                relocateJoystick(relocateWithAnimation);
            }
            else
            {
                onExecuteEventHandle();
            }
        }

        public void setControlHandler(UnityAction<float, float> act)
        {
            onControlHandler = act;
        }

        protected void relocateJoystick(bool anim)
        {
            if (anim)
            {
                iTween.MoveTo(joyStick, iTween.Hash(
                    "position", transform.TransformPoint(centerPoint),
                    "time", MOVE_DELAY,
                    "onupdate", "onExecuteEventHandle",
                    "onupdatetarget", gameObject));
            }
            else
            {
                joyStick.transform.localPosition = centerPoint;
            }
        }

        protected void stopAnimateUpdate()
        {
            if (joyStick)
            {
                iTween.Stop(joyStick);
            }
        }

        void onExecuteEventHandle()
        {
            if (onControlHandler == null)
            {
                return;
            }

            if (joyStick)
            {
                Vector2 v = joyStick.transform.localPosition;
                Vector2 offset = v - centerPoint;
                onControlHandler.Invoke(offset.x / PARAM_PRE, offset.y / PARAM_PRE);
            }
            else
            {
                Vector2 offset = lastPoint - centerPoint;
                onControlHandler.Invoke(offset.x / PARAM_PRE, offset.y / PARAM_PRE);
            }
        }

        void Start()
        {
            setControlHandler((float x, float y) =>
            {

                //Debug.Log(" offset x,y " + x + " " + y);
            });
        }
    }

}