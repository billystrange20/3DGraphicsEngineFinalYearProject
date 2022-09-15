using System;
using OpenGL;

namespace _3DEngine
{
    public class Camera
    {
        private Vector3 position;
        private Quaternion orientation;
        private Matrix4 viewMatrix; // a cached version of the view matrix
        private bool dirty = true;  // true if the viewMatrix must be recalculated

        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                dirty = true;
            }
        }

        public Quaternion Orientation
        {
            get { return orientation; }
            set
            {
                orientation = value;
                dirty = true;
            }
        }

        public Matrix4 ViewMatrix
        {
            get
            {
                if (dirty)
                {
                    viewMatrix = Matrix4.CreateTranslation(-position) * orientation.Matrix4;
                    dirty = false;
                }
                return viewMatrix;
            }
        }

        public Camera(Vector3 position, Quaternion orientation)
        {
            this.Position = position;
            this.Orientation = orientation;
        }

        public void SetDirection(Vector3 direction)
        {
            if (direction == Vector3.Zero) return;

            Vector3 zvec = -direction.Normalize();
            Vector3 xvec = Vector3.UnitY.Cross(zvec).Normalize();
            Vector3 yvec = zvec.Cross(xvec).Normalize();
            Orientation = Quaternion.FromAxis(xvec, yvec, zvec);
        }

        public void Move(Vector3 by)
        {
            Position += by;
        }

        public void MoveRelative(Vector3 by)
        {
            Position += Orientation * by;
        }

        public void Rotate(Quaternion rotation)
        {
            Orientation = rotation * orientation;
        }

        public void Rotate(float angle, Vector3 axis)
        {
            Rotate(Quaternion.FromAngleAxis(angle, axis));
        }

        public void Roll(float angle)
        {
            Vector3 axis = orientation * Vector3.UnitZ;
            Rotate(angle, axis);
        }

        public void Yaw(float angle)
        {
            Rotate(angle, Vector3.UnitY);
        }

        public void Pitch(float angle)
        {
            Vector3 axis = orientation * Vector3.UnitX;
            Rotate(angle, axis);
        }
    }
}
