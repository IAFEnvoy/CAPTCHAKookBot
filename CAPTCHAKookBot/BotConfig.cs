using CAPTCHAKookBot.Utils;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace CAPTCHAKookBot {
    internal class BotConfig : Config {
        [JsonInclude]
        [JsonPropertyName("token")]
        public string kookBotToken = "";
        [JsonInclude]
        [JsonPropertyName("admins")]
        public List<ulong> botAdmins = new();

        public BotConfig() : base("") { }

        public BotConfig(string path) : base(path) { }

        public override void ForceSave() {
            StreamWriter sw = new(this.configPath, false);
            sw.Write(JsonSerializer.Serialize(this));
            sw.Close();
        }

        public override void Load() {
            StreamReader sr = new(this.configPath);
            BotConfig another = JsonSerializer.Deserialize<BotConfig>(sr.ReadToEnd()) ?? new();
            this.CopyFrom(another);
            sr.Close();
        }

        private void CopyFrom(BotConfig another) {
            this.kookBotToken = another.kookBotToken;
            this.botAdmins = another.botAdmins;
        }
    }
}
