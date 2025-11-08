using Godot;

public partial class Beaver : CharacterBody3D
{
    private Node3D _pivot;
    private Camera3D _camera;

    [Export]
    public float Speed = 5.0f;
    [Export]
    public float JumpVelocity = 4.5f;
    [Export]
    public float MouseSensitivity = 0.002f;
    [Export]
    public float CameraPitchLimit = 1.5f;

    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _Ready()
    {
        _pivot = GetNode<Node3D>("Pivot");
        _camera = GetNode<Camera3D>("Pivot/Camera");
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseMotion eventMouseMotion)
        {
            // Rotate player left/right
            RotateY(-eventMouseMotion.Relative.X * MouseSensitivity);

            // Rotate camera up/down
            float newPitch = _pivot.Rotation.X - eventMouseMotion.Relative.Y * MouseSensitivity;
            newPitch = Mathf.Clamp(newPitch, -CameraPitchLimit, CameraPitchLimit);
            _pivot.Rotation = new Vector3(newPitch, _pivot.Rotation.Y, _pivot.Rotation.Z);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;

        // Add gravity
        if (!IsOnFloor())
        {
            velocity.Y -= gravity * (float)delta;
        }

        // Handle Jump
        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
        }

        // Get input direction
        Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

        // Apply movement
        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}
