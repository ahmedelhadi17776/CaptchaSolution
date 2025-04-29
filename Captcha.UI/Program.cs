using Captcha.Core.Enums;
using Captcha.Core.Interfaces;
using Captcha.Core.Models;
using Captcha.Generators;
using Captcha.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Captcha.UI;

static class Program
{
    [STAThread]
    static void Main()
    {
        // Initialize the application
        ApplicationConfiguration.Initialize();

        // Uncomment the following line to test CaptchaValidator
        TestCaptchaValidator();

        // Run the main application
        var services = new ServiceCollection();
        ConfigureServices(services);

        using var serviceProvider = services.BuildServiceProvider();
        var form = serviceProvider.GetRequiredService<Form1>();

        Application.Run(form);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<Form1>();
        services.AddTransient<ICaptchaGenerator, TextCaptchaGenerator>();
        services.AddTransient<ICaptchaGenerator, MathCaptchaGenerator>();
        services.AddTransient<ICaptchaGenerator, ImageCaptchaGenerator>();
        services.AddTransient<ICaptchaGenerator, ReCaptchaGenerator>();
        services.AddSingleton<ICaptchaValidator, CaptchaValidator>();
        services.Configure<CaptchaOptions>(options =>
        {
            options.TextLength = 6;
            options.AllowedCharacters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            options.MaxAttempts = 3;
        });
    }

    private static void TestCaptchaValidator()
    {
        // Create an instance of CaptchaValidator
        var validator = new CaptchaValidator();

        // Test Text CAPTCHA
        var textCaptcha = new CaptchaModel
        {
            Type = CaptchaType.Text,
            Challenge = "ABC123",
            Answer = "ABC123"
        };

        Console.WriteLine("Testing Text CAPTCHA:");
        Console.WriteLine($"Input: ABC123, Expected: True, Result: {validator.Validate(textCaptcha, "ABC123")}");
        Console.WriteLine($"Input: abc123, Expected: True, Result: {validator.Validate(textCaptcha, "abc123")}");
        Console.WriteLine($"Input: XYZ789, Expected: False, Result: {validator.Validate(textCaptcha, "XYZ789")}");
        Console.WriteLine();

        // Test Math CAPTCHA
        var mathCaptcha = new CaptchaModel
        {
            Type = CaptchaType.Math,
            Challenge = "5 + 7 = ?",
            Answer = "12"
        };

        Console.WriteLine("Testing Math CAPTCHA:");
        Console.WriteLine($"Input: 12, Expected: True, Result: {validator.Validate(mathCaptcha, "12")}");
        Console.WriteLine($"Input: 15, Expected: False, Result: {validator.Validate(mathCaptcha, "15")}");
        Console.WriteLine($"Input: abc, Expected: False, Result: {validator.Validate(mathCaptcha, "abc")}");
        Console.WriteLine();

        // Test Image CAPTCHA
        var imageCaptcha = new CaptchaModel
        {
            Type = CaptchaType.Image,
            Challenge = "XYZ789",
            Answer = "XYZ789"
        };

        Console.WriteLine("Testing Image CAPTCHA:");
        Console.WriteLine($"Input: XYZ789, Expected: True, Result: {validator.Validate(imageCaptcha, "XYZ789")}");
        Console.WriteLine($"Input: xyz789, Expected: True, Result: {validator.Validate(imageCaptcha, "xyz789")}");
        Console.WriteLine($"Input: ABC123, Expected: False, Result: {validator.Validate(imageCaptcha, "ABC123")}");
        Console.WriteLine();
    }
}
