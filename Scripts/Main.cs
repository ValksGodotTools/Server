global using Godot;
global using GodotUtils;
global using System;

namespace Multiplayer;

public partial class Main : Node
{
	[Export] public NodePath NodePathTest { get; set; }

	public override void _Ready()
	{
		
	}

	public override void _PhysicsProcess(double delta)
	{
		Logger.Update();
	}

	public override async void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
		{
			GetTree().AutoAcceptQuit = false;
			await Cleanup();
		}
	}

	private async Task Cleanup()
	{
		await CleanupNet();

		if (Logger.StillWorking())
			await Task.Delay(1);

		GetTree().Quit();
	}

	private async Task CleanupNet()
	{
		if (ENetLow.ENetInitialized)
		{
			if (Net.Server != null)
			{
				Net.Server.Stop();

				while (Net.Server.IsRunning)
					await Task.Delay(1);
			}

			ENet.Library.Deinitialize();
		}
	}

	private void _on_start_server_pressed()
	{
		if (Net.Server.IsRunning)
		{
			Net.Server.Log("The server is running already");
			return;
		}

		var ignoredPackets = new Type[]
		{
			typeof(CPacketPlayerPosition)
		};

		Net.Server.Start(25565, 100, new ENetOptions
		{
			PrintPacketData = false
		}, ignoredPackets);
	}
}
