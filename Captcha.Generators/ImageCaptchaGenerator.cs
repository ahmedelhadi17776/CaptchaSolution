using System.Drawing;
using System.Drawing.Imaging;
using Captcha.Core.Interfaces;
using Captcha.Core.Models;
using Captcha.Core.Enums;

namespace Captcha.Generators;

public class ImageCaptchaGenerator : ICaptchaGenerator
{
    public CaptchaType Type => CaptchaType.Image;

    public ICaptcha Generate()
    {
        var textGenerator = new TextCaptchaGenerator();
        var textCaptcha = textGenerator.Generate();

        using var bmp = new Bitmap(200, 60);
        using var g = Graphics.FromImage(bmp);

        g.Clear(Color.White);

        // Add noise
        var random = new Random();
        for (int i = 0; i < 10; i++)
        {
            g.DrawLine(Pens.Gray,
                random.Next(200), random.Next(60),
                random.Next(200), random.Next(60));
        }

        // Draw text
        using var font = new Font("Arial", 24, FontStyle.Bold);
        g.TranslateTransform(30, 30);
        g.RotateTransform(random.Next(-15, 15));
        g.DrawString(textCaptcha.Challenge, font, Brushes.Black, 0, 0);

        using var ms = new MemoryStream();
        bmp.Save(ms, ImageFormat.Png);

        return new CaptchaModel
        {
            Challenge = textCaptcha.Challenge,
            Answer = textCaptcha.Answer,
            ImageData = ms.ToArray(),
            Type = this.Type 
        };
    }
}
