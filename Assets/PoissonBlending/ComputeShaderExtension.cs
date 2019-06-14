using UnityEngine;
using Unity.Mathematics;

namespace PoissonBlending
{
    public static class ComputeShaderExtension
    {
        public static uint3 GetThreadGroupSize(this ComputeShader compute, int kernelIndex)
        {
            uint x, y, z;
            compute.GetKernelThreadGroupSizes(kernelIndex, out x, out y, out z);
            return new uint3(x, y, z);
        }
    }
}