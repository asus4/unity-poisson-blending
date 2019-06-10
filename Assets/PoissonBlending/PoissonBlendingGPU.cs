using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

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
            Debug.Assert(source.width == mask.width && source.width == target.width);
            Debug.Assert(source.height == mask.height && source.height == target.height);

            tex = new RenderTexture(source.width, source.height, 0);
            tex.enableRandomWrite = true;
            tex.Create();

            int kernal = compute.FindKernel("PoissonBlending");
            uint3 threads = compute.GetThreadGroupSize(kernal);
            Debug.Assert(source.width % threads.x == 0);
            Debug.Assert(source.height % threads.y == 0);

            compute.SetTexture(kernal, "Source", source);
            compute.SetTexture(kernal, "Mask", mask);
            compute.SetTexture(kernal, "Target", target);
            compute.SetTexture(kernal, "Result", tex);
            compute.Dispatch(kernal, source.width / (int)threads.x, source.height / (int)threads.y, 1);
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
