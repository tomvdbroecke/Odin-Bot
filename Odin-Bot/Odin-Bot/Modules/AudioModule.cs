using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Odin_Bot;
using Odin_Bot.Services;
using System.Threading.Tasks;

namespace Odin_Bot.Modules {
    public class AudioModule : ModuleBase<SocketCommandContext> {
        private AudioService _musicService;

        public AudioModule(AudioService musicService) {
            _musicService = musicService;
        }

        [Command("Join")]
        public async Task Join() {
            var user = Context.User as SocketGuildUser;
            if (user.VoiceChannel is null) {
                await ReplyAsync("You need to connect to a voice channel.");
                return;
            } else {
                await _musicService.ConnectAsync(user.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync($"now connected to {user.VoiceChannel.Name}");
            }
        }

        [Command("Leave")]
        public async Task Leave() {
            var user = Context.User as SocketGuildUser;
            if (user.VoiceChannel is null) {
                await ReplyAsync("Please join the channel the bot is in to make it leave.");
            } else {
                await _musicService.LeaveAsync(user.VoiceChannel);
                await ReplyAsync($"Bot has now left {user.VoiceChannel.Name}");
            }
        }

        [Command("Play")]
        public async Task Play([Remainder]string query) {
            // CHECK IF BOT HAS JOINED THE CHANNEL FIRST

            var result = await _musicService.PlayAsync(query, Context.Guild.Id);
            await ReplyAsync(result);
        }

        [Command("Stop")]
        public async Task Stop() {
            await _musicService.StopAsync();
            await ReplyAsync("Music Playback Stopped.");
        }

        [Command("Skip")]
        public async Task Skip() {
            var result = await _musicService.SkipAsync();
            await ReplyAsync(result);
        }

        [Command("Volume")]
        public async Task Volume(int vol)
            => await ReplyAsync(await _musicService.SetVolumeAsync(vol));

        [Command("Pause")]
        public async Task Pause()
            => await ReplyAsync(await _musicService.PauseOrResumeAsync());

        [Command("Resume")]
        public async Task Resume()
            => await ReplyAsync(await _musicService.ResumeAsync());
    }
}