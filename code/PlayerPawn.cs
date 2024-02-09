using Sandbox;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

public class PlayerPawn : Component
{
	[Sync] public NetList<Color> playerColours { get; set; } = new();

	//string[] playerEmojis = new string[19];

	[Property] public GameObject playerCursorPrefab { get; set; }

	[Property] public GameObject playerSpawn { get; set; }

	[Property] public GameObject playerHead { get; set; }
	[Property] public GameObject headPosition { get; set; }
	[Property] public List<PlayerController> playerControllers { get; set; } = new();

	[Property] public List<GameObject> playerCursors { get; set; } = new();

	[Property] public GameObject pawnCrosshair { get; set; }

	[Property] public CameraComponent pawnCamera { get; set; }

	[Property] public Angles averageMoveAngle { get; set; }

	[Property] public Vector3 averageViewVector { get; set; }

	[Property] public Vector3 averageInputVelocity { get; set; }

	[Property] public Angles localViewAngle { get; set; }

	[Property] public Angles localAltAngle { get; set; }

	[Property] Vector3 cameraTarget;

	[Property] [Sync] public Vector3 normalisedVelocity { get; set; }

	[Property] public CharacterController characterController;

	[Property] public int forwardCount { get; set; } = 0;
	[Property] public int backwardCount { get; set; } = 0;
	[Property] public int leftCount { get; set; } = 0;
	[Property] public int rightCount { get; set; } = 0;

	[Property] public int totalPlayers { get; set; } = 0;

	[Property] public int minVoteCount { get; set; } = 0;


	
	[Property] public GameObject forwardSound { get; set; }
	[Property] public GameObject backwardSound { get; set; }
	[Property] public GameObject leftSound { get; set; }
	[Property] public GameObject rightSound { get; set; }

	[Property] public SoundEvent movementSounds { get; set; }

	[Property] public SoundEvent jumpSounds { get; set; }

	[Property] public SoundEvent goSounds { get; set; }

	[Property] public SoundEvent nogoSounds { get; set; }

	[Property] public  float movementPitchOffset { get; set; } = 0.1f;
	[Property] public float minMovementPitch { get; set; } = 0f;
	[Property] public float maxMovementPitch { get; set; } = 0.5f;

	[Property] public float minMovementVolume { get; set; } = 0.5f;
	[Property] public float maxMovementVolume { get; set; } = 1f;

	[Property] public float minJumpVolume { get; set; } = 0.5f;
	[Property] public float maxJumpVolume { get; set; } = 1f;


	[Property] public float minJumpPitch { get; set; } = 0.2f;
	[Property] public float maxJumpPitch { get; set; } =1f;

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
	bool hasJumped = false;

	//Vector3 Position

	void Shuffle<T>( NetList<T> inputList )
	{
		for ( int i = 0; i < inputList.Count - 1; i++ )
		{
			T temp = inputList[i];
			int rand = Random.Shared.Int( i, inputList.Count-1 ); //Random.Range( i, inputList.Count );
			inputList[i] = inputList[rand];
			inputList[rand] = temp;
		}
	}

	protected override void OnAwake()
	{
	


		base.OnAwake();


		//do this better in the future
		playerColours.Add( new Color( 0.956f, 0.262f, 0.211f ));
		playerColours.Add( new Color( 0.913f, 0.117f, 0.388f) );
		playerColours.Add( new Color( 0.611f, 0.152f, 0.690f ) );
		playerColours.Add( new Color( 0.403f, 0.227f, 0.717f ) );
		playerColours.Add( new Color( 0.247f, 0.317f, 0.709f ) );
		playerColours.Add( new Color( 0.129f, 0.588f, 0.952f ) );
		playerColours.Add( new Color( 0.011f, 0.662f, 0.956f ) );
		playerColours.Add( new Color( 0f, 0.737f, 0.831f ) );
		playerColours.Add( new Color( 0f, 0.588f, 0.533f ) );
		playerColours.Add( new Color( 0.298f, 0.298f, 0.313f ) );
		playerColours.Add( new Color( 0.545f, 0.764f, 0.290f ) );
		playerColours.Add( new Color( 0.803f, 0.862f, 0.223f ) );
		playerColours.Add( new Color( 1f, 0.921f, 0.231f ) );
		playerColours.Add( new Color( 1f, 0.756f, 0.02f ) );
		playerColours.Add( new Color( 1f, 0.596f, 0f ) );
		playerColours.Add( new Color( 1f, 0.341f, 0.133f ) );
		playerColours.Add( new Color( 0.474f, 0.333f, 0.282f ) );
		playerColours.Add( new Color( 0.619f, 0.619f, 0.619f ) );
		playerColours.Add( new Color( 0.376f, 0.490f, 0.545f ) );


		if ( !IsProxy )
		{

			//shuffle the list of colours
			//but make sure this is networked

			Shuffle( playerColours );
			

		}


		//playerEmojis[0] = "🍎";
		//playerEmojis[1] = "🐷";
		//playerEmojis[2] = "🍇";
		//playerEmojis[3] = "💣";





	}

