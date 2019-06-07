using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Study
{
    /// <summary>
    /// https://blog.yucchiy.com/2019/01/03/tutorial-for-unity-compute-shader/
    /// </summary>
    public class Sample01 : MonoBehaviour
    {
        public ComputeShader compute;
        public float a = 2;
        public float p = 0;
        public float q = 0;
        public uint curveLength = 32;

        ComputeBuffer buffer;

        void Start()
        {
            var kernelIndex = compute.FindKernel("CalculateParabolaCurve");

            buffer = new ComputeBuffer((int)curveLength, sizeof(float));
            compute.SetBuffer(kernelIndex, "buffer", buffer);

            compute.SetFloat("a", a);
            compute.SetFloat("p", p);
            compute.SetFloat("q", q);

            uint sizeX, sizeY, sizeZ;
            compute.GetKernelThreadGroupSizes(
                kernelIndex,
                out sizeX,
                out sizeY,
                out sizeZ
            );

            compute.Dispatch(kernelIndex, (int)(curveLength / sizeX), 1, 1);

            var result = new float[curveLength];
            buffer.GetData(result);
            foreach (var eachResult in result)
            {
                Debug.Log(eachResult);
            }
        }

        void OnDestroy()
        {
            buffer.Release();
            buffer = null;
        }

    }

}