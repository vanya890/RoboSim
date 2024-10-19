using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using MG_BlocksEngine2.Block;
using MG_BlocksEngine2.EditorScript;
using MG_BlocksEngine2.Utils;
using MG_BlocksEngine2.Environment;

namespace MG_BlocksEngine2.DragDrop
{
    public class BE2_Raycaster : MonoBehaviour, I_BE2_Raycaster
    {
        BE2_DragDropManager _dragDropManager;
        PointerEventData _pointerEventData;

        [SerializeField]
        GraphicRaycaster[] raycasters;

        [SerializeField]
        EventSystem eventSystem;

        void Awake()
        {
            _dragDropManager = GetComponent<BE2_DragDropManager>();
        }

        //void Start()
        //{
        //
        //}

        //void Update()
        //{
        //    
        //}

        // v2.4 - added methods to add/remove raycasters from the BE2 Raycaster component
        public GraphicRaycaster[] AddRaycaster(GraphicRaycaster raycaster = null)
        {
            if (raycaster != null)
            {
                if (BE2_ArrayUtils.Find(ref raycasters, (x => x == raycaster)) == null)
                {
                    BE2_ArrayUtils.Add(ref raycasters, raycaster);
                }
            }

            return raycasters;
        }
        public GraphicRaycaster[] RemoveRaycaster(GraphicRaycaster raycaster)
        {
            if (BE2_ArrayUtils.Find(ref raycasters, (x => x == raycaster)) != null)
            {
                BE2_ArrayUtils.Remove(ref raycasters, raycaster);
            }

            return raycasters;
        }

        public I_BE2_Drag GetDragAtPosition(Vector2 position)
        {
            _pointerEventData = new PointerEventData(eventSystem);

            // v2.6 - Raycaster ray position adjusted base on the Canvas render mode
            if (BE2_Inspector.Instance.CanvasRenderMode == RenderMode.ScreenSpaceOverlay)
            {
                _pointerEventData.position = position;
            }
            else
            {
                _pointerEventData.position = BE2_Inspector.Instance.Camera.WorldToScreenPoint(BE2_Pointer.Instance.transform.position);
            }

            List<RaycastResult> globalResults = new List<RaycastResult>();
            int rayCount = raycasters.Length;
            for (int i = 0; i < rayCount; i++)
            {
                List<RaycastResult> results = new List<RaycastResult>();
                raycasters[i].Raycast(_pointerEventData, results);
                globalResults.AddRange(results);
            }

            int resultCount = globalResults.Count;
            for (int i = 0; i < resultCount; i++)
            {
                GameObject resultGameObject = globalResults[i].gameObject;

                I_BE2_Drag drag = resultGameObject.GetComponentInParent<I_BE2_Drag>();
                if (drag != null)
                {
                    return drag;
                }
            }

            return null;
        }

        public I_BE2_Spot GetSpotAtPosition(Vector3 position)
        {
            _pointerEventData = new PointerEventData(eventSystem);

            // v2.6 - Raycaster ray position adjusted base on the Canvas render mode
            if (BE2_Inspector.Instance.CanvasRenderMode == RenderMode.ScreenSpaceOverlay)
            {
                _pointerEventData.position = position;
            }
            else
            {
                _pointerEventData.position = BE2_Inspector.Instance.Camera.WorldToScreenPoint(BE2_Pointer.Instance.transform.position);
            }

            List<RaycastResult> globalResults = new List<RaycastResult>();
            int rayCount = raycasters.Length;
            for (int i = 0; i < rayCount; i++)
            {
                List<RaycastResult> results = new List<RaycastResult>();
                raycasters[i].Raycast(_pointerEventData, results);
                globalResults.AddRange(results);
            }

            int resultCount = globalResults.Count;
            for (int i = 0; i < resultCount; i++)
            {
                RaycastResult result = globalResults[i];
                if (result.gameObject.activeSelf)
                {
                    I_BE2_Spot spot = result.gameObject.GetComponent<I_BE2_Spot>();
                    if (spot != null)
                    {
                        return spot;
                    }
                }
            }

            return null;
        }

        public I_BE2_Spot FindClosestSpotOfType<T>(I_BE2_Drag drag, float maxDistance)
        {
            float minDistance = Mathf.Infinity;
            I_BE2_Spot foundSpot = null;
            int spotsCount = _dragDropManager.SpotsList.Count;
            for (int i = 0; i < spotsCount; i++)
            {
                I_BE2_Spot spot = _dragDropManager.SpotsList[i];
                if (spot is T && spot.Transform.gameObject.activeSelf)
                {
                    I_BE2_Drag d = spot.Transform.GetComponentInParent<I_BE2_Drag>();

                    // v2.4 - added programming env check to the BE2_Raycaster to verify if the block is placed in a visible or hidden environment 
                    I_BE2_ProgrammingEnv programmingEnv = d.Transform.GetComponentInParent<BE2_ProgrammingEnv>();

                    if (d != drag && programmingEnv.Visible)
                    {
                        float distance = Vector2.Distance(drag.RayPoint, spot.DropPosition);
                        if (distance < minDistance && distance <= maxDistance)
                        {
                            foundSpot = spot;
                            minDistance = distance;
                        }
                    }
                }
            }

            return foundSpot;
        }

        public I_BE2_Spot FindClosestSpotForBlock(I_BE2_Drag drag, float maxDistance)
        {
            float minDistance = Mathf.Infinity;
            I_BE2_Spot foundSpot = null;
            int spotsCount = _dragDropManager.SpotsList.Count;
            for (int i = 0; i < spotsCount; i++)
            {
                I_BE2_Spot spot = _dragDropManager.SpotsList[i];

                if ((spot is BE2_SpotBlockBody || (spot is BE2_SpotOuterArea && spot.Block.ParentSection != null)) && spot.Transform.gameObject.activeSelf)
                {
                    I_BE2_Drag d = spot.Transform.GetComponentInParent<I_BE2_Drag>();

                    // v2.4 - added programming env check to the BE2_Raycaster to verify if the block is placed in a visible or hidden environment 
                    I_BE2_ProgrammingEnv programmingEnv = d.Transform.GetComponentInParent<BE2_ProgrammingEnv>();

                    // v2.5 - bugfix: fixed raycast bug that locked movement of blocks 
                    if (d != drag && programmingEnv != null && programmingEnv.Visible)
                    {
                        float distance = Vector2.Distance(drag.RayPoint, spot.DropPosition);
                        if (distance < minDistance && distance <= maxDistance)
                        {
                            foundSpot = spot;
                            minDistance = distance;
                        }
                    }
                }
            }

            return foundSpot;
        }
    }
}