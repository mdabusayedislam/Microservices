using System.Threading.Tasks;
using MassTransit;
using Play.Catalog.Contracts;
using Play.Common;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Consumers{
    public class CatalogItemCreatedCustomer : IConsumer<CatalogItemCreated>
    {
        private readonly IRepository<CatalogItem> repository;
        public CatalogItemCreatedCustomer(IRepository<CatalogItem> repository){
          this.repository=repository;
        }
        public async Task Consume(ConsumeContext<CatalogItemCreated> context)
        {
            var message=context.Message;
            var item=await repository.GetAsync(message.Id);
            if(item!=null){
                return;
            }
            item=new CatalogItem{
                Id=message.Id,
                Name=message.Name,
                Description=message.Description
            };
            await repository.CreateAsync(item);
        }
    }
}