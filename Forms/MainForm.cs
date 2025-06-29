﻿using CarRentalSystem.Interfaces;
using CarRentalSystem.Models;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CarRentalSystem.Forms
{
    public partial class MainForm : Form
    {
        private readonly IVehicleService _vehicleService;
        private readonly ICustomerService _customerService;
        private readonly IRentalService _rentalService;
        private readonly ILogger _logger;

        private TabControl mainTabControl = null!;
        private TabPage dashboardTab = null!;
        private TabPage vehiclesTab = null!;
        private TabPage customersTab = null!;
        private TabPage rentalsTab = null!;

        // Dashboard controls
        private Panel dashboardPanel = null!;
        private Label titleLabel = null!;
        private Panel statsPanel = null!;
        private Panel availableVehiclesCard = null!;
        private Panel totalRentalsCard = null!;
        private Panel totalCustomersCard = null!;
        private Panel revenueCard = null!;

        // Vehicle controls
        private DataGridView vehiclesGrid = null!;
        private Button addVehicleBtn = null!;
        private Button editVehicleBtn = null!;
        private Button deleteVehicleBtn = null!;
        private Button refreshVehiclesBtn = null!;
        private TextBox searchVehiclesBox = null!;
        private Label searchVehiclesLabel = null!;

        // Customer controls
        private DataGridView customersGrid = null!;
        private Button addCustomerBtn = null!;
        private Button refreshCustomersBtn = null!;

        // Rental controls
        private DataGridView rentalsGrid = null!;
        private DataGridView dgvRentals = null!;
        private Button createRentalBtn = null!;
        private Button endRentalBtn = null!;
        private Button refreshRentalsBtn = null!;

        public MainForm(IVehicleService vehicleService, ICustomerService customerService,
                       IRentalService rentalService, ILogger logger)
        {
            _vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _rentalService = rentalService ?? throw new ArgumentNullException(nameof(rentalService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            InitializeComponent();
            SetupRentalsDataGridView();
            LoadAllData();
            _logger.LogInfo("Aplikacja CarRentalSystem uruchomiona pomyślnie");
        }

        private void InitializeComponent()
        {
            this.Text = "🚗 System Wypożyczalni Samochodów";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1000, 600);

            CreateMainTabControl();
            CreateDashboardTab();
            CreateVehiclesTab();
            CreateCustomersTab();
            CreateRentalsTab();

            this.Controls.Add(mainTabControl);
        }

        private void CreateMainTabControl()
        {
            mainTabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };

            dashboardTab = new TabPage("📊 Dashboard");
            vehiclesTab = new TabPage("🚗 Pojazdy");
            customersTab = new TabPage("👥 Klienci");


            mainTabControl.TabPages.AddRange(new TabPage[] {
                dashboardTab, vehiclesTab, customersTab, 
            });
        }

        private void CreateDashboardTab()
        {
            dashboardTab.BackColor = Color.FromArgb(240, 248, 255);

            dashboardPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            titleLabel = new Label
            {
                Text = "🚗 System Wypożyczalni Samochodów",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 118, 210),
                AutoSize = true,
                Location = new Point(20, 20)
            };

            statsPanel = new Panel
            {
                Location = new Point(20, 80),
                Size = new Size(1100, 200),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            CreateStatisticsCards();

            dashboardPanel.Controls.AddRange(new Control[] { titleLabel, statsPanel });
            dashboardTab.Controls.Add(dashboardPanel);
        }

        private void CreateStatisticsCards()
        {
            availableVehiclesCard = CreateStatCard("🟢 Dostępne Pojazdy",
                _vehicleService.GetAvailableVehicles().Count().ToString(),
                Color.FromArgb(76, 175, 80), new Point(20, 20));

            totalRentalsCard = CreateStatCard("🚗 Wszystkie Pojazdy",
                _vehicleService.GetAllVehicles().Count().ToString(),
                Color.FromArgb(33, 150, 243), new Point(280, 20));

            totalCustomersCard = CreateStatCard("👥 Klienci",
                _customerService.GetAllCustomers().Count().ToString(),
                Color.FromArgb(255, 152, 0), new Point(540, 20));

            revenueCard = CreateStatCard("💸 Wypożyczenia",
                _rentalService.GetAllRentals().Count().ToString(),
                Color.FromArgb(156, 39, 176), new Point(800, 20));

            statsPanel.Controls.AddRange(new Control[] {
                availableVehiclesCard, totalRentalsCard, totalCustomersCard, revenueCard
            });
        }

        private Panel CreateStatCard(string title, string value, Color color, Point location)
        {
            Panel card = new Panel
            {
                Size = new Size(240, 120),
                Location = location,
                BackColor = color,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label titleLbl = new Label
            {
                Text = title,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 15)
            };

            Label valueLbl = new Label
            {
                Text = value,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 50)
            };

            card.Controls.AddRange(new Control[] { titleLbl, valueLbl });
            return card;
        }

        private void CreateVehiclesTab()
        {
            vehiclesTab.BackColor = Color.FromArgb(250, 250, 250);

            searchVehiclesLabel = new Label
            {
                Text = "🔍 Szukaj:",
                Location = new Point(20, 20),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            searchVehiclesBox = new TextBox
            {
                Location = new Point(110, 18),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 10)
            };
            searchVehiclesBox.TextChanged += SearchVehiclesBox_TextChanged;

            addVehicleBtn = CreateStyledButton("➕ Dodaj Pojazd", new Point(20, 60), Color.FromArgb(76, 175, 80));
            editVehicleBtn = CreateStyledButton("✏️ Edytuj", new Point(160, 60), Color.FromArgb(33, 150, 243));
            deleteVehicleBtn = CreateStyledButton("🗑️ Usuń", new Point(280, 60), Color.FromArgb(244, 67, 54));
            refreshVehiclesBtn = CreateStyledButton("🔄 Odśwież", new Point(400, 60), Color.FromArgb(156, 39, 176));

            addVehicleBtn.Click += AddVehicleBtn_Click;
            editVehicleBtn.Click += EditVehicleBtn_Click;
            deleteVehicleBtn.Click += DeleteVehicleBtn_Click;
            refreshVehiclesBtn.Click += (s, e) => LoadVehicleData();

            vehiclesGrid = new DataGridView
            {
                Location = new Point(20, 110),
                Size = new Size(1120, 500),
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D
            };

            SetupVehiclesGridColumns();

            vehiclesTab.Controls.AddRange(new Control[] {
                searchVehiclesLabel, searchVehiclesBox, addVehicleBtn, editVehicleBtn,
                deleteVehicleBtn, refreshVehiclesBtn, vehiclesGrid
            });
        }

        private void SetupVehiclesGridColumns()
        {
            vehiclesGrid.Columns.AddRange(new DataGridViewColumn[] {
                new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", DataPropertyName = "Id", Width = 60 },
                new DataGridViewTextBoxColumn { Name = "Make", HeaderText = "Marka", DataPropertyName = "Make", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "Model", HeaderText = "Model", DataPropertyName = "Model", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "Year", HeaderText = "Rok", DataPropertyName = "Year", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "LicensePlate", HeaderText = "Nr Rej.", DataPropertyName = "LicensePlate", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "DailyRate", HeaderText = "Cena/dzień", DataPropertyName = "DailyRate", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Category", HeaderText = "Kategoria", DataPropertyName = "Category", Width = 100 },
                new DataGridViewCheckBoxColumn { Name = "IsAvailable", HeaderText = "Dostępny", DataPropertyName = "IsAvailable", Width = 80 }
            });
        }

    

        private void CreateCustomersTab()
        {
            customersTab.BackColor = Color.FromArgb(250, 250, 250);

            addCustomerBtn = CreateStyledButton("➕ Dodaj Klienta", new Point(20, 20), Color.FromArgb(76, 175, 80));
            var editCustomerBtn = CreateStyledButton("✏️ Edytuj", new Point(160, 20), Color.FromArgb(33, 150, 243));
            var deleteCustomerBtn = CreateStyledButton("🗑️ Usuń", new Point(280, 20), Color.FromArgb(244, 67, 54));
            refreshCustomersBtn = CreateStyledButton("🔄 Odśwież", new Point(400, 20), Color.FromArgb(156, 39, 176));

            addCustomerBtn.Click += AddCustomerBtn_Click;
            editCustomerBtn.Click += EditCustomerBtn_Click;
            deleteCustomerBtn.Click += DeleteCustomerBtn_Click;
            refreshCustomersBtn.Click += (s, e) => LoadCustomerData();

            customersGrid = new DataGridView
            {
                Location = new Point(20, 70),
                Size = new Size(1120, 540),
                AutoGenerateColumns = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                BackgroundColor = Color.White
            };

            customersTab.Controls.AddRange(new Control[] {
        addCustomerBtn, editCustomerBtn, deleteCustomerBtn, refreshCustomersBtn, customersGrid
    });
        }
        private void EditCustomerBtn_Click(object sender, EventArgs e)
        {
            if (customersGrid.SelectedRows.Count > 0)
            {
                try
                {
                    var selectedCustomer = (Customer)customersGrid.SelectedRows[0].DataBoundItem;
                    var customerForm = new CustomerFormDialog(_customerService, _logger);
                    if (customerForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadCustomerData();
                        RefreshDashboardStats();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Błąd edycji klienta", ex);
                    MessageBox.Show("Błąd edycji klienta", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Wybierz klienta do edycji", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DeleteCustomerBtn_Click(object sender, EventArgs e)
        {
            if (customersGrid.SelectedRows.Count > 0)
            {
                try
                {
                    var selectedCustomer = (Customer)customersGrid.SelectedRows[0].DataBoundItem;
                    var result = MessageBox.Show($"Czy na pewno chcesz usunąć klienta {selectedCustomer.FullName}?",
                        "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        _customerService.DeleteCustomer(selectedCustomer.Id);
                        LoadCustomerData();
                        RefreshDashboardStats();
                        _logger.LogInfo($"Usunięto klienta: {selectedCustomer.FullName}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Błąd usuwania klienta", ex);
                    MessageBox.Show("Błąd usuwania klienta", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Wybierz klienta do usunięcia", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SetupRentalsDataGridView()
        {
            if (dgvRentals == null) return;

            dgvRentals.Columns.Clear();

            dgvRentals.Columns.Add("Id", "ID");
            dgvRentals.Columns.Add("VehicleInfo", "Pojazd");
            dgvRentals.Columns.Add("CustomerInfo", "Klient");
            dgvRentals.Columns.Add("StartDate", "Data od");
            dgvRentals.Columns.Add("EndDate", "Data do");
            dgvRentals.Columns.Add("Status", "Status");
            dgvRentals.Columns.Add("TotalCost", "Koszt");

            // Ustawienia szerokości kolumn
            dgvRentals.Columns["Id"].Width = 50;
            dgvRentals.Columns["VehicleInfo"].Width = 150;
            dgvRentals.Columns["CustomerInfo"].Width = 120;
            dgvRentals.Columns["StartDate"].Width = 100;
            dgvRentals.Columns["EndDate"].Width = 100;
            dgvRentals.Columns["Status"].Width = 80;
            dgvRentals.Columns["TotalCost"].Width = 80;

            // Formatowanie kolumn dat
            dgvRentals.Columns["StartDate"].DefaultCellStyle.Format = "dd.MM.yyyy";
            dgvRentals.Columns["EndDate"].DefaultCellStyle.Format = "dd.MM.yyyy";
            dgvRentals.Columns["TotalCost"].DefaultCellStyle.Format = "C2";
        }

        private Button CreateStyledButton(string text, Point location, Color color)
        {
            return new Button
            {
                Text = text,
                Location = location,
                Size = new Size(130, 35),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
        }

        // Data Loading Methods
        private void LoadAllData()
        {
            LoadVehicleData();
            LoadCustomerData();
            LoadRentalData();
            RefreshDashboardStats();
        }

        private void LoadVehicleData()
        {
            try
            {
                vehiclesGrid.DataSource = null;
                vehiclesGrid.DataSource = _vehicleService.GetAllVehicles().ToList();
                _logger.LogInfo("Załadowano dane pojazdów");
            }
            catch (Exception ex)
            {
                _logger.LogError("Błąd ładowania pojazdów", ex);
                MessageBox.Show("Błąd ładowania danych pojazdów", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCustomerData()
        {
            try
            {
                customersGrid.DataSource = null;
                customersGrid.DataSource = _customerService.GetAllCustomers().ToList();
                _logger.LogInfo("Załadowano dane klientów");
            }
            catch (Exception ex)
            {
                _logger.LogError("Błąd ładowania klientów", ex);
                MessageBox.Show("Błąd ładowania danych klientów", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadRentalData()
        {
            try
            {
                if (dgvRentals == null) return;

                dgvRentals.Rows.Clear();

                foreach (var rental in _rentalService.GetAllRentals())
                {
                    var vehicle = _vehicleService.GetVehicleById(rental.VehicleId);
                    var customer = _customerService.GetCustomerById(rental.CustomerId);

                    string vehicleInfo = vehicle != null ? $"{vehicle.Make} {vehicle.Model}" : "Nieznany";
                    string customerInfo = customer != null ? customer.FullName : "Nieznany";

                    dgvRentals.Rows.Add(
                        rental.Id,
                        vehicleInfo,
                        customerInfo,
                        rental.StartDate.ToShortDateString(),
                        rental.EndDate.ToShortDateString(),
                        rental.Status.ToString(),
                        rental.TotalCost.ToString("C2")
                    );

                    // Kolorowanie wierszy według statusu
                    var row = dgvRentals.Rows[dgvRentals.Rows.Count - 1];
                    switch (rental.Status)
                    {
                        case RentalStatus.Active:
                            row.DefaultCellStyle.BackColor = Color.LightGreen;
                            break;
                        case RentalStatus.Completed:
                            row.DefaultCellStyle.BackColor = Color.LightBlue;
                            break;
                        case RentalStatus.Cancelled:
                            row.DefaultCellStyle.BackColor = Color.LightCoral;
                            break;
                    }
                }

                _logger.LogInfo("Załadowano dane wypożyczeń");
            }
            catch (Exception ex)
            {
                _logger.LogError("Błąd ładowania wypożyczeń", ex);
                MessageBox.Show("Błąd ładowania danych wypożyczeń", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshDashboardStats()
        {
            if (availableVehiclesCard?.Controls.Count > 1)
                ((Label)availableVehiclesCard.Controls[1]).Text = _vehicleService.GetAvailableVehicles().Count().ToString();

            if (totalRentalsCard?.Controls.Count > 1)
                ((Label)totalRentalsCard.Controls[1]).Text = _vehicleService.GetAllVehicles().Count().ToString();

            if (totalCustomersCard?.Controls.Count > 1)
                ((Label)totalCustomersCard.Controls[1]).Text = _customerService.GetAllCustomers().Count().ToString();

            if (revenueCard?.Controls.Count > 1)
                ((Label)revenueCard.Controls[1]).Text = _rentalService.GetAllRentals().Count().ToString();
        }

        // Event Handlers
        private void SearchVehiclesBox_TextChanged(object sender, EventArgs e)
        {
            if (searchVehiclesBox == null) return;

            string searchText = searchVehiclesBox.Text.ToLower();
            var filteredVehicles = _vehicleService.GetAllVehicles()
                .Where(v => v.Make.ToLower().Contains(searchText) ||
                           v.Model.ToLower().Contains(searchText) ||
                           v.LicensePlate.ToLower().Contains(searchText))
                .ToList();
            vehiclesGrid.DataSource = filteredVehicles;
        }

        private void AddVehicleBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var vehicleForm = new VehicleForm(_vehicleService, _logger);
                if (vehicleForm.ShowDialog() == DialogResult.OK)
                {
                    LoadVehicleData();
                    RefreshDashboardStats();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Błąd dodawania pojazdu", ex);
                MessageBox.Show("Błąd dodawania pojazdu", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditVehicleBtn_Click(object sender, EventArgs e)
        {
            if (vehiclesGrid.SelectedRows.Count > 0)
            {
                try
                {
                    var selectedVehicle = (Vehicle)vehiclesGrid.SelectedRows[0].DataBoundItem;
                    var vehicleForm = new VehicleForm(_vehicleService, _logger, selectedVehicle);
                    if (vehicleForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadVehicleData();
                        RefreshDashboardStats();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Błąd edycji pojazdu", ex);
                    MessageBox.Show("Błąd edycji pojazdu", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Wybierz pojazd do edycji", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DeleteVehicleBtn_Click(object sender, EventArgs e)
        {
            if (vehiclesGrid.SelectedRows.Count > 0)
            {
                try
                {
                    var selectedVehicle = (Vehicle)vehiclesGrid.SelectedRows[0].DataBoundItem;
                    var result = MessageBox.Show($"Czy na pewno chcesz usunąć pojazd {selectedVehicle.Make} {selectedVehicle.Model}?",
                        "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        _vehicleService.DeleteVehicle(selectedVehicle.Id);
                        LoadVehicleData();
                        RefreshDashboardStats();
                        _logger.LogInfo($"Usunięto pojazd: {selectedVehicle.Make} {selectedVehicle.Model}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Błąd usuwania pojazdu", ex);
                    MessageBox.Show("Błąd usuwania pojazdu", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Wybierz pojazd do usunięcia", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void AddCustomerBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var customerForm = new CustomerFormDialog(_customerService, _logger);
                if (customerForm.ShowDialog() == DialogResult.OK)
                {
                    LoadCustomerData();
                    RefreshDashboardStats();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Błąd dodawania klienta", ex);
                MessageBox.Show("Błąd dodawania klienta", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CreateRentalsTab()
        {
            // Tworzenie zakładki wypożyczeń
            rentalsTab = new TabPage("💸 Wypożyczenia");
            rentalsTab.UseVisualStyleBackColor = true;
            rentalsTab.BackColor = Color.FromArgb(250, 250, 250);

            // Dodanie przycisków
            createRentalBtn = CreateStyledButton("➕ Nowe Wypożyczenie",
                new Point(20, 20), Color.FromArgb(76, 175, 80));
            endRentalBtn = CreateStyledButton("✅ Zakończ",
                new Point(180, 20), Color.FromArgb(255, 152, 0));
            refreshRentalsBtn = CreateStyledButton("🔄 Odśwież",
                new Point(320, 20), Color.FromArgb(156, 39, 176));

            // Obsługa zdarzeń
            createRentalBtn.Click += CreateRentalBtn_Click;
            endRentalBtn.Click += EndRentalBtn_Click;
            refreshRentalsBtn.Click += (s, e) => LoadRentalData();

            // DataGridView dla wypożyczeń
            dgvRentals = new DataGridView
            {
                Location = new Point(20, 70),
                Size = new Size(1120, 540),
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                BackgroundColor = Color.White,
                MultiSelect = false
            };

            // Dodanie kontrolek do zakładki
            rentalsTab.Controls.AddRange(new Control[] {
        createRentalBtn, endRentalBtn, refreshRentalsBtn, dgvRentals
    });

            // Dodanie zakładki do TabControl
            mainTabControl.TabPages.Add(rentalsTab);
        }

        private void CreateRentalBtn_Click(object sender, EventArgs e)
        {
            try
            {
                using var form = new RentalForm(
                    _vehicleService,
                    _customerService,
                    _rentalService,
                    _logger);

                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadRentalData();
                    RefreshDashboardStats();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Błąd tworzenia wypożyczenia", ex);
                MessageBox.Show("Błąd tworzenia wypożyczenia", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EndRentalBtn_Click(object sender, EventArgs e)
        {
            if (dgvRentals?.SelectedRows.Count > 0)
            {
                try
                {
                    var selectedRow = dgvRentals.SelectedRows[0];
                    int rentalId = Convert.ToInt32(selectedRow.Cells["Id"].Value);
                    var rental = _rentalService.GetRentalById(rentalId);

                    if (rental != null)
                    {
                        _rentalService.EndRental(rentalId, DateTime.Now);
                        LoadAllData();
                        _logger.LogInfo($"Zakończono wypożyczenie {rental.Id}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Błąd kończenia wypożyczenia", ex);
                    MessageBox.Show("Błąd kończenia wypożyczenia", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Wybierz wypożyczenie do zakończenia", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
