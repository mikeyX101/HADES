using HADES.Data;
using HADES.Models;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Services
{
	public class UserConfigService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public UserConfigViewModel UserConfig(UserConfig userConfig)
        {
            var emails = db.Email.Where(m => m.UserConfigId == userConfig.Id).ToList();

            var viewModel = new UserConfigViewModel
            {
                UserConfig = userConfig,
                Emails = emails
            };
            return viewModel;
        }
        public async Task UpdateUserConfig(UserConfigViewModel viewModel)
        {
            if (viewModel.Emails != null)
            {
                foreach (var email in viewModel.Emails)
                {
                    db.Update(email);
                }
            }

            if (viewModel.UserConfig != null)
                db.Update(viewModel.UserConfig);

            await db.SaveChangesAsync();
        }

        public bool UserConfigExists(UserConfig userConfig)
        {
            return db.UserConfig.Any(e => e.Id == userConfig.Id);
        }

        public async Task AddEmail(Email email)
        {
            db.Add(email);
            await db.SaveChangesAsync();
        }

        public async Task DeleteEmail(int? id)
        {
            var email = await db.Email.FindAsync(id);
            if (email != null)
			{
                // We should check if the current user has the email we want to delete first
                db.Email.Remove(email);
                await db.SaveChangesAsync();
            }
        }

        public async Task<int> RedirectId(int? id)
        {
            var email = await db.Email.FindAsync(id);
            return email.UserConfigId;
        }
    }
}
