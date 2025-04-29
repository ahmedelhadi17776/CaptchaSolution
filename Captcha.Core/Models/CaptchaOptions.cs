using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captcha.Core.Models
{
    public class CaptchaOptions
    {
        public int TextLength { get; set; } = 6;
        public string AllowedCharacters { get; set; } = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        public int MaxAttempts { get; set; } = 3;
    }
}
