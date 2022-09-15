using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace _3DEngine
{
    class Chunk
    {
        public const int size = 16;

        private VertexArrayObject[,] vao = new VertexArrayObject[size, size];

        public Chunk()
        {
            for (var x = 0; x < size; x++)
                for (var z = 0; z < size; z++)
                    vao[x, z] = new VertexArrayObject(new Vector3(x, 1, z));
        }

        public void Draw()
        {
            for (var x = 0; x < size; x++)
                for (var z = 0; z < size; z++)
                    vao[x, z].Draw();
        }
    }
}
