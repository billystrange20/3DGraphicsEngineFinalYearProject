using System;
using System.Collections.Generic;
using OpenGL;

namespace _3DEngine
{
    class VertexArrayObject
    {
        public VBO<Vector3> cube, cubeNormals, cubeTangents;
        public VBO<Vector2> cubeUV;
        public VBO<uint> cubeTriangles;

        public VertexArrayObject(Vector3 pos)
        {
            // can change height and position (cannot change width/length)
            Vector3[] vertices = new Vector3[] {
                new Vector3(pos.X, pos.Y, pos.Z-1), new Vector3(pos.X-1, pos.Y, pos.Z-1), new Vector3(pos.X-1, pos.Y, pos.Z), new Vector3(pos.X, pos.Y, pos.Z),         // top
                new Vector3(pos.X, 0, pos.Z), new Vector3(pos.X-1, 0, pos.Z), new Vector3(pos.X-1, 0, pos.Z-1), new Vector3(pos.X, 0, pos.Z-1),     // bottom
                new Vector3(pos.X, pos.Y, pos.Z), new Vector3(pos.X-1, pos.Y, pos.Z), new Vector3(pos.X-1, 0, pos.Z), new Vector3(pos.X, 0, pos.Z),         // front face
                new Vector3(pos.X, 0, pos.Z-1), new Vector3(pos.X-1, 0, pos.Z-1), new Vector3(pos.X-1, pos.Y, pos.Z-1), new Vector3(pos.X, pos.Y, pos.Z-1),     // back face
                new Vector3(pos.X-1, pos.Y, pos.Z), new Vector3(pos.X-1, pos.Y, pos.Z-1), new Vector3(pos.X-1, 0, pos.Z-1), new Vector3(pos.X-1, 0, pos.Z),     // left
                new Vector3(pos.X, pos.Y, pos.Z-1), new Vector3(pos.X, pos.Y, pos.Z), new Vector3(pos.X, 0, pos.Z), new Vector3(pos.X, 0, pos.Z-1) };
            cube = new VBO<Vector3>(vertices);

            Vector2[] uvs = new Vector2[] {
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
            cubeUV = new VBO<Vector2>(uvs);

            List<uint> triangles = new List<uint>();
            for (uint i = 0; i < 6; i++)
            {
                triangles.Add(i * 4);
                triangles.Add(i * 4 + 1);
                triangles.Add(i * 4 + 2);
                triangles.Add(i * 4);
                triangles.Add(i * 4 + 2);
                triangles.Add(i * 4 + 3);
            }
            cubeTriangles = new VBO<uint>(triangles.ToArray(), BufferTarget.ElementArrayBuffer);

            Vector3[] normals = Geometry.CalculateNormals(vertices, triangles.ToArray());
            cubeNormals = new VBO<Vector3>(normals);

            Vector3[] tangents = CalculateTangents(vertices, normals, triangles.ToArray(), uvs);
            cubeTangents = new VBO<Vector3>(tangents);
        }

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

        public void Draw()
        {
            // draw the object
            Gl.BindBufferToShaderAttribute(cube, Program.program, "vertexPosition");
            Gl.BindBufferToShaderAttribute(cubeNormals, Program.program, "vertexNormal");
            Gl.BindBufferToShaderAttribute(cubeTangents, Program.program, "vertexTangent");
            Gl.BindBufferToShaderAttribute(cubeUV, Program.program, "vertexUV");
            Gl.BindBuffer(cubeTriangles);

            Gl.DrawElements(BeginMode.Triangles, cubeTriangles.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }
    }
}
