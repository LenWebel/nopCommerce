using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.AutoMapper;
using Nop.Plugin.Api.DTO.ProductAttributes;
using Nop.Plugin.Api.DTO.Products;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class ProductAttributeDtoMappings
    {
        public static ProductAttributeDto ToDto(this ProductAttribute productAttribute)
        {
            return productAttribute.MapTo<ProductAttribute, ProductAttributeDto>();
        }
        
        public static ProductAttributeMappingDto ToDto(this ProductAttributeMapping productAttribute)
        {
            return productAttribute.MapTo<ProductAttributeMapping, ProductAttributeMappingDto>();
        }
    }
}
