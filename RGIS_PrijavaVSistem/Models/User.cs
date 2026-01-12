using System;

namespace RGIS_PrijavaVSistem.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = "";
        public string Password { get; set; } = ""; // za nalogo OK (NE za produkcijo)
    }
}
