using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Script.KeyholeImage
{
    public class KeyholeProjector : MonoBehaviour
    {
        [SerializeField]private DecalProjector projector;
        [SerializeField]private Transform keyhole;
        [SerializeField]private Transform screen;
        [SerializeField]private Transform stuff;
        private Material decalMaterial;
        
        private Vector3 keyholeCenterPosition;//小孔中心位置
        private float detectionDistance;
        private int lightLayer;
        private static readonly int scaleFactorID = Shader.PropertyToID("_ScaleFactor");
        private RenderTexture texture;

        private ScreenSetting screenSetting;
        
        void Start()
        {
            screenSetting = new ScreenSetting(projector);//初始化投影器材质
            
            texture = Resources.Load<RenderTexture>("Environment/HoleImage");//投影图片
            
            lightLayer = LayerMask.GetMask("Light");
            
            detectionDistance = transform.GetComponent<Collider>().bounds.extents.z;

            KeyholePosition();
           
        }
        void Update()
        {
            if (IsMakeImage())
            {
                SetMaterialTexture(texture, scaleFactorID, GetScaleFactor());
            }
            else
            {
                ResetTexture();
            }
        }
        
        //设置小孔位置
        private void KeyholePosition()
        {
            Vector3 centerPosition = transform.GetComponent<Collider>().bounds.center;
            //float halfHeight = keyhole.GetComponent<Collider>().bounds.extents.y;
            keyhole.position = new Vector3(centerPosition.x, keyhole.position.y, centerPosition.z); //centerPosition + halfHeight * Vector3.up;
            Physics.SyncTransforms();//同步物理更新
            keyholeCenterPosition = keyhole.GetComponent<Collider>().bounds.center;
        }
        //是否投影
        private bool IsMakeImage()
        {
            bool result = Physics.Raycast(keyholeCenterPosition, Vector3.forward, out RaycastHit hit, detectionDistance, lightLayer);
            if (!result) return result;
            
            return hit.collider.gameObject.CompareTag("CandleBody") ? false : true;
        }
        
        //设置投影图片
        private void SetMaterialTexture(RenderTexture t,int scaleFactorID, Vector2 scaleFactor)
        {
            screenSetting.SetTexture(t, scaleFactorID, scaleFactor);
        }
        //计算投影图片比例
        private Vector2 GetScaleFactor()
        {
            float stuffPosition = Vector3.Distance(keyhole.position, stuff.position);
            float imagePosition = Vector3.Distance(keyhole.position, screen.position);
            float scaleFactor = stuffPosition / imagePosition;
            return new Vector2(scaleFactor,scaleFactor);
        }

        private void ResetTexture()
        {
            screenSetting.ResetTexture();
        }
        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(keyholeCenterPosition, Vector3.forward * detectionDistance);
        }
    }
}
