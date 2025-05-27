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
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string SocialMedia { get; set; }
        public string Picture { get; set; }
        public bool Payment { get; set; }
        public int Role { get; set; }
        public Address Address { get; set; }
        public List<Concept> Concepts { get; set; } = new List<Concept>();

        public Contact(int id, string name, string phone, string email, string socialMedia, string picture, bool payment, int role, Address address)
        {
            Id = id;
            Name = name;
            Phone = phone;
            Email = email;
            SocialMedia = socialMedia;
            Picture = picture;
            Payment = payment;
            Role = role;
            Address = address;
        }
    }
}
