using Captcha.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captcha.Core.Interfaces;

public interface ICaptcha
{
    Guid Id { get; }
    string Challenge { get; }
    string Answer { get; }
    byte[]? ImageData { get; }
    CaptchaType Type { get; }
}
