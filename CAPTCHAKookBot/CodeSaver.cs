namespace CAPTCHAKookBot {
    internal class CodeSaver {
        private static Dictionary<ulong, string> codes = new();

        public static string Generate(ulong user_id) {
            string code = "";
            Random rand = new();
            for (int i = 0; i < 8; i++) 
                code += rand.Next(0, 9).ToString();
            codes.Add(user_id, code);
            return code;
        }

        public static bool Verify(ulong user_id,string inputcode) {
            if (inputcode == null || !codes.ContainsKey(user_id)) return false;
            return codes[user_id] == inputcode;
        }

        public static void Clear(ulong user_id) {
            codes.Remove(user_id);
        }
    }
}
