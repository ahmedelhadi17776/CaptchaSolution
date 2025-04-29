using Captcha.Core.Interfaces;
using Captcha.Core.Models;
using Captcha.Core.Enums;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Captcha.Generators;

public class ReCaptchaGenerator : ICaptchaGenerator
{
    public CaptchaType Type => CaptchaType.reCAPTCHA;

    public ICaptcha Generate()
    {
        using var bmp = new Bitmap(200, 150);
        using var g = Graphics.FromImage(bmp);

        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

        // Fill background
        g.Clear(Color.White);

        // Draw main border
        using (var pen = new Pen(Color.FromArgb(220, 220, 220), 1))
        {
            g.DrawRectangle(pen, 1, 1, bmp.Width - 3, bmp.Height - 3);
        }

        // Draw reCAPTCHA logo
        using (var font = new Font("Arial", 12, FontStyle.Bold))
        {
            g.DrawString("reCAPTCHA", font, Brushes.DarkGray, 20, 10);
        }

        // Draw checkbox with border
        using (var checkboxBorder = new Pen(Color.FromArgb(180, 180, 180), 1))
        {
            // Draw checkbox border
            g.DrawRectangle(checkboxBorder, 20, 60, 20, 20);

            // Draw inner white background
            using (var whiteBrush = new SolidBrush(Color.White))
            {
                g.FillRectangle(whiteBrush, 21, 61, 18, 18);
            }
        }

        // Draw "I'm not a robot" text with better positioning
        using (var font = new Font("Arial", 11))
        {
            g.DrawString("I'm not a robot", font, Brushes.DarkGray, 50, 62);
        }

        // Add reCAPTCHA version and powered by text
        using (var font = new Font("Arial", 8))
        {
            g.DrawString("reCAPTCHA", font, Brushes.Gray, 140, 100);
            g.DrawString("Privacy - Terms", font, Brushes.Gray, 140, 115);
        }

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
