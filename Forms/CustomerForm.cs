using CarRentalSystem.Interfaces;
using CarRentalSystem.Models;
using CarRentalSystem.Utils;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace CarRentalSystem.Forms
{
    [DesignerCategory("")]
    public class CustomerFormDialog : Form
    {
        // === POLA SERWISÓW ===
        private ICustomerService customerService;
        private ILogger logger;
        private readonly Customer? _editingCustomer;
        private readonly bool _isEditMode;

        // === KONTROLKI UI ===
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private DateTimePicker dtpBirthDate;
        private Button btnSave;
        private Button btnCancel;

        // === KONSTRUKTOR ===
        public CustomerFormDialog(ICustomerService customerService, ILogger logger)
        {
            this.customerService = customerService;
            this.logger = logger;
            _isEditMode = false;

            BuildForm(); // ← TYLKO TO, BEZ InitializeComponent()
        }

        // === BUDOWANIE FORMULARZA ===
        private void BuildForm()
        {
            SetupFormProperties();
            CreateControls();
            SetupLayout();
            AttachEvents();
            LoadCustomerDataForEdit();
        }
        private void LoadCustomerData()
        {
            if (_isEditMode && _editingCustomer != null)
            {
                txtFirstName.Text = _editingCustomer.FirstName;
                txtLastName.Text = _editingCustomer.LastName;
                txtEmail.Text = _editingCustomer.Email;
                txtPhone.Text = _editingCustomer.PhoneNumber;
                dtpBirthDate.Value = _editingCustomer.DateOfBirth;
            }
        }
        private void SetupFormProperties()
        {
            Text = _isEditMode ? "✏️ Edytuj Klienta" : "🏢 Nowy Klient - System Wypożyczalni";
            Size = new Size(450, 350); // ✅ ZWIĘKSZONY ROZMIAR
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(240, 248, 255);
            Font = new Font("Segoe UI", 9);
        }
        public CustomerFormDialog(ICustomerService customerService, ILogger logger, Customer editingCustomer)
        {
            this.customerService = customerService;
            this.logger = logger;
            _editingCustomer = editingCustomer;
            _isEditMode = true;

            BuildForm(); // ← TYLKO TO, BEZ InitializeComponent()
            LoadCustomerDataForEdit(); // Wypełnij danymi
        }
        private void CreateControls()
        {
            // === SPÓJNE POZYCJONOWANIE ===
            int labelWidth = 100;
            int fieldWidth = 250;
            int leftMargin = 25;
            int labelW = 110, fieldW = 230;
            int left = 25, fieldLeft = left + labelW + 10;
            int rowHeight = 40;
            int startY = 25;
            
            
            // === ETYKIETY ===
            var lblFirstName = new Label
            {
                Text = "👤 Imię:",
                Location = new Point(leftMargin, startY),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblLastName = new Label
            {
                Text = "👤 Nazwisko:",
                Location = new Point(left, 65),
                Size = new Size(labelW, 25),
                AutoSize = false,              // nie pozwól, by AutoSize zmieniło szerokość
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                BackColor = Color.Transparent
            };

            var lblEmail = new Label
            {
                Text = "✉️ Email:",
                Location = new Point(leftMargin, startY + rowHeight * 2),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblPhone = new Label
            {
                Text = "📞 Telefon:",
                Location = new Point(leftMargin, startY + rowHeight * 3),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblBirthDate = new Label
            {
                Text = "🎂 Data ur.:",
                Location = new Point(leftMargin, startY + rowHeight * 4),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // === POLA TEKSTOWE - SPÓJNE POZYCJE ===
            txtFirstName = new TextBox
            {
                Location = new Point(fieldLeft, startY),
                Size = new Size(fieldWidth, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            txtLastName = new TextBox
            {
                Location = new Point(fieldLeft, 65),
                Size = new Size(fieldW, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            txtEmail = new TextBox
            {
                Location = new Point(fieldLeft, startY + rowHeight * 2),
                Size = new Size(fieldWidth, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            txtPhone = new TextBox
            {
                Location = new Point(fieldLeft, startY + rowHeight * 3),
                Size = new Size(fieldWidth, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            // === DATA PICKER ===
            dtpBirthDate = new DateTimePicker
            {
                Location = new Point(fieldLeft, startY + rowHeight * 4),
                Size = new Size(fieldWidth, 25),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short
            };

            // === PRZYCISKI ===
            btnSave = new Button
            {
                Text = _isEditMode ? "Aktualizuj" : "Zapisz",
                Location = new Point(fieldLeft, startY + rowHeight * 5 + 20),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            btnCancel = new Button
            {
                Text = "Anuluj",
                Location = new Point(fieldLeft + 130, startY + rowHeight * 5 + 20),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                DialogResult = DialogResult.Cancel
            };

            // === DODAJ WSZYSTKO DO FORMULARZA ===
            this.Controls.Clear();
            this.Controls.AddRange(new Control[]
            {
        // Etykiety
        lblFirstName, lblLastName, lblEmail, lblPhone, lblBirthDate,
        // Pola
        txtFirstName, txtLastName, txtEmail, txtPhone, dtpBirthDate,
        // Przyciski
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
        private void LoadCustomerDataForEdit()
        {
            if (_isEditMode && _editingCustomer != null)
            {
                txtFirstName.Text = _editingCustomer.FirstName;
                txtLastName.Text = _editingCustomer.LastName;
                txtEmail.Text = _editingCustomer.Email;
                txtPhone.Text = _editingCustomer.PhoneNumber;
                dtpBirthDate.Value = _editingCustomer.DateOfBirth;

                Text = "✏️ Edytuj Klienta";
                btnSave.Text = "Aktualizuj";
            }
        }
        private void AttachEvents()
        {
            btnSave.Click += OnSaveClick;
            btnCancel.Click += OnCancelClick;

            // Walidacja w czasie rzeczywistym
            txtEmail.Leave += (s, e) => ValidateEmailField();
        }

        // === OBSŁUGA ZDARZEŃ ===
        private void OnSaveClick(object sender, EventArgs e)
        {
            try
            {
                if (!PerformValidation())
                    return;

                if (_isEditMode && _editingCustomer != null)
                {
                    // === TRYB EDYCJI ===
                    _editingCustomer.FirstName = txtFirstName.Text.Trim();
                    _editingCustomer.LastName = txtLastName.Text.Trim();
                    _editingCustomer.Email = txtEmail.Text.Trim();
                    _editingCustomer.PhoneNumber = txtPhone.Text.Trim();
                    _editingCustomer.DateOfBirth = dtpBirthDate.Value;

                    customerService.UpdateCustomer(_editingCustomer);
                    logger.LogInfo($"✅ Zaktualizowano klienta: {_editingCustomer.FullName}");
                    ShowSuccessMessage($"Klient {_editingCustomer.FullName} zaktualizowany!");
                }
                else
                {
                    // === TRYB DODAWANIA ===
                    var customer = CreateCustomerFromForm();
                    customerService.AddCustomer(customer);
                    logger.LogInfo($"✅ Dodano klienta: {customer.FullName}");
                    ShowSuccessMessage($"Klient {customer.FullName} dodany do systemu!");
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                logger.LogError("❌ Błąd podczas zapisywania klienta", ex);
                ShowErrorMessage($"Błąd: {ex.Message}");
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

            textBox.BackColor = Color.White; // ✅ NORMALNY KOLOR
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
