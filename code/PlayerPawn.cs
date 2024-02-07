using Sandbox;
using System.Collections.Generic;
using System.Linq;

public class PlayerPawn : Component
{
	Color[] playerColours = new Color[19];
	//string[] playerEmojis = new string[19];

	[Property] public GameObject playerCursorPrefab { get; set; }

	[Property] public GameObject playerSpawn { get; set; }
	[Property] public List<PlayerController> playerControllers { get; set; } = new();

	[Property] public List<GameObject> playerCursors { get; set; } = new();

	[Property] public GameObject pawnCrosshair { get; set; }

	[Property] public CameraComponent pawnCamera { get; set; }

	Angles averageEyeAngle;
	Vector3 averageInputVelocity = 0;
	[Property] public int testInt;

	[Property] public CharacterController characterController;

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

	Vector3 WishVelocity = Vector3.Zero;

	protected override void OnAwake()
	{
	


		base.OnAwake();


		//do this better in the future
		playerColours[0] = new Color( 0.956f, 0.262f, 0.211f );
		playerColours[1] = new Color( 0.913f, 0.117f, 0.388f);
		playerColours[2] = new Color( 0.611f, 0.152f, 0.690f );
		playerColours[3] = new Color( 0.403f, 0.227f, 0.717f );
		playerColours[4] = new Color( 0.247f, 0.317f, 0.709f );
		playerColours[5] = new Color( 0.129f, 0.588f, 0.952f );
		playerColours[6] = new Color( 0.011f, 0.662f, 0.956f );
		playerColours[7] = new Color( 0f, 0.737f, 0.831f );
		playerColours[8] = new Color( 0f, 0.588f, 0.533f );
		playerColours[9] = new Color( 0.298f, 0.298f, 0.313f );
		playerColours[10] = new Color( 0.545f, 0.764f, 0.290f );
		playerColours[11] = new Color( 0.803f, 0.862f, 0.223f );
		playerColours[12] = new Color( 1f, 0.921f, 0.231f );
		playerColours[13] = new Color( 1f, 0.756f, 0.02f );
		playerColours[14] = new Color( 1f, 0.596f, 0f );
		playerColours[15] = new Color( 1f, 0.341f, 0.133f );
		playerColours[16] = new Color( 0.474f, 0.333f, 0.282f );
		playerColours[17] = new Color( 0.619f, 0.619f, 0.619f );
		playerColours[18] = new Color( 0.376f, 0.490f, 0.545f );

		//playerEmojis[0] = "🍎";
		//playerEmojis[1] = "🐷";
		//playerEmojis[2] = "🍇";
		//playerEmojis[3] = "💣";





	}

	protected override void OnUpdate()
	{





		averageEyeAngle = Angles.Zero;
		var averageVector = Vector3.Zero;

		for ( int i = 0; i < playerControllers.Count; i++ )
		{

			averageEyeAngle += playerControllers[i].eyeAngle;
			playerCursors[i].Transform.LocalRotation = playerControllers[i].eyeAngle.ToRotation();

			averageVector += playerControllers[i].eyeAngle.Forward;

			if ( playerControllers[i].Network.IsOwner )//if this controller is one we own
			{
				pawnCamera.Transform.LocalRotation = playerControllers[i].eyeAngle.ToRotation();

			}


		}


		averageVector = averageVector.Normal;
		averageEyeAngle = averageEyeAngle / playerControllers.Count;
		pawnCrosshair.Transform.Rotation = Rotation.LookAt( averageVector );
		

	}

	protected override void OnFixedUpdate()
	{
		if ( !IsProxy )
		{

			BuildAverageVelocity();
			Move();

			if ( Transform.Position.z < -500f )
			{

				Log.Info( "You died" );

				Transform.Position = playerSpawn.Transform.Position;
				Transform.Rotation = playerSpawn.Transform.Rotation;

				Death();
			}

			
		}





	}

	[Broadcast]
	public void Death()
	{

		Sound.PlayFile( SoundFile.Load( "sounds/kenney/ui/back_004.vsnd_c" ) );
	}

	void BuildAverageVelocity()
	{

		averageInputVelocity = Vector3.Zero;

		var normalisedVelocity = Vector3.Zero;

		if ( playerControllers.Count > 0 )
		{
			var totalPlayers = playerControllers.Count;

			var minVoteCount = (playerControllers.Count + 1) / 2;
			if ( playerControllers.Count == 2 )
			{
				minVoteCount = 2;
			}


			var forwardCount = 0;
			var backwardCount = 0;
			var leftCount = 0;
			var rightCount = 0;

			for ( int i = 0; i < playerControllers.Count; i++ )
			{
				switch (playerControllers[i].forwardInput )
				{
					case 1:
						forwardCount++;break;
					case -1:
						backwardCount++;break;

				}
				switch ( playerControllers[i].strafeInput )
				{
					case 1:
						leftCount++; break;
					case -1:
						rightCount++; break;

				}

				normalisedVelocity = new Vector3( playerControllers[i].forwardInput, playerControllers[i].strafeInput, 0 ).Normal;

			}


			if ( forwardCount >= minVoteCount || backwardCount >= minVoteCount || leftCount >= minVoteCount || rightCount >= minVoteCount )
			{

				//if any of the inputs are enough to swing the vote, 

				averageInputVelocity += normalisedVelocity;

			}

			

			averageInputVelocity = averageInputVelocity * averageEyeAngle * Speed;//GET RID OF AVERAGE EYE ANGLE HERE!!!!
	




		}

		//Log.Info( "Average Input Velocity: " + averageInputVelocity );
	}

	void Move()
	{
		
		// Get gravity from our scene
		var gravity = Scene.PhysicsWorld.Gravity;

		if ( characterController.IsOnGround )
		{
			// Apply Friction/Acceleration
			characterController.Velocity = characterController.Velocity.WithZ( 0 );
			characterController.Accelerate( averageInputVelocity );
			characterController.ApplyFriction( GroundControl );
		}
		else
		{
			// Apply Air Control / Gravity
			characterController.Velocity += gravity * Time.Delta * 0.5f;
			characterController.Accelerate( averageInputVelocity.ClampLength( MaxForce ) );
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



	[Broadcast]
	public void RefreshPlayerControllers()
	{

	
		playerControllers = Scene.GetAllComponents<PlayerController>().ToList();


		//reset cursors
		for ( int i = 0; i < playerCursors.Count; i++ )
		{
			playerCursors[i].Destroy();
		}

		playerCursors.Clear();

		for ( int i = 0; i < playerControllers.Count; i++ ) 
		{
			var cursorGO = playerCursorPrefab.Clone();
			cursorGO.Parent = pawnCrosshair.Parent;
			cursorGO.Transform.LocalPosition = Vector3.Zero;
			cursorGO.Components.GetInChildren<SpriteRenderer>().Color = playerColours[i];


			cursorGO.Components.GetInChildren<TextRenderer>().Text = playerControllers[i].Network.OwnerConnection.DisplayName;


			playerCursors.Add( cursorGO );
		}
		
	}








}
