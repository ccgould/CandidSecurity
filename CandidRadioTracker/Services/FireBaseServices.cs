using CandidRadioTracker.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;

namespace CandidRadioTracker;
public partial class FireBaseServices : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Employee> employees;
    [ObservableProperty]
    private ObservableCollection<Radio> radios;
    public FireBaseServices()
    {
        Task.Run(Register);
    }

    internal async Task SaveRadioLog(string barcode, int id, DateOnly date, TimeOnly outTime)
    {
        FirebaseClient client = new FirebaseClient("https://candid-9aadf-default-rtdb.firebaseio.com/");

        await client.Child("RadioLogs").PostAsync(new RadioLog
        {
            OutTime = outTime.ToString(),
            Date = date.ToString(),
            EmployeeId = id,
            RadioId = barcode
        });
    }

    private async Task Register()
    {
        FirebaseClient client = new FirebaseClient("https://candid-9aadf-default-rtdb.firebaseio.com/");
        var radios = await client.Child("Radios").OnceAsync<Radio>();
        if (!radios.Any())
        {
            await client.Child("Radios").PostAsync(new Radio { Id = "10001" });
            await client.Child("Radios").PostAsync(new Radio { Id = "10002" });
            await client.Child("Radios").PostAsync(new Radio { Id = "10003" });
            await client.Child("Radios").PostAsync(new Radio { Id = "10004" });
            await client.Child("Radios").PostAsync(new Radio { Id = "10005" });
            await client.Child("Radios").PostAsync(new Radio { Id = "10006" });
            await client.Child("Radios").PostAsync(new Radio { Id = "10007" });
            await client.Child("Radios").PostAsync(new Radio { Id = "10008" });
            await client.Child("Radios").PostAsync(new Radio { Id = "10009" });
            await client.Child("Radios").PostAsync(new Radio { Id = "10010" });
            await client.Child("Radios").PostAsync(new Radio { Id = "10011" });
            await client.Child("Radios").PostAsync(new Radio { Id = "10012" });
            await client.Child("Radios").PostAsync(new Radio { Id = "10013" });
            await client.Child("Radios").PostAsync(new Radio { Id = "10014" });
        }

        Radios = new ObservableCollection<Radio>(radios.Select(x => x.Object).ToList());

         var employees = await client.Child("Employees").OnceAsync<Employee>();
        var i = 0;

        if (!employees.Any())
        {
            var names = new List<string>
            {
                "Annya Ward", "Antonia Lesbott", "Ashton Rolle", "Bradley Strachan", "Creswell Gould",
                "Dakajah Holmes", "Daron Major", "Davon Ferguson", "Deborah Nixon", "Devon Burrows",
                "Domonique Higgs", "Dieuna Abraham", "Dwayne Duncombe", "Earl Hall Jr.", "Gloyd Wilson Jr",
                "Jalomey Brooks", "Jeremiah Kemp", "Jessica Rolle", "Kamille Paul", "Kenya Johnson",
                "Livingdra Frazier", "Lorraine Moncur", "Louidna Forest", "Marcelo Bain", "McDahl Dean",
                "Merlin Bowe", "Michaella Larimore", "Prince Rigby", "Robert Parker", "Synae Smith",
                "Shereka Greene", "Sterleka Forbes", "Tanika Sweeting", "Tatyanna Bonaby", "Tony Bowe",
                "Wendron Mortimer"
            };

            foreach (var name in names)
            {
                await client.Child("Employees").PostAsync(new Employee { Id = i++, Name = name });
            }
        }

        Employees = new ObservableCollection<Employee>(employees.Select(x => x.Object).ToList());

    }
}
