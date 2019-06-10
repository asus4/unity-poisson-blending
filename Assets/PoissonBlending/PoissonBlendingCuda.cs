using System;
using System.Diagnostics;
using UnityEngine;
using Unity.Mathematics;
using Debug = UnityEngine.Debug;


namespace PoissonBlending
{
    public class PoissonBlendingCuda : MonoBehaviour
    {
        [SerializeField] Texture2D source = null;
        [SerializeField] Texture2D mask = null;
        [SerializeField] Texture2D target = null;
        [SerializeField] ComputeShader compute = null;

        RenderTexture result;
        RenderTexture border;
        int kernelBorder;
        int kernelBlend;
        int3 tGroups;

        void Start()
        {
            Debug.Assert(source.width == mask.width && source.width == target.width);
            Debug.Assert(source.height == mask.height && source.height == target.height);

            result = new RenderTexture(source.width, source.height, 0);
            result.enableRandomWrite = true;
            result.Create();

            border = new RenderTexture(source.width, source.height, 0);
            border.enableRandomWrite = true;
            border.Create();

            kernelBorder = compute.FindKernel("MakeBorder");
            kernelBlend = compute.FindKernel("PoissonBlending");
            uint3 threads = compute.GetThreadGroupSize(kernelBlend);
            Debug.Assert(source.width % threads.x == 0);
            Debug.Assert(source.height % threads.y == 0);

            tGroups = new int3(source.width / (int)threads.x, source.height / (int)threads.y, 1);
        }

        void OnGUI()
        {
            int w = Screen.width / 2;
            int h = Screen.height / 2;

            GUI.DrawTexture(new Rect(0, 0, w, h), border);
            GUI.DrawTexture(new Rect(w, 0, w, h), mask);
            GUI.DrawTexture(new Rect(0, h, w, h), target);
            GUI.DrawTexture(new Rect(w, h, w, h), result);
        }

        void Update()
        {
            var sw = Stopwatch.StartNew();

            compute.SetTexture(kernelBorder, "Mask", mask);
            compute.SetTexture(kernelBorder, "Border", border);
            compute.Dispatch(kernelBorder, tGroups.x, tGroups.y, tGroups.z);

            compute.SetTexture(kernelBlend, "Source", source);
            compute.SetTexture(kernelBlend, "Mask", mask);
            compute.SetTexture(kernelBlend, "Target", target);
            compute.SetTexture(kernelBlend, "Result", result);
            compute.Dispatch(kernelBlend, tGroups.x, tGroups.y, tGroups.z);

            sw.Stop();
            double d = (double)sw.ElapsedTicks / TimeSpan.TicksPerMillisecond;
            Debug.Log($"Cuda: {d} ms");
        }

        void OnDestroy()
        {
            result.Release();
            border.Release();
        }
    }
}