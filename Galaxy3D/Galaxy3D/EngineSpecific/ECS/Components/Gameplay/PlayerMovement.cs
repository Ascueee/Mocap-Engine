namespace Galaxy3D.ECS.Components.Gameplay;

public struct PlayerMovement : IComponent
{
    int _componentID;
    bool _isActive;
    private float _movementSpeed;
    private float _jumpForce;

    public PlayerMovement(float movementSpeed, float jumpForce)
    {
        _movementSpeed = movementSpeed;
        _jumpForce = jumpForce;
    }
    
    public int componentID { get; set; }
    public bool isActive { get; set; }
    
    public float movementSpeed { get => _movementSpeed; set => _movementSpeed = value; }
    public float jumpForce { get => _jumpForce; set => _jumpForce = value; }
}