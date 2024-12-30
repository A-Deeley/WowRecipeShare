﻿using Microsoft.AspNetCore.Mvc;

namespace recipe_share_api.Files;

[ApiController]
[Route("[controller]")]
public class FilesController : Controller
{
    [HttpGet("addon")]
    public ActionResult GetAddon()
    {
        var zipFile = System.IO.File.ReadAllBytes("Files/RecipeShare_0.1.6.zip");
        return File(zipFile, "application/zip", "RecipeShare_0.1.6.zip");
    }
}