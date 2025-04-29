using System.Security.Cryptography;
using Captcha.Core.Interfaces;
using Captcha.Core.Models;
using Captcha.Core.Enums;

namespace Captcha.Generators;

public class TextCaptchaGenerator : ICaptchaGenerator
{
    public CaptchaType Type => CaptchaType.Text;

    public ICaptcha Generate()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var rng = RandomNumberGenerator.Create();
        byte[] data = new byte[6];
        rng.GetBytes(data);
        string text = new string(data.Select(b => chars[b % chars.Length]).ToArray());

        return new CaptchaModel
        {
            Challenge = text,
            Answer = text,
            Type = Type
        };
    }
}
