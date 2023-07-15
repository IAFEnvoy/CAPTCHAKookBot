using Kook;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Color = System.Drawing.Color;

namespace CAPTCHAKookBot {
    internal class CAPTCHA {

        /// <summary>
        /// 生成验证码图片
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static MemoryStream GenerateImg(string code) {
            Bitmap image = new(code.Length * 10, 25);
            Graphics g = Graphics.FromImage(image);
            try {
                //清空图片背景色
                g.Clear(Color.White);

                //增加背景干扰线
                Random random = new();
                for (int i = 0; i < 30; i++) {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    //颜色可自定义
                    g.DrawLine(new(Color.FromArgb(186, 212, 231)), x1, y1, x2, y2);
                }

                //定义验证码字体
                Font font = new("Arial", 10, (FontStyle.Bold | FontStyle.Italic | FontStyle.Strikeout));
                //定义验证码的刷子，这里采用渐变的方式，颜色可自定义
                LinearGradientBrush brush = new(new(0, 0, image.Width, image.Height), Color.FromArgb(67, 93, 230), Color.FromArgb(70, 128, 228), 1.5f, true);

                //增加干扰点
                for (int i = 0; i < 100; i++) {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    //颜色可自定义
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }

                //将验证码写入图片
                g.DrawString(code, font, brush, 5, 5);

                //图片边框
                g.DrawRectangle(new(Color.FromArgb(93, 142, 228)), 0, 0, image.Width - 1, image.Height - 1);

                //保存图片数据
                MemoryStream stream = new();
                image.Save(stream, ImageFormat.Png);
                return stream;
            } finally {
                g.Dispose();
                image.Dispose();
            }
        }

        public static ICard GetVerifyCard() {
            CardBuilder builder = new();
            builder.WithTheme(CardTheme.Info);
            builder.AddModule(new ContextModuleBuilder().AddElement(new KMarkdownElementBuilder().WithContent("🤖")).AddElement(new PlainTextElementBuilder().WithContent("服务器人机验证")));
            builder.AddModule(new DividerModuleBuilder());
            builder.AddModule(new HeaderModuleBuilder().WithText("我们怀疑你是人，请通过我们的机器人验证"));
            builder.AddModule(new SectionModuleBuilder().WithText("*点击下方按钮进行验证*",true));
            builder.AddModule(new ActionGroupModuleBuilder().AddElement(new ButtonElementBuilder().WithTheme(ButtonTheme.Success).WithText("我是人类").WithValue("verify").WithClick(ButtonClickEventType.ReturnValue)));
            builder.AddModule(new DividerModuleBuilder());
            builder.AddModule(new ContextModuleBuilder().AddElement(new PlainTextElementBuilder().WithContent("Powered By IAFEnvoy")));
            return builder.Build();
        }

        public static ICard GetToDmCard() {
            CardBuilder builder = new();
            builder.WithTheme(CardTheme.Info);
            builder.AddModule(new ContextModuleBuilder().AddElement(new KMarkdownElementBuilder().WithContent("🤖")).AddElement(new PlainTextElementBuilder().WithContent("服务器人机验证")));
            builder.AddModule(new DividerModuleBuilder());
            builder.AddModule(new HeaderModuleBuilder().WithText("请前往私聊界面完成验证"));
            builder.AddModule(new DividerModuleBuilder());
            builder.AddModule(new ContextModuleBuilder().AddElement(new PlainTextElementBuilder().WithContent("Powered By IAFEnvoy")));
            return builder.Build();
        }

        public static ICard GetCodeCard(string image_url) {
            CardBuilder builder = new();
            builder.WithTheme(CardTheme.Info);
            builder.AddModule(new ContextModuleBuilder().AddElement(new KMarkdownElementBuilder().WithContent("🤖")).AddElement(new PlainTextElementBuilder().WithContent("服务器人机验证")));
            builder.AddModule(new DividerModuleBuilder());
            builder.AddModule(new HeaderModuleBuilder().WithText("请输入验证码"));
            builder.AddModule(new ContainerModuleBuilder().AddElement(new ImageElementBuilder().WithSource(image_url).WithSize(ImageSize.Large)));
            builder.AddModule(new ActionGroupModuleBuilder().AddElement(new ButtonElementBuilder().WithTheme(ButtonTheme.Info).WithText("看不清？换一张").WithValue("verify_new").WithClick(ButtonClickEventType.ReturnValue)));
            builder.AddModule(new DividerModuleBuilder());
            builder.AddModule(new ContextModuleBuilder().AddElement(new PlainTextElementBuilder().WithContent("Powered By IAFEnvoy")));
            return builder.Build();
        }

        public static ICard GetSuccessCard() {
            CardBuilder builder = new();
            builder.WithTheme(CardTheme.Success);
            builder.AddModule(new ContextModuleBuilder().AddElement(new KMarkdownElementBuilder().WithContent("🤖")).AddElement(new PlainTextElementBuilder().WithContent("服务器人机验证")));
            builder.AddModule(new DividerModuleBuilder());
            builder.AddModule(new HeaderModuleBuilder().WithText("验证通过！"));
            builder.AddModule(new SectionModuleBuilder().WithText("**欢迎来到此服务器**", true));
            builder.AddModule(new DividerModuleBuilder());
            builder.AddModule(new ContextModuleBuilder().AddElement(new PlainTextElementBuilder().WithContent("Powered By IAFEnvoy")));
            return builder.Build();
        }

        public static ICard GetFailCard() {
            CardBuilder builder = new();
            builder.WithTheme(CardTheme.Danger);
            builder.AddModule(new ContextModuleBuilder().AddElement(new KMarkdownElementBuilder().WithContent("🤖")).AddElement(new PlainTextElementBuilder().WithContent("服务器人机验证")));
            builder.AddModule(new DividerModuleBuilder());
            builder.AddModule(new HeaderModuleBuilder().WithText("验证失败！"));
            builder.AddModule(new SectionModuleBuilder().WithText("**你难道真的不是人？**", true));
            builder.AddModule(new DividerModuleBuilder());
            builder.AddModule(new ContextModuleBuilder().AddElement(new PlainTextElementBuilder().WithContent("Powered By IAFEnvoy")));
            return builder.Build();
        }
    }
}
