using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureScale : MonoBehaviour
{
    [SerializeField] private float tileX = 1;
    [SerializeField] private float tileY = 1;
    private Mesh mesh;
    private Material mat;

    private void Start()
    {
        mat = GetComponent<Renderer>().material;
        mesh = GetComponent<MeshFilter>().mesh;
        mat.mainTextureScale = new Vector2((mesh.bounds.size.x *
transform.localScale.x) / 100 * tileX, (mesh.bounds.size.y * transform.localScale.y) / 100 * tileY);
    }
}