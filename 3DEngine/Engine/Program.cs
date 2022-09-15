﻿using System.Collections.Generic;
using Tao.FreeGlut;
using OpenGL;

namespace _3DEngine 
{
    class Program 
    {
        private static int width = 1280, height = 720;
        private static System.Diagnostics.Stopwatch watch;

        public static ShaderProgram program;
        private static bool lighting = true, fullscreen = false, normalMapping = false;
        private static bool left, right, up, down, space;

        private static Camera camera;

        private static BMFont font;
        private static ShaderProgram fontProgram;
        private static FontVAO information;

        public static OnRender renderObjects;

        public static List<List<Vector3>> objectVerticies;
        public static int count = 0;

        static void Main(string[] args)
        {
            Logger.Info("Starting...");

            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH | Glut.GLUT_MULTISAMPLE);   // multisampling
            Glut.glutInitWindowSize(width, height);
            Glut.glutCreateWindow("3DEngine");

            Glut.glutIdleFunc(OnRenderFrame);
            Glut.glutDisplayFunc(OnDisplay);

            Glut.glutKeyboardFunc(OnKeyboardDown);
            Glut.glutKeyboardUpFunc(OnKeyboardUp);

            Glut.glutCloseFunc(OnClose);
            Glut.glutReshapeFunc(OnReshape);

            // add mouse callbacks
            Glut.glutMouseFunc(OnMouse);
            Glut.glutMotionFunc(OnMove);

            Gl.Enable(EnableCap.DepthTest);
            Gl.Enable(EnableCap.Blend);
            Gl.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            // create shader program
            program = new ShaderProgram(VertexShader, FragmentShader);

            // create camera
            CameraPosition();
            CameraDirection();

            // set up the projection and view matrix
            program.Use();
            program["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)width / height, 0.1f, 1000f));

            LightDirection();
            program["enable_lighting"].SetValue(lighting);
            program["normalTexture"].SetValue(1);
            program["enable_mapping"].SetValue(normalMapping);

            // initialises GameObjects
            objectVerticies = new List<List<Vector3>>();
            renderObjects = new OnRender();

            // load the bitmap font
            font = new BMFont("font24.fnt", "font24.png");
            fontProgram = new ShaderProgram(BMFont.FontVertexSource, BMFont.FontFragmentSource);

            fontProgram.Use();
            fontProgram["ortho_matrix"].SetValue(Matrix4.CreateOrthographic(width, height, 0, 1000));
            fontProgram["color"].SetValue(new Vector3(1, 1, 1));

            information = font.CreateString(fontProgram, "Game Engine");

            watch = System.Diagnostics.Stopwatch.StartNew();
            
            Logger.Info("Startup complete!");

            Glut.glutMainLoop();
        }

        public static void CameraPosition(float x = 0, float y = 2, float z = 10)
        {
            camera = new Camera(new Vector3(x, y, z), Quaternion.Identity);
        }

        public static void CameraDirection(float x = 0, float y = 0, float z = -1)
        {
            camera.SetDirection(new Vector3(x, y, z));
        }

        public static void LightDirection(float x = 1, float y = 1, float z = 1)
        {
            program["light_direction"].SetValue(new Vector3(x, y, z));
        }

        private static void OnClose()
        {
            fontProgram.DisposeChildren = true;
            fontProgram.Dispose();
            font.FontTexture.Dispose();
            information.Dispose();

            program.DisposeChildren = true;
            program.Dispose();
        }

        private static void OnDisplay()
        {
        }

        private static void OnRenderFrame()
        {
            watch.Stop();
            float deltaTime = (float)watch.ElapsedTicks / System.Diagnostics.Stopwatch.Frequency;
            watch.Restart();

            CollisionDetection(deltaTime);

            // set up the viewport and clear the previous depth and color buffers
            Gl.Viewport(0, 0, width, height);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // make sure the shader program and texture are being used
            Gl.UseProgram(program);

            // apply camera view matrix to the shader view matrix (this can be used for all objects in the scene)
            program["view_matrix"].SetValue(camera.ViewMatrix);

            // set up the model matrix
            program["model_matrix"].SetValue(Matrix4.Identity);
            program["enable_lighting"].SetValue(lighting);
            program["enable_mapping"].SetValue(normalMapping);

            renderObjects.render();

            // bind the font program as well as the font texture
            Gl.UseProgram(fontProgram.ProgramID);
            Gl.BindTexture(font.FontTexture);

            // draw the information, which is static
            information.Draw();

            Glut.glutSwapBuffers();
        }

        private static Vector3 prevPos;

        private static void CollisionDetection(float deltaTime)
        {
            bool isHit = false;

            for (int x = 0; x < objectVerticies.Count; x++)
            {
                bool xx = false;
                bool yy = false;
                bool zz = false;

                for (int y = 1; y <= 6; y++)
                {
                    if ((camera.Position.X <= objectVerticies[x][0].X) && (camera.Position.X >= objectVerticies[x][y].X))
                    {
                        xx = true;
                    }
                    else if ((camera.Position.Y <= objectVerticies[x][0].Y) && (camera.Position.Y >= objectVerticies[x][y].Y))
                    {       
                        yy = true;
                    }
                    else if ((camera.Position.Z <= objectVerticies[x][0].Z) && (camera.Position.Z >= objectVerticies[x][y].Z))
                    {
                        zz = true;
                    }
                }

                if ((xx == true) && (yy == true) && (zz == true))
                {
                    camera.Position = prevPos;

                    isHit = true;
                } 

            }

            if (!isHit)
            {
                prevPos = camera.Position;

                // update camera by moving it up to 5 units per second in each direction
                if (down) camera.MoveRelative(Vector3.UnitZ * deltaTime * 5);
                if (up) camera.MoveRelative(-Vector3.UnitZ * deltaTime * 5);
                if (left) camera.MoveRelative(-Vector3.UnitX * deltaTime * 5);
                if (right) camera.MoveRelative(Vector3.UnitX * deltaTime * 5);
            }
        }

