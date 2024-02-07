using Sandbox;
using System;
using System.Collections.Generic;

public sealed class GameNetworkManager : Component, Component.INetworkListener
{
	[Property] public GameObject playerControllerPrefab { get; set; } 
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

		var playerControllerGO = playerControllerPrefab.Clone( SpawnPoint.Transform.World );
		playerControllerGO.NetworkSpawn( connection );

		
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
