using CandidQV.Models.Items;
using CandidQV.Repositories;

namespace CandidQV.Views;

public partial class VoucherCreationPage : ContentPage
{
    private readonly VoucherRepository voucherRepository;
    private readonly EmployeeRepository employeeRepository;
    private readonly AirlineRepository airlineRepository;

    public VoucherCreationPage(VoucherRepository voucherRepository, EmployeeRepository employeeRepository,AirlineRepository airlineRepository)
	{
		InitializeComponent();
        this.voucherRepository = voucherRepository;
        this.employeeRepository = employeeRepository;
        this.airlineRepository = airlineRepository;
        startTimePicker.Time = DateTime.Now.TimeOfDay;
        endTimePicker.Time = DateTime.Now.TimeOfDay;
    }

    private async void saveBtn_Clicked(object sender, EventArgs e)
    {
        await voucherRepository.Create(new Models.Items.Voucher
        {
            PassengerName = passengerNameEntryField.Text,
            IsUsDeparture = usDepartureRad.Checked,
            EmployeeID = ((Employee)officerNamePicker.SelectedItem).Id,
            AirlineId = ((Airline)airlinePicker.SelectedItem).Id,
            StartTime = startTimePicker.Time,
            EndTime = endTimePicker.Time,
            Date = DateOnly.FromDateTime(DateTime.Now)
        });
    }

    private void takePhotoBtn_Clicked(object sender, EventArgs e)
    {

    }
}