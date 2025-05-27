using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;

namespace StudioManager
{
    class Contract
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public Contact? Signee { get; set; }
        public bool IsSigned { get; set; }
        public DateTime? SignedOn { get; set; }
        public User Author { get; set; }
        public Shoot? Shoot { get; set; }

        public Contract(int id, string body, Contact? signee, DateTime? signedOn, User author, Shoot? shoot)
        {
            Id = id;
            Body = body;
            Signee = signee;
            IsSigned = false;
            SignedOn = signedOn;
            Author = author;
            Shoot = shoot;
        }
    }
}
