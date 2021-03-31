using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc; 
using Editor.Services;
using System.Threading;

namespace Editor.Controllers
{
    [ApiController]
    [Route("generate")]
    public class GenerateController : ControllerBase
    {
        private IGenerateService service;
        public GenerateController(IGenerateService service)
        {
            this.service = service;
        }

        [HttpPost]
        [Route("generate")]
        public IActionResult Generate([FromBody] GenerateRequest request)
        {
            var response = service.Generate(request);
            #if DEBUG
            Thread.Sleep(3000);
            #endif
            return Ok(response);
        }

        [HttpPost]
        [Route("download")]
        public IActionResult Download([FromBody] GenerateRequest request)
        {
            var result = service.Download(request);
            if (result == null)
                return BadRequest();
            #if DEBUG
            Thread.Sleep(3000);
            #endif
            return File(result, "application/zip", $"{request.NameSpace}.zip");
        }
    }
}