	protected override void OnUpdate()
	{


			//jumping code

			if ( characterController.IsOnGround == true )
		{
			hasJumped = false;

		}
		if ( hasJumped != true & Input.Pressed( "Jump" ) )
		{

			jump();
			hasJumped = true;


		}

		BuildViewDirection();

	}

	private void BuildViewDirection()
	{
		averageMoveAngle = Angles.Zero;
		averageViewVector = Vector3.Zero;
		var localForwardInput = 0f;
		var localStrafeInput = 0f;


		for ( int i = 0; i < playerControllers.Count; i++ )
		{
			averageViewVector += playerControllers[i].eyeAngle.Forward;

			if ( playerControllers[i].Network.IsOwner )//if this controller is one we own
			{
				localAltAngle = playerControllers[i].altEyeAngle;
				localViewAngle = playerControllers[i].eyeAngle;


				localForwardInput = playerControllers[i].forwardInput;
				localStrafeInput = playerControllers[i].strafeInput;
				//var myViewDir = new Angles( 0, playerControllers[i].eyeAngle.yaw, 0 ); //get the angle of my view, we only want the pitch though

				if ( Input.Pressed( "Jump" ) )
				{
					playerControllers[i].jumpTimer = 0.5f;

				}
			}
		}
		averageViewVector = averageViewVector.Normal;
		averageMoveAngle = new Angles( 0, Rotation.LookAt( averageViewVector ).Angles().yaw, 0 );
		cameraTarget = new Vector3( localForwardInput, localStrafeInput, 0f ).Normal * averageMoveAngle * 5;
	}

	[Broadcast]
	public void jump()
	{
		if ( !IsProxy )
		{

			if ( characterController.Velocity.z<0f )//if we're falling
			{
			
				characterController.Velocity = new Vector3( characterController.Velocity.x, characterController.Velocity.y,0 );
				characterController.Punch( Vector3.Up * JumpForce / playerControllers.Count );
			}
			else
			{

				characterController.Punch( Vector3.Up * JumpForce / playerControllers.Count );
			}

			
			
		}

		Log.Info( "jumped" );
		var pitch = 0.3f;
		var numJumpers = 0;
		var volume = 1f;


		for ( int i = 0; i < playerControllers.Count; i++ )
		{

			if ( playerControllers[i].jumpTimer > 0.01f )
			{
				numJumpers++;

			}



		}



		pitch = MathX.Lerp( minJumpPitch, maxJumpPitch, numJumpers * 1.0f / totalPlayers * 1.0f );
		volume = MathX.Lerp( minJumpVolume, maxJumpVolume, numJumpers * 1.0f / totalPlayers * 1.0f );

		jumpSounds.Pitch = pitch;
		jumpSounds.Volume = volume;
		Log.Info( jumpSounds.Volume );
		Log.Info( jumpSounds.Pitch );

		//Sound.Play( jumpSounds, this.Transform.Position ); //WHY DOESN'T THIS WORK ON CLIENT??
		Sound.PlayFile( SoundFile.Load( "sounds/kenney/ui/drop_004.vsnd" ),volume,pitch );

	}

	protected override void OnFixedUpdate()
	{
		BuildAverageVelocity();

		if ( !IsProxy )
		{

			
			Move();

			if ( Transform.Position.z < -500f )
			{
				Death();
			}


		}





	}

	public void Death()
	{
		if ( !IsProxy )
		{
			Log.Info( "You died" );

			Transform.Position = playerSpawn.Transform.Position;
			Transform.Rotation = playerSpawn.Transform.Rotation;

			DeathSound();
		}
	}

	protected override void OnPreRender()
	{
		base.OnPreRender();


		playerHead.Transform.Position = Vector3.Lerp( playerHead.Transform.Position, headPosition.Transform.Position, Time.Delta * 10f );
		pawnCamera.Transform.LocalPosition = Vector3.Lerp( pawnCamera.Transform.LocalPosition, cameraTarget, Time.Delta * 5f );
		pawnCamera.Transform.LocalRotation = new Angles( localViewAngle + localAltAngle ).ToRotation();
		for ( int i = 0; i < playerControllers.Count; i++ )
		{
			playerCursors[i].Transform.LocalPosition = pawnCamera.Transform.LocalPosition;//do this in prerender so it doesn't lag?
			playerCursors[i].Transform.LocalRotation = playerControllers[i].eyeAngle.ToRotation();
		}

	

		
		pawnCrosshair.Transform.Rotation = Rotation.LookAt( averageViewVector );
		pawnCrosshair.Transform.LocalPosition = pawnCamera.Transform.LocalPosition;

		
	}

	[Broadcast]
	public void DeathSound()
	{

		Sound.PlayFile( SoundFile.Load( "sounds/kenney/ui/back_004.vsnd_c" ) );
	}

