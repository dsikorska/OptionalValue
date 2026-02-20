using ExampleApi.Models;

namespace ExampleApi.Services;

public interface IUserService
{
	IEnumerable<User> GetAll();
	User? GetById(Guid id);
	User Create(User user);
	User? Update(Guid id, PatchUserRequest request);
	bool Delete(Guid id);
}
