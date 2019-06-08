using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PoissonBlending
{

    public class PoissonBlendingGPU : MonoBehaviour
    {
        [SerializeField] Texture2D source = null;
        [SerializeField] Texture2D mask = null;
        [SerializeField] Texture2D target = null;
        [SerializeField] ComputeShader compute = null;

        RenderTexture tex;


        void Start()
        {
            tex = new RenderTexture(64, 64, 0);
            tex.enableRandomWrite = true;
            tex.Create();

            int kernal = compute.FindKernel("MakeMask");

            compute.SetTexture(kernal, "Result", tex);
            compute.Dispatch(kernal, 4, 4, 1);
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
