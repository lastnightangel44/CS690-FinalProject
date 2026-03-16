using PetCareManagementSystem.Models;
using PetCareManagementSystem.Services;

namespace PetCareManagementSystem.UI
{
    public class ConsoleMenu
    {
        private UserService userService = new UserService();
        private PetService petService = new PetService();
        private AppointmentService appointmentService = new AppointmentService();
        private SupplyService supplyService = new SupplyService();
        private VaccinationService vaccinationService = new VaccinationService();

        private User currentUser;

        public void Run()
        {
            currentUser = Start();

            while (true)
            {
                Console.Clear();

                ShowDashboard();

                Console.WriteLine("\n==== MAIN MENU ====");
                Console.WriteLine("1 Manage Pets");
                Console.WriteLine("2 Schedule Appointment");
                Console.WriteLine("3 Track Supplies");
                Console.WriteLine("4 Vaccinations");
                Console.WriteLine("0 Exit");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ManagePets();
                        break;

                    case "2":
                        ScheduleAppointment();
                        break;

                    case "3":
                        ManageSupplies();
                        break;

                    case "4":
                        ManageVaccinations();
                        break;

                    case "0":
                        return;
                }
            }
        }

        public User Start()
        {
            while (true)
            {
                Console.WriteLine("==== Pet Care System ====");
                Console.WriteLine("1 Select User");
                Console.WriteLine("2 Create User");
                Console.WriteLine("0 Exit");

                var choice = Console.ReadLine();

                if (choice == "1")
                    return SelectUser();

                if (choice == "2")
                    CreateUser();

                if (choice == "0")
                    Environment.Exit(0);
            }
        }

        private User SelectUser()
        {
            var users = userService.GetUsers();

            if (users.Count == 0)
            {
                Console.WriteLine("No users found. Please create a user.");
                CreateUser();
                users = userService.GetUsers();
            }

            Console.WriteLine("\nSelect a user:");

            for (int i = 0; i < users.Count; i++)
                Console.WriteLine($"{i + 1}. {users[i].Name}");

            int choice = int.Parse(Console.ReadLine());

            return users[choice - 1];
        }

        private void CreateUser()
        {
            Console.Write("Enter name: ");
            var name = Console.ReadLine();

            userService.CreateUser(name);

            Console.WriteLine("User created.\n");
        }

        private void ShowDashboard()
        {
            Console.WriteLine("===== PET CARE DASHBOARD =====");

            var pets = petService.GetPetsByUser(currentUser.Id);

            Console.WriteLine("\nPets:");
            if (pets.Count == 0)
                Console.WriteLine("No pets registered.");
            else
                foreach (var pet in pets)
                    Console.WriteLine($"{pet.Name} ({pet.Species}) Age {pet.Age}");

            Console.WriteLine("\nUpcoming Appointments:");

            bool found = false;

            foreach (var pet in pets)
            {
                var appointments = appointmentService.GetAppointments(pet.Id);

                foreach (var a in appointments)
                {
                    if (a.Date >= DateTime.Now)
                    {
                        Console.WriteLine($"{pet.Name} - {a.AppointmentType} on {a.Date.ToShortDateString()}");
                        found = true;
                    }
                }
            }

            if (!found)
                Console.WriteLine("No upcoming appointments.");

            Console.WriteLine("\nLow Supplies:");

            var lowSupplies = supplyService.GetLowSupplies();

            if (lowSupplies.Count == 0)
                Console.WriteLine("All supplies sufficient.");
            else
                foreach (var s in lowSupplies)
                    Console.WriteLine($"{s.Name} - {supplyService.GetDaysRemaining(s)} days remaining");

            Console.WriteLine("\nVaccinations Due:");

            var due = vaccinationService.GetDueVaccinations();

            if (due.Count == 0)
                Console.WriteLine("No vaccinations due.");
            else
                foreach (var v in due)
                    Console.WriteLine($"{v.VaccineName} for Pet {v.PetId} due {v.NextDueDate.ToShortDateString()}");
        }

