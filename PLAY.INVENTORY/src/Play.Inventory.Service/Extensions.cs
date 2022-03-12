using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service{
    public static class Extenstions{
        public static InventoryItemDto AsDto(this InventoryItem item,string name,string description){
            return new InventoryItemDto(item.CatalogItemId,name,description,item.Quantity,item.AcquiredDate);
        }
    }
}