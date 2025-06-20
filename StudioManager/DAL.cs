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

        // READ
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

        // CREATE
        public int AddAddress(Address address)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = @"
        INSERT INTO Address (Street, HouseNumber, PostalCode, City, Country)
        VALUES (@Street, @HouseNumber, @PostalCode, @City, @Country);
        SELECT SCOPE_IDENTITY();";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Street", address.Street);
            cmd.Parameters.AddWithValue("@HouseNumber", address.HouseNumber ?? "");
            cmd.Parameters.AddWithValue("@PostalCode", address.PostalCode);
            cmd.Parameters.AddWithValue("@City", address.City);
            cmd.Parameters.AddWithValue("@Country", address.Country);

            int newId = Convert.ToInt32(cmd.ExecuteScalar());
            return newId;
        }


        // UPDATE
        public void UpdateAddress(Address address)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = @"
                UPDATE Address SET
                    Street = @Street,
                    HouseNumber = @HouseNumber,
                    PostalCode = @PostalCode,
                    City = @City,
                    Country = @Country
                WHERE Id = @Id";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Id", address.Id);
            cmd.Parameters.AddWithValue("@Street", address.Street);
            cmd.Parameters.AddWithValue("@HouseNumber", address.HouseNumber ?? "");
            cmd.Parameters.AddWithValue("@PostalCode", address.PostalCode);
            cmd.Parameters.AddWithValue("@City", address.City);
            cmd.Parameters.AddWithValue("@Country", address.Country);

            cmd.ExecuteNonQuery();
        }

        // DELETE
        public void DeleteAddress(int id)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = "DELETE FROM Address WHERE Id = @Id";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            cmd.ExecuteNonQuery();
        }

        // CONCEPT

        // READ
        public List<Concept> GetAllConcepts()
        {
            List<Concept> concepts = new();
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = "SELECT Id, Name, Description, Sketch, ShootId, AddressId FROM Concept";
            using SqlCommand cmd = new(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {

                int conceptId = reader.GetInt32(0);

                Concept concept = new(
                    id: conceptId,
                    name: reader["Name"]?.ToString(),
                    address: reader["AddressId"] != DBNull.Value ? GetAddressById((int)reader["AddressId"]) : null,
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

        // CREATE
        public void AddConcept(Concept concept)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            if (concept.Address != null && concept.Address.Id == 0)
            {
                int addressId = AddAddress(concept.Address);
                concept.Address.Id = addressId;
            }


            string query = @"
                INSERT INTO Concept (Name, Description, Sketch, ShootId, AddressId)
                VALUES (@Name, @Description, @Sketch, @ShootId, @AddressId);
                SELECT SCOPE_IDENTITY();";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Name", concept.Name ?? "");
            cmd.Parameters.AddWithValue("@Description", concept.Description ?? "");
            cmd.Parameters.AddWithValue("@Sketch", concept.Sketch ?? "");
            cmd.Parameters.AddWithValue("@ShootId", concept.Shoot?.Id ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@AddressId", concept.Address?.Id ?? (object)DBNull.Value);


            int newId = Convert.ToInt32(cmd.ExecuteScalar());
            concept.Id = newId;

            foreach (string pic in concept.Pictures)
                AddPictureToConcept(newId, pic);

            foreach (var prop in concept.Props)
                AddPropToConcept(newId, prop.Id);

            foreach (var model in concept.Models)
                AddContactToConcept(newId, model.Id);
        }

        // UPDATE
        public void UpdateConcept(Concept concept)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            if (concept.Address != null && concept.Address.Id == 0)
            {
                int addressId = AddAddress(concept.Address);
                concept.Address.Id = addressId;
            }

            string query = @"
                UPDATE Concept
                SET Name = @Name,
                    Description = @Description,
                    Sketch = @Sketch,
                    ShootId = @ShootId,
                    AddressId = @AddressId
                WHERE Id = @Id";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Name", concept.Name ?? "");
            cmd.Parameters.AddWithValue("@Id", concept.Id);
            cmd.Parameters.AddWithValue("@Description", concept.Description ?? "");
            cmd.Parameters.AddWithValue("@Sketch", concept.Sketch ?? "");
            cmd.Parameters.AddWithValue("@ShootId", concept.Shoot?.Id ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@AddressId", concept.Address?.Id ?? (object)DBNull.Value);

            cmd.ExecuteNonQuery();

            DeleteAllConceptRelations(concept.Id);

            foreach (string pic in concept.Pictures)
                AddPictureToConcept(concept.Id, pic);

            foreach (var prop in concept.Props)
                AddPropToConcept(concept.Id, prop.Id);

            foreach (var model in concept.Models)
                AddContactToConcept(concept.Id, model.Id);
        }

        // DELETE
        public void DeleteConcept(int conceptId)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            int? addressId = null;
            using (SqlCommand getAddressCmd = new("SELECT AddressId FROM Concept WHERE Id = @ConceptId", conn))
            {
                getAddressCmd.Parameters.AddWithValue("@ConceptId", conceptId);
                object result = getAddressCmd.ExecuteScalar();
                if (result != DBNull.Value && result != null)
                    addressId = Convert.ToInt32(result);
            }

            DeleteAllConceptRelations(conceptId);

            string deleteConcept = "DELETE FROM Concept WHERE Id = @ConceptId";
            using (SqlCommand deleteCmd = new(deleteConcept, conn))
            {
                deleteCmd.Parameters.AddWithValue("@ConceptId", conceptId);
                deleteCmd.ExecuteNonQuery();
            }

            if (addressId.HasValue)
            {
                string checkUsage = @"
            SELECT COUNT(*) FROM (
                SELECT AddressId FROM Contact WHERE AddressId = @AddressId
                UNION ALL
                SELECT AddressId FROM Shoot WHERE AddressId = @AddressId
                UNION ALL
                SELECT AddressId FROM Concept WHERE AddressId = @AddressId
            ) AS Uses";

                using SqlCommand checkCmd = new(checkUsage, conn);
                checkCmd.Parameters.AddWithValue("@AddressId", addressId.Value);
                int usageCount = (int)checkCmd.ExecuteScalar();

                if (usageCount == 0)
                {
                    DeleteAddress(addressId.Value);
                }
            }
        }


        // CONTACTS

        // READ
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
                    //payment: (bool)reader["Payment"],
                    //role: (int)reader["Role"],
                    address: addressId.HasValue ? GetAddressById(addressId.Value) : null!
                );

                contacts.Add(contact);
            }

            return contacts;
        }

        // CREATE
        public void AddContact(Contact contact)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = @"
                INSERT INTO Contact 
                (FirstName, LastName, Phone, Email, SocialMedia, Picture, Role, AddressId, Payment)
                VALUES 
                (@FirstName, @LastName, @Phone, @Email, @SocialMedia, @Picture, @Role, @AddressId, @Payment)";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@FirstName", contact.FirstName);
            cmd.Parameters.AddWithValue("@LastName", contact.LastName);
            cmd.Parameters.AddWithValue("@Phone", contact.Phone ?? "");
            cmd.Parameters.AddWithValue("@Email", contact.Email ?? "");
            cmd.Parameters.AddWithValue("@SocialMedia", contact.SocialMedia ?? "");
            //cmd.Parameters.AddWithValue("@Picture", contact.Picture ?? "");
            //cmd.Parameters.AddWithValue("@Role", contact.Role);
            cmd.Parameters.AddWithValue("@AddressId", contact.Address?.Id ?? (object)DBNull.Value);
            //cmd.Parameters.AddWithValue("@Payment", contact.Payment);

            cmd.ExecuteNonQuery();
        }

        // UPDATE
        public void UpdateContact(Contact contact)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = @"
                UPDATE Contact SET
                    FirstName = @FirstName,
                    LastName = @LastName,
                    Phone = @Phone,
                    Email = @Email,
                    SocialMedia = @SocialMedia,
                    Picture = @Picture,
                    Role = @Role,
                    AddressId = @AddressId,
                    Payment = @Payment
                WHERE Id = @Id";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Id", contact.Id);
            cmd.Parameters.AddWithValue("@FirstName", contact.FirstName);
            cmd.Parameters.AddWithValue("@LastName", contact.LastName);
            cmd.Parameters.AddWithValue("@Phone", contact.Phone ?? "");
            cmd.Parameters.AddWithValue("@Email", contact.Email ?? "");
            cmd.Parameters.AddWithValue("@SocialMedia", contact.SocialMedia ?? "");
            //cmd.Parameters.AddWithValue("@Picture", contact.Picture ?? "");
            //cmd.Parameters.AddWithValue("@Role", contact.Role);
            cmd.Parameters.AddWithValue("@AddressId", contact.Address?.Id ?? (object)DBNull.Value);
            //cmd.Parameters.AddWithValue("@Payment", contact.Payment);

            cmd.ExecuteNonQuery();
        }

        // DELETE
        public void DeleteContact(int contactId)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string deleteContact = "DELETE FROM Contact WHERE Id = @ContactId";
            using SqlCommand cmd2 = new(deleteContact, conn);
            cmd2.Parameters.AddWithValue("@ContactId", contactId);
            cmd2.ExecuteNonQuery();
        }

        // CONTRACT

        // READ
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
                    shoot: shootId.HasValue ? GetShootById(shootId.Value) : null,
                    payment: (bool)reader["Payment"]
                );

                contracts.Add(contract);
            }

            return contracts;
        }

        // CREATE
        public void AddContract(Contract contract)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = @"
                INSERT INTO Contract (Body, SigneeContactId, SignedOn, IsSigned, ShootId)
                VALUES (@Body, @SigneeContactId, @SignedOn, @IsSigned, @ShootId);
                SELECT SCOPE_IDENTITY();";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Body", contract.Body ?? "");
            cmd.Parameters.AddWithValue("@SigneeContactId", contract.Signee?.Id ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@SignedOn", contract.SignedOn ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IsSigned", contract.IsSigned);
            cmd.Parameters.AddWithValue("@ShootId", contract.Shoot?.Id ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Payment", contract.Payment);


            int newId = Convert.ToInt32(cmd.ExecuteScalar());
            contract.Id = newId;
        }

        // UPDATE
        public void UpdateContract(Contract contract)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = @"
                UPDATE Contract SET
                    Body = @Body,
                    SigneeContactId = @SigneeContactId,
                    SignedOn = @SignedOn,
                    IsSigned = @IsSigned,
                    ShootId = @ShootId
                    Payment = @Payment
                WHERE Id = @Id";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Id", contract.Id);
            cmd.Parameters.AddWithValue("@Body", contract.Body ?? "");
            cmd.Parameters.AddWithValue("@SigneeContactId", contract.Signee?.Id ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@SignedOn", contract.SignedOn ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IsSigned", contract.IsSigned);
            cmd.Parameters.AddWithValue("@ShootId", contract.Shoot?.Id ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Payment", contract.Payment);


            cmd.ExecuteNonQuery();
        }

        // DELETE
        public void DeleteContract(int contractId)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = "DELETE FROM Contract WHERE Id = @Id";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Id", contractId);
            cmd.ExecuteNonQuery();
        }

        // PROJECT

        // READ
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
                int projectId = reader.GetInt32(0);

                Project project = new(
                    id: projectId,
                    name: reader["Name"].ToString(),
                    deadline: reader.IsDBNull(2) ? DateTime.MinValue : reader.GetDateTime(2),
                    notes: reader["Notes"]?.ToString()
                );

                project.Concepts = GetConceptsByProjectId(projectId);
                projects.Add(project);
            }

            return projects;
        }

        // CREATE
        public void AddProject(Project project)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = @"
                INSERT INTO Project (Name, Deadline, Notes)
                VALUES (@Name, @Deadline, @Notes);
                SELECT SCOPE_IDENTITY();";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Name", project.Name);
            cmd.Parameters.AddWithValue("@Deadline", project.Deadline == DateTime.MinValue ? (object)DBNull.Value : project.Deadline);
            cmd.Parameters.AddWithValue("@Notes", project.Notes ?? "");

            int newId = Convert.ToInt32(cmd.ExecuteScalar());
            project.Id = newId;

            foreach (var concept in project.Concepts)
                AddConceptToProject(newId, concept.Id);
        }

        // UPDATE
        public void UpdateProject(Project project)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = @"
                UPDATE Project SET
                    Name = @Name,
                    Deadline = @Deadline,
                    Notes = @Notes
                WHERE Id = @Id";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Id", project.Id);
            cmd.Parameters.AddWithValue("@Name", project.Name);
            cmd.Parameters.AddWithValue("@Deadline", project.Deadline == DateTime.MinValue ? (object)DBNull.Value : project.Deadline);
            cmd.Parameters.AddWithValue("@Notes", project.Notes ?? "");

            cmd.ExecuteNonQuery();

            DeleteConceptProjectRelation(project.Id);
            foreach (var concept in project.Concepts)
                AddConceptToProject(project.Id, concept.Id);
        }

        // DELETE
        public void DeleteProject(int projectId)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string deleteProject = "DELETE FROM Project WHERE Id = @Id";
            using SqlCommand cmd = new(deleteProject, conn);
            cmd.Parameters.AddWithValue("@Id", projectId);
            cmd.ExecuteNonQuery();
        }

        // PROP

        // READ
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

        // CREATE
        public void AddProp(Prop prop)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = @"
                INSERT INTO Prop (Name, Description, IsAvailable)
                VALUES (@Name, @Description, @IsAvailable);
                SELECT SCOPE_IDENTITY();";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Name", prop.Name);
            cmd.Parameters.AddWithValue("@Description", prop.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IsAvailable", prop.IsAvailable);

            int newId = Convert.ToInt32(cmd.ExecuteScalar());
            prop.Id = newId;
        }

        // UPDATE
        public void UpdateProp(Prop prop)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = @"
                UPDATE Prop SET
                    Name = @Name,
                    Description = @Description,
                    IsAvailable = @IsAvailable
                WHERE Id = @Id";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Id", prop.Id);
            cmd.Parameters.AddWithValue("@Name", prop.Name);
            cmd.Parameters.AddWithValue("@Description", prop.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IsAvailable", prop.IsAvailable);

            cmd.ExecuteNonQuery();
        }

        // DELETE
        public void DeleteProp(int propId)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string deleteProp = "DELETE FROM Prop WHERE Id = @Id";
            using SqlCommand cmd2 = new(deleteProp, conn);
            cmd2.Parameters.AddWithValue("@Id", propId);
            cmd2.ExecuteNonQuery();
        }

        // SHOOT

        // READ
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

        // CREATE
        public void AddShoot(Shoot shoot)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = @"
                INSERT INTO Shoot (Date, AddressId)
                VALUES (@Date, @AddressId);
                SELECT SCOPE_IDENTITY();";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Date", shoot.Date ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@AddressId", shoot.Location?.Id ?? (object)DBNull.Value);

            int newId = Convert.ToInt32(cmd.ExecuteScalar());
            shoot.Id = newId;
        }

        // UPDATE
        public void UpdateShoot(Shoot shoot)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = @"
                UPDATE Shoot SET
                    Date = @Date,
                    AddressId = @AddressId
                WHERE Id = @Id";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Id", shoot.Id);
            cmd.Parameters.AddWithValue("@Date", shoot.Date ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@AddressId", shoot.Location?.Id ?? (object)DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        // DELETE
        public void DeleteShoot(int shootId)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = "DELETE FROM Shoot WHERE Id = @Id";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Id", shootId);
            cmd.ExecuteNonQuery();
        }

        // HOMEADDRESS

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



        // DATABASE HELPERS
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
                    //payment: (bool)reader["Payment"],
                    //role: (int)reader["Role"],
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
                SELECT s.Id, s.Date, a.Id AS AddressId, a.Street, a.HouseNumber, a.PostalCode, a.City, a.Country
                FROM Shoot s
                LEFT JOIN Address a ON s.AddressId = a.Id
                INNER JOIN Concept c ON s.Id = c.ShootId
                WHERE c.Id = @ConceptId";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@ConceptId", conceptId);

            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                Address? address = null;

                // Check of het adres daadwerkelijk bestaat (a.Id is niet null)
                if (!reader.IsDBNull(reader.GetOrdinal("AddressId")))
                {
                    address = new Address(
                        id: reader.GetInt32(reader.GetOrdinal("AddressId")),
                        street: reader["Street"]?.ToString() ?? "",
                        houseNumber: reader["HouseNumber"]?.ToString() ?? "",
                        postalCode: reader["PostalCode"]?.ToString() ?? "",
                        city: reader["City"]?.ToString() ?? "",
                        country: reader["Country"]?.ToString() ?? ""
                    );
                }

                return new Shoot(
                    id: reader.GetInt32(reader.GetOrdinal("Id")),
                    date: reader.IsDBNull(reader.GetOrdinal("Date")) ? null : reader.GetDateTime(reader.GetOrdinal("Date")),
                    location: address
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
                    //payment: (bool)reader["Payment"],
                    //role: (int)reader["Role"],
                    address: addressId.HasValue ? GetAddressById(addressId.Value) : null!
                ));
            }

            return contacts;
        }

        private List<Concept> GetConceptsByProjectId(int projectId)
        {
            List<Concept> concepts = new();
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = @"
                SELECT c.Id,c.Name, c.Description, c.Sketch, c.ShootId, c.AddressId
                FROM Concept c
                INNER JOIN ConceptProject cp ON c.Id = cp.ConceptId
                WHERE cp.ProjectId = @ProjectId";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@ProjectId", projectId);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int conceptId = reader.GetInt32(0);

                Concept concept = new(
                    id: conceptId,
                    name: reader["Name"]?.ToString(),
                    address: reader["AddressId"] != DBNull.Value ? GetAddressById((int)reader["AddressId"]) : null,
                    description: reader["Description"]?.ToString(),
                    sketch: reader["Sketch"]?.ToString(),
                    props: GetPropsByConceptId(conceptId),
                    shoot: GetShootByConceptId(conceptId)
                );

                foreach (string pic in GetPicturesByConceptId(conceptId))
                    concept.AddPictures(pic);

                concept.Models = GetContactsByConceptId(conceptId);
                concepts.Add(concept);
            }

            return concepts;
        }

        public bool ConceptNameExists(string name)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = "SELECT COUNT(*) FROM Concept WHERE Name = @Name";
            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Name", name);

            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }

        public bool PropNameExists(string name, int? excludeId = null)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = "SELECT COUNT(*) FROM Prop WHERE Name = @Name";
            if (excludeId.HasValue)
                query += " AND Id != @Id";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Name", name);
            if (excludeId.HasValue)
                cmd.Parameters.AddWithValue("@Id", excludeId.Value);

            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }


        private void AddPictureToConcept(int conceptId, string picture)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = "INSERT INTO ConceptPicture (ConceptId, Picture) VALUES (@ConceptId, @Picture)";
            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@ConceptId", conceptId);
            cmd.Parameters.AddWithValue("@Picture", picture);
            cmd.ExecuteNonQuery();
        }

        private void AddPropToConcept(int conceptId, int propId)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = "INSERT INTO ConceptProp (ConceptId, PropId) VALUES (@ConceptId, @PropId)";
            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@ConceptId", conceptId);
            cmd.Parameters.AddWithValue("@PropId", propId);
            cmd.ExecuteNonQuery();
        }

        private void AddContactToConcept(int conceptId, int contactId)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = "INSERT INTO ConceptContact (ConceptId, ContactId) VALUES (@ConceptId, @ContactId)";
            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@ConceptId", conceptId);
            cmd.Parameters.AddWithValue("@ContactId", contactId);
            cmd.ExecuteNonQuery();
        }

        private void AddConceptToProject(int projectId, int conceptId)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = "INSERT INTO ConceptProject (ProjectId, ConceptId) VALUES (@ProjectId, @ConceptId)";
            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@ProjectId", projectId);
            cmd.Parameters.AddWithValue("@ConceptId", conceptId);
            cmd.ExecuteNonQuery();
        }



        private void DeleteAllConceptRelations(int conceptId)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string deletePictures = "DELETE FROM ConceptPicture WHERE ConceptId = @ConceptId";
            string deleteProps = "DELETE FROM ConceptProp WHERE ConceptId = @ConceptId";
            string deleteContacts = "DELETE FROM ConceptContact WHERE ConceptId = @ConceptId";

            foreach (var query in new[] { deletePictures, deleteProps, deleteContacts })
            {
                using SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@ConceptId", conceptId);
                cmd.ExecuteNonQuery();
            }
        }

        private void DeleteConceptProjectRelation(int projectId)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = "DELETE FROM ConceptProject WHERE ProjectId = @ProjectId";
            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@ProjectId", projectId);
            cmd.ExecuteNonQuery();
        }
    }
}
