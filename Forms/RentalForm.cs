using CarRentalSystem.Interfaces;
using CarRentalSystem.Models;
using CarRentalSystem.Utils;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CarRentalSystem.Forms
{
    public class RentalForm : Form
    {
        private readonly IRentalService _rentalService;
        private readonly IVehicleService _vehicleService;
        private readonly ICustomerService _customerService;
        private readonly ILogger _logger;
        private Rental _editingRental;
        private bool _isEditMode;

        // Kontrolki UI - teraz będą używane
        private ComboBox cmbVehicle;
        private ComboBox cmbCustomer;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private DateTimePicker dtpActualReturn;
        private ComboBox cmbStatus;
        private NumericUpDown nudAdditionalCharges;
        private Label lblTotalCost;
        private Button btnSave;
        private Button btnCancel;
        private Panel pnActualReturn;

        public RentalForm(
            IVehicleService vehicleService,
            ICustomerService customerService,
            IRentalService rentalService,
            ILogger logger,
            Rental editingRental = null)
        {
            _vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _rentalService = rentalService ?? throw new ArgumentNullException(nameof(rentalService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _editingRental = editingRental;
            _isEditMode = editingRental != null;

            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = _isEditMode ? "✏️ Edytuj Wypożyczenie" : "➕ Nowe Wypożyczenie";
            this.Size = new Size(500, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // Pojazd
            var lblVehicle = new Label
            {
                Text = "Pojazd:",
                Location = new Point(20, 20),
                AutoSize = true
            };
            cmbVehicle = new ComboBox
            {
                Location = new Point(120, 18),
                Width = 320,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Klient
            var lblCustomer = new Label
            {
                Text = "Klient:",
                Location = new Point(20, 60),
                AutoSize = true
            };
            cmbCustomer = new ComboBox
            {
                Location = new Point(120, 58),
                Width = 320,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Data rozpoczęcia
            var lblStartDate = new Label
            {
                Text = "Data od:",
                Location = new Point(20, 100),
                AutoSize = true
            };
            dtpStartDate = new DateTimePicker
            {
                Location = new Point(120, 98),
                Format = DateTimePickerFormat.Short
            };
            dtpStartDate.ValueChanged += (s, e) => CalculateTotalCost();

            // Data zakończenia
            var lblEndDate = new Label
            {
                Text = "Data do:",
                Location = new Point(20, 140),
                AutoSize = true
            };
            dtpEndDate = new DateTimePicker
            {
                Location = new Point(120, 138),
                Format = DateTimePickerFormat.Short
            };
            dtpEndDate.ValueChanged += (s, e) => CalculateTotalCost();

            // Status
            var lblStatus = new Label
            {
                Text = "Status:",
                Location = new Point(20, 180),
                AutoSize = true
            };
            cmbStatus = new ComboBox
            {
                Location = new Point(120, 178),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStatus.Items.AddRange(Enum.GetNames(typeof(RentalStatus)));
            cmbStatus.SelectedIndexChanged += CmbStatus_SelectedIndexChanged;

            // Panel dla rzeczywistej daty zwrotu
            pnActualReturn = new Panel
            {
                Location = new Point(20, 220),
                Size = new Size(420, 30),
                Visible = _isEditMode
            };

            var lblActualReturn = new Label
            {
                Text = "Rzeczywisty zwrot:",
                Location = new Point(0, 5),
                AutoSize = true
            };
            dtpActualReturn = new DateTimePicker
            {
                Location = new Point(120, 2),
                Format = DateTimePickerFormat.Short
            };

            pnActualReturn.Controls.AddRange(new Control[] { lblActualReturn, dtpActualReturn });

            // Dodatkowe opłaty
            var lblAdditional = new Label
            {
                Text = "Dodatkowe opłaty:",
                Location = new Point(20, 260),
                AutoSize = true
            };
            nudAdditionalCharges = new NumericUpDown
            {
                Location = new Point(120, 258),
                Width = 100,
                DecimalPlaces = 2,
                Maximum = 10000,
                Minimum = 0
            };
            nudAdditionalCharges.ValueChanged += (s, e) => CalculateTotalCost();

            // Całkowity koszt
            lblTotalCost = new Label
            {
                Text = "Całkowity koszt: 0,00 zł",
                Location = new Point(20, 300),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold)
            };

            // Przyciski
            btnSave = new Button
            {
                Text = _isEditMode ? "Aktualizuj" : "Zapisz",
                Location = new Point(150, 350),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Anuluj",
                Location = new Point(270, 350),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };

            // Dodaj wszystkie kontrolki do formularza
            this.Controls.AddRange(new Control[]
            {
                lblVehicle, cmbVehicle,
                lblCustomer, cmbCustomer,
                lblStartDate, dtpStartDate,
                lblEndDate, dtpEndDate,
                lblStatus, cmbStatus,
                pnActualReturn,
                lblAdditional, nudAdditionalCharges,
                lblTotalCost,
                btnSave, btnCancel
            });
        }

        private void LoadData()
        {
            // Załaduj pojazdy
            cmbVehicle.Items.Clear();
            foreach (var vehicle in _vehicleService.GetAllVehicles())
            {
                if (!_isEditMode && !vehicle.IsAvailable) continue;
                cmbVehicle.Items.Add(vehicle);
            }

            // Załaduj klientów
            cmbCustomer.Items.Clear();
            foreach (var customer in _customerService.GetAllCustomers())
            {
                cmbCustomer.Items.Add(customer);
            }

            if (_isEditMode && _editingRental != null)
            {
                // Wypełnij danymi w trybie edycji
                var vehicle = _vehicleService.GetVehicleById(_editingRental.VehicleId);
                var customer = _customerService.GetCustomerById(_editingRental.CustomerId);

                cmbVehicle.SelectedItem = vehicle;
                cmbCustomer.SelectedItem = customer;
                dtpStartDate.Value = _editingRental.StartDate;
                dtpEndDate.Value = _editingRental.EndDate;
                cmbStatus.SelectedItem = _editingRental.Status.ToString();
                nudAdditionalCharges.Value = _editingRental.AdditionalCharges;

                if (_editingRental.ActualReturnDate.HasValue)
                {
                    dtpActualReturn.Value = _editingRental.ActualReturnDate.Value;
                }

                // W trybie edycji nie można zmieniać pojazdu i klienta
                cmbVehicle.Enabled = false;
                cmbCustomer.Enabled = false;
            }
            else
            {
                // Tryb dodawania
                dtpStartDate.Value = DateTime.Now.Date;
                dtpEndDate.Value = DateTime.Now.Date.AddDays(1);
                cmbStatus.SelectedItem = RentalStatus.Active.ToString();

                if (cmbVehicle.Items.Count > 0) cmbVehicle.SelectedIndex = 0;
                if (cmbCustomer.Items.Count > 0) cmbCustomer.SelectedIndex = 0;
            }

            CalculateTotalCost();
        }

        private void CmbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            var status = (RentalStatus)Enum.Parse(typeof(RentalStatus), cmbStatus.SelectedItem.ToString());
            pnActualReturn.Visible = status == RentalStatus.Completed;
        }

        private void CalculateTotalCost()
        {
            if (cmbVehicle.SelectedItem is Vehicle vehicle)
            {
                int days = Math.Max(1, (dtpEndDate.Value.Date - dtpStartDate.Value.Date).Days + 1);
                decimal totalCost = days * vehicle.DailyRate + nudAdditionalCharges.Value;
                lblTotalCost.Text = $"Całkowity koszt: {totalCost:C}";
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbVehicle.SelectedItem == null || cmbCustomer.SelectedItem == null)
                {
                    MessageBox.Show("Wybierz pojazd i klienta", "Uwaga",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var vehicle = (Vehicle)cmbVehicle.SelectedItem;
                var customer = (Customer)cmbCustomer.SelectedItem;
                var status = (RentalStatus)Enum.Parse(typeof(RentalStatus), cmbStatus.SelectedItem.ToString());

                if (dtpStartDate.Value >= dtpEndDate.Value)
                {
                    MessageBox.Show("Data końcowa musi być późniejsza niż początkowa", "Uwaga",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_isEditMode)
                {
                    // Aktualizacja istniejącego wypożyczenia
                    _editingRental.StartDate = dtpStartDate.Value.Date;
                    _editingRental.EndDate = dtpEndDate.Value.Date;
                    _editingRental.Status = status;
                    _editingRental.AdditionalCharges = nudAdditionalCharges.Value;

                    if (pnActualReturn.Visible)
                    {
                        _editingRental.ActualReturnDate = dtpActualReturn.Value.Date;
                    }

                    int days = (_editingRental.EndDate - _editingRental.StartDate).Days + 1;
                    _editingRental.TotalCost = days * vehicle.DailyRate;

                    _rentalService.UpdateRental(_editingRental);
                    MessageBox.Show("Wypożyczenie zostało zaktualizowane", "Sukces",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Tworzenie nowego wypożyczenia
                    var rental = _rentalService.CreateRental(
                        vehicle.Id,
                        customer.Id,
                        dtpStartDate.Value.Date,
                        dtpEndDate.Value.Date);

                    if (nudAdditionalCharges.Value > 0)
                    {
                        rental.AdditionalCharges = nudAdditionalCharges.Value;
                        _rentalService.UpdateRental(rental);
                    }

                    MessageBox.Show($"Wypożyczenie #{rental.Id} zostało utworzone", "Sukces",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError("Błąd zapisu wypożyczenia", ex);
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
