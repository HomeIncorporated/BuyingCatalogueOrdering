using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Application.Persistence
{
    public interface IServiceRecipientRepository
    {
        Task<IEnumerable<ServiceRecipient>> ListServiceRecipientsByOrderIdAsync(int orderId);
        
        Task<int> GetCountByOrderIdAsync(int orderId);
        
        Task UpdateAsync(int orderId, IEnumerable<ServiceRecipient> recipientsUpdates);
    }
}
