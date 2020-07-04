namespace PetStore.Services
{
	public interface IUserService
	{
		bool Exists(int userId);
		void Register(string name, string email);
	}
}
