using PetCareManagementSystem.Models;
using PetCareManagementSystem.Services;

namespace PetCareManagementSystem.UI
{
    /// <summary>
    /// Console-based user interface for the Pet Care Management System.
    /// </summary>
    public class ConsoleMenu
    {
        private readonly UserService userService = new UserService();
        private readonly PetService petService = new PetService();
        private readonly AppointmentService appointmentService = new AppointmentService();
        private readonly SupplyService supplyService = new SupplyService();
        private readonly VaccinationService vaccinationService = new VaccinationService();
        private readonly MedicationService medicationService   = new MedicationService();

        private User currentUser;

        /// <summary>
        /// Starts the UI. Handles user selection, then loops through the main menu.
        /// </summary>
        public void Run()
        {
            currentUser = Start();

            while (true)
            {
                Console.Clear();
                ShowDashboard();

                Console.WriteLine("\n==== MAIN MENU ====");
                Console.WriteLine("1 Manage Pets");
                Console.WriteLine("2 Manage Appointment");
                Console.WriteLine("3 Manage Supplies");
                Console.WriteLine("4 Manage Vaccinations");
                Console.WriteLine("5 Manage Medications");
                Console.WriteLine("6 Manage Users");
                Console.WriteLine("0 Exit");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": ManagePets();             break;
                    case "2": ManageAppointments();     break;
                    case "3": ManageSupplies();         break;
                    case "4": ManageVaccinations();     break;
                    case "5": ManageMedications();      break;
                    case "6": ManageUsers();            break;
                    case "0": return;
                }
            }
        }

        /// <summary>
        /// Shows the startup screen and prompts the user to select or create a profile.
        /// Loops until a valid user is returned.
        /// </summary>
        public User Start()
        {
            while (true)
            {
                Console.WriteLine("==== Pet Care System ====");
                Console.WriteLine("1 Select User");
                Console.WriteLine("2 Create User");
                Console.WriteLine("0 Exit");

                var choice = Console.ReadLine();

                if (choice == "1") return SelectUser();
                if (choice == "2") CreateUser();
                if (choice == "0") Environment.Exit(0);
            }
        }

        /// <summary>
        /// Displays existing users and returns the one selected.
        /// If no users exist, prompts to create one first.
        /// </summary>        
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
                Console.WriteLine($"  {i + 1}. {users[i].Name}");

