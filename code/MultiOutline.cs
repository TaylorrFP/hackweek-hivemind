using Sandbox;
using System;
using System.Collections.Generic;

public sealed class MultiOutline : Component
{
	[Property] public int outlineCount { get; set; }

	[Property] public Material seeThrough { get; set; }
	[Property] public ModelRenderer sourceModel { get; set; }
	[Property] public List<GameObject> objects { get; set; } = new List<GameObject>();

	[Property] public float outlineWidth { get; set; } 
	[Property] public GameObject cameraObject { get; set; }

	[Property] HighlightOutline originalOutline { get; set; }

	[Sync] public NetList<Color> playerColours { get; set; } = new();

	protected override void OnStart()
	{
		
		base.OnStart();


		playerColours.Add( new Color( 0.956f, 0.262f, 0.211f ) );
		playerColours.Add( new Color( 0.913f, 0.117f, 0.388f ) );
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

		originalOutline = this.Components.Get<HighlightOutline>();
		outlineWidth = originalOutline.Width;


		Shuffle( playerColours );
		AddOutlines();


	}

	protected override void OnUpdate()
	{
		var lookatDir = this.Transform.Position - cameraObject.Transform.Position;
		lookatDir = lookatDir.Normal;

		for ( int i = 0; i < objects.Count; i++ )
		{
			objects[i].Transform.LocalPosition = Vector3.Zero;
			objects[i].Transform.Position += lookatDir * (i+1) * 2f;

		}

	}

	void AddOutlines()
	{

		for ( int i = 0; i <outlineCount; i++ )
		{
			var GO = new GameObject();
			GO.Parent = this.GameObject;
			GO.Transform.LocalPosition = Vector3.Zero;
			GO.Transform.LocalRotation = Rotation.Identity;
			var modelRenderer = GO.Components.Create<ModelRenderer>();
			modelRenderer.Model = sourceModel.Model;
			modelRenderer.Tint = new Color( 0, 0 );
			objects.Add( GO );
			var outline = GO.Components.Create<HighlightOutline>();
			outline.Width = outlineWidth * (i+1) ;
			outline.Color = playerColours[i];

		}

	}





	void Shuffle<T>( NetList<T> inputList )
	{
		for ( int i = 0; i < inputList.Count - 1; i++ )
		{
			T temp = inputList[i];
			int rand = Random.Shared.Int( i, inputList.Count - 1 ); //Random.Range( i, inputList.Count );
			inputList[i] = inputList[rand];
			inputList[rand] = temp;
		}
	}

}
