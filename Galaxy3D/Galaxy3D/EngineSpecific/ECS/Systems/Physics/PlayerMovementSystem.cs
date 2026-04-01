using Galaxy3D.ECS.Components;
using Galaxy3D.ECS.Components.Gameplay;
using Galaxy3D.ECS.Components.Physics;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Galaxy3D.ECS.Systems;

public class PlayerMovementSystem : ISystem
{
    private Entity[] _playerMovementEntities;
    private readonly IdGenerator _idGenerator;
    private int _currentSystemAmount;
    int _maxEntities = 100;

    public PlayerMovementSystem(int MAX_ENTITIES)
    {
        _playerMovementEntities = new Entity[MAX_ENTITIES];
        _idGenerator = new IdGenerator(MAX_ENTITIES);
    }

    public void UpdateSystem(KeyboardState keyboardState)
    {
        for (int i = 0; i < _currentSystemAmount; i++)
        {
            PhysicsBody body = _playerMovementEntities[i].GetComponent<PhysicsBody>();
            PlayerMovement playerMovement = _playerMovementEntities[i].GetComponent<PlayerMovement>();
            Camera camera = _playerMovementEntities[i].GetChild("Camera").GetComponent<Camera>();
            
            Vector3 input = GetInputVector(keyboardState); 
            
            Vector3 cameraForward = camera.front;
            Vector3 cameraRight = Vector3.Cross(cameraForward, camera.up).Normalized();
            
            cameraForward.Y = 0;
            cameraRight.Y = 0;
            cameraForward.Normalize();
            cameraForward = -cameraForward;
            cameraRight.Normalize();
            
            Vector3 movement = (cameraForward * input.Z + cameraRight * input.X) * playerMovement.movementSpeed;
            
            
            body.velocity = new Vector3(movement.X, body.velocity.Y, movement.Z);

            _playerMovementEntities[i].SetComponent(body);
        }
    }
    
    Vector3 GetInputVector(KeyboardState keyboardState)
    {

        Vector3 input = Vector3.Zero;

        if (keyboardState.IsKeyDown(Keys.W))
            input.Z -= 1;

        if (keyboardState.IsKeyDown(Keys.S))
            input.Z += 1;

        if (keyboardState.IsKeyDown(Keys.A))
            input.X -= 1;

        if (keyboardState.IsKeyDown(Keys.D))
            input.X += 1;

        if (input.LengthSquared > 0)
            input = input.Normalized();

        return input;
    }

    public void AddEntityToSystem(Entity e)
    {
        if (_currentSystemAmount == _playerMovementEntities.Length)
        {
            _maxEntities += 100;
            _idGenerator.maxAmountOfIds = _maxEntities;
            var updatedArray = new Entity[_maxEntities];
            _playerMovementEntities.CopyTo(updatedArray, 0);
            _playerMovementEntities = updatedArray;
        }
        
        var id = _idGenerator.CreateEntityID();
        _playerMovementEntities[id] = e;
        _currentSystemAmount++;
    }
}