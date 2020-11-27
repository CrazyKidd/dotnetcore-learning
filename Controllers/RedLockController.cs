using Microsoft.AspNetCore.Mvc;
using NetNote.Models;
using RedLockNet;
using System;
using System.Threading.Tasks;

namespace NetNote.Controllers {

    public class RedLockController: Controller {
        private readonly IDistributedLockFactory _distributedLockFactory;
        private readonly ProductService _productService;

        public RedLockController(IDistributedLockFactory distributedLockFactory, ProductService productService) {
            _distributedLockFactory = distributedLockFactory;
            _productService = productService;
        }

        [HttpGet]
        public async Task <bool> DistributedLockTest() {
            string productId = "id";
            using(var redLock = await _distributedLockFactory.CreateLockAsync(productId, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(20))) {
                if (redLock.IsAcquired) {
                    var result = await _productService.BuyAsync();
                    return result;
                } else {
                    Console.WriteLine("获取锁失败！！");
                }
            }
            return false;
        }
    }
}