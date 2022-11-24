using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PixelArtPostProcess : MonoBehaviour
{
    public Material Material;
    
    private void OnRenderImage (RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit (src, dest, this.Material);
    }
}
