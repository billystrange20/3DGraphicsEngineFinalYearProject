using System;
using System.Collections.Generic;
using Tao.FreeGlut;
using OpenGL;

namespace _3DEngine
{
    class Wall : GameObject
    {
        public Wall(Vector3 pos, int sizeX, int sizeZ, bool collDetectOn, string brickDiffuse = "AlternatingBrick-ColorMap.png", string brickNormals = "AlternatingBrick-NormalMap.png")
        {
            this.brickDiffuse = new Texture(brickDiffuse);
            this.brickNormals = new Texture(brickNormals);

            // can change height and position and width/length
            Vector3[] vertices = new Vector3[] {
                new Vector3(pos.X, pos.Y, pos.Z-sizeZ), new Vector3(pos.X-sizeX, pos.Y, pos.Z-sizeZ), new Vector3(pos.X-sizeX, pos.Y, pos.Z), new Vector3(pos.X, pos.Y, pos.Z),    // top
                new Vector3(pos.X, 0, pos.Z), new Vector3(pos.X-sizeX, 0, pos.Z), new Vector3(pos.X-sizeX, 0, pos.Z-sizeZ), new Vector3(pos.X, 0, pos.Z-sizeZ),                    // bottom
                new Vector3(pos.X, pos.Y, pos.Z), new Vector3(pos.X-sizeX, pos.Y, pos.Z), new Vector3(pos.X-sizeX, 0, pos.Z), new Vector3(pos.X, 0, pos.Z),                      // front face
                new Vector3(pos.X, 0, pos.Z-sizeZ), new Vector3(pos.X-sizeX, 0, pos.Z-sizeZ), new Vector3(pos.X-sizeX, pos.Y, pos.Z-sizeZ), new Vector3(pos.X, pos.Y, pos.Z-sizeZ),  // back face
                new Vector3(pos.X-sizeX, pos.Y, pos.Z), new Vector3(pos.X-sizeX, pos.Y, pos.Z-sizeZ), new Vector3(pos.X-sizeX, 0, pos.Z-sizeZ), new Vector3(pos.X-sizeX, 0, pos.Z),  // left
                new Vector3(pos.X, pos.Y, pos.Z-sizeZ), new Vector3(pos.X, pos.Y, pos.Z), new Vector3(pos.X, 0, pos.Z), new Vector3(pos.X, 0, pos.Z-sizeZ) };                    // right
            cube = new VBO<Vector3>(vertices);

            if (collDetectOn)
            {

                List<Vector3> verts = new List<Vector3>();

                verts.Add(new Vector3(pos.X + 0.15f, pos.Y + 0.15f, pos.Z + 0.15f));      //1
                verts.Add(new Vector3(pos.X - sizeX - 0.15f, pos.Y + 0.15f, pos.Z + 0.15f));      //2
                verts.Add(new Vector3(pos.X + 0.15f, pos.Y + 0.15f, pos.Z - sizeZ - 0.15f)); //3
                verts.Add(new Vector3(pos.X - sizeX - 0.15f, pos.Y + 0.15f, pos.Z - sizeZ - 0.15f)); //4

                verts.Add(new Vector3(pos.X + 0.15f, 0 - 0.15f, pos.Z + 0.15f));      //5
                verts.Add(new Vector3(pos.X - sizeX - 0.15f, 0 - 0.15f, pos.Z + 0.15f));      //6
                verts.Add(new Vector3(pos.X + 0.15f, 0 - 0.15f, pos.Z - sizeZ - 0.15f)); //7
                verts.Add(new Vector3(pos.X - sizeX - 0.15f, 0 - 0.15f, pos.Z - sizeZ - 0.15f)); //8

                Program.objectVerticies.Add(verts);

            }

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