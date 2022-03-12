using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service
{

    [ApiController]
    [Route("items")]
    public class ItemsController:ControllerBase{
        private readonly IRepository<InventoryItem> inventoryItemRepository;
         private readonly IRepository<CatalogItem> catalogItemRepository;
       
        public ItemsController(IRepository<InventoryItem> inventoryItemRepository,IRepository<CatalogItem> catalogItemRepository){
            this.inventoryItemRepository=inventoryItemRepository;
            this.catalogItemRepository=catalogItemRepository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId){
        if(userId==Guid.Empty){
            return BadRequest();
        }
        
        var inventoryItemEntitiess=await inventoryItemRepository.GetAllAsync(item=>item.UserId==userId);
        var itemIds=inventoryItemEntitiess.Select(item=>item.CatalogItemId);
         var catalogItemEntitiess=await catalogItemRepository.GetAllAsync(item=>itemIds.Contains(item.Id));
        var inventoryItemDtos=inventoryItemEntitiess.Select(inventoryItem=>{
            var catalogItem=catalogItemEntitiess.Single(catalogItem=>catalogItem.Id==inventoryItem.CatalogItemId);
            return inventoryItem.AsDto(catalogItem.Name,catalogItem.Description);
        });
        return Ok(inventoryItemDtos);
        } 
       [HttpPost]
        public async Task<ActionResult> PostAsync(GrandItemsDto grandItemsDto){
            var inventoryItem=await inventoryItemRepository.GetAsync(
                item=>item.UserId==grandItemsDto.UserId && item.CatalogItemId==grandItemsDto.CatalogItemId);
                if(inventoryItem==null){
                    inventoryItem=new InventoryItem{
                        CatalogItemId=grandItemsDto.CatalogItemId,
                        UserId=grandItemsDto.UserId,
                        Quantity=grandItemsDto.Quantity,
                        AcquiredDate=DateTimeOffset.UtcNow
                    };
                    await inventoryItemRepository.CreateAsync(inventoryItem);
                }
                else{
                    inventoryItem.Quantity+=grandItemsDto.Quantity;
                     await inventoryItemRepository.UpdateAsync(inventoryItem);
                }
                return Ok();
            
        }

    }
}

