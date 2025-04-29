using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Captcha.Core.Interfaces;
using Captcha.Core.Models;
using Captcha.Core.Enums;

namespace Captcha.Generators;

public class ImageCaptchaGenerator : ICaptchaGenerator
{
    private readonly Random _random = new();
    public CaptchaType Type => CaptchaType.Image;

    public ICaptcha Generate()
    {
        var textGenerator = new TextCaptchaGenerator();
        var textCaptcha = textGenerator.Generate();

        using var bmp = new Bitmap(200, 60);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

        // Background with gradient
        using var brush = new LinearGradientBrush(
            new Rectangle(0, 0, bmp.Width, bmp.Height),
            GetRandomLightColor(),
            GetRandomLightColor(),
            (float)_random.NextDouble() * 360);
        g.FillRectangle(brush, new Rectangle(0, 0, bmp.Width, bmp.Height));

        // Add noise patterns
        AddNoisePatterns(g, bmp.Width, bmp.Height);

        // Add random shapes
        AddRandomShapes(g, bmp.Width, bmp.Height);

        // Draw text with wave effect
        DrawWaveText(g, textCaptcha.Challenge);

        // Add noise dots
        AddNoiseDots(g, bmp.Width, bmp.Height);

        using var ms = new MemoryStream();
        bmp.Save(ms, ImageFormat.Png);

        return new CaptchaModel
        {
            Challenge = textCaptcha.Challenge,
            Answer = textCaptcha.Answer,
            ImageData = ms.ToArray(),
            Type = Type
        };
    }

    private Color GetRandomLightColor()
    {
        return Color.FromArgb(
            _random.Next(180, 256),
            _random.Next(180, 256),
            _random.Next(180, 256));
    }

    private void AddNoisePatterns(Graphics g, int width, int height)
    {
        using var pen = new Pen(Color.FromArgb(30, 0, 0, 0));
        for (int i = 0; i < 20; i++)
        {
            g.DrawBezier(pen,
                new Point(_random.Next(width), _random.Next(height)),
                new Point(_random.Next(width), _random.Next(height)),
                new Point(_random.Next(width), _random.Next(height)),
                new Point(_random.Next(width), _random.Next(height)));
        }
    }

    private void AddRandomShapes(Graphics g, int width, int height)
    {
        for (int i = 0; i < 5; i++)
        {
            int x = _random.Next(width);
            int y = _random.Next(height);
            int size = _random.Next(5, 15);

            using var pen = new Pen(Color.FromArgb(50, _random.Next(256), _random.Next(256), _random.Next(256)));
            if (_random.Next(2) == 0)
                g.DrawEllipse(pen, x, y, size, size);
            else
                g.DrawRectangle(pen, x, y, size, size);
        }
    }

    private void DrawWaveText(Graphics g, string text)
    {
        using var font = new Font("Arial", 24, FontStyle.Bold);
        float x = 20;
        float y = 15;

        foreach (char c in text)
        {
            // Rotate each character slightly
            float angle = _random.Next(-15, 15);
            g.TranslateTransform(x, y);
            g.RotateTransform(angle);

            // Random dark color for each character
            using var brush = new SolidBrush(Color.FromArgb(
                _random.Next(0, 100),
                _random.Next(0, 100),
                _random.Next(0, 100)));

            g.DrawString(c.ToString(), font, brush, 0, 0);

            g.RotateTransform(-angle);
            g.TranslateTransform(-x, -y);

            x += 20 + _random.Next(-5, 5);
            y += _random.Next(-3, 3);
        }
    }

    private void AddNoiseDots(Graphics g, int width, int height)
    {
        for (int i = 0; i < 100; i++)
        {
            int x = _random.Next(width);
            int y = _random.Next(height);
            using var brush = new SolidBrush(Color.FromArgb(
                _random.Next(50, 100),
                _random.Next(256),
                _random.Next(256),
                _random.Next(256)));
            g.FillEllipse(brush, x, y, 2, 2);
        }
    }
}
