using System;
using System.Collections.Generic;
using System.Linq;
using WorkerSample.Application;
using WorkerSample.Domain;

namespace WorkerSample.Repositories
{
    public class ProductSubscriptionsRepository : IProductSubscriptionsRepository
    {
        private readonly AppDbContext _appDbContext;

        public ProductSubscriptionsRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IEnumerable<ProductSubscription> GetExpiredSubscriptions(DateTime now)
        {
            return _appDbContext.ProductSubscriptions
                .Where(x => x.ExpirationDateTime < now)
                .ToList();
        }
    }
}
