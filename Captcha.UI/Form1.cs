using Microsoft.Extensions.DependencyInjection;
using Captcha.Core.Interfaces;
using Captcha.Core.Enums;
using System.Drawing;

namespace Captcha.UI
{
    public partial class Form1 : Form
    {
        private readonly IServiceProvider _serviceProvider;
        private ICaptcha? _currentCaptcha;

        private ComboBox cmbType = null!;
        private Panel pnlDisplay = null!;
        private TextBox txtAnswer = null!;
        private Label lblAnswer = null!;
        private Button btnValidate = null!;
        private Button btnRefresh = null!;
        private Label lblResult = null!;
        private CheckBox? currentRecaptchaCheckbox;

        public Form1(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeComponent();
            InitializeControls();

            if (cmbType != null)
            {
                cmbType.SelectedIndexChanged += CmbType_SelectedIndexChanged;
            }

            if (btnValidate != null)
            {
                btnValidate.Click += BtnValidate_Click;
            }

            if (btnRefresh != null)
            {
                btnRefresh.Click += BtnRefresh_Click;
            }

            if (txtAnswer != null)
            {
                txtAnswer.KeyPress += TxtAnswer_KeyPress;
            }

            LoadCaptcha();
        }

        private void TxtAnswer_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                BtnValidate_Click(sender ?? this, e);
            }
        }

        private void LoadCaptcha()
        {
            try
            {
                if (_serviceProvider == null)
                {
                    MessageBox.Show("Service provider not initialized", "Error");
                    return;
                }

                if (cmbType?.SelectedIndex < 0 || pnlDisplay == null) return;

                var type = (CaptchaType)cmbType.SelectedIndex;
                var generators = _serviceProvider.GetServices<ICaptchaGenerator>();

                var generator = generators.FirstOrDefault(g => g.Type == type);
                if (generator == null)
                {
                    MessageBox.Show($"No generator found for CAPTCHA type: {type}", "Error");
                    return;
                }

                _currentCaptcha = generator.Generate();
                if (_currentCaptcha == null) return;

                pnlDisplay.Controls.Clear();
                currentRecaptchaCheckbox = null;

                switch (type)
                {
                    case CaptchaType.Text:
                    case CaptchaType.Math:
                        var lbl = new Label
                        {
                            Text = _currentCaptcha.Challenge,
                            Dock = DockStyle.Fill,
                            TextAlign = ContentAlignment.MiddleCenter
                        };
                        pnlDisplay.Controls.Add(lbl);
                        break;

                    case CaptchaType.Image:
                    case CaptchaType.reCAPTCHA:
                        if (_currentCaptcha.ImageData == null) return;

                        var pictureBox = new PictureBox
                        {
                            Size = new Size(200, type == CaptchaType.reCAPTCHA ? 80 : 150),
                            SizeMode = PictureBoxSizeMode.Zoom,
                            Location = new Point(0, 0),
                            Dock = DockStyle.Top
                        };

                        using (var ms = new MemoryStream(_currentCaptcha.ImageData))
                        {
                            pictureBox.Image = Image.FromStream(ms);
                        }

                        if (type == CaptchaType.reCAPTCHA)
                        {
                            var recaptchaLabel = new Label
                            {
                                Location = new Point(20, 10),
                                Size = new Size(160, 30),
                                Text = "reCAPTCHA",
                                Font = new Font("Arial", 12, FontStyle.Bold),
                                TextAlign = ContentAlignment.MiddleLeft,
                                BackColor = Color.Transparent
                            };

                            var checkbox = new CheckBox
                            {
                                Location = new Point(20, 90),
                                Size = new Size(150, 24),
                                Text = "I'm not a robot",
                                TextAlign = ContentAlignment.MiddleLeft,
                                FlatStyle = FlatStyle.Standard,
                                AutoSize = true
                            };

                            currentRecaptchaCheckbox = checkbox;

                            checkbox.CheckedChanged += async (s, e) =>
                            {
                                if (txtAnswer != null && checkbox != null)
                                {
                                    txtAnswer.Text = checkbox.Checked.ToString().ToLower();
                                    if (checkbox.Checked)
                                    {
                                        var validator = _serviceProvider.GetRequiredService<ICaptchaValidator>();
                                        if (_currentCaptcha != null)
                                        {
                                            bool isValid = validator.Validate(_currentCaptcha, txtAnswer.Text);
                                            if (lblResult != null)
                                            {
                                                lblResult.Text = isValid ? "✓ Verified!" : "✗ Verification failed";
                                                lblResult.ForeColor = isValid ? Color.Green : Color.Red;

                                                if (!isValid)
                                                {
                                                    checkbox.Checked = false;
                                                }
                                                else
                                                {
                                                    await Task.Delay(1500);
                                                    if (!IsDisposed && checkbox.Checked)
                                                    {
                                                        BeginInvoke(() => LoadCaptcha());
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            };

                            pnlDisplay.Controls.Add(pictureBox);
                            pnlDisplay.Controls.Add(recaptchaLabel);
                            pnlDisplay.Controls.Add(checkbox);
                        }
                        else
                        {
                            pnlDisplay.Controls.Add(pictureBox);
                        }
                        break;
                }

                if (txtAnswer != null)
                {
                    txtAnswer.Clear();
                }
                if (lblResult != null)
                {
                    lblResult.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading CAPTCHA: {ex.Message}", "Error");
            }
        }

        private void BtnValidate_Click(object? sender, EventArgs e)
        {
            if (_currentCaptcha == null || txtAnswer == null || lblResult == null) return;

            var type = cmbType?.SelectedIndex >= 0 ? (CaptchaType)cmbType.SelectedIndex : CaptchaType.Text;
            if (type == CaptchaType.reCAPTCHA && currentRecaptchaCheckbox != null)
            {
                currentRecaptchaCheckbox.Checked = !string.IsNullOrEmpty(txtAnswer.Text) && txtAnswer.Text.ToLower() == "true";
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAnswer.Text))
            {
                lblResult.Text = "Please enter an answer";
                lblResult.ForeColor = Color.Red;
                return;
            }

            try
            {
                var validator = _serviceProvider.GetRequiredService<ICaptchaValidator>();
                bool isValid = validator.Validate(_currentCaptcha, txtAnswer.Text);
                lblResult.Text = isValid ? "✓ Correct!" : "✗ Try again";
                lblResult.ForeColor = isValid ? Color.Green : Color.Red;

                if (isValid)
                {
                    LoadCaptcha();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error validating CAPTCHA: {ex.Message}", "Error");
            }
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadCaptcha();
        }

        private void CmbType_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (txtAnswer == null || lblAnswer == null || cmbType?.SelectedIndex < 0) return;
            if (cmbType?.Items == null || cmbType.Items.Count == 0) return;

            var type = (CaptchaType)cmbType.SelectedIndex;
            txtAnswer.Visible = type != CaptchaType.reCAPTCHA;
            lblAnswer.Visible = type != CaptchaType.reCAPTCHA;
            LoadCaptcha();
        }

        private void InitializeControls()
        {
            var lblType = new Label
            {
                Location = new Point(12, 12),
                Size = new Size(200, 20),
                Text = "CAPTCHA Type:",
                TextAlign = ContentAlignment.MiddleLeft
            };

            cmbType = new ComboBox
            {
                Location = new Point(12, 35),
                Size = new Size(200, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbType.Items.AddRange(Enum.GetNames(typeof(CaptchaType)));
            cmbType.SelectedIndex = 0;

            pnlDisplay = new Panel
            {
                Location = new Point(12, 70),
                Size = new Size(200, 150),
                BorderStyle = BorderStyle.FixedSingle
            };

            txtAnswer = new TextBox
            {
                Location = new Point(12, 253),
                Size = new Size(200, 23)
            };

            btnValidate = new Button
            {
                Location = new Point(12, 285),
                Size = new Size(75, 23),
                Text = "Validate"
            };

            btnRefresh = new Button
            {
                Location = new Point(93, 285),
                Size = new Size(75, 23),
                Text = "Refresh"
            };

            lblAnswer = new Label
            {
                Location = new Point(12, 230),
                Size = new Size(200, 20),
                Text = "Your Answer:",
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblResult = new Label
            {
                Location = new Point(12, 315),
                Size = new Size(200, 23),
                TextAlign = ContentAlignment.MiddleCenter
            };

            this.MinimumSize = new Size(240, 385);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            var toolTip = new ToolTip();
            toolTip.SetToolTip(btnRefresh, "Generate new CAPTCHA");

            Controls.AddRange(new Control[]
            {
                lblType, cmbType,
                pnlDisplay,
                lblAnswer, txtAnswer,
                btnValidate, btnRefresh,
                lblResult
            });
        }
    }
}
