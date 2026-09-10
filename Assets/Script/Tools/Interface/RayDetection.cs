using UnityEngine;

namespace Script.Tools.Interface
{
    public interface IRayDetection
    {
        public void Raycast(float maxDistance, int layer);
    }
}