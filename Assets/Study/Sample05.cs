using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Study
{

    public class Sample05 : MonoBehaviour
    {
        [SerializeField] ComputeShader compute = null;
        // CommandBuffer commandBuffer = null;

        void Start()
        {
            // commandBuffer = new CommandBuffer();

            ComputeBuffer buffer = new ComputeBuffer(4 * 4 * 2 * 2, sizeof(int));
            int kernel = compute.FindKernel("CSMain");

            compute.SetBuffer(kernel, "buffer", buffer);
            compute.Dispatch(kernel, 2, 2, 1);

            int[] data = new int[4 * 4 * 2 * 2];

            buffer.GetData(data);

            for (int i = 0; i < 8; i++)
            {
                string line = "";
                for (int j = 0; j < 8; j++)
                {
                    line += " " + data[j + i * 8];
                }
                Debug.Log(line);
            }
            buffer.Release();
        }

    }
}
