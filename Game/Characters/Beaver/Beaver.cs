using Godot;
using System;

public partial class Beaver : CharacterBody3D
{
    private Node3D _pivot;
    private Camera3D _camera;
    private RayCast3D _holdRayCast;

    [ExportGroup("Player Settings")]
    [Export]
    public float Speed = 5.0f;
    [Export]
    public float JumpVelocity = 4.5f;
    [Export]
    public float MouseSensitivity = 0.002f;

    [ExportGroup("Camera Settings")]
    [Export]
    public float CameraMinPitch = -20.0f;
    [Export]
    public float CameraMaxPitch = 20.0f;
    [Export]
    public float RotationSpeed = 10f;

    [Signal]
    public delegate void InteractObjectEventHandler(GodotObject obj);

    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _Ready()
    {
        _pivot = GetNode<Node3D>("Pivot");
        _camera = GetNode<Camera3D>("Pivot/Camera");
        _holdRayCast = GetNode<RayCast3D>("HoldRayCast");
        Input.MouseMode = Input.MouseModeEnum.Captured;
        AddToGroup("Player");
    }

    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseMotion eventMouseMotion)
        {
            // Rotate player left/right
            RotateY(-eventMouseMotion.Relative.X * MouseSensitivity);

            float currentPitch = _pivot.Rotation.X;
            float newPitch = currentPitch - eventMouseMotion.Relative.Y * MouseSensitivity;

            float minPitchRad = Mathf.DegToRad(CameraMinPitch);
            float maxPitchRad = Mathf.DegToRad(CameraMaxPitch);
            newPitch = Mathf.Clamp(newPitch, minPitchRad, maxPitchRad);
            _pivot.Rotation = new Vector3(newPitch, _pivot.Rotation.Y, _pivot.Rotation.Z);
        }
    }

    public override void _Process(double delta)
    {
        bool isColliding = _holdRayCast.IsColliding();

        if (isColliding)
        {
            GodotObject collider = _holdRayCast.GetCollider();
            EmitSignal(SignalName.InteractObject, collider);
        }
        else
        {
            EmitSignal(SignalName.InteractObject, null);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleMovement(delta);
    }

    private void HandleMovement(double delta)
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
