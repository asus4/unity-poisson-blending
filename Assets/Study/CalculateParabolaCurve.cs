using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Study
{
    /// <summary>
    /// https://blog.yucchiy.com/2019/01/03/tutorial-for-unity-compute-shader/
    /// </summary>
    public class CalculateParabolaCurve : MonoBehaviour
    {
        public ComputeShader Shader;
        public float a = 2;
        public float p = 0;
        public float q = 0;
        public uint CurveLength = 32;

        ComputeBuffer Buffer;

        void Start()
        {
            var kernelIndex = Shader.FindKernel("CalculateParabolaCurve");

            Buffer = new ComputeBuffer((int)CurveLength, sizeof(float));
            Shader.SetBuffer(kernelIndex, "buffer", Buffer);

            Shader.SetFloat("a", a);
            Shader.SetFloat("p", p);
            Shader.SetFloat("q", q);

            uint sizeX, sizeY, sizeZ;
            Shader.GetKernelThreadGroupSizes(
                kernelIndex,
                out sizeX,
                out sizeY,
                out sizeZ
            );

            Shader.Dispatch(kernelIndex, (int)(CurveLength / sizeX), 1, 1);

            var result = new float[CurveLength];
            Buffer.GetData(result);
            foreach (var eachResult in result)
            {
                Debug.Log(eachResult);
            }
        }

        void OnDestroy()
        {
            Buffer.Release();
            Buffer = null;
        }

    }

}