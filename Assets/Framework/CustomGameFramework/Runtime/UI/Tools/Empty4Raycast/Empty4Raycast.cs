//
//  Empty4Raycast.cs 
//
//  Author: HGT 
//
//  Copyright (c) 2021 hegametech.com 
//

using UnityEngine;
using UnityEngine.UI;

namespace CustomGameFramework.Runtime
{
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(Button))]
    public class Empty4Raycast : MaskableGraphic
    {
        protected Empty4Raycast()
        {
            useLegacyMeshGeneration = false;
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            toFill.Clear();
        }
    }
}