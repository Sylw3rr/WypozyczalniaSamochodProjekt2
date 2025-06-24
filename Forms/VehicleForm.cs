using CarRentalSystem.Interfaces;
using CarRentalSystem.Models;
using CarRentalSystem.Utils;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CarRentalSystem.Forms
{
    // UWAGA: BEZ "partial" - eliminuje konflikty z Designer.cs!
    public class VehicleForm : Form
    {
        // Dependency Injection - polimorfizm
        private readonly IVehicleService _vehicleService;
        private readonly ILogger _logger;
        private readonly Vehicle _editingVehicle;
        private readonly bool _isEditMode;

        // Enkapsulacja - prywatne kontrolki
        private TextBox makeTextBox;
        private TextBox modelTextBox;
        private NumericUpDown yearNumeric;
        private TextBox licensePlateTextBox;
        private NumericUpDown dailyRateNumeric;
        private ComboBox categoryComboBox;
        private CheckBox availableCheckBox;
        private Button saveButton;
        private Button cancelButton;

        // Konstruktor z DI - abstrakcja
        public VehicleForm(IVehicleService vehicleService, ILogger logger, Vehicle editingVehicle = null)
        {
            _vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _editingVehicle = editingVehicle;
            _isEditMode = editingVehicle != null;

            InitializeComponent();

            if (_isEditMode)
            {
                LoadVehicleData();
            }
        }

        // Abstrakcja - ukrycie szczegółów tworzenia UI
        private void InitializeComponent()
        {
            // Konfiguracja formularza
            this.Text = _isEditMode ? "✏️ Edytuj Pojazd" : "➕ Dodaj Pojazd";
            this.Size = new Size(450, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(248, 249, 250);

            // Kompozycja - budowanie interfejsu
            CreateFormControls();
            LayoutControls();
            AttachEventHandlers();
        }

        // Enkapsulacja - prywatne tworzenie kontrolek
        private void CreateFormControls()
        {
            int yPosition = 20;
            int labelWidth = 100;
            int controlWidth = 250;
            int spacing = 35;

            // Marka pojazdu
            CreateLabelAndControl("Marka:", ref yPosition, labelWidth, controlWidth, spacing,
                out Label makeLabel, out makeTextBox);

            // Model pojazdu
            CreateLabelAndControl("Model:", ref yPosition, labelWidth, controlWidth, spacing,
                out Label modelLabel, out modelTextBox);

            // Rok produkcji
            var yearLabel = CreateLabel("Rok:", new Point(20, yPosition), new Size(labelWidth, 23));
            yearNumeric = new NumericUpDown
            {
                Location = new Point(130, yPosition - 2),
                Size = new Size(120, 23),
                Minimum = 1990,
                Maximum = DateTime.Now.Year + 1,
                Value = DateTime.Now.Year,
                Font = new Font("Segoe UI", 10)
            };
            yPosition += spacing;

            // Numer rejestracyjny
            CreateLabelAndControl("Nr Rej.:", ref yPosition, labelWidth, controlWidth, spacing,
                out Label plateLabel, out licensePlateTextBox);

            // Cena za dzień
            var rateLabel = CreateLabel("Cena/dzień:", new Point(20, yPosition), new Size(labelWidth, 23));
            dailyRateNumeric = new NumericUpDown
            {
                Location = new Point(130, yPosition - 2),
                Size = new Size(120, 23),
                Minimum = 1,
                Maximum = 10000,
                DecimalPlaces = 2,
                Value = 100,
                Font = new Font("Segoe UI", 10)
            };
            var currencyLabel = CreateLabel("zł", new Point(260, yPosition), new Size(20, 23));
            yPosition += spacing;

            // Kategoria
            var categoryLabel = CreateLabel("Kategoria:", new Point(20, yPosition), new Size(labelWidth, 23));
            categoryComboBox = new ComboBox
            {
                Location = new Point(130, yPosition - 2),
                Size = new Size(150, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            categoryComboBox.Items.AddRange(new string[] {
                "Sedan", "SUV", "Hatchback", "Sport", "Van", "Truck", "Coupe", "Convertible"
            });
            yPosition += spacing;

            // Dostępność
            availableCheckBox = new CheckBox
            {
                Text = "✅ Pojazd dostępny do wypożyczenia",
                Location = new Point(130, yPosition),
                Size = new Size(250, 23),
                Checked = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(76, 175, 80)
            };
            yPosition += 50;

            // Przyciski
            saveButton = CreateStyledButton("💾 Zapisz", new Point(180, yPosition),
                Color.FromArgb(76, 175, 80));
            cancelButton = CreateStyledButton("❌ Anuluj", new Point(290, yPosition),
                Color.FromArgb(244, 67, 54));

            // Dodanie wszystkich kontrolek do formularza
            this.Controls.AddRange(new Control[] {
                makeLabel, makeTextBox, modelLabel, modelTextBox,
                yearLabel, yearNumeric, plateLabel, licensePlateTextBox,
                rateLabel, dailyRateNumeric, currencyLabel,
                categoryLabel, categoryComboBox, availableCheckBox,
                saveButton, cancelButton
            });
        }

        // Abstrakcja - pomocnicza metoda tworzenia pary label/textbox
        private void CreateLabelAndControl(string labelText, ref int yPosition, int labelWidth,
            int controlWidth, int spacing, out Label label, out TextBox textBox)
        {
            label = CreateLabel(labelText, new Point(20, yPosition), new Size(labelWidth, 23));
            textBox = new TextBox
            {
                Location = new Point(130, yPosition - 2),
                Size = new Size(controlWidth, 23),
                Font = new Font("Segoe UI", 10)
            };
            yPosition += spacing;
        }

        // Enkapsulacja - tworzenie labeli
        private Label CreateLabel(string text, Point location, Size size)
        {
            return new Label
            {
                Text = text,
                Location = location,
                Size = size,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
        }

        // Enkapsulacja - tworzenie stylowanych przycisków
        private Button CreateStyledButton(string text, Point location, Color color)
        {
            return new Button
            {
                Text = text,
                Location = location,
                Size = new Size(100, 35),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
        }

        // Abstrakcja - konfiguracja layoutu
        private void LayoutControls()
        {
            this.Padding = new Padding(15);
        }

        // Enkapsulacja - przypisywanie event handlerów
        private void AttachEventHandlers()
        {
            saveButton.Click += SaveButton_Click;
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            // Efekty hover - polimorfizm zdarzeń
            AddHoverEffects(saveButton, Color.FromArgb(76, 175, 80), Color.FromArgb(66, 165, 70));
            AddHoverEffects(cancelButton, Color.FromArgb(244, 67, 54), Color.FromArgb(234, 57, 44));

            // Walidacja w czasie rzeczywistym
            licensePlateTextBox.TextChanged += (s, e) =>
            {
                licensePlateTextBox.Text = licensePlateTextBox.Text.ToUpper();
                licensePlateTextBox.SelectionStart = licensePlateTextBox.Text.Length;
            };
        }

        // Abstrakcja - efekty hover
        private void AddHoverEffects(Button button, Color normalColor, Color hoverColor)
        {
            button.MouseEnter += (s, e) => button.BackColor = hoverColor;
            button.MouseLeave += (s, e) => button.BackColor = normalColor;
        }

        // Enkapsulacja - ładowanie danych w trybie edycji
        private void LoadVehicleData()
        {
            if (_editingVehicle != null)
            {
                makeTextBox.Text = _editingVehicle.Make;
                modelTextBox.Text = _editingVehicle.Model;
                yearNumeric.Value = _editingVehicle.Year;
                licensePlateTextBox.Text = _editingVehicle.LicensePlate;
                dailyRateNumeric.Value = _editingVehicle.DailyRate;
                categoryComboBox.Text = _editingVehicle.Category;
                availableCheckBox.Checked = _editingVehicle.IsAvailable;
            }
        }

        // Enkapsulacja - obsługa zapisywania
        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInput()) return;

                if (_isEditMode)
                {
                    UpdateExistingVehicle();
                }
                else
                {
                    CreateNewVehicle();
                }

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError("Błąd zapisywania pojazdu", ex);
                MessageBox.Show("Błąd zapisywania pojazdu: " + ex.Message, "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Abstrakcja - aktualizacja istniejącego pojazdu
        private void UpdateExistingVehicle()
        {
            _editingVehicle.Make = makeTextBox.Text.Trim();
            _editingVehicle.Model = modelTextBox.Text.Trim();
            _editingVehicle.Year = (int)yearNumeric.Value;
            _editingVehicle.LicensePlate = licensePlateTextBox.Text.Trim().ToUpper();
            _editingVehicle.DailyRate = dailyRateNumeric.Value;
            _editingVehicle.Category = categoryComboBox.Text;
            _editingVehicle.IsAvailable = availableCheckBox.Checked;

            _vehicleService.UpdateVehicle(_editingVehicle);
            _logger.LogInfo($"Zaktualizowano pojazd: {_editingVehicle.Make} {_editingVehicle.Model}");

            MessageBox.Show($"Pomyślnie zaktualizowano pojazd: {_editingVehicle.Make} {_editingVehicle.Model}",
                "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Abstrakcja - tworzenie nowego pojazdu
        private void CreateNewVehicle()
        {
            var newVehicle = new Vehicle(
                0,
                makeTextBox.Text.Trim(),
                modelTextBox.Text.Trim(),
                (int)yearNumeric.Value,
                licensePlateTextBox.Text.Trim().ToUpper(),
                dailyRateNumeric.Value,
                categoryComboBox.Text
            )
            {
                IsAvailable = availableCheckBox.Checked
            };

            _vehicleService.AddVehicle(newVehicle);
            _logger.LogInfo($"Dodano nowy pojazd: {newVehicle.Make} {newVehicle.Model}");

            MessageBox.Show($"Pomyślnie dodano pojazd: {newVehicle.Make} {newVehicle.Model}",
                "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Abstrakcja - walidacja danych wejściowych
        private bool ValidateInput()
        {
            // Walidacja marki
            if (string.IsNullOrWhiteSpace(makeTextBox.Text))
            {
                ShowValidationError("Podaj markę pojazdu", makeTextBox);
                return false;
            }

            // Walidacja modelu
            if (string.IsNullOrWhiteSpace(modelTextBox.Text))
            {
                ShowValidationError("Podaj model pojazdu", modelTextBox);
                return false;
            }

            // Walidacja numeru rejestracyjnego
            if (string.IsNullOrWhiteSpace(licensePlateTextBox.Text))
            {
                ShowValidationError("Podaj numer rejestracyjny", licensePlateTextBox);
                return false;
            }

            // Sprawdzenie unikalności numeru rejestracyjnego
            var existingVehicle = _vehicleService.GetAllVehicles()
                .FirstOrDefault(v => v.LicensePlate.Equals(licensePlateTextBox.Text.Trim(),
                    StringComparison.OrdinalIgnoreCase) &&
                    (!_isEditMode || v.Id != _editingVehicle.Id));

            if (existingVehicle != null)
            {
                ShowValidationError($"Pojazd z numerem {licensePlateTextBox.Text} już istnieje!", licensePlateTextBox);
                return false;
            }

            // Walidacja kategorii
            if (categoryComboBox.SelectedIndex == -1)
            {
                ShowValidationError("Wybierz kategorię pojazdu", categoryComboBox);
                return false;
            }

            // Walidacja roku
            if (!DataValidator.IsValidYear((int)yearNumeric.Value))
            {
                ShowValidationError("Nieprawidłowy rok produkcji", yearNumeric);
                return false;
            }

            // Walidacja ceny
            if (!DataValidator.IsValidDailyRate(dailyRateNumeric.Value))
            {
                ShowValidationError("Nieprawidłowa cena wynajmu (minimum 1 zł)", dailyRateNumeric);
                return false;
            }

            return true;
        }

        // Enkapsulacja - wyświetlanie błędów walidacji
        private void ShowValidationError(string message, Control control)
        {
            MessageBox.Show(message, "Walidacja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            control.Focus();
        }

        // Polimorfizm - override metody z klasy bazowej
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            makeTextBox.Focus(); // Fokus na pierwsze pole
        }
    }
}
