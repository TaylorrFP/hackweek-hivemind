using Sandbox;
using Sandbox.Citizen;
using System.Diagnostics;

public sealed class Player : Component
{
	[Property] public GameObject Body { get; set; }
	[Property] public GameObject Head { get; set; }

	[Property] public GameObject SpriteHolder { get; set; }

	//Camera
	[Property] public CameraComponent Camera { get; set; }

	// Movement Properties
	[Property] public float GroundControl { get; set; } = 4.0f;
	[Property] public float AirControl { get; set; } = 0.1f;
	[Property] public float MaxForce { get; set; } = 50f;
	[Property] public float Speed { get; set; } = 160f;
	[Property] public float RunSpeed { get; set; } = 290f;
	[Property] public float CrouchSpeed { get; set; } = 90f;
	[Property] public float JumpForce { get; set; } = 400f;

	public bool IsCrouching = false;
	public bool IsSprinting = false;
	private CharacterController characterController;

	[Property] [Sync] public Angles eyeAngle { get; set; }

	Vector3 WishVelocity = Vector3.Zero;

	protected override void OnAwake()
	{
		characterController = Components.Get<CharacterController>();




	}

	protected override void OnStart()
	{
		base.OnStart();


		if (!IsProxy) //if we own this
		{
			//SpriteHolder.Enabled = false;


		}
		else
		{
			Camera.Enabled = false;

		}


	}

	protected override void OnFixedUpdate()
	{
		if (!IsProxy )
		{
			BuildWishVelocity();
			Move();

		}

	}
	protected override void OnUpdate()
	{

		if ( !IsProxy ) //if you own this
		{





			eyeAngle = new Angles( (eyeAngle.pitch + Input.MouseDelta.y * 0.1f).Clamp( -89.9f, 89.9f ), eyeAngle.yaw - Input.MouseDelta.x * 0.1f, 0f );

			Head.Transform.Rotation = eyeAngle.ToRotation();






		}

		if ( Camera != null )
		{
			Camera.Transform.Rotation = eyeAngle;
			SpriteHolder.Transform.Rotation = eyeAngle;
		}
















	}

	void BuildWishVelocity()
	{
		WishVelocity = 0;

		var rot = Head.Transform.Rotation;
		if ( Input.Down( "Forward" ) ) WishVelocity += rot.Forward;
		if ( Input.Down( "Backward" ) ) WishVelocity += rot.Backward;
		if ( Input.Down( "Left" ) ) WishVelocity += rot.Left;
		if ( Input.Down( "Right" ) ) WishVelocity += rot.Right;

		WishVelocity = WishVelocity.WithZ( 0 );

		if ( !WishVelocity.IsNearZeroLength ) WishVelocity = WishVelocity.Normal;

		if ( IsCrouching ) WishVelocity *= CrouchSpeed; // Crouching takes presedence over sprinting
		else if ( IsSprinting ) WishVelocity *= RunSpeed; // Sprinting takes presedence over walking
		else WishVelocity *= Speed;
	}

	void Move()
	{
		// Get gravity from our scene
		var gravity = Scene.PhysicsWorld.Gravity;

		if ( characterController.IsOnGround )
		{
			// Apply Friction/Acceleration
			characterController.Velocity = characterController.Velocity.WithZ( 0 );
			characterController.Accelerate( WishVelocity );
			characterController.ApplyFriction( GroundControl );
		}
		else
		{
			// Apply Air Control / Gravity
			characterController.Velocity += gravity * Time.Delta * 0.5f;
			characterController.Accelerate( WishVelocity.ClampLength( MaxForce ) );
			characterController.ApplyFriction( AirControl );
		}

		// Move the character controller
		characterController.Move();

		// Apply the second half of gravity after movement
		if ( !characterController.IsOnGround )
		{
			characterController.Velocity += gravity * Time.Delta * 0.5f;
		}
		else
		{
			characterController.Velocity = characterController.Velocity.WithZ( 0 );
		}
	}
}
