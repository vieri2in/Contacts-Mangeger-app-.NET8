using Entities;
using System.Linq.Expressions;

namespace ContactsMangeger.Core.Domain.RepositoryContracts
{
    public interface IPersonsRepository
    {
        Task<Person> AddPerson(Person person);
        Task<List<Person>> GetAllPersons();
        Task<Person?> GetPersonByPersonId(Guid personId);
        Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predecate);
        Task<bool> DeletePersonByPersonId(Guid personId);
        Task<Person> UpdatePerson(Person person);
    }
}
