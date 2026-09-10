using UnityEngine;

namespace Script.Tools.Interface
{
    interface IDragFactory
    {
        public IDragInteraction CreateDrag(Transform transform, int obstacleLayer);
    }
}