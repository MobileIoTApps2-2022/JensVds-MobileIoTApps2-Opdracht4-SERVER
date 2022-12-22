using System;
using System.Net.Http;
using server.Hubs;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

[Route("api/[controller]")]
[ApiController]
public class StateController : ControllerBase
{
    string status = "";
    private readonly IHubContext<Loghub> _hub;
    private readonly TimerManager _timer;

    public StateController(IHubContext<Loghub> hub, TimerManager timer)
    {
        _hub = hub;
        _timer = timer;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        if (!_timer.IsTimerStarted)
        {
            _timer.PrepareTimer( async () =>
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("accept", "application/json");
                    client.DefaultRequestHeaders.Add("authorization", "Bearer bdeb6ae900e63ad6e8c13afa19fa2ce4b053838c7d1efd91cddc209b95b6acec74740b2c3602946e");

                    var response = await client.GetAsync("https://api.nuki.io/smartlock");
                    var responseString = await response.Content.ReadAsStringAsync();
                    var json = JArray.Parse(responseString);

                    if (json[0]["state"]["state"].Value<int>() == 1)
                    {
                        status = "lock";
                    }
                    else if (json[0]["state"]["state"].Value<int>() == 3)
                    {
                        status = "lock_open";
                    }
                    else
                    {
                        status = "sync";
                    }
                    _hub.Clients.All.SendAsync("TransferState", status);
                }
            });
        }
        return Ok(status);
    }
}