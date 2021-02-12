using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace Agenda
{
    class Program
    {
        static void Main(string[] args)
        {
            NewUser();
        }
        static void NewUser()
        {
            //vars
            String fileName = "Agenda.txt";
            String name, surname, dni, phone, bday, email;
            //console input
            Console.WriteLine("Nom: ");
            name = Name(Console.ReadLine());
            Console.WriteLine("Cognom: ");
            surname = Name(Console.ReadLine());
            Console.WriteLine("DNI: ");
            dni = Dni(Console.ReadLine());
            Console.WriteLine("Telefon: ");
            phone = PhoneCheckValid(Console.ReadLine());
            Console.WriteLine("Data de naixement: ");
            bday = BDate(Console.ReadLine());
            Console.WriteLine("Correu electronic: ");
            email = Email(Console.ReadLine());
            //add data to file
            using (StreamWriter agenda = new StreamWriter(fileName, true))
            {
                //agenda.Write(Name(Console.ReadLine()) + ";"); this could be repeated if I didn't need to show the answers on screen
                agenda.Write(name + ";");
                agenda.Write(surname + ";");
                agenda.Write(dni + ";");
                agenda.Write(phone + ";");
                agenda.Write(bday + ";");
                agenda.WriteLine(email);
            }
        }
        static void EditUser()
        {

        }
        static string Name(String name)
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
        static string Dni(String dni)
        {
            string pattern = @"^[\d]+\w$";
            Regex dniFilter = new Regex(pattern);
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
            string pattern = @"^[\d]+$";
            Regex phoneFilter = new Regex(pattern);
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
        static string BDate(String date) //returns a valid birth date
        {
            while (true)
            {
                if (DateTime.TryParse(date, out DateTime bday))
                {
                    //TimeSpan anys = DateTime.Today - auxDate;
                    //Console.WriteLine(new DateTime(anys.Ticks).Year - 1);
                    DateTime now = DateTime.Now;
                    int age = now.Year - bday.Year;
                    if (now < bday.AddYears(age)) age--;
                    Console.WriteLine(age);
                    return bday.ToString().Substring(0,10); //convert to str and remove the time
                }
                else
                {
                    Console.WriteLine("Data no és valida, prova un altra vegada (format dd mm aaaa): ");
                    date = Console.ReadLine();
                }
            }
        }
        static string Email(String email)
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
