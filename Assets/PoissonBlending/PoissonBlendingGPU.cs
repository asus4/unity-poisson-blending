﻿using System;
using System.Collections.Generic;
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

            int initKernel = compute.FindKernel("Init");
            compute.SetTexture(initKernel, "Source", source);
            compute.SetTexture(initKernel, "Mask", mask);
            compute.SetTexture(initKernel, "Target", target);
            compute.SetTexture(initKernel, "Result", tex);
            compute.Dispatch(initKernel, source.width / (int)threads.x, source.height / (int)threads.y, 1);

        }

        void OnGUI()
        {
            int w = Screen.width / 2;
            int h = Screen.height / 2;
            int s = 512;

            GUI.DrawTexture(new Rect(w - s / 2, h - s / 2, s, s), tex);
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
            Debug.Log($"GetMask: {d} ms");
        }

        void OnDestroy()
        {
            tex.Release();
        }


    }
}
