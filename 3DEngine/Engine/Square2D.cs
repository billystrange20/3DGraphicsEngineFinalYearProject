using System;
using System.Collections.Generic;
using Tao.FreeGlut;
using OpenGL;

namespace _3DEngine
{
    class Square2D : GameObject
    {
        public Vector3[] vertices;

        public Square2D(Vector3 pos, int size, char axis, bool collDetectOn, string brickDiffuse = "AlternatingBrick-ColorMap.png", string brickNormals = "AlternatingBrick-NormalMap.png")
        {
            this.brickDiffuse = new Texture(brickDiffuse);
            this.brickNormals = new Texture(brickNormals);

            // creates 2D shape/square (Cannot change height of shape on x and z)
            switch (axis)
            {
                case 'x':
                    Vector3[] verticesx = new Vector3[] {
                        new Vector3(pos.X, pos.Y, pos.Z), new Vector3(pos.X-size, pos.Y, pos.Z), new Vector3(pos.X-size, 0, pos.Z), new Vector3(pos.X, 0, pos.Z) };  // front face
                    vertices = verticesx;
                    break;
                case 'z':
                    Vector3[] verticesz = new Vector3[] {
                        new Vector3(pos.X, pos.Y, pos.Z-size), new Vector3(pos.X, pos.Y, pos.Z), new Vector3(pos.X, 0, pos.Z), new Vector3(pos.X, 0, pos.Z-size) };  // right face
                    vertices = verticesz;
                    break;
                case 'y':
                    Vector3[] verticesy = new Vector3[] {
                        new Vector3(pos.X, pos.Y, pos.Z-size), new Vector3(pos.X-size, pos.Y, pos.Z-size), new Vector3(pos.X-size, pos.Y, pos.Z), new Vector3(pos.X, pos.Y, pos.Z) };  // bottom face
                    vertices = verticesy;
                    break;
            }
            
            cube = new VBO<Vector3>(vertices);

            Vector2[] uvs = new Vector2[] {
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
            cubeUV = new VBO<Vector2>(uvs);

            List<uint> triangles = new List<uint>();
            for (uint i = 0; i < 1; i++)
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

            Logger.Debug(this.GetType().Name + " compiled successfully!");
        }

        public void Draw()
        {
            // draw the object
            Gl.BindBufferToShaderAttribute(cube, Program.program, "vertexPosition");
            Gl.BindBufferToShaderAttribute(cubeNormals, Program.program, "vertexNormal");
            Gl.BindBufferToShaderAttribute(cubeTangents, Program.program, "vertexTangent");
            Gl.BindBufferToShaderAttribute(cubeUV, Program.program, "vertexUV");
            Gl.BindBuffer(cubeTriangles);

            Gl.ActiveTexture(TextureUnit.Texture1);
            Gl.BindTexture(brickNormals);
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(brickDiffuse);

            Gl.DrawElements(BeginMode.Triangles, cubeTriangles.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }
    }
}