        public static void BackgroundColor(float red, float green, float blue, float alpha)
        {
            Gl.ClearColor(red, green, blue, alpha);
        }

        private static void OnReshape(int width, int height)
        {
            Program.width = width;
            Program.height = height;

            Gl.UseProgram(program.ProgramID);
            program["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)width / height, 0.1f, 1000f));

            Gl.UseProgram(fontProgram.ProgramID);
            fontProgram["ortho_matrix"].SetValue(Matrix4.CreateOrthographic(width, height, 0, 1000));

            information.Position = new Vector2(-width / 2 + 10, height / 2 - font.Height - 10);
        }

        private static bool mouseDown = false;
        private static int downX, downY;
        private static int prevX, prevY;

        private static void OnMouse(int button, int state, int x, int y)
        {
            if (button != Glut.GLUT_LEFT_BUTTON) return;

            // this method gets called whenever a new mouse button event happens
            mouseDown = (state == Glut.GLUT_DOWN);

            // if the mouse has just been clicked then we hide the cursor and store the position
            if (mouseDown)
            {
                Glut.glutSetCursor(Glut.GLUT_CURSOR_NONE);
                prevX = downX = x;
                prevY = downY = y;
            }
            else // unhide the cursor if the mouse has just been released
            {
                Glut.glutSetCursor(Glut.GLUT_CURSOR_LEFT_ARROW);
                Glut.glutWarpPointer(downX, downY);
            }
        }

        private static void OnMove(int x, int y)
        {
            // if the mouse move event is caused by glutWarpPointer then do nothing
            if (x == prevX && y == prevY) return;

            // move the camera when the mouse is down
            if (mouseDown)
            {
                float yaw = (prevX - x) * 0.002f;
                camera.Yaw(yaw);

                if (space) {
                    float pitch = (prevY - y) * 0.002f;
                    camera.Pitch(pitch);
                }

                prevX = x;
                prevY = y;
            }

            if (x < 0) Glut.glutWarpPointer(prevX = width, y);
            else if (x > width) Glut.glutWarpPointer(prevX = 0, y);

            if (y < 0) Glut.glutWarpPointer(x, prevY = height);
            else if (y > height) Glut.glutWarpPointer(x, prevY = 0);
        }
        private static void OnKeyboardDown(byte key, int x, int y)
        {
            if (key == 'w') up = true;
            else if (key == 's') down = true;
            else if (key == 'd') right = true;
            else if (key == 'a') left = true;
            else if (key == ' ') space = true;
            else if (key == 27) Glut.glutLeaveMainLoop();
        }

        private static void OnKeyboardUp(byte key, int x, int y)
        {
            if (key == 'w') up = false;
            else if (key == 's') down = false;
            else if (key == 'd') right = false;
            else if (key == 'a') left = false;
            else if (key == ' ') space = false;
            else if (key == 'l') lighting = !lighting;
            else if (key == 'm') normalMapping = !normalMapping;
            else if (key == 'f')
            {
                fullscreen = !fullscreen;
                if (fullscreen) Glut.glutFullScreen();
                else
                {
                    Glut.glutPositionWindow(0, 0);
                    Glut.glutReshapeWindow(1280, 720);
                }
            }
        }

        private static string VertexShader = @"
#version 130
in vec3 vertexPosition;
in vec3 vertexNormal;
in vec3 vertexTangent;
in vec2 vertexUV;
uniform vec3 light_direction;
out vec3 normal;
out vec2 uv;
out vec3 light;
uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 model_matrix;
uniform bool enable_mapping;
void main(void)
{
    normal = normalize((model_matrix * vec4(floor(vertexNormal), 0)).xyz);
    uv = vertexUV;
    mat3 tbnMatrix = mat3(vertexTangent, cross(vertexTangent, normal), normal);
    light = (enable_mapping ? light_direction * tbnMatrix : light_direction);
    gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);
}
";

        private static string FragmentShader = @"
#version 130
uniform sampler2D colorTexture;
uniform sampler2D normalTexture;
uniform bool enable_lighting;
uniform mat4 model_matrix;
uniform bool enable_mapping;
in vec3 normal;
in vec2 uv;
in vec3 light;
out vec4 fragment;
void main(void)
{
    vec3 fragmentNormal = texture2D(normalTexture, uv).xyz * 2 - 1;
    vec3 selectedNormal = (enable_mapping ? fragmentNormal : normal);
    float diffuse = max(dot(selectedNormal, light), 0);
    float ambient = 0.3;
    float lighting = (enable_lighting ? max(diffuse, ambient) : 1);
    fragment = vec4(lighting * texture2D(colorTexture, uv).xyz, 1);
}
";
    }
}