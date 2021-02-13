using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Globalization;

namespace Agenda
{
    class Program
    {
        public static string PathToAgenda = "Agenda.txt";
        public static string PathToTemp = "tempUsers.txt";
        static void Main(string[] args)
        {
            char choice = Interface();
            while (!(choice == 'q' || choice == 'Q')) 
            {
                Console.Clear();
                switch (choice)
                {
                    case '1':
                        NewUser();
                        break;
                    case '2':
                        Console.Write("Nom de l'usuari a cercar: ");
                        Console.WriteLine(SearchUser(Console.ReadLine()));
                        Thread.Sleep(4000);
                        break;
                    case '3':
                        Console.Write("Nom de l'usuari a editar: ");
                        EditUser(Console.ReadLine());
                        break;
                    case '4':
                        Console.Write("Nom de l'usuari a eliminar: ");
                        DelUser(SearchUser(Console.ReadLine()));
                        break;
                    case '5':
                        ShowUsers();
                        break;
                    case '6':
                        SortAll(PathToAgenda);
                        break;
                    default:
                        Console.WriteLine("Opció no valida, torna-ho a provar: ");
                        Thread.Sleep(1000);
                        break;
                }
                choice = Interface();
            }
            File.WriteAllText(PathToTemp, String.Empty); //clean temp file
        }
        static char Interface() //show options, return selected
        {
            Console.Clear();
            Console.WriteLine("================================================");
            Console.WriteLine("|                   Menu Agenda                |");
            Console.WriteLine("================================================");
            Console.WriteLine("");
            Console.WriteLine("Selecciona una opció:");
            Console.WriteLine("");
            Console.WriteLine("1) Afegir usuari\t4) Eliminar usuari");
            Console.WriteLine("2) Cercar usuari\t5) Mostrar usuaris");
            Console.WriteLine("3) Editar usuari\t6) Ordenar usuaris");
            Console.WriteLine("");
            Console.WriteLine("Presiona [Q] per sortir.");
            return Console.ReadKey().KeyChar;
        }
        static void NewUser() //adds a new user to agenda.txt
        {
            //vars
            String name, surname, dni, phone, bday, email;

            //console input
            Console.WriteLine("Nom: ");
            name = NameCheckValid(Console.ReadLine());
            Console.WriteLine("Cognom: ");
            surname = NameCheckValid(Console.ReadLine());
            Console.WriteLine("DNI: ");
            dni = DniCheckValid(Console.ReadLine());
            Console.WriteLine("Telefon: ");
            phone = PhoneCheckValid(Console.ReadLine());
            Console.WriteLine("Data de naixement: ");
            bday = BDateCheckValid(Console.ReadLine());
            Console.WriteLine("Correu electronic: ");
            email = EmailCheckValid(Console.ReadLine());

            //show data
            Console.Clear();
            Console.WriteLine(name);
            Console.WriteLine(surname);
            Console.WriteLine(dni);
            Console.WriteLine(phone);
            Console.Write(bday);
            Console.WriteLine("\tEdat: " + Age(Convert.ToDateTime(bday)));
            Console.WriteLine(email);
            Thread.Sleep(10000);
            Console.Clear();

            //append data to file
            using (var agenda = new StreamWriter(PathToAgenda, true))
            {
                agenda.Write(name + ";");
                agenda.Write(surname + ";");
                agenda.Write(dni + ";");
                agenda.Write(phone + ";");
                agenda.Write(bday + ";");
                agenda.WriteLine(email);
            }
        }
        static string SearchUser(string text) //returns all info about 1 user
        {
            //vars
            int selectedUser;
            string line;
            Regex filter = new Regex("^" + text + ".*", RegexOptions.IgnoreCase); //string will start with text
            File.WriteAllText(PathToTemp, String.Empty); //clean temp file
            Console.WriteLine("Selecciona de la llista: ");
            using (var agenda = new StreamReader(PathToAgenda))
            using (var tempFile = new StreamWriter(PathToTemp))
            {
                //check every line, count and print matches
                for (int i = 1; (line = agenda.ReadLine()) != null;)
                {
                    if (filter.IsMatch(line))
                    {
                        tempFile.WriteLine(line);
                        Console.WriteLine("{0}) {1}", i, line.Replace(";", "\t")); //show data
                        i++;
                    }
                }
            }
            selectedUser = Console.ReadKey().KeyChar - 48;
            Console.Clear();
            return File.ReadLines(PathToTemp).ElementAt(selectedUser - 1);            
        }
        static void EditUser(string targetUser)
        {
            int targetAtribute;
            string updatedUser;
            targetUser = SearchUser(targetUser); //search and save the target user
            string[] userArray = targetUser.Split(";".ToCharArray()); //divide categories
            Console.WriteLine(userArray);
            Console.WriteLine("Quin atribut vols editar de {0}?: ", userArray[0]);
            Console.WriteLine("");
            Console.WriteLine("1) Nom");
            Console.WriteLine("2) Cognoms");
            Console.WriteLine("3) DNI");
            Console.WriteLine("4) Telefon");
            Console.WriteLine("5) Data de naixement");
            Console.WriteLine("6) Correu electronic");
            targetAtribute = Console.ReadKey().KeyChar - 49;//(char)'1'- 49 = (int)0
            Console.Clear();
            Console.Write("Nou camp: ");
            userArray[targetAtribute] = Console.ReadLine();
            updatedUser = string.Join(";", userArray);
            DelUser(targetUser);
            using (var agenda = new StreamWriter(PathToAgenda, true))
                agenda.Write(updatedUser);
        }
        static void DelUser(string delUser) //creates a temp file without the selected user and replaces original
        {
            string tempPath = Path.GetTempFileName();

            using (var agenda = new StreamReader(PathToAgenda))
            using (var tempFile = new StreamWriter(tempPath))
            {
                string line;
                while ((line = agenda.ReadLine()) != null)
                {
                    if (line != delUser)
                        tempFile.WriteLine(line);
                }
            }
            //overwrite agenda
            File.Delete(PathToAgenda);
            File.Move(tempPath, PathToAgenda);
        }
        static void ShowUsers() //prints all names, surnames and phone nums
        {
            //copy agenda to tmp
            File.Copy(PathToAgenda, PathToTemp, true);
            //sort tmp before printing
            SortAll(PathToTemp);
            string line;
            Console.WriteLine("Nom\t\t\tCognoms\t\t\tTelefon");
            Console.WriteLine("");
            using (var agenda = new StreamReader(PathToTemp))
            {
                while ((line = agenda.ReadLine()) != null)
                {
                    string[] userArray = line.Split(";".ToCharArray()); //divide categories
                    Console.WriteLine("{0}\t\t\t{1}\t\t\t{2}", userArray[0], userArray[1], userArray[3]);
                }
            }
            File.WriteAllText(PathToTemp, String.Empty); //clean tmp file
            Thread.Sleep(7000);
        }
        static void SortAll(string inFile) //alphabetically sorts a file
        {
            var contents = File.ReadAllLines(inFile);
            Array.Sort(contents);
            File.WriteAllLines(inFile, contents);
        }
        //data check methods
        static string NameCheckValid(String name)
        {
            Regex nameFilter = new Regex(@"^[a-zA-Z\s,]*$");//filter allows only spaces & lower/upper case
            while (true)
            {
                if (nameFilter.IsMatch(name))
                {
                    name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLower());
                    //name = name[0].ToString().ToUpper() + name.Substring(1).ToLower();//first up, rest low
                    return name;
                }
                else
                {
                    Console.WriteLine("Ha de contenir nomes caràcters alfabetics. Torna a provar: ");
                    name = Console.ReadLine();
                }
            }
        }
        static string DniCheckValid(String dni)
        {
            Regex dniFilter = new Regex(@"^[\d]+\w$");
            String lettersDni = "TRWAGMYFPDXBNJZSQVHLCKE";
            while (true)
            {
                if (dni.Length == 9 && dniFilter.IsMatch(dni))
                {
                    dni = dni.ToUpper();//so that always matches with lettersDni
                    int nie = Convert.ToInt32(dni.Substring(0, 8));
                    char letter = dni[dni.Length - 1];
                    if (letter == lettersDni[nie % 23])
                        return dni;
                    else
                    {
                        Console.WriteLine("Incorrecte, prova un altra vegada: ");
                        dni = Console.ReadLine();
                    }
                }
                else
                {
                    Console.WriteLine("Incorrecte, prova un altra vegada: ");
                    dni = Console.ReadLine();
                }
            }
        }
        static string PhoneCheckValid(String phone) //returns a valid phone number
        {
            Regex phoneFilter = new Regex(@"^[\d]+$");
            while (true) 
            {
                if (phone.Length == 9 && phoneFilter.IsMatch(phone)) 
                    return phone;
                else
                {
                    Console.WriteLine("Telefon no és valid, prova un altra vegada:");
                    phone = Console.ReadLine();
                }
            }
        }
        static string BDateCheckValid(String date) //returns a valid birth date
        {
            while (true)
            {
                if (DateTime.TryParse(date, out DateTime bday))
                    return bday.ToString().Substring(0,10); //convert to str and remove the time
                else
                {
                    Console.WriteLine("Data no és valida, prova un altra vegada (format dd MM aaaa): ");
                    date = Console.ReadLine();
                }
            }
        }
        static int Age(DateTime bday)
        {
            DateTime now = DateTime.Now;
            int age = now.Year - bday.Year;
            if (now < bday.AddYears(age)) age--;
            return age;
        }
        static string EmailCheckValid(String email)
        {
            Regex emailFilter = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            email.ToLower();
            while (true)
            {
                if (emailFilter.IsMatch(email))
                    return email;
                else
                {
                    Console.WriteLine("Correu electronic invalid, prova un altra vegada: ");
                    email = Console.ReadLine().ToLower();
                }
            }
        }
    }
}
