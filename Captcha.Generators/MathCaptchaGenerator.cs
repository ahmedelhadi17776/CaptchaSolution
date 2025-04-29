using Captcha.Core.Interfaces;
using Captcha.Core.Models;
using Captcha.Core.Enums;

namespace Captcha.Generators;

public class MathCaptchaGenerator : ICaptchaGenerator
{
    public CaptchaType Type => CaptchaType.Math;

    public ICaptcha Generate()
    {
        var rand = new Random();
        int a = rand.Next(1, 10), b = rand.Next(1, 10);
        string op = new[] { "+", "-", "*" }[rand.Next(3)];
        int answer = op switch
        {
            "+" => a + b,
            "-" => a - b,
            _ => a * b
        };

        return new CaptchaModel
        {
            Challenge = $"{a} {op} {b} = ?",
            Answer = answer.ToString(),
            Type = Type
        };
    }
}