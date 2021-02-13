using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Agenda
{
    class Program
    {
        public static string PathToFile = "Agenda.txt";
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
                        SearchUser();
                        break;
                    case '3':
                        EditUser();
                        break;
                    case '4':
                        DelUser();
                        break;
                    case '5':
                        ShowUsers();
                        break;
                    case '6':
                        SortAll();
                        break;
                    default:
                        Console.WriteLine("Opció no valida, torna a provar: ");
                        Thread.Sleep(1000);
                        break;
                }
                choice = Interface();
            }
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
        static void NewUser()
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
            Console.WriteLine(bday);
            Console.Write("\tEdat: " + Age(Convert.ToDateTime(bday)));
            Console.WriteLine(email);
            Thread.Sleep(3000);
            Console.Clear();

            //append data to file
            using (StreamWriter agenda = new StreamWriter(PathToFile, true))
            {
                agenda.Write(name + ";");
                agenda.Write(surname + ";");
                agenda.Write(dni + ";");
                agenda.Write(phone + ";");
                agenda.Write(bday + ";");
                agenda.WriteLine(email);
            }
        }
        static string SearchUser()
        {
            //vars
            int selectedUser;
            string line;
            string tempPath = Path.GetTempPath();
            Console.Write("Nom de l'usuari a cercar: ");
            string text = Console.ReadLine();
            Regex filter = new Regex("^" + text + ".*", RegexOptions.IgnoreCase);
            using (StreamReader agenda = new StreamReader(PathToFile))
            using (StreamWriter tempFile = new StreamWriter(tempPath))
            {
                //check every line, count and print matches
                for (int i = 1; (line = agenda.ReadLine()) != null;)
                {
                    if (filter.IsMatch(line))
                    {
                        tempFile.WriteLine(line);
                        Console.WriteLine("{0}) {1}", i, line.Replace(";", "\t"));
                        i++;
                    }
                }
            }
            Console.WriteLine("Selecciona de la llista: ");
            selectedUser = Console.ReadKey().KeyChar - 48;
            return File.ReadLines(tempPath).ElementAt(selectedUser - 1);            
        }
        static void EditUser()
        {

        }
        static void DelUser()
        {
            string tempFile = Path.GetTempFileName();

            using (var sr = new StreamReader("file.txt"))
            using (var sw = new StreamWriter(tempFile))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line != "removeme")
                        sw.WriteLine(line);
                }
            }

            File.Delete("file.txt");
            File.Move(tempFile, "file.txt");
        }
        static void ShowUsers()
        {

        }
        static void SortAll()
        {

        }

        //data check methods
        static string NameCheckValid(String name)
        {
            Regex nameFilter = new Regex(@"^[a-zA-Z\s,]*$");//filter allows only spaces & lower/upper case
            while (true)
            {
                if (nameFilter.IsMatch(name))
                {
                    name = name[0].ToString().ToUpper() + name.Substring(1).ToLower();//first up, rest low
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
