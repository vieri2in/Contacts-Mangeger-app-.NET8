using ContactsMangeger.Core.Domain.RepositoryContracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<PersonsRepository> _logger;
        public PersonsRepository(ApplicationDbContext db, ILogger<PersonsRepository> logger)
        {
            _db = db;
            _logger = logger;
        }
        public async Task<Person> AddPerson(Person person)
        {
            _db.Add(person);
            await _db.SaveChangesAsync();
            return person;
        }

        public async Task<bool> DeletePersonByPersonId(Guid personId)
        {
            _db.Persons.RemoveRange(_db.Persons.Where(temp => temp.PersonId == personId));
            int rowsDeleted = await _db.SaveChangesAsync();
            return rowsDeleted > 0;
        }

        public async Task<List<Person>> GetAllPersons()
        {
            _logger.LogInformation("GetAllPersons method of PersonsRepository");
            return await _db.Persons.Include("Country").ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predecate)
        {
            _logger.LogInformation("GetFilteredPersons method of PersonsRepository");
            return await _db.Persons.Include("Country").Where(predecate).ToListAsync();
        }

        public async Task<Person?> GetPersonByPersonId(Guid personId)
        {
            return await _db.Persons.Include("Country").FirstOrDefaultAsync(temp => temp.PersonId == personId);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            Person? matchingPerson = await _db.Persons.Include("Country").FirstOrDefaultAsync(temp => temp.PersonId == person.PersonId);
            if (matchingPerson == null) { return person; }
            matchingPerson.PersonName = person.PersonName;
            matchingPerson.Email = person.Email;
            matchingPerson.DateOfBirth = person.DateOfBirth;
            matchingPerson.Gender = person.Gender;
            matchingPerson.CountryId = person.CountryId;
            matchingPerson.Address = person.Address;
            matchingPerson.ReceiveNewLetters = person.ReceiveNewLetters;
            matchingPerson.PersonName = person.PersonName;
            int rowsUpdated = await _db.SaveChangesAsync();
            return matchingPerson;
        }
    }
}
