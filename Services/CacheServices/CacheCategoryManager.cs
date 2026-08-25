//using Domain.Entities;
//using DTO.Category;
//using Microsoft.Extensions.Caching.Memory;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Services.CacheServices
//{
//    /// <summary>
//    /// مدیریت دسته بندی های درون کش
//    /// </summary>
//    public static class CacheCategoryManager
//    {
//        public static readonly string Key = "Cats";



//        public static List<HomeCategoryDTO> GetCategories(this IMemoryCache cache)
//        {
//            return cache.Get<List<HomeCategoryDTO>>(Key);
//        }


//        public static void SetCategories(this IMemoryCache cache, List<HomeCategoryDTO> value)
//        {
//            cache.Set<List<HomeCategoryDTO>>(Key, value);
//        }



//        public static void RemoveCategories(this IMemoryCache cache)
//        {
//            cache.Remove(Key);
//        }

//    }
//}
