using PetCareManagementSystem.Models;
using PetCareManagementSystem.UI;
using PetCareManagementSystem.Services;

namespace PetCareManagementSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleMenu menu = new ConsoleMenu();
            PetService petService = new PetService();
            AppointmentService appointmentService = new AppointmentService();
            SupplyService supplyService = new SupplyService();
            VaccinationService vaccinationService = new VaccinationService();
            
            User currentUser = menu.Start();

            if (currentUser == null)
                return;

            Console.WriteLine($"Welcome {currentUser.Name}!");
            ShowDashboard(currentUser, petService, appointmentService, supplyService, vaccinationService);

            while (true)
            {
                Console.WriteLine("\nActions:");
                Console.WriteLine("1 Manage Pets");
                Console.WriteLine("2 Manage Appointments");
                Console.WriteLine("3 Manage Vaccincations");
                Console.WriteLine("4 Manage Medications");
                Console.WriteLine("5 Manage Supplies");
                Console.WriteLine("0 Exit");

                var choice = Console.ReadLine();

                if (choice == "0")
                    break;
            }

            static void ShowDashboard(
                User user,
                PetService petService,
                AppointmentService appointmentService,
                SupplyService supplyService,
                VaccinationService vaccinationService)
            {
                Console.WriteLine("\n===== PET CARE DASHBOARD =====");

                var pets = petService.GetPetsByUser(user.Id);

                Console.WriteLine("\nPets:");
                if (pets.Count == 0)
                {
                    Console.WriteLine("No pets registered.");
                }
                else
                {
                    foreach (var pet in pets)
                    {
                        Console.WriteLine($"- {pet.Name} ({pet.Species}) Age {pet.Age}");
                    }
                }

                Console.WriteLine("\nUpcoming Appointments:");

                bool appointmentFound = false;

                foreach (var pet in pets)
                {
                    var appointments = appointmentService.GetAppointments(pet.Id);

                    foreach (var a in appointments)
                    {
                        if (a.Date >= DateTime.Now)
                        {
                            Console.WriteLine($"{pet.Name} - {a.AppointmentType} on {a.Date.ToShortDateString()}");
                            appointmentFound = true;
                        }
                    }
                }

                if (!appointmentFound)
                    Console.WriteLine("No upcoming appointments.");

                Console.WriteLine("\nLow Supplies:");

                var lowSupplies = supplyService.GetLowSupplies();

                if (lowSupplies.Count == 0)
                {
                    Console.WriteLine("All supplies are sufficient.");
                }
                else
                {
                    foreach (var supply in lowSupplies)
                    {
                        int days = supplyService.GetDaysRemaining(supply);
                        Console.WriteLine($"{supply.Name} - {days} days remaining");
                    }
                }

                Console.WriteLine("\nVaccinations Due:");

                var dueVaccines = vaccinationService.GetDueVaccinations();

                if (dueVaccines.Count == 0)
                {
                    Console.WriteLine("No vaccinations due.");
                }
                else
                {
                    foreach (var v in dueVaccines)
                    {
                        Console.WriteLine($"{v.VaccineName} for Pet {v.PetId} due {v.NextDueDate.ToShortDateString()}");
                    }
                }

                Console.WriteLine("\n===============================");
            }
        }
    }
}