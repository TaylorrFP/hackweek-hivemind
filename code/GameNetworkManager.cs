using Sandbox;
using System;
using System.Collections.Generic;

public sealed class GameNetworkManager : Component, Component.INetworkListener
{
	[Property] public GameObject PlayerPrefab { get; set; } 
	[Property] public GameObject SpawnPoint { get; set; }

	[Property] public PlayerPawn playerPawn { get; set; }

	[Property] public List<GameObject> playerControllers { get; set; }
	/// <summary>
	/// Called on the host when someone successfully joins the server (including the local player)
	/// </summary>
	/// 


	protected override void OnStart()
	{
		playerControllers = new List<GameObject>();
	}

		public void OnActive( Connection connection )
	{

		var playerGO = PlayerPrefab.Clone( SpawnPoint.Transform.World );
		playerGO.NetworkSpawn( connection );

		//playerControllers.Add( playerGO );

		//Log.Info( connection.DisplayName + " Connected" );

		

		if ( connection.IsHost )
		{
			//Log.Info( connection.DisplayName + " is Host" );

		}
		else
		{

			//Log.Info( connection.DisplayName + " is not Host" );
		}


		playerPawn.RefreshPlayerControllers();
	




	

	}

	public void OnDisconnected( Connection connection )
	{
		for ( int i = 0; i < playerPawn.playerControllers.Count; i++ )
		{
			if ( playerPawn.playerControllers[i].Network.OwnerConnection == connection )
			{
				playerPawn.playerControllers[i].Destroy();

			}
		}

		playerPawn.RefreshPlayerControllers();

	}



}
