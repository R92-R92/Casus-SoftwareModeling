using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioManager
{
    public class Contact
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";

        public string Phone { get; set; }
        public string Email { get; set; }
        public string SocialMedia { get; set; }
        public Address Address { get; set; }
        public List<Concept> Concepts { get; set; } = new List<Concept>();
        new DAL dal = new DAL();


        public Contact(int id, string firstName, string lastName, string phone, string email, string socialMedia, string picture, Address address)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            Email = email;
            SocialMedia = socialMedia;
            Address = address;
        }

        public void Create()
        {
            dal.AddContact(this);
        }
    }
}
