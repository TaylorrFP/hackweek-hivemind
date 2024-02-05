using Sandbox;
using System;

public sealed class GameNetworkManager : Component, Component.INetworkListener
{
	[Property] public GameObject PlayerPrefab { get; set; } 
	[Property] public GameObject SpawnPoint { get; set; }

	[Property] public PlayerPawn playerPawn { get; set; }

	/// <summary>
	/// Called on the host when someone successfully joins the server (including the local player)
	/// </summary>
	/// 


	public void OnActive( Connection connection )
	{

		var playerGO = PlayerPrefab.Clone( SpawnPoint.Transform.World );
		playerGO.NetworkSpawn( connection );
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
		

	}



}
