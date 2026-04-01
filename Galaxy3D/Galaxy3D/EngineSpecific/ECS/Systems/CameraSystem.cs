using Galaxy3D.ECS.Components;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Galaxy3D.ECS.Systems;

public class CameraSystem : ISystem
{
    private Entity[] _cameraEntities;
    private readonly IdGenerator _idGenerator;
    private int _currentSystemAmount;
    int _maxEntities = 100;
    private bool firstMove = true;
    private Vector2 mousePos;
    
    public CameraSystem(int MAX_ENTITIES)
    {
        _cameraEntities = new Entity[MAX_ENTITIES];
        _idGenerator = new IdGenerator(MAX_ENTITIES);
    }
    
    public void UpdateSystem(MouseState mouseState)
    {
        for (var i = 0; i < _currentSystemAmount; i++)
        {
            Camera camera = _cameraEntities[i].GetComponent<Camera>();
            Transform transform = _cameraEntities[i].GetComponent<Transform>();
            
            camera.projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(camera.fov, camera.aspectRatio,
                0.01f, 300f);
            
            if (firstMove)
            {
                mousePos = new Vector2(mouseState.X, mouseState.Y);
                firstMove = false;
            }
            else
            {
                float deltaX = mouseState.X - mousePos.X;
                float deltaY = mouseState.Y - mousePos.Y;
                mousePos = new Vector2(mouseState.X, mouseState.Y);

                camera.yaw -= deltaX * 0.1f;
                camera.pitch -= deltaY * 0.1f;
            }

            if (camera.unfocused == true)
            {
                camera.pitch = Math.Clamp(camera.pitch, -90f, 90f);

                // Pitch only on the camera (child)
                transform.rotation = new Vector3(
                    MathHelper.DegreesToRadians(camera.pitch),
                    0,
                    0
                );
                transform.isDirty = true;

                // Yaw on the parent (player body)
                if (_cameraEntities[i].parent is not null)
                {
                    Transform parentTransform = _cameraEntities[i].parent.GetComponent<Transform>();
                    parentTransform.rotation = new Vector3(
                        parentTransform.rotation.X,
                        MathHelper.DegreesToRadians(camera.yaw),
                        parentTransform.rotation.Z
                    );
                    parentTransform.isDirty = true;
                    _cameraEntities[i].parent.SetComponent(parentTransform);
                }
                
                Vector3 front;
                front.X = -MathF.Sin(MathHelper.DegreesToRadians(camera.yaw)) * MathF.Cos(MathHelper.DegreesToRadians(camera.pitch));
                front.Y =  MathF.Sin(MathHelper.DegreesToRadians(camera.pitch));
                front.Z = -MathF.Cos(MathHelper.DegreesToRadians(camera.yaw)) * MathF.Cos(MathHelper.DegreesToRadians(camera.pitch));
                camera.front = Vector3.Normalize(front);
            }
                
            Vector3 rotatedOffset = Vector3.TransformVector(transform.position, _cameraEntities[i].parent.GetComponent<Transform>().rotationMatrix);
            Vector3 worldPosition = _cameraEntities[i].parent.GetComponent<Transform>().position + rotatedOffset;
            
            camera.right = Vector3.Normalize(Vector3.Cross(camera.front, Vector3.UnitY));
            camera.up = Vector3.Normalize(Vector3.Cross(camera.right, camera.front));
            camera.viewMatrix = Matrix4.LookAt(worldPosition, worldPosition + camera.front, camera.up);
            
            _cameraEntities[i].SetComponent(camera);
            _cameraEntities[i].SetComponent(transform);
        }
    }
    
    public void AddEntityToSystem(Entity e)
    {
        if (_currentSystemAmount == _cameraEntities.Length)
        {
            _maxEntities += 100;
            _idGenerator.maxAmountOfIds = _maxEntities;
            var updatedArray = new Entity[_maxEntities];
            _cameraEntities.CopyTo(updatedArray, 0);
            _cameraEntities = updatedArray;
        }
        
        var id = _idGenerator.CreateEntityID();
        _cameraEntities[id] = e;
        _currentSystemAmount++;
    }
}