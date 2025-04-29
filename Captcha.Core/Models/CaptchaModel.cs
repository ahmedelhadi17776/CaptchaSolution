using Captcha.Core.Enums;
using Captcha.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captcha.Core.Models;

public class CaptchaModel : ICaptcha
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Challenge { get; init; } = string.Empty;
    public string Answer { get; init; } = string.Empty;
    public byte[]? ImageData { get; init; }
    public CaptchaType Type { get; set; }
}
