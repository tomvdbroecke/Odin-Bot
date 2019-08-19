using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Odin_Bot.Handlers;
using Odin_Bot.Services;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Entities;

namespace Odin_Bot.Services {
    public class AudioService {
        private LavaRestClient _lavaRestClient;
        private LavaSocketClient _lavaSocketClient;
        private DiscordSocketClient _client;
        private LavaPlayer _player;

        public AudioService(LavaRestClient lavaRestClient, DiscordSocketClient client, LavaSocketClient lavaSocketClient) {
            _client = client;
            _lavaRestClient = lavaRestClient;
            _lavaSocketClient = lavaSocketClient;
        }

        public Task InitializeAsync() {
            _client.Ready += ClientReadyAsync;
            _lavaSocketClient.Log += LogAsync;
            _lavaSocketClient.OnTrackFinished += TrackFinished;
            return Task.CompletedTask;
        }

        public async Task ConnectAsync(SocketVoiceChannel voiceChannel, ITextChannel textChannel)
            => await _lavaSocketClient.ConnectAsync(voiceChannel, textChannel);

        public async Task LeaveAsync(SocketVoiceChannel voiceChannel)
            => await _lavaSocketClient.DisconnectAsync(voiceChannel);

        public async Task<Embed> PlayAsync(string query, ulong guildId) {
            _player = _lavaSocketClient.GetPlayer(guildId);
            var results = await _lavaRestClient.SearchYouTubeAsync(query);
            if (results.LoadType == LoadType.NoMatches || results.LoadType == LoadType.LoadFailed) {
                return await EmbedHandler.CreateBasicEmbed("No Results", "Your search returned no results.", Color.Blue);
            }

            var track = results.Tracks.FirstOrDefault();

            if (_player.IsPlaying) {
                _player.Queue.Enqueue(track);
                return await EmbedHandler.CreateMusicQueueEmbed(":arrow_right: " + track.Title + " added to queue", track.Uri.ToString());
            } else {
                await _player.PlayAsync(track);
                return await EmbedHandler.CreateMusicEmbed(":musical_note: Now playing " + track.Title + " :musical_note:", track.Uri.ToString());
            }
        }

        public async Task StopAsync() {
            if (_player is null)
                return;
            await _player.StopAsync();
        }

        public async Task<string> SkipAsync() {
            if (_player is null || _player.Queue.Items.Count() is 0)
                return Config.pre.error + " Nothing in queue.";

            var oldTrack = _player.CurrentTrack;
            await _player.SkipAsync();
            return Config.pre.success + $" Skipped: {oldTrack.Title} \nNow Playing: {_player.CurrentTrack.Title}";
        }

        public async Task<string> SetVolumeAsync(int vol) {
            if (_player is null)
                return Config.pre.error + " Player isn't playing.";

            if (vol > 150 || vol <= 2) {
                return Config.pre.error + " Please use a number between 2 - 150";
            }

            Config.mem.musicVolume = vol;
            var config = new Config();
            await config.SaveMemory();

            await _player.SetVolumeAsync(vol);
            return Config.pre.success + $": Volume set to: {vol}";
        }

        public async Task<string> PauseOrResumeAsync() {
            if (_player is null)
                return Config.pre.error + " Player isn't playing.";

            if (!_player.IsPaused) {
                await _player.PauseAsync();
                return Config.pre.success + " Player is Paused.";
            } else {
                await _player.ResumeAsync();
                return Config.pre.success + " Playback resumed.";
            }
        }

        public async Task<string> ResumeAsync() {
            if (_player is null)
                return Config.pre.error + " Player isn't playing.";

            if (_player.IsPaused) {
                await _player.ResumeAsync();
                return Config.pre.success + " Playback resumed.";
            }

            return Config.pre.error + " Player is not paused.";
        }


        private async Task ClientReadyAsync() {
            await _lavaSocketClient.StartAsync(_client);
        }

        private async Task TrackFinished(LavaPlayer player, LavaTrack track, TrackEndReason reason) {
            if (!reason.ShouldPlayNext())
                return;

            if (!player.Queue.TryDequeue(out var item) || !(item is LavaTrack nextTrack)) {
                await player.TextChannel.SendMessageAsync(Config.pre.error + " There are no more tracks in the queue.");
                return;
            }

            await player.PlayAsync(nextTrack);

            Embed embed = await EmbedHandler.CreateMusicEmbed(":musical_note: Now playing " + nextTrack.Title + " :musical_note:", nextTrack.Uri.ToString());
            await player.TextChannel.SendMessageAsync(null, false, embed);
        }

        private Task LogAsync(LogMessage logMessage) {
            Console.WriteLine(logMessage.Message);
            return Task.CompletedTask;
        }
    }
}