            int choice = int.Parse(Console.ReadLine());
            return users[choice - 1];
        }

        /// <summary>
        /// Prompts for a name and creates a new user profile.
        /// </summary>
        private void CreateUser()
        {
            Console.Write("Enter name: ");
            var name = Console.ReadLine();
            userService.CreateUser(name);
            Console.WriteLine("User created.\n");
        }

        /// <summary>
        /// Shows an overview of the user's pets, upcoming appointments,
        /// low supplies, and vaccinations that are due.
        /// </summary>
        private void ShowDashboard()
        {
            Console.WriteLine("===== PET CARE DASHBOARD =====");

            var pets = petService.GetPetsByUser(currentUser.Id);

            Console.WriteLine("\nPets:");
            if (pets.Count == 0)
                Console.WriteLine("  No pets registered.");
            else
                foreach (var pet in pets)
                    Console.WriteLine($"  {pet.Name} ({pet.Species}) Age {pet.Age}");

            // --- Upcoming Appointments --- 
            Console.WriteLine("\nUpcoming Appointments:");
            bool found = false;
            foreach (var pet in pets)
            {
                foreach (var a in appointmentService.GetAppointments(pet.Id))
                {
                    if (a.Date >= DateTime.Now)
                    {
                        Console.WriteLine($"  {pet.Name} - {a.AppointmentType} on {a.Date.ToShortDateString()}");
                        found = true;
                    }
                }
            }
            if (!found)
                Console.WriteLine("  No upcoming appointments.");

            // --- Low Supplies --- 
            Console.WriteLine("\nLow Supplies:");
            var lowSupplies = supplyService.GetLowSupplies();
            if (lowSupplies.Count == 0)
                Console.WriteLine("All supplies sufficient.");
            else
                foreach (var s in lowSupplies)
                    Console.WriteLine($"  {s.Name} - {supplyService.GetDaysRemaining(s)} days remaining");

            // --- Vaccinations Due ---
            Console.WriteLine("\nVaccinations Due:");
            bool anyDue = false;
            foreach (var pet in pets)
            {
                foreach (var v in vaccinationService.GetVaccinations(pet.Id))
                {
                    if (v.NextDueDate <= DateTime.Now)
                    {
                        Console.WriteLine($"  {v.VaccineName} for {pet.Name} due {v.NextDueDate.ToShortDateString()}");
                        anyDue = true;
                    }
                }
            }
            if (!anyDue) Console.WriteLine("  No vaccinations due.");

            // --- Active Medications ---
            Console.WriteLine("\nActive Medications:");
            bool anyMeds = false;
            foreach (var pet in pets)
            {
                var meds = medicationService.GetActiveMedications(pet.Id);
                foreach (var m in meds)
                {
                    string endDisplay = m.EndDate.HasValue
                        ? $"until {m.EndDate.Value.ToShortDateString()}"
                        : "ongoing";
                    Console.WriteLine($"  {pet.Name} - {m.Name} {m.Dosage} {m.Frequency} ({endDisplay})");
                    anyMeds = true;
                }
            }
            if (!anyMeds) Console.WriteLine("  No active medications.");
        }

        /// <summary>
        /// Displays the pet menu and handles all pet actions
        /// including adding, viewing, editing, and deleting pets.
        /// Delegates all data operations to PetService.
        /// </summary>
        private void ManagePets()
        {
            Console.WriteLine("\n==== MANAGE PETS ====");
            Console.WriteLine("1 Add Pet");
            Console.WriteLine("2 List Pets");
            Console.WriteLine("3 Edit Pet");
            Console.WriteLine("4 Delete Pet");
            Console.WriteLine("0 Back");

            var choice = Console.ReadLine();

            if (choice == "1")
            {
                // add pet
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
                Console.WriteLine("Pet added! Press any key to continue.");
                Console.ReadKey();
            }
             else if (choice == "2")
            {
                // list pet
                var pets = petService.GetPetsByUser(currentUser.Id);

                foreach (var pet in pets)
                    Console.WriteLine($"  {pet.Name} ({pet.Species}) Age {pet.Age}");
                
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
            else if (choice == "3")
            {
                // Edit pet 
                var pet = SelectPet();
                if (pet == null) return;

                Console.WriteLine($"\nEditing {pet.Name}. Press Enter to keep the current value.");

                Console.Write($"Name [{pet.Name}]: ");
                var input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) pet.Name = input;

                Console.Write($"Species [{pet.Species}]: ");
                input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) pet.Species = input;

                Console.Write($"Breed [{pet.Breed}]: ");
                input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) pet.Breed = input;

                Console.Write($"Age [{pet.Age}]: ");
                input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) pet.Age = int.Parse(input);

                petService.UpdatePet(pet);
                Console.WriteLine("Pet updated! Press any key to continue.");
                Console.ReadKey();
            }
            else if (choice == "4")
            {
                // delete pet
                var pet = SelectPet();
                if (pet == null) return;

                Console.Write($"Are you sure you want to delete {pet.Name}? (y/n): ");
                if (Console.ReadLine()?.ToLower() == "y")
                {
                    petService.DeletePet(pet.Id);
                    Console.WriteLine("Pet deleted.");
                }
                else
                {
                    Console.WriteLine("Cancelled.");
                }
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Displays the appointment menu and handles all appointment management actions
        /// including adding, viewing, editing, and deleting appointments.
        /// Delegates all data operations to AppointmentService.
        /// </summary>
        private void ManageAppointments()
        {
            Console.WriteLine("\n==== MANAGE APPOINTMENTS ====");
            Console.WriteLine("1 Schedule Appointment");
            Console.WriteLine("2 View All Appointments");
            Console.WriteLine("3 Edit Appointment");
            Console.WriteLine("4 Delete Appointment");
            Console.WriteLine("0 Back");

            var choice = Console.ReadLine();

            if (choice == "1")
            {
                // Schedule appointment
                var pet = SelectPet();
                if (pet == null) return;

                var appointment = new Appointment { PetId = pet.Id };

                Console.Write("Appointment Type (Vet/Grooming): ");
                appointment.AppointmentType = Console.ReadLine();

                Console.Write("Date (yyyy-mm-dd): ");
                appointment.Date = DateTime.Parse(Console.ReadLine());

                Console.Write("Location: ");
                appointment.Location = Console.ReadLine();

                appointmentService.AddAppointment(appointment);
                Console.WriteLine($"Appointment scheduled for {pet.Name}! Press any key to continue.");
                Console.ReadKey();
            }
            else if (choice == "2")
            {
                // view all appointments
                var pets = petService.GetPetsByUser(currentUser.Id);
                bool any = false;

                foreach (var pet in pets)
                {
                    var appts = appointmentService.GetAppointments(pet.Id);
                    foreach (var a in appts)
                    {
                        Console.WriteLine($"{pet.Name} | {a.AppointmentType} | {a.Date.ToShortDateString()} | {a.Location}");
                        any = true;
                    }
                }

                if (!any) Console.WriteLine("No appointments found.");
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
            else if (choice == "3")
            {
                // edit appointments
                var appointments = GetCurrentUserAppointments(out var petLookup);

                if (appointments.Count == 0)
                {
                    Console.WriteLine("No appointments to edit. Press any key to continue.");
                    Console.ReadKey();
                    return;
                }

                PrintAppointmentList(appointments, petLookup);

                Console.Write("Select appointment number to edit: ");
                int idx = int.Parse(Console.ReadLine()) - 1;
                var original = appointments[idx];
                var updated = CopyAppointment(original);

                Console.WriteLine("\nPress Enter to keep the current value.");

                Console.Write($"Type [{updated.AppointmentType}]: ");
                var input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) updated.AppointmentType = input;

                Console.Write($"Date [{updated.Date.ToShortDateString()}]: ");
                input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) updated.Date = DateTime.Parse(input);

                Console.Write($"Location [{updated.Location}]: ");
                input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) updated.Location = input;

                appointmentService.UpdateAppointment(original, updated);
                Console.WriteLine("Appointment updated! Press any key to continue.");
                Console.ReadKey();
            }
            else if (choice == "4")
            {
                // delete appointments
                var appointments = GetCurrentUserAppointments(out var petLookup);

                if (appointments.Count == 0)
                {
                    Console.WriteLine("No appointments to delete. Press any key to continue.");
                    Console.ReadKey();
                    return;
                }

                PrintAppointmentList(appointments, petLookup);

                Console.Write("Select appointment number to delete: ");
                int idx = int.Parse(Console.ReadLine()) - 1;
                var target = appointments[idx];

                Console.Write($"Delete {target.AppointmentType} on {target.Date.ToShortDateString()}? (y/n): ");
                if (Console.ReadLine()?.ToLower() == "y")
                {
                    appointmentService.DeleteAppointment(target);
                    Console.WriteLine("Appointment deleted.");
                }
                else
                {
                    Console.WriteLine("Cancelled.");
                }
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Displays the supplies menu and handles all supply management actions
        /// including adding, viewing, editing, and deleting supplies.
        /// Delegates all data operations to SupplyService.
        /// </summary>
        private void ManageSupplies()
        {
            Console.WriteLine("\n==== MANAGE SUPPLIES ====");
            Console.WriteLine("1 Add Supply");
            Console.WriteLine("2 View All Supplies");
            Console.WriteLine("3 View Low Supplies");
            Console.WriteLine("4 Edit Supply");
            Console.WriteLine("5 Delete Supply");
            Console.WriteLine("0 Back");

            var choice = Console.ReadLine();

            if (choice == "1")
            {
                // add supply
                var supply = new Supply { PurchaseDate = DateTime.Now };

                Console.Write("Supply Name: ");
                supply.Name = Console.ReadLine();

                Console.Write("Duration (days): ");
                supply.DurationDays = int.Parse(Console.ReadLine());

                supplyService.AddSupply(supply);
                Console.WriteLine("Supply added! Press any key to continue.");
                Console.ReadKey();
            }
            else if (choice == "2")
            {
                // view all supplies
                var supplies = supplyService.GetSupplies();

                if (supplies.Count == 0)
                    Console.WriteLine("No supplies recorded.");
                else
                    foreach (var s in supplies)
                        Console.WriteLine($"{s.Name} | Purchased {s.PurchaseDate.ToShortDateString()} | {supplyService.GetDaysRemaining(s)} days remaining");

                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
            else if (choice == "3")
            {
                // view low supplies
                var low = supplyService.GetLowSupplies();

                if (low.Count == 0)
                    Console.WriteLine("All supplies sufficient.");
                else
                    foreach (var s in low)
                        Console.WriteLine($"{s.Name} - {supplyService.GetDaysRemaining(s)} days remaining");
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
            else if (choice == "4")
            {
                // edit supplies
                var supplies = supplyService.GetSupplies();

                if (supplies.Count == 0)
                {
                    Console.WriteLine("No supplies to edit. Press any key to continue.");
                    Console.ReadKey();
                    return;
                }

                PrintSupplyList(supplies);

                Console.Write("Select supply number to edit: ");
                int idx = int.Parse(Console.ReadLine()) - 1;
                var original = supplies[idx];
                var updated = new Supply
                {
                    Name         = original.Name,
                    PurchaseDate = original.PurchaseDate,
                    DurationDays = original.DurationDays
                };

                Console.WriteLine("\nPress Enter to keep the current value.");

                Console.Write($"Name [{updated.Name}]: ");
                var input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) updated.Name = input;

                Console.Write($"Duration days [{updated.DurationDays}]: ");
                input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) updated.DurationDays = int.Parse(input);

                Console.Write($"Reset purchase date to today? (y/n): ");
                if (Console.ReadLine()?.ToLower() == "y")
                    updated.PurchaseDate = DateTime.Now;

                supplyService.UpdateSupply(original, updated);
                Console.WriteLine("Supply updated! Press any key to continue.");
                Console.ReadKey();
            }
            else if (choice == "5")
            {
                // delete supplies
                var supplies = supplyService.GetSupplies();

                if (supplies.Count == 0)
                {
                    Console.WriteLine("No supplies to delete. Press any key to continue. ");
                    Console.ReadKey();
                    return;
                }

                PrintSupplyList(supplies);

                Console.Write("Select supply number to delete: ");
                int idx = int.Parse(Console.ReadLine()) - 1;
                var target = supplies[idx];

                Console.Write($"Delete {target.Name}? (y/n): ");
                if (Console.ReadLine()?.ToLower() == "y")
                {
                    supplyService.DeleteSupply(target);
                    Console.WriteLine("Supply deleted.");
                }
                else
                {
                    Console.WriteLine("Cancelled.");
                }
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Displays the vaccinations menu and handles all vaccination management actions
        /// including adding, viewing, editing, and deleting vaccination records.
        /// Delegates all data operations to VaccinationService.
        /// </summary>
        private void ManageVaccinations()
        {
            Console.WriteLine("\n==== MANAGE VACCINATIONS ====");
            Console.WriteLine("1 Add Vaccination");
            Console.WriteLine("2 View All Vaccinations");
            Console.WriteLine("3 View Due Vaccinations");
            Console.WriteLine("4 Edit Vaccination");
            Console.WriteLine("5 Delete Vaccination");
            Console.WriteLine("0 Back");

            var choice = Console.ReadLine();

            if (choice == "1")
            {
                // new vaccination
                var pet = SelectPet();
                if (pet == null) return;

                var vaccination = new Vaccination
                {
                    PetId     = pet.Id,
                    DateGiven = DateTime.Now
                };

                Console.Write("Vaccine Name: ");
                vaccination.VaccineName = Console.ReadLine();

                Console.Write("Next Due Date (yyyy-mm-dd): ");
                vaccination.NextDueDate = DateTime.Parse(Console.ReadLine());

                vaccinationService.AddVaccination(vaccination);
                Console.WriteLine($"Vaccination recorded for {pet.Name}! Press any key to continue.");
                Console.ReadKey();
            }
            else if (choice == "2")
            {
                // View all vaccinantions
                var pets = petService.GetPetsByUser(currentUser.Id);
                bool any = false;

                foreach (var pet in pets)
                {
                    var vaccinations = vaccinationService.GetVaccinations(pet.Id);
                    foreach (var v in vaccinations)
                    {
                        Console.WriteLine($"{pet.Name} | {v.VaccineName} | Given {v.DateGiven.ToShortDateString()} | Due {v.NextDueDate.ToShortDateString()}");
                        any = true;
                    }
                }

                if (!any) Console.WriteLine("No vaccinations recorded. Press any key to continue.");
                Console.ReadKey();
            }
            else if (choice == "3")
            {
                //view due or over due vaccinations
                var due = vaccinationService.GetDueVaccinations();

                if (due.Count == 0)
                    Console.WriteLine("No vaccinations due.");
                else
                    foreach (var v in due)
                        Console.WriteLine($"{v.VaccineName} due {v.NextDueDate.ToShortDateString()}");
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
            else if (choice == "4")
            {
                //edit vaccinations
                var vaccinations = GetCurrentUserVaccinations(out var petLookup);

                if (vaccinations.Count == 0)
                {
                    Console.WriteLine("No vaccinations to edit. Press any key to continue.");
                    Console.ReadKey();
                    return;
                }

                PrintVaccinationList(vaccinations, petLookup);

                Console.Write("Select vaccination number to edit: ");
                int idx = int.Parse(Console.ReadLine()) - 1;
                var original = vaccinations[idx];
                var updated = CopyVaccination(original);

                Console.WriteLine("\nPress Enter to keep the current value.");

                Console.Write($"Vaccine Name [{updated.VaccineName}]: ");
                var input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) updated.VaccineName = input;

                Console.Write($"Next Due Date [{updated.NextDueDate.ToShortDateString()}]: ");
                input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) updated.NextDueDate = DateTime.Parse(input);

                vaccinationService.UpdateVaccination(original, updated);
                Console.WriteLine("Vaccination updated! Press any key to continue.");
                Console.ReadKey();
            }
            else if (choice == "5")
            {
                // delete vaccinations
                var vaccinations = GetCurrentUserVaccinations(out var petLookup);

                if (vaccinations.Count == 0)
                {
                    Console.WriteLine("No vaccinations to delete. Press any key to continue.");
                    Console.ReadKey();
                    return;
                }

                PrintVaccinationList(vaccinations, petLookup);

                Console.Write("Select vaccination number to delete: ");
                int idx = int.Parse(Console.ReadLine()) - 1;
                var target = vaccinations[idx];

                Console.Write($"Delete {target.VaccineName}? (y/n): ");
                if (Console.ReadLine()?.ToLower() == "y")
                {
                    vaccinationService.DeleteVaccination(target);
                    Console.WriteLine("Vaccination deleted.");
                }
                else
                {
                    Console.WriteLine("Cancelled.");
                }
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Displays the medications menu and handles all medication and treatment
        /// management actions including adding, viewing, editing, and deleting records.
        /// </summary>
        private void ManageMedications()
        {
            Console.WriteLine("\n==== MANAGE MEDICATIONS & TREATMENTS ====");
            Console.WriteLine("1 Add Medication");
            Console.WriteLine("2 View All Medications");
            Console.WriteLine("3 View Active Medications");
            Console.WriteLine("4 Edit Medication");
            Console.WriteLine("5 Delete Medication");
            Console.WriteLine("0 Back");

            var choice = Console.ReadLine();

            if (choice == "1")
            {
                var pet = SelectPet();
                if (pet == null) return;

                var medication = new Medication
                {
                    Id        = Guid.NewGuid().ToString(),
                    PetId     = pet.Id,
                    StartDate = DateTime.Today
                };

                Console.Write("Medication / Treatment Name: ");
                medication.Name = Console.ReadLine();

                Console.Write("Dosage (e.g., 5mg, 1 tablet): ");
                medication.Dosage = Console.ReadLine();

                Console.Write("Frequency (e.g., Once daily, Every 8 hours): ");
                medication.Frequency = Console.ReadLine();

                Console.Write("End Date (yyyy-mm-dd) or press Enter if ongoing: ");
                var endInput = Console.ReadLine();
                medication.EndDate = string.IsNullOrWhiteSpace(endInput)
                    ? null
                    : DateTime.Parse(endInput);

                Console.Write("Notes (e.g., Give with food) or press Enter to skip: ");
                medication.Notes = Console.ReadLine() ?? string.Empty;

                medicationService.AddMedication(medication);
                Console.WriteLine($"Medication recorded for {pet.Name}!");
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
            else if (choice == "2")
            {
                var pets = petService.GetPetsByUser(currentUser.Id);
                bool any = false;

                foreach (var pet in pets)
                {
                    foreach (var m in medicationService.GetMedications(pet.Id))
                    {
                        string endDisplay = m.EndDate.HasValue
                            ? m.EndDate.Value.ToShortDateString()
                            : "Ongoing";
                        Console.WriteLine($"{pet.Name} | {m.Name} | {m.Dosage} | {m.Frequency} | Until {endDisplay}");
                        if (!string.IsNullOrWhiteSpace(m.Notes))
                            Console.WriteLine($"         Notes: {m.Notes}");
                        any = true;
                    }
                }

                if (!any) Console.WriteLine("No medications recorded.");
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
            else if (choice == "3")
            {
                var pets = petService.GetPetsByUser(currentUser.Id);
                bool any = false;

                foreach (var pet in pets)
                {
                    foreach (var m in medicationService.GetActiveMedications(pet.Id))
                    {
                        string endDisplay = m.EndDate.HasValue
                            ? $"until {m.EndDate.Value.ToShortDateString()}"
                            : "ongoing";
                        Console.WriteLine($"{pet.Name} | {m.Name} | {m.Dosage} | {m.Frequency} ({endDisplay})");
                        any = true;
                    }
                }

                if (!any) Console.WriteLine("No active medications.");
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
            else if (choice == "4")
            {
                var medications = GetCurrentUserMedications(out var petLookup);

                if (medications.Count == 0)
                {
                    Console.WriteLine("No medications to edit.");
                    Console.WriteLine("Press any key to continue.");
                    Console.ReadKey();
                    return;
                }

                PrintMedicationList(medications, petLookup);

                Console.Write("Select medication number to edit: ");
                int idx = int.Parse(Console.ReadLine()) - 1;
                var med = medications[idx];

                Console.WriteLine("\nPress Enter to keep the current value.");

                Console.Write($"Name [{med.Name}]: ");
                var input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) med.Name = input;

                Console.Write($"Dosage [{med.Dosage}]: ");
                input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) med.Dosage = input;

                Console.Write($"Frequency [{med.Frequency}]: ");
                input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) med.Frequency = input;

                string currentEnd = med.EndDate.HasValue
                    ? med.EndDate.Value.ToShortDateString()
                    : "Ongoing";
                Console.Write($"End Date [{currentEnd}] (yyyy-mm-dd or press Enter to keep, type 'clear' for ongoing): ");
                input = Console.ReadLine();
                if (input?.ToLower() == "clear")
                    med.EndDate = null;
                else if (!string.IsNullOrWhiteSpace(input))
                    med.EndDate = DateTime.Parse(input);

                Console.Write($"Notes [{med.Notes}]: ");
                input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) med.Notes = input;

                medicationService.UpdateMedication(med);
                Console.WriteLine("Medication updated!");
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
            else if (choice == "5")
            {
                var medications = GetCurrentUserMedications(out var petLookup);

                if (medications.Count == 0)
                {
                    Console.WriteLine("No medications to delete.");
                    Console.WriteLine("Press any key to continue.");
                    Console.ReadKey();
                    return;
                }

                PrintMedicationList(medications, petLookup);

                Console.Write("Select medication number to delete: ");
                int idx = int.Parse(Console.ReadLine()) - 1;
                var target = medications[idx];

                Console.Write($"Delete {target.Name} for {petLookup[target.PetId].Name}? (y/n): ");
                if (Console.ReadLine()?.ToLower() == "y")
                {
                    medicationService.DeleteMedication(target.Id);
                    Console.WriteLine("Medication deleted.");
                }
                else
                {
                    Console.WriteLine("Cancelled.");
                }
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Displays the user menu and handles all user actions
        /// including viewing users, editing user name, and deleting users.
        /// Delegates all data operations to UserService.
        /// </summary>
        private void ManageUsers()
        {
            Console.WriteLine("\n==== MANAGE USERS ====");
            Console.WriteLine("1 List Users");
            Console.WriteLine("2 Edit Current User Name");
            Console.WriteLine("3 Delete a User");
            Console.WriteLine("0 Back");

            var choice = Console.ReadLine();

            if (choice == "1")
            {
                // list users
                var users = userService.GetUsers();
                foreach (var u in users)
                    Console.WriteLine($"{u.Name} (id: {u.Id})");
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
            else if (choice == "2")
            {
                // edit current user name
                Console.Write($"New name for {currentUser.Name}: ");
                var newName = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(newName))
                {
                    currentUser.Name = newName;
                    userService.UpdateUser(currentUser);
                    Console.WriteLine("User name updated!");
                }
                else
                {
                    Console.WriteLine("Name cannot be blank. No changes made.");
                }
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
            else if (choice == "3")
            {
                // delete users
                var users = userService.GetUsers();

                for (int i = 0; i < users.Count; i++)
                    Console.WriteLine($"{i + 1}. {users[i].Name}");

                Console.Write("Select user number to delete: ");
                int idx = int.Parse(Console.ReadLine()) - 1;
                var target = users[idx];

                // Prevent the currently logged-in user from deleting themselves
                if (target.Id == currentUser.Id)
                {
                    Console.WriteLine("You cannot delete the currently active user. Switch users first.");
                    Console.WriteLine("Press any key to continue.");
                    Console.ReadKey();
                    return;
                }

                Console.Write($"Are you sure you want to delete user '{target.Name}'? (y/n): ");
                if (Console.ReadLine()?.ToLower() == "y")
                {
                    userService.DeleteUser(target.Id);
                    Console.WriteLine("User deleted.");
                }
                else
                {
                    Console.WriteLine("Cancelled.");
                }
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Displays a numbered list of the current user's pets and returns the selected one.
        /// Returns null if the user has no pets registered.
        /// </summary>        
        private Pet SelectPet()
        {
            var pets = petService.GetPetsByUser(currentUser.Id);

            if (pets.Count == 0)
            {
                Console.WriteLine("No pets found. Please add a pet first.");
                Console.WriteLine("Press any key to continue.");
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

        /// <summary>
        /// Collects all appointments belonging to the current user's pets.
        /// </summary>
        private List<Appointment> GetCurrentUserAppointments(out Dictionary<string, Pet> petLookup)
        {
            var pets = petService.GetPetsByUser(currentUser.Id);
            petLookup = pets.ToDictionary(p => p.Id);
            var all = new List<Appointment>();

            foreach (var pet in pets)
                all.AddRange(appointmentService.GetAppointments(pet.Id));

            return all;
        }

        /// <summary>
        /// Collects all vaccinations belonging to the current user's pets.
        /// </summary>
        private List<Vaccination> GetCurrentUserVaccinations(out Dictionary<string, Pet> petLookup)
        {
            var pets = petService.GetPetsByUser(currentUser.Id);
            petLookup = pets.ToDictionary(p => p.Id);
            var all = new List<Vaccination>();

            foreach (var pet in pets)
                all.AddRange(vaccinationService.GetVaccinations(pet.Id));

            return all;
        }

        /// <summary>
        /// Collects all medications belonging to the current user's pets.
        /// </summary>
        private List<Medication> GetCurrentUserMedications(out Dictionary<string, Pet> petLookup)
        {
            var pets = petService.GetPetsByUser(currentUser.Id);
            petLookup = pets.ToDictionary(p => p.Id);
            var all = new List<Medication>();
            foreach (var pet in pets)
                all.AddRange(medicationService.GetMedications(pet.Id));
            return all;
        }

        /// <summary>
        /// displays a numbered list of appointments to the console
        /// </summary>
        private void PrintAppointmentList(List<Appointment> appointments, Dictionary<string, Pet> petLookup)
        {
            for (int i = 0; i < appointments.Count; i++)
            {
                var a = appointments[i];
                var petName = petLookup.ContainsKey(a.PetId) ? petLookup[a.PetId].Name : a.PetId;
                Console.WriteLine($"{i + 1}. {petName} | {a.AppointmentType} | {a.Date.ToShortDateString()} | {a.Location}");
            }
        }

        /// <summary>
        /// displays a numbered list of supplies to the console
        /// </summary>
        private void PrintSupplyList(List<Supply> supplies)
        {
            for (int i = 0; i < supplies.Count; i++)
                Console.WriteLine($"{i + 1}. {supplies[i].Name} | {supplyService.GetDaysRemaining(supplies[i])} days remaining");
        }

        /// <summary>
        /// displays a numbered list of vaccinations to the console
        /// </summary>
        private void PrintVaccinationList(List<Vaccination> vaccinations, Dictionary<string, Pet> petLookup)
        {
            for (int i = 0; i < vaccinations.Count; i++)
            {
                var v = vaccinations[i];
                var petName = petLookup.ContainsKey(v.PetId) ? petLookup[v.PetId].Name : v.PetId;
                Console.WriteLine($"{i + 1}. {petName} | {v.VaccineName} | Due {v.NextDueDate.ToShortDateString()}");
            }
        }

        /// <summary>
        /// displays a numbered list of medications to the console
        /// </summary>
        private void PrintMedicationList(List<Medication> medications, Dictionary<string, Pet> petLookup)
        {
            for (int i = 0; i < medications.Count; i++)
            {
                var m = medications[i];
                var petName = petLookup.ContainsKey(m.PetId) ? petLookup[m.PetId].Name : m.PetId;
                string endDisplay = m.EndDate.HasValue
                    ? m.EndDate.Value.ToShortDateString()
                    : "Ongoing";
                Console.WriteLine($"{i + 1}. {petName} | {m.Name} | {m.Dosage} | {m.Frequency} | Until {endDisplay}");
            }
        }

        /// <summary>
        /// creates a copy of the original appointment that will be edited by the user
        /// </summary>
        private static Appointment CopyAppointment(Appointment a)
        {
            return new Appointment
            {
                PetId           = a.PetId,
                AppointmentType = a.AppointmentType,
                Date            = a.Date,
                Location        = a.Location
            };
            
        }

        /// <summary>
        /// creates a copy of the original vaccination that will be edited by the user
        /// </summary>
        private static Vaccination CopyVaccination(Vaccination v)
        {
            return new Vaccination
            {
                PetId       = v.PetId,
                VaccineName = v.VaccineName,
                DateGiven   = v.DateGiven,
                NextDueDate = v.NextDueDate
            };
           
        }
    
    }
}