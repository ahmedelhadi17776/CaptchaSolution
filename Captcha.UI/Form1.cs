using Microsoft.Extensions.DependencyInjection;
using Captcha.Core.Interfaces;
using Captcha.Core.Enums;
using System.Drawing;

namespace Captcha.UI
{
    public partial class Form1 : Form
    {
        private readonly IServiceProvider _serviceProvider;
        private ICaptcha _currentCaptcha;

        public Form1(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeComponent();
            InitializeControls();

            cmbType.SelectedIndexChanged += CmbType_SelectedIndexChanged;
            btnValidate.Click += BtnValidate_Click;
            btnRefresh.Click += BtnRefresh_Click;

            txtAnswer.KeyPress += TxtAnswer_KeyPress;

            LoadCaptcha();
        }
        private void TxtAnswer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                BtnValidate_Click(sender, e);
            }
        }

        private void LoadCaptcha()
        {
            try
            {
                var type = (CaptchaType)cmbType.SelectedIndex;
                var generators = _serviceProvider.GetServices<ICaptchaGenerator>();
                var generator = generators.FirstOrDefault(g => g.Type == type);

                if (generator == null)
                {
                    MessageBox.Show($"No generator found for CAPTCHA type: {type}", "Error");
                    return;
                }

                _currentCaptcha = generator.Generate();

                pnlDisplay.Controls.Clear();
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
                        var pb = new PictureBox
                        {
                            Dock = DockStyle.Fill,
                            SizeMode = PictureBoxSizeMode.CenterImage
                        };
                        using (var ms = new MemoryStream(_currentCaptcha.ImageData))
                            pb.Image = Image.FromStream(ms);

                        if (type == CaptchaType.reCAPTCHA)
                        {
                            var checkbox = new CheckBox
                            {
                                Location = new Point(20, 60),
                                Size = new Size(20, 20),
                                Text = ""
                            };
                            checkbox.CheckedChanged += (s, e) =>
                            {
                                txtAnswer.Text = checkbox.Checked.ToString().ToLower();
                                if (checkbox.Checked)
                                    BtnValidate_Click(s, e);
                            };
                            pnlDisplay.Controls.Add(checkbox);
                        }

                        pnlDisplay.Controls.Add(pb);
                        break;
                }

                txtAnswer.Clear();
                lblResult.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading CAPTCHA: {ex.Message}", "Error");
            }
        }



        private void BtnValidate_Click(object sender, EventArgs e)
        {
            var type = (CaptchaType)cmbType.SelectedIndex;
            if (type == CaptchaType.reCAPTCHA)
            {
                lblResult.Text = "reCAPTCHA validation not implemented";
                lblResult.ForeColor = Color.Red;
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


        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadCaptcha();
        }

        private void CmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var type = (CaptchaType)cmbType.SelectedIndex;
            txtAnswer.Visible = type != CaptchaType.reCAPTCHA;
            lblAnswer.Visible = type != CaptchaType.reCAPTCHA;
            LoadCaptcha();
        }

        private void InitializeControls()
        {
            // Create and configure UI controls

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


            this.lblAnswer = new Label
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

        private ComboBox cmbType;
        private Panel pnlDisplay;
        private TextBox txtAnswer;
        private Label lblAnswer;
        private Button btnValidate;
        private Button btnRefresh;
        private Label lblResult;
    }
}
