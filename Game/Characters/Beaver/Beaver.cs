using Godot;

public partial class Beaver : CharacterBody3D
{
	private Node3D _pivot;
	private Camera3D _camera;
	private float _cameraAngle = 0F;
    private float _mouseSensitivity = 0.1F;
	private const float _speed = 5.0f;
	private const float _jumpVelocity = 4.5f;

	public override void _Ready()
	{
		_pivot = GetNode<Node3D>("Pivot");
		_camera = GetNode<Camera3D>("Pivot/Camera");
    }

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("ui_cancel"))
            Input.MouseMode = Input.MouseModeEnum.Visible;
	}

	public override void _PhysicsProcess(double delta)
	{
		Walk();
	}

	public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is not InputEventMouseMotion motion)
			return;

        _pivot.RotateY(Mathf.DegToRad(-motion.Relative.X * _mouseSensitivity));
        float change = -motion.Relative.Y * _mouseSensitivity;

        if (!((change + _cameraAngle) < 90F) || !((change + _cameraAngle) > -90F))
			return;
        
        _camera.RotateX(Mathf.DegToRad(change));
        _cameraAngle += change;
    }

	private void Walk()
    {
        Vector3 direction = new();
        Basis aim = _camera.GlobalTransform.Basis;

        if (Input.IsActionPressed("ui_up"))
            direction -= aim.Z;
        
        if (Input.IsActionPressed("ui_down"))
            direction += aim.Z;
        
        if (Input.IsActionPressed("ui_left"))
            direction -= aim.X;

        if (Input.IsActionPressed("ui_right"))
            direction += aim.X;

        Velocity = direction.Normalized() * _speed;
        MoveAndSlide();
    }
}
