using Captcha.Core.Interfaces;
using Captcha.Core.Models;
using Captcha.Core.Enums;
using System.Drawing;
using System.Drawing.Imaging;

namespace Captcha.Generators;

public class ReCaptchaGenerator : ICaptchaGenerator
{
    public CaptchaType Type => CaptchaType.reCAPTCHA;

    public ICaptcha Generate()
    {
        // Create a simple image with "I'm not a robot" text
        using var bmp = new Bitmap(200, 150);
        using var g = Graphics.FromImage(bmp);

        // Fill background
        g.Clear(Color.WhiteSmoke);

        // Draw checkbox
        g.DrawRectangle(Pens.Gray, 20, 60, 20, 20);

        // Draw text
        using var font = new Font("Arial", 12);
        g.DrawString("I'm not a robot", font, Brushes.Black, 50, 60);

        // Add reCAPTCHA logo
        g.DrawString("reCAPTCHA", new Font("Arial", 8), Brushes.Gray, 140, 120);

        using var ms = new MemoryStream();
        bmp.Save(ms, ImageFormat.Png);

        return new CaptchaModel
        {
            Challenge = "Click checkbox to verify",
            Answer = "true",
            ImageData = ms.ToArray(),
            Type = Type
        };
    }
}
