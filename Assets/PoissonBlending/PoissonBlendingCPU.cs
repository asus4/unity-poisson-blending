using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;
using Unity.Mathematics;
using Debug = UnityEngine.Debug;

namespace PoissonBlending
{

    public class PoissonBlendingCPU : MonoBehaviour
    {
        [SerializeField]
        Texture2D source;
        [SerializeField]
        Texture2D mask;
        [SerializeField]
        Texture2D target;

        [SerializeField]
        RawImage rawImage;


        IEnumerator Start()
        {
            Debug.Log($"Starting: {target.format}");

            var result = new Texture2D(target.width, target.height, target.format, 0, false);
            rawImage.texture = result;

            yield return null;

            var sw = Stopwatch.StartNew();
            var interior = GetMask(mask);
            sw.Stop();
            double d = (double)sw.ElapsedTicks / (double)TimeSpan.TicksPerMillisecond;
            Debug.Log($"GetMask: {d} ms");

            var arr = target.GetPixels();
            result.SetPixels(arr);
            result.Apply();
        }

        void OnDestroy()
        {
            Destroy(rawImage.texture);
        }


        uint2[] GetMask(Texture2D maskTex)
        {
            var mask = new List<uint2>();

            var buffer = maskTex.GetPixels();
            int width = maskTex.width;
            int height = maskTex.height;

            for (uint y = 0; y < height; y++)
            {
                for (uint x = 0; x < width; x++)
                {
                    if (buffer[width * y + x].r > 0)
                    {
                        mask.Add(new uint2(x, y));
                    }
                }
            }

            var interior = new List<uint2>();
            foreach (var idx in mask)
            {
                if (mask.Contains(new uint2(idx.x, idx.y + 1))
                    && mask.Contains(new uint2(idx.x, idx.y - 1))
                    && mask.Contains(new uint2(idx.x + 1, idx.y))
                    && mask.Contains(new uint2(idx.x - 1, idx.y)))
                {
                    interior.Add(idx);
                }
            }

            return interior.ToArray();
        }

    }
}