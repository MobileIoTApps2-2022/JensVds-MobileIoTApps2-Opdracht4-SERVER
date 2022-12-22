using Microsoft.AspNetCore.SignalR;
using server.Hubs;
using server.Models;
using server.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogController : ControllerBase
{
    private readonly LogService _LogService;
    private readonly IHubContext<Loghub> _hubContext;

    public LogController(LogService LogService, IHubContext<Loghub> hubContext)
    {
        _LogService = LogService;
        _hubContext = hubContext;
    }

    [HttpGet]
    public async Task<List<Log>> Get() =>
        await _LogService.GetAsync();

    [HttpGet("{Id:length(24)}")]
    public async Task<ActionResult<Log>> Get(string Id)
    {
        var Log = await _LogService.GetAsync(Id);

        if (Log is null)
        {
            return NotFound();
        }

        return Log;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Log newLog)
    {
        string url = $"https://api.nuki.io/smartlock/645574324/action/{newLog.action}";

        var request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "POST";
        request.Accept = "application/json";
        request.Headers.Add("authorization", "Bearer bdeb6ae900e63ad6e8c13afa19fa2ce4b053838c7d1efd91cddc209b95b6acec74740b2c3602946e");

        var response = (HttpWebResponse)request.GetResponse();
        string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        Console.WriteLine(responseString);

        await _LogService.CreateAsync(newLog);

        await _hubContext.Clients.All.SendAsync("Locktoggled", newLog.userId, newLog.action, newLog.timestamp);

        return CreatedAtAction(nameof(Get), new { Id = newLog.Id }, newLog);
    }

    [HttpPut("{Id:length(24)}")]
    public async Task<IActionResult> Update(string Id, Log updatedLog)
    {
        var Log = await _LogService.GetAsync(Id);

        if (Log is null)
        {
            return NotFound();
        }

        updatedLog.Id = Log.Id;

        await _LogService.UpdateAsync(Id, updatedLog);

        return NoContent();
    }

    [HttpDelete("{Id:length(24)}")]
    public async Task<IActionResult> Delete(string Id)
    {
        var Log = await _LogService.GetAsync(Id);

        if (Log is null)
        {
            return NotFound();
        }

        await _LogService.RemoveAsync(Id);

        return NoContent();
    }
}