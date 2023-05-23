﻿using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VTrack.Api.Services.Environment;

namespace VTrack.Api.Controllers;

[ApiController]
[Route("environment")]
public class EnvironmentController : ControllerBase
{
    private readonly IEnvironmentService environmentService;

    public EnvironmentController(IEnvironmentService environmentService)
    {
        this.environmentService = environmentService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status200OK)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Get()
    {
        Dictionary<string, string> settings = await environmentService.Get();

        return new JsonResult(settings);
    }
}