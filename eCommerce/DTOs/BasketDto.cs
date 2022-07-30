using System.Collections.Generic;

namespace eCommerce.DTOs
{
    public class BasketDto
    {
        public int Id { get; set; }
        public String BuyerId { get; set; }

        public List<BasketItemDto> Items { get; set; }
    }
}