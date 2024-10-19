using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MG_BlocksEngine2.Block;

namespace MG_BlocksEngine2.DragDrop
{
    public interface I_BE2_Raycaster
    {
        /// <summary>
        /// Adds a canvas raycaster to the list of canvases to be looked up when parforming the BE2 pointer actions
        /// </summary>
        GraphicRaycaster[] AddRaycaster(GraphicRaycaster raycaster);

        /// <summary>
        /// Removes a canvas raycaster from the list making the BE2 actions, as drag and drop, not be performed on the regarding canvas 
        /// </summary>
        /// <param name="raycaster"></param>
        /// <returns></returns>
        GraphicRaycaster[] RemoveRaycaster(GraphicRaycaster raycaster);

        /// <summary>
        /// Returns the first draggable component (blocks by default) at the position 
        /// </summary>
        I_BE2_Drag GetDragAtPosition(Vector2 position);

        /// <summary>
        /// Returns the first spot component (used to place draggable components at) at the position
        /// </summary>
        I_BE2_Spot GetSpotAtPosition(Vector3 position);

        /// <summary>
        /// Returns the first spot component of an specific type (used to place draggable components at) that is closer to the given draggable and inside the range
        /// </summary>
        I_BE2_Spot FindClosestSpotOfType<T>(I_BE2_Drag drag, float maxDistance);

        /// <summary>
        /// Returns the first spot component of types BE2_SpotBlockBody or BE2_SpotOuterArea (used to place draggable components at) that is closer to the given draggable and inside the range
        /// </summary>
        I_BE2_Spot FindClosestSpotForBlock(I_BE2_Drag drag, float maxDistance);
    }
}