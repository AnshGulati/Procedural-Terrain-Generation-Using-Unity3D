using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    public Mesh mesh;
    public Vector3[] vertices;
    public int[] triangles;
    public Color[] colors;
    public Gradient gradient;

    public int xsize = 20;
    public int zsize = 20;

    float minTerrainHeight;
    float maxTerrainHeight;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        updateMesh();
    }

    void CreateShape()
    {
        vertices = new Vector3[(xsize + 1) * (zsize + 1)];
        triangles = new int[xsize * zsize * 6];
        int vert = 0;
        int tris = 0;

        for (int i = 0, z = 0; z <= zsize; z++)
        {
            for (int x = 0; x <= xsize; x++)
            {
                float y = Mathf.PerlinNoise(x*.3f, z*.3f)*2f;
                vertices[i] = new Vector3(x, y, z);

                if (y < minTerrainHeight) minTerrainHeight = y;
                if (y > maxTerrainHeight) maxTerrainHeight = y;

                i++;
            }
        }

        for (int z = 0; z < zsize; z++)
        {
            for (int i = 0; i < xsize; i++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xsize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xsize + 1;
                triangles[tris + 5] = vert + xsize + 2;
                vert++;
                tris += 6;
            }
            vert++;
        }

        colors= new Color[vertices.Length];
        for (int i = 0,z = 0;z <= zsize; z++)
        {
            for(int x=0;x<=xsize;x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight,maxTerrainHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }
    }

    void updateMesh()
    {
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();
    }

    //private void OnDrawGizmos()
    //{
    //    if (vertices == null) return;

    //    for(int i = 0; i < vertices.Length; i++)
    //    {
    //        Gizmos.DrawSphere(vertices[i],0.1f);
    //    }
    //}
}
