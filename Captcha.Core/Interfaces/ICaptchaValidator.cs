using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captcha.Core.Interfaces;

public interface ICaptchaValidator
{
    bool Validate(ICaptcha captcha, string userInput);
}
