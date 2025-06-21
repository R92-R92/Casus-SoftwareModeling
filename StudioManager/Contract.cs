using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;

namespace StudioManager
{
    public class Contract
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public Contact? Signee { get; set; }
        public bool IsSigned { get; set; }
        public DateTime? SignedOn { get; set; }
        public Shoot? Shoot { get; set; }
        new DAL dal = new DAL();


        public Contract(int id, string body, Contact? signee, bool isSigned, DateTime? signedOn, Shoot? shoot)
        {
            Id = id;
            Body = body;
            Signee = signee;
            IsSigned = isSigned;
            SignedOn = signedOn;
            Shoot = shoot;
        }

        public void Create()
        {
            dal.AddContract(this);
        }

        public void Update()
        {
            dal.UpdateContract(this);
        }

        public void Delete()
        {
            dal.DeleteContract(this.Id);
        }
    }
}
