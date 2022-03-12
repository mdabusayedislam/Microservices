using System;

namespace Play.Catalog.Contracts
{
        public record CatalogItemCreated(Guid Id,String Name,String Description);
    public record CatalogItemUpdated(Guid Id,String Name,String Description);   
    public record CatalogItemDeleted(Guid Id);   
}
