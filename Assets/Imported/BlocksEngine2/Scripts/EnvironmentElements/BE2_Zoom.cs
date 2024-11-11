using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MG_BlocksEngine2.Core;
using MG_BlocksEngine2.DragDrop;
using MG_BlocksEngine2.EditorScript;
using MG_BlocksEngine2.Block;

namespace MG_BlocksEngine2.Environment
{
    // v2.10 - Added zoom feature to the ProgrammingEnv with the BE2_Zoom component
    public class BE2_Zoom : MonoBehaviour
    {
        public float startSize = 1;
        public float minSize = 0.1f;
        public float maxSize = 7;
        public float zoomRate = 1.3f;

        Vector3 unscaledMousePositin;
        I_BE2_ProgrammingEnv _programmingEnv;

        void OnEnable()
        {
            BE2_ExecutionManager.Instance.AddToLateUpdate(HandleZoom);

            BE2_MainEventsManager.Instance.StartListening(BE2EventTypes.OnPrimaryKeyDown, SetPrimaryKeyDown);
            BE2_MainEventsManager.Instance.StartListening(BE2EventTypes.OnPrimaryKeyUp, SetPrimaryKeyUp);
            BE2_MainEventsManager.Instance.StartListening(BE2EventTypes.OnAuxKeyDown, SetAuxKeyDown);
            BE2_MainEventsManager.Instance.StartListening(BE2EventTypes.OnAuxKeyUp, SetAuxKeyUp);

            _programmingEnv = GetComponent<I_BE2_ProgrammingEnv>();
        }

        void OnDisable()
        {
            // v2.10.2 - bugfix: BE2_ExecutionManager.Instance being null before it is called inside OnDisable when scene is closed
            BE2_ExecutionManager.Instance?.RemoveFromLateUpdate(HandleZoom);

            BE2_MainEventsManager.Instance.StopListening(BE2EventTypes.OnPrimaryKeyDown, SetPrimaryKeyDown);
            BE2_MainEventsManager.Instance.StopListening(BE2EventTypes.OnPrimaryKeyUp, SetPrimaryKeyUp);
            BE2_MainEventsManager.Instance.StopListening(BE2EventTypes.OnAuxKeyDown, SetAuxKeyDown);
            BE2_MainEventsManager.Instance.StopListening(BE2EventTypes.OnAuxKeyUp, SetAuxKeyUp);
        }

        bool primaryKey;
        bool auxKey;
        void SetPrimaryKeyDown() { primaryKey = true; }
        void SetPrimaryKeyUp() { primaryKey = false; }
        void SetAuxKeyDown() { auxKey = true; }
        void SetAuxKeyUp() { auxKey = false; }

        public void HandleZoom()
        {
            float scrollWheel = -Input.GetAxis("Mouse ScrollWheel");

            if (scrollWheel != 0)
            {
                if (!primaryKey && auxKey)
                    Zoom(scrollWheel, BE2_InputManager.Instance.CanvasPointerPosition);
            }
        }

        public void Zoom(float scrollWheel, Vector3 zoomAnchorPosition)
        {
            RectTransform rt = transform as RectTransform;

            Vector3[] vstart = new Vector3[4];
            rt.GetWorldCorners(vstart);

            if (scrollWheel > 0 && transform.localScale.y > minSize)
            {
                float targetSize = Mathf.Clamp(transform.localScale.y / zoomRate, minSize, maxSize);
                transform.localScale = new Vector3(targetSize, targetSize, 1);
            }
            else if (scrollWheel < 0 && transform.localScale.y < maxSize)
            {
                float targetSize = Mathf.Clamp(transform.localScale.y * zoomRate, minSize, maxSize);
                transform.localScale = new Vector3(targetSize, targetSize, 1);
            }

            unscaledMousePositin = zoomAnchorPosition;

            Vector3[] vend = new Vector3[4];
            rt.GetWorldCorners(vend);
            Vector3 scaledMousePosition = new Vector3(
                ConvertValueToNewRange(unscaledMousePositin.x, new Vector2(vstart[1].x, vstart[2].x), new Vector2(vend[1].x, vend[2].x)),
                ConvertValueToNewRange(unscaledMousePositin.y, new Vector2(vstart[1].y, vstart[0].y), new Vector2(vend[1].y, vend[0].y)),
                unscaledMousePositin.z
            );

            TranslateView(-(scaledMousePosition.x - unscaledMousePositin.x),
                        -(scaledMousePosition.y - unscaledMousePositin.y));
        }

        public void ZoomIn()
        {
            RectTransform rt = transform.parent as RectTransform;
            Vector3[] vstart = new Vector3[4];
            rt.GetWorldCorners(vstart);

            Zoom(-1, new Vector3((vstart[1].x + vstart[2].x) / 2, (vstart[1].y + vstart[0].y) / 2, transform.position.z));
        }

        public void ZoomOut()
        {
            RectTransform rt = transform.parent as RectTransform;
            Vector3[] vstart = new Vector3[4];
            rt.GetWorldCorners(vstart);

            Zoom(1, new Vector3((vstart[1].x + vstart[2].x) / 2, (vstart[1].y + vstart[0].y) / 2, transform.position.z));
        }

        public void ZoomCenter()
        {
            Vector3 position = Vector3.zero;
            _programmingEnv.UpdateBlocksList();
            transform.localScale = new Vector3(1, 1, 1);

            // v2.10.1 - bugfix: error when centering zoom without blocks in the ProgrammingEnv
            if (_programmingEnv.BlocksList.Count > 0)
            {
                foreach (I_BE2_Block block in _programmingEnv.BlocksList)
                {
                    RectTransform rtblock = block.Transform as RectTransform;
                    Vector3[] vstartblock = new Vector3[4];
                    rtblock.GetWorldCorners(vstartblock);


                    position += new Vector3((vstartblock[1].x + vstartblock[2].x) / 2, (vstartblock[1].y + vstartblock[0].y) / 2, block.Transform.position.z);
                }
                position /= _programmingEnv.BlocksList.Count;

                RectTransform rt = transform.parent as RectTransform;
                Vector3[] vstart = new Vector3[4];
                rt.GetWorldCorners(vstart);

                TranslateView(-(position.x - (vstart[1].x + vstart[2].x) / 2),
                            -(position.y - (vstart[1].y + vstart[0].y) / 2));
            }
        }

        void TranslateView(float x, float y)
        {
            Vector3 pos = transform.position;
            pos.x += x;
            pos.y += y;
            transform.position = pos;
        }

        float ConvertValueToNewRange(float oldValue, Vector2 oldScale, Vector2 newScale)
        {
            return (((oldValue - oldScale.y) * (newScale.x - newScale.y)) / (oldScale.x - oldScale.y)) + newScale.y;
        }
    }
}
