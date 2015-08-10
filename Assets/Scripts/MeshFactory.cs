using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class MeshFactory
{

    private const int SEGMENTS = 64;
    private const int MARGIN_PIXELS = 2;

    static readonly Vector2[] _vertices;
    static readonly int[] _triangles;
    static readonly Vector3[] _normals;

    private static Vector3[] _vertexBuffer;
    private static Vector2[] _uvBuffer;

    //static readonly Dictionary<float, Mesh> _meshes = new Dictionary<float, Mesh>();

    static MeshFactory()
    {
        var count = SEGMENTS * 2;

        _vertices = new Vector2[SEGMENTS];
        _triangles = new int[count * 3];

        var segmentAngle = 2 * Mathf.PI / SEGMENTS;

        var normal = Vector3.back;
        _normals = Enumerable.Repeat(normal, count).ToArray();

        for (var i = 0; i < SEGMENTS; i++)
        {
            var angle = segmentAngle * i;
            _vertices[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            var j = i + SEGMENTS;
            var ti = i * 6;
            var i1 = (i + 1) % SEGMENTS;
            var j1 = i1 + SEGMENTS;

            _triangles[ti] = i1;
            _triangles[ti + 1] = i;
            _triangles[ti + 2] = j1;

            _triangles[ti + 3] = j1;
            _triangles[ti + 4] = i;
            _triangles[ti + 5] = j;
        }


        _vertexBuffer = new Vector3[count];
        _uvBuffer = new Vector2[count];
    }

    //public static Mesh GetRingMesh(float desiredRadius, float thickness, ref Mesh mesh, bool useCache = true)
    public static Mesh GetRingMesh(float desiredRadius, float thickness, ref Mesh mesh)
    {
        /*
        useCache &= mesh == null;

        if (useCache)
        {
            _meshes.TryGetValue(desiredRadius, out mesh);
            if (mesh != null) return mesh;
        }
         * */

        var margin = MARGIN_PIXELS * GameManager.Instance.UnitsPerPixel;

        var segments = SEGMENTS;
        var radius = desiredRadius + margin;
        var innerRadius = desiredRadius - thickness - margin;

        var count = segments * 2;

        var vertices = _vertexBuffer;
        var uvs = _uvBuffer;

        var qo = desiredRadius / radius;
        var qi = 1 - (thickness + margin) / radius / qo;
        var uvOffset = new Vector2(0.5f, 0.5f);

        for (var i = 0; i < segments; i++)
        {
            var j = i + SEGMENTS;

            vertices[i] = _vertices[i] * radius;
            vertices[j] = _vertices[i] * innerRadius;

            uvs[i] = (_vertices[i] / qo) / 2 + uvOffset;
            uvs[j] = (_vertices[i] * qi) / 2 + uvOffset;
        }

        if (mesh == null)
        {
            mesh = new Mesh()
            {
                vertices = vertices,
                triangles = _triangles,
                normals = _normals,
                uv = uvs
            };
        }
        else
        {
            mesh.MarkDynamic();
            mesh.vertices = vertices;
            mesh.uv = uvs;
        }
        /*
        if (useCache)
        {
            _meshes[desiredRadius] = mesh;
        }
        */

        return mesh;
    }
}
