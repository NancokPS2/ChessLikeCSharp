using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtendedXmlSerializer;
using Godot;

[GlobalClass]
public partial class SoundManager : Node
{
	const string META_KEY_CHANNEL = "SFXManager_META_AudioStreamPlayerChannel";
	protected readonly AudioStream ERROR_STREAM;
	protected readonly AudioStream TEST_STREAM;

	protected static SoundManager Instance = null!;

	protected Dictionary<string, AudioStream> Streams = new(){};
	protected List<AudioStreamPlayer> StreamPlayers = new();

	public SoundManager()
	{
		Instance = this;

		ERROR_STREAM = GD.Load<AudioStream>("uid://d0a1gvcp55p7n") ?? throw new Exception();
		TEST_STREAM = GD.Load<AudioStream>("uid://r2sagyx0kcd1") ?? throw new Exception();

		Streams["ERROR"] = ERROR_STREAM;
		Streams["TEST"] = TEST_STREAM;
	}

	public static void PlayAudio(string streamIdentifier, string playerChannel, float volume)
	{
		PlayAudio(streamIdentifier, playerChannel, new SoundParameters(){Volume = volume});
	}

	public static void PlayAudio(string streamIdentifier, string playerChannel, SoundParameters? parameters)
	{
		parameters ??= new();

		var player = Instance.GetFreePlayer();
		player.Stream = Instance.GetStream(streamIdentifier);
		player.VolumeLinear = parameters.Volume;
		Instance.PlayerSetChannel(player, playerChannel);

		if (!player.IsInsideTree())
			Instance.AddChild(player);

		player.Play();
	}

	public void StopAudio(string playerChannel)
	{
		AudioStreamPlayer player = StreamPlayers.First(x => PlayerGetChannel(x) == playerChannel);
		player?.Stop();
	}
	
	protected AudioStreamPlayer GetFreePlayer()
	{
		AudioStreamPlayer player = StreamPlayers.FirstOrDefault(x => x.GetPlaybackPosition() == 0) ?? CreatePlayer();
		return player;
	}

	protected AudioStream GetStream(string identifier)
	{
		Streams.TryGetValue(identifier, out AudioStream? stream);
		return stream ?? ERROR_STREAM;
	}

	protected AudioStreamPlayer CreatePlayer()
	{
		AudioStreamPlayer newPlayer = new();
		StreamPlayers.Add(newPlayer);
		return newPlayer;
	}


	protected string PlayerGetChannel(AudioStreamPlayer player)
	{
		return player.HasMeta(META_KEY_CHANNEL) ? player.GetMeta(META_KEY_CHANNEL, false).As<string>() : throw new Exception("This AudioStreamPlayer does not have a channel set.");
	}
	protected void PlayerSetChannel(AudioStreamPlayer player, string channel)
	{
		player.SetMeta(META_KEY_CHANNEL, channel);
	}
}
