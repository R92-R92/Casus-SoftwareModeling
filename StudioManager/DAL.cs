using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;

namespace StudioManager
{
    public class DAL
    {
        private readonly string connectionString = "Server=localhost;Database=MDMA;Trusted_Connection=True;TrustServerCertificate=True;";

        public DAL() { }

        // ADDRESS

        public List<Address> GetAllAddresses()
        {
            List<Address> addresses = new();
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = "SELECT Id, Street, HouseNumber, PostalCode, City, Country FROM Address";
            using SqlCommand cmd = new(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Address address = new(
                    id: reader.GetInt32(0),
                    street: reader["Street"].ToString(),
                    houseNumber: reader["HouseNumber"]?.ToString(),
                    postalCode: reader["PostalCode"].ToString(),
                    city: reader["City"].ToString(),
                    country: reader["Country"].ToString()
                );

                addresses.Add(address);
            }

            return addresses;
        }

        // CONCEPT

        public List<Concept> GetAllConcepts()
        {
            List<Concept> concepts = new();
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = "SELECT Id, Description, Sketch, ShootId FROM Concept";
            using SqlCommand cmd = new(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {

                int conceptId = reader.GetInt32(0);

                Concept concept = new(
                    id: conceptId,
                    description: reader["Description"]?.ToString(),
                    sketch: reader["Sketch"]?.ToString(),
                    props: GetPropsByConceptId(conceptId),
                    shoot: GetShootByConceptId(conceptId)
                );

                foreach (string picture in GetPicturesByConceptId(conceptId))
                {
                    concept.AddPictures(picture);
                }

                concept.Models = GetContactsByConceptId(conceptId);
                concepts.Add(concept);
            }

            return concepts;
        }

        // CONTACTS

        public List<Contact> GetAllContacts()
        {
            List<Contact> contacts = new();
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = "SELECT Id, FirstName, LastName, Phone, Email, SocialMedia, Picture, Role, AddressId, Payment FROM Contact";
            using SqlCommand cmd = new(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int? addressId = reader.IsDBNull(8) ? null : reader.GetInt32(8); 

                Contact contact = new(
                    id: reader.GetInt32(0),
                    firstName: reader["FirstName"].ToString(),
                    lastName: reader["LastName"].ToString(),
                    phone: reader["Phone"]?.ToString(),
                    email: reader["Email"]?.ToString(),
                    socialMedia: reader["SocialMedia"]?.ToString(),
                    picture: reader["Picture"]?.ToString(),
                    payment: (bool)reader["Payment"],              
                    role: (int)reader["Role"],                      
                    address: addressId.HasValue ? GetAddressById(addressId.Value) : null!
                );

                contacts.Add(contact);
            }

            return contacts;
        }


        // CONTRACT

        public List<Contract> GetAllContracts()
        {
            List<Contract> contracts = new();
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = "SELECT Id, Body, SigneeContactId, SignedOn, IsSigned, ShootId FROM Contract";
            using SqlCommand cmd = new(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int? signeeId = reader.IsDBNull(2) ? null : reader.GetInt32(2);
                int? shootId = reader.IsDBNull(5) ? null : reader.GetInt32(5);

                Contract contract = new(
                    id: reader.GetInt32(0),
                    body: reader["Body"].ToString(),
                    signee: signeeId.HasValue ? GetContactById(signeeId.Value) : null,
                    isSigned: (bool)reader["IsSigned"],
                    signedOn: reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                    shoot: shootId.HasValue ? GetShootById(shootId.Value) : null
                );

                contracts.Add(contract);
            }

            return contracts;
        }

        // PROJECT

        public List<Project> GetAllProjects()
        {
            List<Project> projects = new();
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = "SELECT Id, Name, Deadline, Notes FROM Project";
            using SqlCommand cmd = new(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Project project = new(
                    id: reader.GetInt32(0),
                    name: reader["Name"].ToString(),
                    deadline: reader.IsDBNull(2) ? DateTime.MinValue : reader.GetDateTime(2),
                    notes: reader["Notes"]?.ToString()
                );

                projects.Add(project);
            }

            return projects;
        }

        // PROP

        public List<Prop> GetAllProps()
        {
            List<Prop> props = new();
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = "SELECT Id, Name, Description, IsAvailable FROM Prop";
            using SqlCommand cmd = new(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Prop prop = new(
                    id: reader.GetInt32(0),
                    name: reader["Name"].ToString(),
                    description: reader["Description"]?.ToString(),
                    isAvailable: (bool)reader["IsAvailable"]
                );

                props.Add(prop);
            }

            return props;
        }

        // SHOOT

        public List<Shoot> GetAllShoots()
        {
            List<Shoot> shoots = new();
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = "SELECT Id, Date, AddressId FROM Shoot";
            using SqlCommand cmd = new(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int shootId = reader.GetInt32(0);

                Shoot shoot = new(
                    id: shootId,
                    date: reader.IsDBNull(1) ? DateTime.MinValue : reader.GetDateTime(1),
                    location: reader.IsDBNull(2) ? null! : GetAddressById(reader.GetInt32(2))
                );

                shoots.Add(shoot);
            }

            return shoots;
        }




        // READ HOME ADDRESS
        public Address? GetHomeAddress()
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = "SELECT Id, Street, HouseNumber, PostalCode, City, Country FROM Address WHERE IsHome = 1";
            using SqlCommand cmd = new(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Address(
                    id: reader.GetInt32(0),
                    street: reader["Street"].ToString(),
                    houseNumber: reader["HouseNumber"]?.ToString(),
                    postalCode: reader["PostalCode"].ToString(),
                    city: reader["City"].ToString(),
                    country: reader["Country"].ToString()
                );
            }

            return null;
        }

