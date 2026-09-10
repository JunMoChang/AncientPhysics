using Obi;
using Script.Tools.Managers;
using UnityEngine;

namespace Script.Pulley
{
    public class RopeController : MonoBehaviour
    {
        [SerializeField]private ObiRope mainRope;
        ObiSolver solver;

        void Start()
        {
            solver = mainRope.solver;
        }
        private void ResetMianRopeState(GameObject obj)
        {
            int activeParticleCount = mainRope.activeParticleCount;
            for (int i = 0; i < activeParticleCount; i++)
            {
                solver.velocities[i] = Vector3.zero;
                solver.positions[i] = solver.renderablePositions[i];
            }
            
            mainRope.UpdateParticleProperties();
            //solver.UpdateBackend();
        }

        void OnEnable()
        {
            EventsManager.OnRayTest += ResetMianRopeState;
        }

        void OnDisable()
        {
            EventsManager.OnRayTest -= ResetMianRopeState;
        }
        //rope.solver.positions[index]粒子相对ObiSolver的位置
        // QueryResult
        /*public Vector4 simplexBary;
        public Vector4 queryPoint;     hit.queryPoint粒子在世界中的位置
        public Vector4 normal;
        public float distance;
        public int simplexIndex;       hit.simplexIndex在ObiSolver中索引
        public int queryIndex;*/
    }
}
