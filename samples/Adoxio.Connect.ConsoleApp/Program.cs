using Adoxio.Dynamics.Connect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adoxio.Connect.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new CrmContext())
            {
                var contacts = context.ServiceContext.CreateQuery("contact").ToList();

                foreach (var contact in contacts)
                {
                    Console.WriteLine(contact.GetAttributeValue<string>("fullname"));
                }
            }

            Console.ReadKey();
        }
    }
}
