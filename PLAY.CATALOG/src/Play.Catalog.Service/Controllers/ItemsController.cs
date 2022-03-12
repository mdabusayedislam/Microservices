using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Contracts;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common;

namespace Play.Catalog.Service.Controllers{
[ApiController]
[Route("items")]
public class ItemsController:ControllerBase
{
    // private static readonly List<ItemDto> items=new()
    // {
    //     new ItemDto(Guid.NewGuid(),"Pen1","writing item",5,DateTimeOffset.UtcNow),
    //     new ItemDto(Guid.NewGuid(),"Pen2","writing item",5,DateTimeOffset.UtcNow),
    //     new ItemDto(Guid.NewGuid(),"Pen3","writing item",5,DateTimeOffset.UtcNow),
    //     new ItemDto(Guid.NewGuid(),"Pen4","writing item",5,DateTimeOffset.UtcNow),
    //     new ItemDto(Guid.NewGuid(),"Pen5","writing item",5,DateTimeOffset.UtcNow),
    // };
    private readonly IRepository<Item> itemsRepository;
    private readonly IPublishEndpoint publishEndpoint;
    //private static int requestCounter=0;
    public ItemsController(IRepository<Item> itemsRepository,IPublishEndpoint publishEndpoint){
        this.itemsRepository=itemsRepository;
        this.publishEndpoint=publishEndpoint;
    }
    //Get /items
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetAllAsync(){
        // requestCounter++;
        // Console.WriteLine($"Request {requestCounter}:Starting...");
        // if(requestCounter<=2){
        // Console.WriteLine($"Request {requestCounter}:Delaying...");
        // await Task.Delay(TimeSpan.FromSeconds(10));
        // }
        // if(requestCounter<=4){
        // Console.WriteLine($"Request {requestCounter}:500 (Internal server error)");
        // return StatusCode(500);
        // }
        var items=(await itemsRepository.GetAllAsync()).Select(item=>item.AsDto());       
        //Console.WriteLine($"Request {requestCounter}:200 (OK)");
        return Ok(items);
    }

    //Get /items/id
    [HttpGet("id")]
    public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id){

        var item=await itemsRepository.GetAsync(id);
        if(item==null){
            return NotFound();
        }
        return item.AsDto();
    }

    //Post /items
    [HttpPost]
    public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto){

       var item=new Item{
           Name=createItemDto.Name,
           Description=createItemDto.Description,
           Price=createItemDto.Price,
           CreatedDate=DateTimeOffset.UtcNow
       };
       await itemsRepository.CreateAsync(item);
       await publishEndpoint.Publish(new CatalogItemCreated(item.Id,item.Name,item.Description));
       return CreatedAtAction(nameof(GetByIdAsync),new{id=item.Id},item);
    }

    //Put /items/id
    [HttpPut("id")]
    public async Task<IActionResult> PutAsync(Guid id,UpdateItemDto updateItemDto){
     var existingItem=await itemsRepository.GetAsync(id);
     if(existingItem==null){
         return NotFound();
     }
    existingItem.Name=updateItemDto.Name;
    existingItem.Description=updateItemDto.Description;
    existingItem.Price=updateItemDto.Price;
    await itemsRepository.UpdateAsync(existingItem);
    await publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id,existingItem.Name,existingItem.Description));
     return NoContent();
    }

    //Delete /items/id
    [HttpDelete("id")]
    public async Task<IActionResult> DeleteAsync(Guid id){
     var item=await itemsRepository.GetAsync(id);
     if(item==null){
         return NotFound();
     }
      await itemsRepository.RemoveAsync(item.Id);
      await publishEndpoint.Publish(new CatalogItemDeleted(id));
     return NoContent();
    }


}
}