	void BuildAverageVelocity()
	{

		averageInputVelocity = Vector3.Zero;
		normalisedVelocity = Vector3.Zero;

		if ( playerControllers.Count > 0 )
		{
			totalPlayers = playerControllers.Count;
			minVoteCount = (playerControllers.Count + 1) / 2;//set minVoteCount



			if ( playerControllers.Count < 3 )//if there are only two require them both to vote, or one player doesn't need to vote
			{
				minVoteCount = totalPlayers;
			}


			var tempforwardCount = 0;
			var tempbackwardCount = 0;
			var templeftCount = 0;
			var temprightCount = 0;

			for ( int i = 0; i < playerControllers.Count; i++ )
			{

				//first go through and add up all the votes and add them to their respective counts
				switch (playerControllers[i].forwardInput )
				{
					case 1:
						tempforwardCount++;break;
					case -1:
						tempbackwardCount++;break;

				}
				switch ( playerControllers[i].strafeInput )
				{
					case 1:
						templeftCount++; break;
					case -1:
						temprightCount++; break;

				}
			}
			var pitch = 0f;
			var volume = 0f;


			if ( tempforwardCount != forwardCount )
			{
				forwardCount = tempforwardCount;


				


				if ( forwardCount == 0 )
				{
					Sound.PlayFile( SoundFile.Load( "sounds/kenney/ui/bong_001.vsnd" ), 0.2f, 0.4f );
				}
				else
				{

					
						pitch = MathX.Lerp( minMovementPitch, maxMovementPitch, forwardCount*1.0f / totalPlayers*1.0f );
						volume = MathX.Lerp( minMovementVolume, maxMovementVolume, forwardCount * 1.0f / totalPlayers * 1.0f );
						pitch += movementPitchOffset * 2;
						movementSounds.Pitch = pitch;
						movementSounds.Volume = volume;
						Sound.Play( movementSounds, forwardSound.Transform.Position );
						Sound.PlayFile( SoundFile.Load( "sounds/kenney/ui/drop_003.vsnd" ), volume, pitch );





				}
			}

			if ( tempbackwardCount != backwardCount )
			{
				backwardCount = tempbackwardCount;

				if ( backwardCount == 0 )
				{
					Sound.PlayFile( SoundFile.Load( "sounds/kenney/ui/bong_001.vsnd" ), 0.2f, 0.4f );
				}
				else
				{

					pitch = MathX.Lerp( minMovementPitch, maxMovementPitch, backwardCount * 1.0f / totalPlayers * 1.0f );
					volume = MathX.Lerp( minMovementVolume, maxMovementVolume, backwardCount * 1.0f / totalPlayers * 1.0f );
					movementSounds.Pitch = pitch;
					movementSounds.Volume = volume;
					Sound.PlayFile( SoundFile.Load( "sounds/kenney/ui/drop_003.vsnd" ), volume, pitch );

				}




			}
			if ( templeftCount != leftCount )
			{




				leftCount = templeftCount;
				if ( leftCount == 0 )
				{
					Sound.PlayFile( SoundFile.Load( "sounds/kenney/ui/bong_001.vsnd" ), 0.2f, 0.4f );
				}
				else
				{

						pitch = MathX.Lerp( minMovementPitch, maxMovementPitch, leftCount * 1.0f / totalPlayers * 1.0f );
						volume = MathX.Lerp( minMovementVolume, maxMovementVolume, leftCount * 1.0f / totalPlayers * 1.0f );
						pitch += movementPitchOffset;
						movementSounds.Pitch = pitch;
						movementSounds.Volume = volume;
						Sound.PlayFile( SoundFile.Load( "sounds/kenney/ui/drop_003.vsnd" ), volume, pitch );




				}


			}
			if ( temprightCount != rightCount )
			{
				
				rightCount = temprightCount;
				if ( rightCount == 0 )
				{
					//Sound.Play( nogoSounds, backwardSound.Transform.Position );
					Sound.PlayFile( SoundFile.Load( "sounds/kenney/ui/bong_001.vsnd"), 0.2f,0.4f );
				}
				else
				{



					
						pitch = MathX.Lerp( minMovementPitch, maxMovementPitch, rightCount * 1.0f / totalPlayers * 1.0f );
						volume = MathX.Lerp( minMovementVolume, maxMovementVolume, rightCount * 1.0f / totalPlayers * 1.0f );
						pitch += movementPitchOffset;
						movementSounds.Pitch = pitch;
						movementSounds.Volume = volume;
						Sound.PlayFile( SoundFile.Load( "sounds/kenney/ui/drop_003.vsnd" ), volume, pitch );



				}




			}

			if ( forwardCount >= minVoteCount )
			{averageInputVelocity = new Vector3 (forwardCount /totalPlayers, averageInputVelocity.y, 0);}
			if ( backwardCount >= minVoteCount )
			{averageInputVelocity = new Vector3( -backwardCount / totalPlayers, averageInputVelocity.y, 0 );}
			if ( leftCount >= minVoteCount )
			{averageInputVelocity = new Vector3( averageInputVelocity.x, leftCount / totalPlayers, 0 );}
			if ( rightCount >= minVoteCount )
			{averageInputVelocity = new Vector3( averageInputVelocity.x, -rightCount / totalPlayers, 0 );}

			averageInputVelocity = averageInputVelocity.Normal * averageMoveAngle * Speed;

			



		}
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

			totalPlayers = playerCursors.Count;
		}
		
	}








}
