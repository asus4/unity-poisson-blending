using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Study
{
    public class Sample03 : MonoBehaviour
    {
        public ComputeShader shader;

        RenderTexture tex;

        void Start()
        {
            tex = new RenderTexture(64, 64, 0);
            tex.enableRandomWrite = true;
            tex.Create();

            shader.SetFloat("w", tex.width);
            shader.SetFloat("h", tex.height);
            shader.SetTexture(0, "tex", tex);
            shader.Dispatch(0, tex.width / 8, tex.height / 8, 1);
        }

        void OnGUI()
        {
            int w = Screen.width / 2;
            int h = Screen.height / 2;
            int s = 512;

            GUI.DrawTexture(new Rect(w - s / 2, h - s / 2, s, s), tex);
        }

        void OnDestroy()
        {
            tex.Release();
        }
    }
}