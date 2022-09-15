using System;
using System.Collections.Generic;
using Tao.FreeGlut;
using OpenGL;

namespace _3DEngine
{
    class GameObject
    {
        public VBO<Vector3> cube, cubeNormals, cubeTangents;
        public VBO<Vector2> cubeUV;
        public VBO<uint> cubeTriangles;
        public Texture brickDiffuse;
        public Texture brickNormals;

        protected static Vector3[] CalculateTangents(Vector3[] vertices, Vector3[] normals, uint[] triangles, Vector2[] uvs)
        {
            Vector3[] tangents = new Vector3[vertices.Length];
            Vector3[] tangentData = new Vector3[vertices.Length];

            for (int i = 0; i < triangles.Length / 3; i++)
            {
                Vector3 v1 = vertices[triangles[i * 3]];
                Vector3 v2 = vertices[triangles[i * 3 + 1]];
                Vector3 v3 = vertices[triangles[i * 3 + 2]];

                Vector2 w1 = uvs[triangles[i * 3]];
                Vector2 w2 = uvs[triangles[i * 3] + 1];
                Vector2 w3 = uvs[triangles[i * 3] + 2];

                float x1 = v2.X - v1.X;
                float x2 = v3.X - v1.X;
                float y1 = v2.Y - v1.Y;
                float y2 = v3.Y - v1.Y;
                float z1 = v2.Z - v1.Z;
                float z2 = v3.Z - v1.Z;

                float s1 = w2.X - w1.X;
                float s2 = w3.X - w1.X;
                float t1 = w2.Y - w1.Y;
                float t2 = w3.Y - w1.Y;
                float r = 1.0f / (s1 * t2 - s2 * t1);
                Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);

                tangents[triangles[i * 3]] += sdir;
                tangents[triangles[i * 3 + 1]] += sdir;
                tangents[triangles[i * 3 + 2]] += sdir;
            }

            for (int i = 0; i < vertices.Length; i++)
                tangentData[i] = (tangents[i] - normals[i] * Vector3.Dot(normals[i], tangents[i])).Normalize();

            return tangentData;
        }
    }
}
