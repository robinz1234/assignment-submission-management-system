using AssignmentManagement.Api.Data;
using AssignmentManagement.Api.DTOs;
using AssignmentManagement.Api.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssignmentManagement.Api.Controllers.Admin;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/settings")]
public class SettingsController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SettingDto>>> GetAll()
    {
        var items = await dbContext.Settings.AsNoTracking().OrderBy(item => item.Key).ToListAsync();
        return Ok(items.Select(item => new SettingDto(item.Id, item.Key, item.Value, item.Description, item.UpdatedAt)));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<SettingDto>> Update(int id, UpdateSettingRequest request)
    {
        var item = await dbContext.Settings.SingleOrDefaultAsync(setting => setting.Id == id)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "Setting was not found.");
        item.Value = request.Value.Trim();
        item.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync();
        return Ok(new SettingDto(item.Id, item.Key, item.Value, item.Description, item.UpdatedAt));
    }
}
