using Script.Tools.Interface;
using UnityEngine;
using Zenject;

namespace Script.Tools.Interaction.Drag
{
    public class DragInstaller : MonoInstaller
    {
        private Vector3 offset;
        private float halfHigh;
        private int obstacleLayer;
        public override void InstallBindings()
        {
            Container.Bind<MouseWorldPosition>().AsSingle();
            Container.Bind<DragInitialSetting>().AsSingle();
            Container.Bind<IDragFactory>().To<DragFactory>().AsSingle();
            
            /*Container.Bind<Vector3>().WithId("offset").FromInstance(offset);
            Container.Bind<float>().WithId("halfHigh").FromInstance(halfHigh);
            Container.Bind<int>().WithId("obstacleLayer").FromInstance(obstacleLayer);*/
        }
    }
}