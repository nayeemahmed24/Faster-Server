using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheStore.FasterCache;
using CacheStore.Redis;
using FASTER.core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Diagnostics.Instrumentation.Extensions.Intercept;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Webservice.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IRedisStore redisStore;
        private const string Key = "key-1";
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IFasterCache fasterCache;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IRedisStore redisStore, IFasterCache fasterCache)
        {
            _logger = logger;
            this.redisStore = redisStore;
            this.fasterCache = fasterCache;
        }


        [HttpGet]
        public IActionResult GetFasterCacheData()
        {
            this.fasterCache.AddData();
            //if (String.IsNullOrEmpty(fasterStringValue))
            //{
            //    return this.Ok("No Data Found");
            //}

            //var redisData = JsonConvert.DeserializeObject<WeatherForecast>(fasterStringValue);
            return this.Ok();
        }

        [HttpGet]
        public IActionResult AddFasterCacheData()
        {
            //var data = new WeatherForecast()
            //{
            //    Date = DateTime.Now,
            //    TemperatureC = 30,
            //};
            //var serializedData = JsonConvert.SerializeObject(data);
            //this.fasterCache.AddData(Key, serializedData);
            return this.Ok();
        }

        [HttpGet]
        public async Task<IActionResult> AddRedisData()
        {
            await this.redisStore.SetCacheData(Key, new WeatherForecast()
            {
                Date = DateTime.Now,
                TemperatureC = 30,
            });
            return this.Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetRedisData()
        {
            var redisDataAsByteArray = await this.redisStore.GetCacheData(Key);
            if (redisDataAsByteArray == null)
            {
                return this.Ok("No Data Found");
            }
            var redisEncodedData = Encoding.UTF8.GetString(redisDataAsByteArray);
            var redisData = JsonConvert.DeserializeObject<WeatherForecast>(redisEncodedData);
            return this.Ok(redisData);
        }

    }
}