        private void ManagePets()
        {
            Console.WriteLine("\n1 Add Pet");
            Console.WriteLine("2 List Pets");

            var choice = Console.ReadLine();

            if (choice == "1")
            {
                Pet pet = new Pet();

                pet.Id = Guid.NewGuid().ToString();
                pet.UserId = currentUser.Id;

                Console.Write("Pet Name: ");
                pet.Name = Console.ReadLine();

                Console.Write("Species: ");
                pet.Species = Console.ReadLine();

                Console.Write("Breed: ");
                pet.Breed = Console.ReadLine();

                Console.Write("Age: ");
                pet.Age = int.Parse(Console.ReadLine());

                petService.AddPet(pet);

                Console.WriteLine("Pet added!");
                Console.ReadKey();
            }

            if (choice == "2")
            {
                var pets = petService.GetPetsByUser(currentUser.Id);

                foreach (var pet in pets)
                    Console.WriteLine($"{pet.Name} ({pet.Species}) Age {pet.Age}");

                Console.ReadKey();
            }
        }

        private void ScheduleAppointment()
        {
            Pet pet = SelectPet();

            appointment.PetId = selectedPet.Id;

            Console.Write("Appointment Type (Vet/Grooming): ");
            appointment.AppointmentType = Console.ReadLine();

            Console.Write("Date (yyyy-mm-dd): ");
            appointment.Date = DateTime.Parse(Console.ReadLine());

            appointmentService.AddAppointment(appointment);

            Console.WriteLine($"Appointment scheduled for {selectedPet.Name}! Select any key to continue.");
            Console.ReadKey();
        }

        private void ManageSupplies()
        {
            Console.WriteLine("\n1 Add Supply");
            Console.WriteLine("2 View Low Supplies");

            var choice = Console.ReadLine();

            if (choice == "1")
            {
                Supply supply = new Supply();

                Console.Write("Supply Name: ");
                supply.Name = Console.ReadLine();

                supply.PurchaseDate = DateTime.Now;

                Console.Write("Duration (days): ");
                supply.DurationDays = int.Parse(Console.ReadLine());

                supplyService.AddSupply(supply);

                Console.WriteLine("Supply added.");
                Console.ReadKey();
            }

            if (choice == "2")
            {
                var low = supplyService.GetLowSupplies();

                foreach (var s in low)
                    Console.WriteLine($"{s.Name} - {supplyService.GetDaysRemaining(s)} days remaining");

                Console.ReadKey();
            }
        }

        private void ManageVaccinations()
        {
            Console.WriteLine("\n1 Add Vaccination");
            Console.WriteLine("2 View Due Vaccinations");

            var choice = Console.ReadLine();

            if (choice == "1")
            {
                Pet pet = SelectPet();

                if (pet == null)
                    return;

                Vaccination vaccination = new Vaccination();

                vaccination.PetId = pet.Id;

                Console.Write("Vaccine Name: ");
                vaccination.VaccineName = Console.ReadLine();

                vaccination.DateGiven = DateTime.Now;

                Console.Write("Next Due Date (yyyy-mm-dd): ");
                vaccination.NextDueDate = DateTime.Parse(Console.ReadLine());

                vaccinationService.AddVaccination(vaccination);

                Console.WriteLine($"Vaccination recorded for {pet.Name}.");
                Console.ReadKey();
            }

            if (choice == "2")
            {
                var due = vaccinationService.GetDueVaccinations();

                foreach (var v in due)
                    Console.WriteLine($"{v.VaccineName} due {v.NextDueDate.ToShortDateString()}");

                Console.ReadKey();
            }
        }

        private Pet SelectPet()
        {
            var pets = petService.GetPetsByUser(currentUser.Id);

            if (pets.Count == 0)
            {
                Console.WriteLine("No pets found. Please add a pet first.");
                Console.ReadKey();
                return null;
            }

            Console.WriteLine("\nSelect a Pet:");

            for (int i = 0; i < pets.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {pets[i].Name}");
            }

            int choice = int.Parse(Console.ReadLine());

            return pets[choice - 1];
        }
    }
}