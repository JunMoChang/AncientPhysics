using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Script.KeyholeImage
{
    public class ScreenSetting
    {
        private Material decalMaterial;//Decal Projector的材质
        private readonly int textureName = Shader.PropertyToID("Base_Map");//投影图片

        public  ScreenSetting(DecalProjector projector)
        {
            ScreenInitialize(projector);
        }
        
        private void ScreenInitialize(DecalProjector projector)
        {
            decalMaterial = projector.material != null ? new Material(projector.material) : new Material(Shader.Find("Shader Graphs/MyDecalShader"));
            decalMaterial.name = "KeyholeImage";
            decalMaterial.SetTexture(textureName,null);
            projector.material = decalMaterial;
        }

        public void SetTexture(RenderTexture texture,int scaleFactorID, Vector2 scaleFactor)
        {
            decalMaterial.SetTexture(textureName,texture);
            if (texture)
            {
                decalMaterial.SetVector(scaleFactorID,scaleFactor);
            }

        }

        public void ResetTexture()
        {
            decalMaterial.SetTexture(textureName,null);
        }
    }
}