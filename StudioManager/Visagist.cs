﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioManager
{
    class Visagist : Contact
    {
        public int Wage { get; set; }

        public Visagist(string firstname, string lastname, string phone, string email, string socialmedia, string picture, Address address)
            : base(0, firstname, lastname, phone, email, socialmedia, picture, "Visagist", false, address)
        {
        }

        public void SetWage(int wage)
        {
            Wage = wage;
        }

    }
}
