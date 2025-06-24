using CarRentalSystem.Interfaces;
using CarRentalSystem.Models;
using CarRentalSystem.Utils;

namespace CarRentalSystem.Forms
{
    public partial class CustomerForm : Form
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger _logger;

        // Kontrolki UI
        private Label titleLabel;
        private Panel mainPanel;
        private Panel buttonPanel;
        private Label firstNameLabel;
        private TextBox firstNameTextBox;
        private Label lastNameLabel;
        private TextBox lastNameTextBox;
        private Label emailLabel;
        private TextBox emailTextBox;
        private Label phoneLabel;
        private TextBox phoneTextBox;
        private Label dobLabel;
        private DateTimePicker dobDateTimePicker;
        private Label ageInfoLabel;
        private Button saveButton;
        private Button cancelButton;

        public CustomerForm(ICustomerService customerService, ILogger logger)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            InitializeComponent();
            SetupForm();
        }

        private void InitializeComponent()
        {
            // Ustawienia formularza
            this.Text = "👥 Dodaj Nowego Klienta";
            this.Size = new Size(500, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 244, 248);

            // Tytuł
            titleLabel = new Label();
            titleLabel.Text = "👥 Dodawanie Nowego Klienta";
            titleLabel.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(37, 99, 235);
            titleLabel.Size = new Size(460, 40);
            titleLabel.Location = new Point(20, 20);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;

            // Panel główny
            mainPanel = new Panel();
            mainPanel.Location = new Point(20, 80);
            mainPanel.Size = new Size(440, 340);
            mainPanel.BackColor = Color.White;
            mainPanel.BorderStyle = BorderStyle.FixedSingle;

            CreateFormControls();

            // Panel z przyciskami
            buttonPanel = new Panel();
            buttonPanel.Location = new Point(20, 440);
            buttonPanel.Size = new Size(440, 50);
            buttonPanel.BackColor = Color.Transparent;

            saveButton = new Button();
            saveButton.Text = "👥 Dodaj Klienta";
            saveButton.Size = new Size(150, 40);
            saveButton.Location = new Point(140, 5);
            saveButton.BackColor = Color.FromArgb(34, 197, 94);
            saveButton.ForeColor = Color.White;
            saveButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            saveButton.FlatStyle = FlatStyle.Flat;
            saveButton.FlatAppearance.BorderSize = 0;
            saveButton.Cursor = Cursors.Hand;
            saveButton.Click += SaveButton_Click;

            cancelButton = new Button();
            cancelButton.Text = "❌ Anuluj";
            cancelButton.Size = new Size(120, 40);
            cancelButton.Location = new Point(300, 5);
            cancelButton.BackColor = Color.FromArgb(107, 114, 128);
            cancelButton.ForeColor = Color.White;
            cancelButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            cancelButton.FlatStyle = FlatStyle.Flat;
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Cursor = Cursors.Hand;
            cancelButton.Click += CancelButton_Click;

            buttonPanel.Controls.AddRange(new Control[] { saveButton, cancelButton });

            this.Controls.AddRange(new Control[] { titleLabel, mainPanel, buttonPanel });
        }

        private void CreateFormControls()
        {
            int yPosition = 30;
            int labelHeight = 25;
            int controlHeight = 30;
            int spacing = 45;
            int leftMargin = 30;
            int controlWidth = 380;

            // Imię
            firstNameLabel = new Label();
            firstNameLabel.Text = "👤 Imię:";
            firstNameLabel.Location = new Point(leftMargin, yPosition);
            firstNameLabel.Size = new Size(100, labelHeight);
            firstNameLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            firstNameTextBox = new TextBox();
            firstNameTextBox.Location = new Point(leftMargin, yPosition + 25);
            firstNameTextBox.Size = new Size(controlWidth, controlHeight);
            firstNameTextBox.Font = new Font("Segoe UI", 10F);

            yPosition += spacing + 25;

            // Nazwisko
            lastNameLabel = new Label();
            lastNameLabel.Text = "👤 Nazwisko:";
            lastNameLabel.Location = new Point(leftMargin, yPosition);
            lastNameLabel.Size = new Size(100, labelHeight);
            lastNameLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            lastNameTextBox = new TextBox();
            lastNameTextBox.Location = new Point(leftMargin, yPosition + 25);
            lastNameTextBox.Size = new Size(controlWidth, controlHeight);
            lastNameTextBox.Font = new Font("Segoe UI", 10F);

            yPosition += spacing + 25;

            // Email
            emailLabel = new Label();
            emailLabel.Text = "📧 Email:";
            emailLabel.Location = new Point(leftMargin, yPosition);
            emailLabel.Size = new Size(100, labelHeight);
            emailLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            emailTextBox = new TextBox();
            emailTextBox.Location = new Point(leftMargin, yPosition + 25);
            emailTextBox.Size = new Size(controlWidth, controlHeight);
            emailTextBox.Font = new Font("Segoe UI", 10F);

            yPosition += spacing + 25;

            // Telefon
            phoneLabel = new Label();
            phoneLabel.Text = "📞 Telefon:";
            phoneLabel.Location = new Point(leftMargin, yPosition);
            phoneLabel.Size = new Size(100, labelHeight);
            phoneLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            phoneTextBox = new TextBox();
            phoneTextBox.Location = new Point(leftMargin, yPosition + 25);
            phoneTextBox.Size = new Size(controlWidth, controlHeight);
            phoneTextBox.Font = new Font("Segoe UI", 10F);

            yPosition += spacing + 25;

            // Data urodzenia
            dobLabel = new Label();
            dobLabel.Text = "🎂 Data urodzenia:";
            dobLabel.Location = new Point(leftMargin, yPosition);
            dobLabel.Size = new Size(150, labelHeight);
            dobLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            dobDateTimePicker = new DateTimePicker();
            dobDateTimePicker.Location = new Point(leftMargin, yPosition + 25);
            dobDateTimePicker.Size = new Size(200, controlHeight);
            dobDateTimePicker.Font = new Font("Segoe UI", 10F);
            dobDateTimePicker.Format = DateTimePickerFormat.Short;
            dobDateTimePicker.MaxDate = DateTime.Now.AddYears(-16); // Minimalny wiek 16 lat
            dobDateTimePicker.MinDate = DateTime.Now.AddYears(-100); // Maksymalny wiek 100 lat
            dobDateTimePicker.Value = DateTime.Now.AddYears(-25); // Domyślnie 25 lat
            dobDateTimePicker.ValueChanged += DobDateTimePicker_ValueChanged;

            // Informacja o wieku
            ageInfoLabel = new Label();
            ageInfoLabel.Location = new Point(leftMargin + 220, yPosition + 25);
            ageInfoLabel.Size = new Size(180, 30);
            ageInfoLabel.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            ageInfoLabel.ForeColor = Color.FromArgb(107, 114, 128);
            UpdateAgeInfo();

            mainPanel.Controls.AddRange(new Control[] {
                firstNameLabel, firstNameTextBox,
                lastNameLabel, lastNameTextBox,
                emailLabel, emailTextBox,
                phoneLabel, phoneTextBox,
                dobLabel, dobDateTimePicker, ageInfoLabel
            });
        }

        private void SetupForm()
        {
            // Dodanie efektów hover do przycisków
            saveButton.MouseEnter += (s, e) => saveButton.BackColor = ControlPaint.Dark(Color.FromArgb(34, 197, 94), 0.1f);
            saveButton.MouseLeave += (s, e) => saveButton.BackColor = Color.FromArgb(34, 197, 94);

            cancelButton.MouseEnter += (s, e) => cancelButton.BackColor = ControlPaint.Dark(Color.FromArgb(107, 114, 128), 0.1f);
            cancelButton.MouseLeave += (s, e) => cancelButton.BackColor = Color.FromArgb(107, 114, 128);

            // Ustawienie klawisza Enter jako domyślnego
            this.AcceptButton = saveButton;
            this.CancelButton = cancelButton;
        }

        private void DobDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            UpdateAgeInfo();
        }

        private void UpdateAgeInfo()
        {
            var age = DateTime.Now.Year - dobDateTimePicker.Value.Year;
            if (DateTime.Now.DayOfYear < dobDateTimePicker.Value.DayOfYear)
                age--;

            ageInfoLabel.Text = $"Wiek: {age} lat";

            if (age < 18)
            {
                ageInfoLabel.ForeColor = Color.FromArgb(239, 68, 68);
                ageInfoLabel.Text += " ⚠️ Za młody";
            }
            else if (age > 75)
            {
                ageInfoLabel.ForeColor = Color.FromArgb(239, 68, 68);
                ageInfoLabel.Text += " ⚠️ Za stary";
            }
            else
            {
                ageInfoLabel.ForeColor = Color.FromArgb(34, 197, 94);
                ageInfoLabel.Text += " ✅ OK";
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Walidacja danych
                if (!ValidateInput())
                {
                    return;
                }

                // Tworzenie nowego klienta
                var customer = new Customer
                {
                    FirstName = firstNameTextBox.Text.Trim(),
                    LastName = lastNameTextBox.Text.Trim(),
                    Email = emailTextBox.Text.Trim(),
                    PhoneNumber = phoneTextBox.Text.Trim(),
                    DateOfBirth = dobDateTimePicker.Value
                };

                _customerService.AddCustomer(customer);
                _logger.LogInfo($"Dodano nowego klienta: {customer.FullName} ({customer.Email})");

                MessageBox.Show($"Klient {customer.FullName} został pomyślnie dodany!", "Sukces",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Błąd podczas dodawania klienta: {ex.Message}", ex);
                MessageBox.Show($"Wystąpił błąd podczas dodawania klienta:\n{ex.Message}",
                    "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            // Walidacja imienia
            if (string.IsNullOrWhiteSpace(firstNameTextBox.Text))
            {
                MessageBox.Show("Proszę wprowadzić imię klienta.", "Błąd walidacji",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                firstNameTextBox.Focus();
                return false;
            }

            // Walidacja nazwiska
            if (string.IsNullOrWhiteSpace(lastNameTextBox.Text))
            {
                MessageBox.Show("Proszę wprowadzić nazwisko klienta.", "Błąd walidacji",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lastNameTextBox.Focus();
                return false;
            }

            // Walidacja email (używając DataValidator)
            if (string.IsNullOrWhiteSpace(emailTextBox.Text) || !DataValidator.IsValidEmail(emailTextBox.Text))
            {
                MessageBox.Show("Proszę wprowadzić prawidłowy adres email.", "Błąd walidacji",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                emailTextBox.Focus();
                return false;
            }

            // Sprawdzenie unikalności email
            var existingCustomers = _customerService.GetAllCustomers();
            var duplicateEmail = existingCustomers.FirstOrDefault(c =>
                c.Email.Equals(emailTextBox.Text.Trim(), StringComparison.OrdinalIgnoreCase));

            if (duplicateEmail != null)
            {
                MessageBox.Show("Klient z tym adresem email już istnieje!", "Błąd walidacji",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                emailTextBox.Focus();
                return false;
            }

            // Walidacja telefonu
            if (string.IsNullOrWhiteSpace(phoneTextBox.Text))
            {
                MessageBox.Show("Proszę wprowadzić numer telefonu.", "Błąd walidacji",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                phoneTextBox.Focus();
                return false;
            }

            // Walidacja wieku
            var age = DateTime.Now.Year - dobDateTimePicker.Value.Year;
            if (DateTime.Now.DayOfYear < dobDateTimePicker.Value.DayOfYear)
                age--;

            if (age < 18)
            {
                MessageBox.Show("Klient musi mieć ukończone 18 lat aby móc wypożyczyć pojazd.", "Błąd walidacji",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dobDateTimePicker.Focus();
                return false;
            }

            if (age > 75)
            {
                MessageBox.Show("Klient nie może mieć więcej niż 75 lat aby móc wypożyczyć pojazd.", "Błąd walidacji",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dobDateTimePicker.Focus();
                return false;
            }

            return true;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}