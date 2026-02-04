using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Galaxy3D;

/// <summary>
/// Camera used to move around the game world during debugging
/// </summary>
public class DebugCamera
{
    //movement
    Vector3 position = new Vector3(0.0f, 60.0f, 3.0f);
    Vector3 front = new Vector3(0.0f, 0.0f, -1.0f);
    Vector3 up = new Vector3(0.0f, 1.0f,  0.0f);
    Vector3 right = new Vector3(1.0f, 0.0f,  0.0f);

    private float pitch;
    private float yaw;
    
    private float fov = MathHelper.DegreesToRadians(90f);

    private float aspectRation;

    private float camSpeed = 10f;
    float cameraSensitivity = 0.3f;
    private bool firstMove = true;
    private Vector2 lastPos;
    

    public DebugCamera(Vector3 position, float aspectRatio)
    {
        this.position = position;
        this.aspectRation = aspectRatio;
    }
    
    public void MoveAround(KeyboardState input, FrameEventArgs e)
    {
        
        if (input.IsKeyDown(Keys.W))
        {
            position += front * camSpeed * (float)e.Time;
        }

        if (input.IsKeyDown(Keys.S))
        {
            position -= front * camSpeed * (float)e.Time;
        }

        if (input.IsKeyDown(Keys.A))
        {
            position -= Vector3.Normalize(Vector3.Cross(front, up)) * camSpeed * (float)e.Time;
        }
        
        if (input.IsKeyDown(Keys.D))
        {
            position += Vector3.Normalize(Vector3.Cross(front, up)) * camSpeed * (float)e.Time;
        }

        if (input.IsKeyDown(Keys.Space))
        {
            position += up * camSpeed * (float)e.Time;
        }

        if (input.IsKeyDown(Keys.LeftShift))
        {
            position -= up * camSpeed * (float)e.Time;
        }
    }

    public void RotateCamera(MouseState mouse)
    {
        if (firstMove)
        {
            lastPos = new Vector2(mouse.X, mouse.Y);
            firstMove = false;
        }
        else
        {
            float deltaX = mouse.X - lastPos.X;
            float deltaY = mouse.Y - lastPos.Y;
            lastPos = new Vector2(mouse.X, mouse.Y);

            Yaw += deltaX * cameraSensitivity;
            Pitch -= deltaY * cameraSensitivity;
        }
    }

    public void UpdateLookVectors()
    {
        front.X = MathF.Cos(pitch) * MathF.Cos(yaw);
        front.Y = MathF.Sin(pitch);
        front.Z = MathF.Cos(pitch) * MathF.Sin(yaw);
        
        front = Vector3.Normalize(front);
        right = Vector3.Normalize(Vector3.Cross(front, Vector3.UnitY));
        up = Vector3.Normalize(Vector3.Cross(right, front));
    }

    public Matrix4 GetProjectionMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView(fov, aspectRation, 0.01f, 300f);
    }
    
    public Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(position, position + front, up);
    }
    
    public float AspectRatio { get => aspectRation; set => aspectRation = value; }
    
    public float Pitch { get => MathHelper.RadiansToDegrees(pitch); set {
            var angle = MathHelper.Clamp(value, -89f, 89f);
            pitch = MathHelper.DegreesToRadians(angle);
            UpdateLookVectors();
        }
    }
    
    public float Yaw { get => MathHelper.RadiansToDegrees(yaw); set {
            yaw = MathHelper.DegreesToRadians(value);
            UpdateLookVectors();
        }
    }
    public Vector3 Position { get => position; set => position = value; }
}