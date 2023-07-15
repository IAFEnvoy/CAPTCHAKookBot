using System.Text.Json.Serialization;
using System.Timers;

namespace CAPTCHAKookBot.Utils {
    internal abstract class Config {
        private static readonly List<Config> configs = new();
        private static readonly int maxUnsaveCount = 10;
        [JsonIgnore]
        protected readonly string configPath;
        [JsonIgnore]
        private bool shouldSave = false;
        [JsonIgnore]
        private int saveCount = 0;

        static Config() {
            System.Timers.Timer timer = new(60 * 1000);
            timer.Elapsed += new ElapsedEventHandler(SaveAllConfig);
            timer.AutoReset = true;
            timer.Start();
        }

        private static void SaveAllConfig(object? sender, ElapsedEventArgs e) {
            lock (configs) {
                foreach (var config in configs)
                    if (config.shouldSave) {
                        Logger.Info($"正在保存{config.configPath}");
                        config.ForceSave();
                        config.saveCount = 0;
                        config.shouldSave = false;
                    }
            }
        }

        public Config(string path) {
            this.configPath = path;
            if (path == "") return;
            if (File.Exists(this.configPath)) {
                Logger.Info($"正在加载 {this.configPath}");
                this.Load();
            } else
                Logger.Warn($"未找到文件 {this.configPath}");
        }

        public void Save() {
            this.shouldSave = true;
            this.saveCount++;
            if (this.saveCount >= maxUnsaveCount) {
                Logger.Info($"正在保存 {this.configPath}");
                this.ForceSave();
                this.saveCount = 0;
                this.shouldSave = false;
            }
        }

        public abstract void ForceSave();

        public abstract void Load();
    }
}
