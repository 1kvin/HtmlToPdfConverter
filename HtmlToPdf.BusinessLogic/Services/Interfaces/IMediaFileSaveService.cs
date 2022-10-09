﻿using Microsoft.AspNetCore.Http;

namespace HtmlToPdf.BusinessLogic.Services.Interfaces;

public interface IMediaFileSaveService
{
    public Task<int> Save(IFormFile file);
}