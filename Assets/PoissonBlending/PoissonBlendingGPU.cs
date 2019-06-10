using System;
using System.Diagnostics;
using UnityEngine;
using Unity.Mathematics;
using Debug = UnityEngine.Debug;


namespace PoissonBlending
{

    public class PoissonBlendingGPU : MonoBehaviour
    {
        [SerializeField] Texture2D source = null;
        [SerializeField] Texture2D mask = null;
        [SerializeField] Texture2D target = null;
        [SerializeField] ComputeShader compute = null;

        RenderTexture tex;
        int kernel;
        uint3 threads;

        void Start()
        {
            Debug.Assert(source.width == mask.width && source.width == target.width);
            Debug.Assert(source.height == mask.height && source.height == target.height);

            tex = new RenderTexture(source.width, source.height, 0);
            tex.enableRandomWrite = true;
            tex.Create();

            kernel = compute.FindKernel("PoissonBlending");
            threads = compute.GetThreadGroupSize(kernel);
            Debug.Assert(source.width % threads.x == 0);
            Debug.Assert(source.height % threads.y == 0);

        }

        void OnGUI()
        {
            int w = Screen.width / 2;
            int h = Screen.height / 2;

            GUI.DrawTexture(new Rect(0, 0, w, h), source);
            GUI.DrawTexture(new Rect(w, 0, w, h), mask);
            GUI.DrawTexture(new Rect(0, h, w, h), target);
            GUI.DrawTexture(new Rect(w, h, w, h), tex);
        }

        void Update()
        {
            var sw = Stopwatch.StartNew();

            compute.SetTexture(kernel, "Source", source);
            compute.SetTexture(kernel, "Mask", mask);
            compute.SetTexture(kernel, "Target", target);
            compute.SetTexture(kernel, "Result", tex);
            compute.Dispatch(kernel, source.width / (int)threads.x, source.height / (int)threads.y, 1);

            sw.Stop();
            double d = (double)sw.ElapsedTicks / (double)TimeSpan.TicksPerMillisecond;
            Debug.Log($"GPU: {d} ms");
        }

        void OnDestroy()
        {
            tex.Release();
        }


    }
}
