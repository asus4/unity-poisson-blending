using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Study
{
    public class Sample06 : MonoBehaviour
    {
        [SerializeField] Texture2D source = null;
        [SerializeField] ComputeShader compute = null;

        RenderTexture tex;

        void Start()
        {
            tex = new RenderTexture(source.width, source.height, 0);
            tex.enableRandomWrite = true;
            tex.Create();

            int kernel = compute.FindKernel("CSMain");

            uint3 groups = compute.GetThreadGroupSize(kernel);
            Debug.Assert(source.width % groups.x == 0);
            Debug.Assert(source.height % groups.y == 0);

            compute.SetTexture(kernel, "Source", source, 0);
            compute.SetTexture(kernel, "Result", tex);

            compute.Dispatch(kernel, tex.width / (int)groups.x, tex.height / (int)groups.y, 1);
        }

        void OnDestroy()
        {
            tex.Release();
        }

        void OnGUI()
        {
            int w = Screen.width / 2;
            int h = Screen.height / 2;
            int s = 512;

            GUI.DrawTexture(new Rect(w - s / 2, h - s / 2, s, s), tex);
        }

    }
}
