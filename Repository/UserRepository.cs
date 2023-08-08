using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Tourism.Model;

namespace Tourism.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ADbContext _context;

        public UserRepository(ADbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserEmail == email);
        }

        public async Task RegisterUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task RegisterTravelAgencyAsync(Travelagencies travelAgency)
        {
            _context.Travelagencies.Add(travelAgency);
            await _context.SaveChangesAsync();
        }


        public async Task<User> GetUserByIdAsync(int userId)
        {
            try
            {
                // Assuming there's an entity DbSet<User> in your DbContext
                var user = await _context.Users.FindAsync(userId);
                return user;
            }
            catch (Exception ex)
            {
                // Handle exceptions and logging here
                throw new Exception("Error retrieving user by ID.", ex);
            }
        }
    }
}

