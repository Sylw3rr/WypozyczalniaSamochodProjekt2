using CarRentalSystem.Interfaces;
using CarRentalSystem.Models;
using CarRentalSystem.Utils;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CarRentalSystem.Forms
{
    public class CustomerFormDialog : Form
    {
        // === POLA SERWISÓW ===
        private ICustomerService customerService;
        private ILogger logger;

        // === KONTROLKI UI ===
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private DateTimePicker dtpBirthDate;
        private Button btnSave;
        private Button btnCancel;

        // === KONSTRUKTOR ===
        public CustomerFormDialog(ICustomerService service, ILogger log)
        {
            customerService = service;
            logger = log;
            BuildForm();
        }

        // === BUDOWANIE FORMULARZA ===
        private void BuildForm()
        {
            SetupFormProperties();
            CreateControls();
            SetupLayout();
            AttachEvents();
        }

        private void SetupFormProperties()
        {
            Text = "🏢 Nowy Klient - System Wypożyczalni";
            Size = new Size(420, 320);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(240, 248, 255);
            Font = new Font("Segoe UI", 9);
        }

        private void CreateControls()
        {
            // Etykiety
            var lblFirstName = new Label
            {
                Text = "👤 Imię:",
                Location = new Point(25, 25),
                Size = new Size(90, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };

            var lblLastName = new Label
            {
                Text = "👤 Nazwisko:",
                Location = new Point(25, 65),
                Size = new Size(90, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };

            var lblEmail = new Label
            {
                Text = "📧 Email:",
                Location = new Point(25, 105),
                Size = new Size(90, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };

            var lblPhone = new Label
            {
                Text = "📞 Telefon:",
                Location = new Point(25, 145),
                Size = new Size(90, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };

            var lblBirthDate = new Label
            {
                Text = "🎂 Data ur.:",
                Location = new Point(25, 185),
                Size = new Size(90, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };

            // Pola tekstowe
            txtFirstName = new TextBox
            {
                Location = new Point(125, 25),
                Size = new Size(260, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            txtLastName = new TextBox
            {
                Location = new Point(125, 65),
                Size = new Size(260, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            txtEmail = new TextBox
            {
                Location = new Point(125, 105),
                Size = new Size(260, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            txtPhone = new TextBox
            {
                Location = new Point(125, 145),
                Size = new Size(260, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            // DateTimePicker
            dtpBirthDate = new DateTimePicker
            {
                Location = new Point(125, 185),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10),
                MaxDate = DateTime.Today.AddYears(-18),
                Value = DateTime.Today.AddYears(-30),
                Format = DateTimePickerFormat.Short
            };

            // Przyciski
            btnSave = new Button
            {
                Text = "💾 Zapisz Klienta",
                Location = new Point(125, 230),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(34, 139, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btnCancel = new Button
            {
                Text = "❌ Anuluj",
                Location = new Point(265, 230),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(220, 20, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            // Dodanie kontrolek do formularza
            Controls.AddRange(new Control[]
            {
                lblFirstName, txtFirstName,
                lblLastName, txtLastName,
                lblEmail, txtEmail,
                lblPhone, txtPhone,
                lblBirthDate, dtpBirthDate,
                btnSave, btnCancel
            });
        }

        private void SetupLayout()
        {
            // Efekty hover dla przycisków
            btnSave.MouseEnter += (s, e) => btnSave.BackColor = Color.FromArgb(0, 100, 0);
            btnSave.MouseLeave += (s, e) => btnSave.BackColor = Color.FromArgb(34, 139, 34);

            btnCancel.MouseEnter += (s, e) => btnCancel.BackColor = Color.FromArgb(139, 0, 0);
            btnCancel.MouseLeave += (s, e) => btnCancel.BackColor = Color.FromArgb(220, 20, 60);
        }

        private void AttachEvents()
        {
            btnSave.Click += OnSaveClick;
            btnCancel.Click += OnCancelClick;

            // Walidacja w czasie rzeczywistym
            txtEmail.Leave += (s, e) => ValidateEmailField();
            txtFirstName.TextChanged += (s, e) => ValidateNameField(txtFirstName);
            txtLastName.TextChanged += (s, e) => ValidateNameField(txtLastName);
        }

        // === OBSŁUGA ZDARZEŃ ===
        private void OnSaveClick(object sender, EventArgs e)
        {
            try
            {
                if (!PerformValidation())
                    return;

                var customer = CreateCustomerFromForm();
                customerService.AddCustomer(customer);

                logger.LogInfo($"✅ Pomyślnie dodano klienta: {customer.FullName}");

                ShowSuccessMessage($"Klient {customer.FullName} został dodany do systemu!");
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                logger.LogError("❌ Błąd podczas zapisywania klienta", ex);
                ShowErrorMessage($"Wystąpił błąd: {ex.Message}");
            }
        }

        private void OnCancelClick(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Czy na pewno chcesz anulować dodawanie klienta?",
                "Potwierdzenie",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        // === WALIDACJA ===
        private bool PerformValidation()
        {
            if (!ValidateNameField(txtFirstName, "imię")) return false;
            if (!ValidateNameField(txtLastName, "nazwisko")) return false;
            if (!ValidateEmailField()) return false;
            if (!ValidatePhoneField()) return false;
            if (!ValidateAgeField()) return false;

            return true;
        }

        private bool ValidateNameField(TextBox textBox, string fieldName = "pole")
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                ShowValidationError($"Proszę podać {fieldName} klienta");
                textBox.Focus();
                textBox.BackColor = Color.FromArgb(255, 230, 230);
                return false;
            }

            if (textBox.Text.Trim().Length < 2)
            {
                ShowValidationError($"Pole {fieldName} musi mieć co najmniej 2 znaki");
                textBox.Focus();
                textBox.BackColor = Color.FromArgb(255, 230, 230);
                return false;
            }

            textBox.BackColor = Color.FromArgb(230, 255, 230);
            return true;
        }

        private bool ValidateEmailField()
        {
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                ShowValidationError("Proszę podać adres email");
                txtEmail.Focus();
                txtEmail.BackColor = Color.FromArgb(255, 230, 230);
                return false;
            }

            if (!DataValidator.IsValidEmail(email))
            {
                ShowValidationError("Proszę podać prawidłowy adres email");
                txtEmail.Focus();
                txtEmail.BackColor = Color.FromArgb(255, 230, 230);
                return false;
            }

            txtEmail.BackColor = Color.FromArgb(230, 255, 230);
            return true;
        }

        private bool ValidatePhoneField()
        {
            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                ShowValidationError("Proszę podać numer telefonu");
                txtPhone.Focus();
                txtPhone.BackColor = Color.FromArgb(255, 230, 230);
                return false;
            }

            string phone = txtPhone.Text.Trim();
            if (phone.Length < 9)
            {
                ShowValidationError("Numer telefonu musi mieć co najmniej 9 cyfr");
                txtPhone.Focus();
                txtPhone.BackColor = Color.FromArgb(255, 230, 230);
                return false;
            }

            txtPhone.BackColor = Color.FromArgb(230, 255, 230);
            return true;
        }

        private bool ValidateAgeField()
        {
            var age = DateTime.Today.Year - dtpBirthDate.Value.Year;
            if (dtpBirthDate.Value.DayOfYear > DateTime.Today.DayOfYear)
                age--;

            if (age < 18)
            {
                ShowValidationError("Klient musi być pełnoletni (minimum 18 lat)");
                dtpBirthDate.Focus();
                return false;
            }

            if (age > 100)
            {
                ShowValidationError("Proszę sprawdzić datę urodzenia (wiek przekracza 100 lat)");
                dtpBirthDate.Focus();
                return false;
            }

            return true;
        }

        // === METODY POMOCNICZE ===
        private Customer CreateCustomerFromForm()
        {
            return new Customer(
                0, // ID będzie przypisane przez serwis
                txtFirstName.Text.Trim(),
                txtLastName.Text.Trim(),
                txtEmail.Text.Trim(),
                txtPhone.Text.Trim(),
                dtpBirthDate.Value
            );
        }

        private void ShowValidationError(string message)
        {
            MessageBox.Show(
                message,
                "⚠️ Błąd walidacji",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(
                message,
                "❌ Błąd",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }

        private void ShowSuccessMessage(string message)
        {
            MessageBox.Show(
                message,
                "✅ Sukces",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
    }
}
