﻿using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Application.Persistence
{
    public interface IServiceRecipientRepository
    {
        Task<IEnumerable<ServiceRecipient>> ListServiceRecipientsByOrderId(string orderId);
        Task UpdateServiceRecipientsAsync(Order order, IEnumerable<ServiceRecipient> recipientsUpdates);
    }
}

