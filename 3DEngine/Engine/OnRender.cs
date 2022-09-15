using OpenGL;

namespace _3DEngine
{
    class OnRender
    {
        public static Cube C1;
        public static Cube C2;
        public static Cube C3;

        public static Cube[] arrC;

        public static Plane plane;

        public static Ramp R1;
        public static Ramp R2;
        public static Ramp R3;
        public static Ramp R4;

        public static Square2D sq;

        public static Chunk chunk;

        public static Wall south;
        public static Wall north;
        public static Wall east;
        public static Wall west;

        public OnRender()
        {
            // change camera direction and position
            Program.CameraPosition(-1, 1.9f, -15);
            Program.CameraDirection(0, 0, 1);
            Program.LightDirection(-1, 1, -1);

            // New Plane
            plane = new Plane(30, true);

            // New cubes
            C1 = new Cube(new Vector3(0, 2, 5), 2, true/*, "Dirt-Block.jpg", "Dirt-Block-Normal.jpg"*/);
            C2 = new Cube(new Vector3(0, 4, 0), 2, true, "Stone-Block.png", "Stone-Block-Normal.png");
            //C3 = new Cube(new Vector3(15, 2, 0), 2, true, "Stone-Block.png", "Stone-Block-Normal.png");
            //C2 = new Cube(new Vector3(0, 2, 0), 2);

            // chunk
            chunk = new Chunk();

            // array of new cubes
            /*arrC = new Cube[4];
            arrC[0] = new Cube(new Vector3(1, 1, 1), 2);
            arrC[1] = new Cube(new Vector3(1, 1, -1), 2);
            arrC[2] = new Cube(new Vector3(-1, 1, 1), 2);
            arrC[3] = new Cube(new Vector3(-1, 1, -1), 2);*/

            // new ramp
            R1 = new Ramp(new Vector3(2, 2, 0), 2, 270, true, "Stone-Block.png", "Stone-Block-Normal.png");
            R2 = new Ramp(new Vector3(-2, 2, 0), 2, 90, true, "Stone-Block.png", "Stone-Block-Normal.png");
            R3 = new Ramp(new Vector3(0, 2, 2), 2, 0, true, "Stone-Block.png", "Stone-Block-Normal.png");
            R4 = new Ramp(new Vector3(0, 2, -2), 2, 180, true, "Stone-Block.png", "Stone-Block-Normal.png");

            // new 2D square
            sq = new Square2D(new Vector3(-4, 1, 0), 2, 'x', true, "Stone-Block.png", "Stone-Block-Normal.png");

            // new wall
            south = new Wall(new Vector3(28, 5, -28), 58, 2, true);
            north = new Wall(new Vector3(30, 5, 30), 58, 2, true);
            east = new Wall(new Vector3(-28, 5, 30), 2, 58, true);
            west = new Wall(new Vector3(30, 5, 28), 2, 58, true);
        }

        public void render()
        {
            // change background colour
            Program.BackgroundColor(0, 0.4f, 1, 1);

            // draw objects
            plane.Draw();

            //C1.Draw();
            //C2.Draw();
            //C3.Draw();
            
            /*foreach (Cube obj in arrC)
            {
                obj.Draw();
            }*/

            /*R1.Draw();
            R2.Draw();
            R3.Draw();
            R4.Draw();*/

            sq.Draw();
            //sp.Draw();

            south.Draw();
            north.Draw();
            east.Draw();
            west.Draw();

            chunk.Draw();
        }
    }
}
