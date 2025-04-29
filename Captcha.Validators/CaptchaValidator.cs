using Captcha.Core.Interfaces;
using Captcha.Core.Enums;

namespace Captcha.Validators;

/// <summary>
/// Validates CAPTCHA challenges against user input
/// </summary>
public class CaptchaValidator : ICaptchaValidator
{
    /// <summary>
    /// Validates the user's input against the CAPTCHA challenge
    /// </summary>
    /// <param name="captcha">The CAPTCHA to validate</param>
    /// <param name="userInput">The user's answer attempt</param>
    /// <returns>True if the answer is correct; otherwise, false</returns>
    public bool Validate(ICaptcha captcha, string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput))
            return false;

        // Sanitize input
        userInput = userInput.Trim();

        return captcha.Type switch
        {
            CaptchaType.Text => ValidateText(captcha.Answer, userInput),
            CaptchaType.Image => ValidateText(captcha.Answer, userInput),
            CaptchaType.Math => ValidateMath(captcha.Answer, userInput),
            CaptchaType.reCAPTCHA => ValidateReCaptcha(userInput),
            _ => false
        };
    }

    private static bool ValidateText(string expected, string actual)
    {
        return string.Equals(expected, actual, StringComparison.OrdinalIgnoreCase);
    }

    private static bool ValidateMath(string expected, string actual)
    {
        if (!int.TryParse(actual, out int userAnswer))
            return false;

        return int.TryParse(expected, out int expectedAnswer) && 
               userAnswer == expectedAnswer;
    }

    private static bool ValidateReCaptcha(string input)
    {
        return string.Equals(input, "true", StringComparison.OrdinalIgnoreCase);
    }
}
