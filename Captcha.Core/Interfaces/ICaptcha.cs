using Captcha.Core.Enums;

namespace Captcha.Core.Interfaces;

/// <summary>
/// Represents a CAPTCHA challenge
/// </summary>
public interface ICaptcha
{
    /// <summary>
    /// Gets the unique identifier for this CAPTCHA instance
    /// </summary>
    Guid Id { get; }
    
    /// <summary>
    /// Gets the challenge text or description presented to the user
    /// </summary>
    string Challenge { get; }
    
    /// <summary>
    /// Gets the expected answer for this CAPTCHA
    /// </summary>
    string Answer { get; }
    
    /// <summary>
    /// Gets the image data for image-based CAPTCHAs
    /// </summary>
    byte[]? ImageData { get; }
    
    /// <summary>
    /// Gets the type of this CAPTCHA
    /// </summary>
    CaptchaType Type { get; }
}
