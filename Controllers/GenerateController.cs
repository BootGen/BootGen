using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc; 
using WebProject.Services;

namespace WebProject.Controllers
{
    [ApiController]
    [Route("generate")]
    public class GenerateController : BaseController
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
            return new ObjectResult(response.ResponseData) { StatusCode = response.StatusCode };
        }

        [HttpPost]
        [Route("download")]
        public IActionResult Download([FromBody] GenerateRequest request)
        {
            var result = service.Download(request);
            if (result.StatusCode != 200)
                return BadRequest();
            return File(result.ResponseData, "application/zip", "project.zip");
        }
    }
}