        // UPDATE HOMEADDRESS
        public void UpdateHomeAddress(Address updatedHome)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = @"
        UPDATE Address
        SET Street = @Street,
            HouseNumber = @HouseNumber,
            PostalCode = @PostalCode,
            City = @City,
            Country = @Country
        WHERE IsHome = 1";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Street", updatedHome.Street);
            cmd.Parameters.AddWithValue("@HouseNumber", updatedHome.HouseNumber ?? "");
            cmd.Parameters.AddWithValue("@PostalCode", updatedHome.PostalCode);
            cmd.Parameters.AddWithValue("@City", updatedHome.City);
            cmd.Parameters.AddWithValue("@Country", updatedHome.Country);

            cmd.ExecuteNonQuery();
        }









        // HELP METHODS (READ ONLY)
        public List<string> GetPicturesByConceptId(int conceptId)
        {
            List<string> pictures = new();
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = "SELECT Picture FROM ConceptPicture WHERE ConceptId = @ConceptId";
            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@ConceptId", conceptId);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                pictures.Add(reader["Picture"].ToString());
            }

            return pictures;
        }

        private Contact? GetContactById(int id)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = "SELECT Id, FirstName, LastName, Phone, Email, SocialMedia, Picture, Role, AddressId, Payment FROM Contact WHERE Id = @Id";
            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int? addressId = reader.IsDBNull(8) ? null : reader.GetInt32(8);

                return new Contact(
                    id: reader.GetInt32(0),
                    firstName: reader["FirstName"].ToString(),
                    lastName: reader["LastName"].ToString(),
                    phone: reader["Phone"]?.ToString(),
                    email: reader["Email"]?.ToString(),
                    socialMedia: reader["SocialMedia"]?.ToString(),
                    picture: reader["Picture"]?.ToString(),
                    payment: (bool)reader["Payment"],
                    role: (int)reader["Role"],
                    address: addressId.HasValue ? GetAddressById(addressId.Value) : null!
                );

            }

            return null;
        }

        private Shoot? GetShootById(int id)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = "SELECT Id, Date, AddressId FROM Shoot WHERE Id = @Id";
            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Shoot(
                    id: reader.GetInt32(0),
                    date: reader.IsDBNull(1) ? DateTime.MinValue : reader.GetDateTime(1),
                    location: reader.IsDBNull(2) ? null! : GetAddressById(reader.GetInt32(2))
                );
            }

            return null;
        }

        private Address? GetAddressById(int id)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = "SELECT Id, Street, HouseNumber, PostalCode, City, Country FROM Address WHERE Id = @Id";
            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Address(
                    id: reader.GetInt32(0),
                    street: reader["Street"].ToString(),
                    houseNumber: reader["HouseNumber"]?.ToString(),
                    postalCode: reader["PostalCode"].ToString(),
                    city: reader["City"].ToString(),
                    country: reader["Country"].ToString()
                );
            }

            return null;
        }


        private Shoot? GetShootByConceptId(int conceptId)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = @"
        SELECT s.Id, s.Date, s.AddressId
        FROM Shoot s
        INNER JOIN Concept c ON s.Id = c.ShootId
        WHERE c.Id = @ConceptId";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@ConceptId", conceptId);

            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Shoot(
                    id: reader.GetInt32(0),
                    date: reader.IsDBNull(1) ? DateTime.MinValue : reader.GetDateTime(1),
                    location: reader.IsDBNull(2) ? null! : GetAddressById(reader.GetInt32(2))
                );
            }

            return null;
        }

        private List<Prop> GetPropsByConceptId(int conceptId)
        {
            List<Prop> props = new();
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = @"
        SELECT p.Id, p.Name, p.Description, p.IsAvailable
        FROM Prop p
        INNER JOIN ConceptProp cp ON p.Id = cp.PropId
        WHERE cp.ConceptId = @ConceptId";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@ConceptId", conceptId);

            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Prop prop = new(
                    id: reader.GetInt32(0),
                    name: reader["Name"].ToString(),
                    description: reader["Description"]?.ToString(),
                    isAvailable: (bool)reader["IsAvailable"]
                );

                props.Add(prop);
            }

            return props;
        }

        private List<Contact> GetContactsByConceptId(int conceptId)
        {
            List<Contact> contacts = new();
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = @"
        SELECT c.Id, c.FirstName, c.LastName, c.Phone, c.Email, c.SocialMedia, c.Picture, c.Payment, c.Role, c.AddressId
        FROM Contact c
        INNER JOIN ConceptContact cc ON c.Id = cc.ContactId
        WHERE cc.ConceptId = @ConceptId";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@ConceptId", conceptId);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int? addressId = reader.IsDBNull(9) ? null : reader.GetInt32(9);

                contacts.Add(new Contact(
                    id: reader.GetInt32(0),
                    firstName: reader["FirstName"].ToString(),
                    lastName: reader["LastName"].ToString(),
                    phone: reader["Phone"]?.ToString(),
                    email: reader["Email"]?.ToString(),
                    socialMedia: reader["SocialMedia"]?.ToString(),
                    picture: reader["Picture"]?.ToString(),
                    payment: (bool)reader["Payment"],
                    role: (int)reader["Role"],
                    address: addressId.HasValue ? GetAddressById(addressId.Value) : null!
                ));
            }

            return contacts;
        }



        //GetProjectsByConceptId(int conceptId)




        // EXTRA JOIN METHODS
        //public List<Contact> GetContactsByConceptId(int conceptId) { }
        //public List<Prop> GetPropsByConceptId(int conceptId) { }
        //public List<Project> GetProjectsByConceptId(int conceptId) { }
        //public List<Concept> GetConceptsByProjectId(int projectId) { }
        //public List<Concept> GetConceptsByShootId(int shootId) { }
        //public List<Contract> GetContractsByShootId(int shootId) { }
        //public string GetPictureByConceptId(int conceptId) { }
    }
}
