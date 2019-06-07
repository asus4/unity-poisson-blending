using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PoissonBlending
{

    public class PoissonBlendingGPU : MonoBehaviour
    {
        [SerializeField] Texture2D source = null;
        [SerializeField] Texture2D mask = null;
        [SerializeField] Texture2D target = null;
        [SerializeField] ComputeShader compute = null;



        IEnumerator Start()
        {
            // Wait for warming up
            yield return new WaitForSeconds(1.0f);

            int kernalMakeMask = compute.FindKernel("MakeMask");

            compute.SetTexture(kernalMakeMask, "Result", target);
            Debug.Log($"compute: {compute}, kernel: {kernalMakeMask}");
        }
    }
}
