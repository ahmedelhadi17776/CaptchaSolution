# CaptchaSolution

A flexible and extensible CAPTCHA implementation in .NET 6.0 that supports multiple CAPTCHA types including text, math, image, and reCAPTCHA-style verification.

## Features

- Multiple CAPTCHA types:
  - Text CAPTCHA: Random alphanumeric characters
  - Math CAPTCHA: Basic arithmetic operations
  - Image CAPTCHA: Distorted text with background noise
  - reCAPTCHA-style: Checkbox verification with "I'm not a robot"

## Prerequisites

- .NET 6.0 SDK or later
- Visual Studio 2022 or Visual Studio Code with C# extensions
- Windows OS (required for System.Drawing.Common functionality)

## Project Structure

- **Captcha.Core**: Core interfaces and models
- **Captcha.Generators**: CAPTCHA generation implementations
- **Captcha.Validators**: CAPTCHA validation logic
- **Captcha.UI**: Windows Forms UI implementation
- **Captcha.Tests**: Unit tests

## Getting Started

1. Clone the repository:
```bash
git clone https://github.com/yourusername/CaptchaSolution.git
cd CaptchaSolution
```

2. Build the solution:
```bash
dotnet build
```

3. Run the application:
```bash
cd Captcha.UI
dotnet run
```

## Usage

1. Launch the application
2. Select a CAPTCHA type from the dropdown menu:
   - Text: Enter the displayed characters
   - Math: Solve the arithmetic problem
   - Image: Enter the characters shown in the distorted image
   - reCAPTCHA: Click the checkbox for verification

3. Enter your answer in the text box (except for reCAPTCHA)
4. Click "Validate" to check your answer
5. Use "Refresh" to generate a new CAPTCHA

## Architecture

### Core Components

- **ICaptcha**: Base interface for all CAPTCHA types
- **ICaptchaGenerator**: Interface for CAPTCHA generation
- **ICaptchaValidator**: Interface for CAPTCHA validation
- **CaptchaModel**: Implementation of the ICaptcha interface
- **CaptchaOptions**: Configuration options for CAPTCHA generation

### CAPTCHA Types

1. **Text CAPTCHA**
   - Random alphanumeric characters
   - Case-insensitive validation
   - Configurable length and character set

2. **Math CAPTCHA**
   - Random arithmetic operations
   - Addition, subtraction, and multiplication
   - Difficulty levels with different number ranges

3. **Image CAPTCHA**
   - Text-based CAPTCHA with visual distortion
   - Random noise and rotation
   - Anti-aliasing for better appearance

4. **reCAPTCHA-style**
   - Simple checkbox verification
   - Visual feedback on verification
   - Mock implementation of reCAPTCHA concept
