using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Study
{
    /// <summary>
    /// https://qiita.com/scnsh/items/20eafafe22204901a3f3#_reference-2c97dc15df14ce96ebb4
    /// </summary>
    public class Sample04 : MonoBehaviour
    {
        public ComputeShader shader;
        public ComputeShader shaderCopy;

        RenderTexture tex;
        RenderTexture texCopy;

        void Start()
        {
            tex = new RenderTexture(64, 64, 0);
            tex.enableRandomWrite = true;
            tex.Create();

            texCopy = new RenderTexture(64, 64, 0);
            texCopy.enableRandomWrite = true;
            texCopy.Create();

            shader.SetTexture(0, "tex", tex);
            shader.Dispatch(0, tex.width / 8, tex.height / 8, 1);

            shaderCopy.SetTexture(0, "tex", tex);
            shaderCopy.SetTexture(0, "texCopy", texCopy);
            shaderCopy.Dispatch(0, texCopy.width / 8, texCopy.height / 8, 1);
        }

        void OnGUI()
        {
            int w = Screen.width / 2;
            int h = Screen.height / 2;
            int s = 512;

            GUI.DrawTexture(new Rect(w - s / 2, h - s / 2, s, s), texCopy);
        }

        void OnDestroy()
        {
            tex.Release();
            texCopy.Release();
        }
    }

}
