using System.Threading.Tasks;
using Template.Models.ServiceModels.Home;

namespace Template.Services.Contracts
{
    public interface IHomeService
    {
        Task<GetHomeResponse> GetHome();
    }
}
