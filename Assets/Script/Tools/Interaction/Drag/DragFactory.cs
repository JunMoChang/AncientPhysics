using System;
using Script.Tools.Interface;
using UnityEngine;
using Zenject;

namespace Script.Tools.Interaction.Drag
{
    
    public class DragFactory : IDragFactory
    {
        public DiContainer container;
        private float maxMass = 20f;
        public IDragInteraction CreateDrag(Transform transform, int obstacleLayer)
        {
            transform.TryGetComponent(out Rigidbody rb);
            
            if (rb != null && !rb.isKinematic)
            {
                //container.Instantiate<DragOfFree>();
                return rb.mass < maxMass ? new DragOfFree(transform, obstacleLayer) : new DragOfLimitY(transform);
            }
            
            return new DragOfLimitXY(transform);
        }
        
    }
}