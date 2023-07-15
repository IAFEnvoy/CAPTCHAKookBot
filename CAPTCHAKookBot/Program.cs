using CAPTCHAKookBot.Utils;
using Kook;
using Kook.WebSocket;
using System.Runtime.InteropServices;

namespace CAPTCHAKookBot {
    internal class Program {
        public static BotConfig config = new(@$"{Application.StartupPath}\main.json");
        private KookSocketClient client = new();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [STAThread]
        public static Task Main(string[] args) {
            return new Program().MainAsync();
        }

        public async Task MainAsync() {
            AllocConsole();

            this.client.Log += this.Log;
            this.client.MessageReceived += this.Client_MessageReceived;
            this.client.DirectMessageReceived += this.Client_DirectMessageReceived;
            this.client.MessageButtonClicked += this.Client_MessageButtonClicked;
            this.client.DirectMessageButtonClicked += this.Client_DirectMessageButtonClicked;

            if (config.kookBotToken == "") {
                Logger.Error("Œ¥’“µΩtoken");
                Environment.Exit(0);
            }
            await this.client.LoginAsync(TokenType.Bot, config.kookBotToken);
            await this.client.StartAsync();

            await Task.Delay(Timeout.Infinite);
        }

        private async Task Client_DirectMessageReceived(SocketMessage message, SocketUser user, SocketDMChannel channel) {
            Logger.Message($"<- DIRECT TEXT {message.Author.Username} [{message.Author.IsBot}] ({message.Author.Id}) {message.Content}");
            if (message.Author.IsBot ?? false || message.Type == MessageType.Card) return;
            if (CodeSaver.Verify(message.Author.Id, message.Content)) {
                await this.client.GetUser(message.Author.Id).SendCardAsync(CAPTCHA.GetSuccessCard());
                CodeSaver.Clear(message.Author.Id);
                await this.client.Rest.AddRoleAsync(3604049251197716, message.Author.Id, 23220829);
            } else {
                await this.client.GetUser(message.Author.Id).SendCardAsync(CAPTCHA.GetFailCard());
                CodeSaver.Clear(message.Author.Id);
            }
            return;
        }

        private Task Client_MessageReceived(SocketMessage message, SocketGuildUser user, SocketTextChannel channel) {
            Logger.Message($"<- TEXT {message.Author.Username} ({message.Author.Id}) [{message.Channel.Name}] {message.Content}");
            if (message.Author.IsBot ?? false) return Task.CompletedTask;
            if (message.Content == "/whoisnigge") return message.Channel.SendTextAsync("NotLegit");
            if (message.Content == "/verify") return message.Channel.SendCardAsync(CAPTCHA.GetVerifyCard());
            return Task.CompletedTask;
        }

        private async Task Client_DirectMessageButtonClicked(string value, Cacheable<SocketUser, ulong> user, Cacheable<IMessage, Guid> message, SocketDMChannel channel) {
            SocketUser duser = await user.DownloadAsync();
            if (duser.IsBot ?? false) return;
            Logger.Message($"<- DIRECT BUTTON {duser.Username} ({duser.Id}) {value}");
            if (value == "verify_new")
                await this.client.GetUser(duser.Id).SendCardAsync(CAPTCHA.GetCodeCard(CodeSaver.Generate(duser.Id)));
            return;
        }

        private async Task Client_MessageButtonClicked(string value, Cacheable<SocketGuildUser, ulong> user, Cacheable<IMessage, Guid> message, SocketTextChannel channel) {
            SocketGuildUser guser = await user.GetOrDownloadAsync();
            Logger.Message($"<- BUTTON {guser.Nickname} ({guser.Id}) [{channel.Name}] {value}");
            if (value == "verify") {
                string url = await this.client.Rest.CreateAssetAsync(CAPTCHA.GenerateImg(CodeSaver.Generate(guser.Id)), "111");
                await channel.SendCardAsync(CAPTCHA.GetToDmCard(), null, guser);
                IUser dmUser = await this.client.GetUserAsync(guser.Id);
                await dmUser.SendCardAsync(CAPTCHA.GetCodeCard(url));
            }
            return;
        }

        private Task Log(LogMessage msg) {
            Logger.Info(msg.ToString());
            return Task.CompletedTask;
        }
    }
}