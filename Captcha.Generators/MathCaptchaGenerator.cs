using Captcha.Core.Interfaces;
using Captcha.Core.Models;
using Captcha.Core.Enums;

namespace Captcha.Generators;

public class MathCaptchaGenerator : ICaptchaGenerator
{
    private readonly Random _random = new();
    public CaptchaType Type => CaptchaType.Math;

    private enum Difficulty { Easy, Medium, Hard }
    private enum Operation { Add, Subtract, Multiply, Divide }

    public ICaptcha Generate()
    {
        var difficulty = (Difficulty)_random.Next(3); // Random difficulty
        var (a, b, op) = GenerateNumbers(difficulty);
        
        string challenge = FormatChallenge(a, b, op);
        int answer = CalculateAnswer(a, b, op);

        return new CaptchaModel
        {
            Challenge = challenge,
            Answer = answer.ToString(),
            Type = Type
        };
    }

    private (int a, int b, Operation op) GenerateNumbers(Difficulty difficulty)
    {
        var (min, max) = difficulty switch
        {
            Difficulty.Easy => (1, 10),
            Difficulty.Medium => (10, 50),
            Difficulty.Hard => (50, 100),
            _ => (1, 10)
        };

        Operation[] allowedOps = difficulty switch
        {
            Difficulty.Easy => new[] { Operation.Add, Operation.Subtract },
            Difficulty.Medium => new[] { Operation.Add, Operation.Subtract, Operation.Multiply },
            Difficulty.Hard => new[] { Operation.Add, Operation.Subtract, Operation.Multiply, Operation.Divide },
            _ => new[] { Operation.Add, Operation.Subtract }
        };

        int a = _random.Next(min, max);
        int b = _random.Next(min, max);
        Operation op = allowedOps[_random.Next(allowedOps.Length)];

        // Ensure division results in whole numbers
        if (op == Operation.Divide)
        {
            b = b == 0 ? 1 : b; // Avoid division by zero
            a = a * b; // Ensure divisible
        }
        // Ensure subtraction doesn't result in negative numbers
        else if (op == Operation.Subtract && b > a)
        {
            (a, b) = (b, a); // Swap numbers
        }

        return (a, b, op);
    }

    private static string FormatChallenge(int a, int b, Operation op)
    {
        string symbol = op switch
        {
            Operation.Add => "+",
            Operation.Subtract => "-",
            Operation.Multiply => "×",
            Operation.Divide => "÷",
            _ => "+"
        };

        return $"{a} {symbol} {b} = ?";
    }

    private static int CalculateAnswer(int a, int b, Operation op) => op switch
    {
        Operation.Add => a + b,
        Operation.Subtract => a - b,
        Operation.Multiply => a * b,
        Operation.Divide => a / b,
        _ => throw new ArgumentException("Invalid operation")
    };
}