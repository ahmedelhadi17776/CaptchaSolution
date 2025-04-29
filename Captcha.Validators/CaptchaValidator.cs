using Captcha.Core.Interfaces;
using Captcha.Core.Enums;

namespace Captcha.Validators;

public class CaptchaValidator : ICaptchaValidator
{
    public bool Validate(ICaptcha captcha, string userInput)
    {
        if (string.IsNullOrEmpty(userInput))
            return false;

        return captcha.Type switch
        {
            CaptchaType.Text or CaptchaType.Image =>
                userInput.Equals(captcha.Answer, StringComparison.OrdinalIgnoreCase),
            CaptchaType.Math =>
                int.TryParse(userInput, out int ans) &&
                ans.ToString() == captcha.Answer,
            CaptchaType.reCAPTCHA =>
                userInput.ToLower() == "true",
            _ => false
        };
    }
}
