// See https://aka.ms/new-console-template for more information
using Flurl;
using Flurl.Http;
using RedfishAvailabilityChecker;

var start = new DateTime(2023, 7, 23);
var end = new DateTime(2023, 7, 29);

var slack = new SlackAlert();

await slack.SendSlackMessage($"Redfish site watcher started. Watching for sites that become available from {start.ToString("d")} to {end.ToString("d")}");

var year = 2023;
var month = 7;

var alertsSentToday = new Dictionary<string, List<DateTime>>();

var noExceptionsCaught = true;
var retryCounter = 0;

while (noExceptionsCaught)
{
    //rest for 15 seconds between calls
    Thread.Sleep(15000);
    Console.WriteLine($"Calling recreation.gov {DateTime.Now.ToString("T")}");
    AvailabilityResponse flurlResponse;

    try
    {
        flurlResponse = await @$"https://www.recreation.gov/api/camps/availability/campground/232050/month"
            .SetQueryParam("start_date", $"{year}-{month.ToString("D2")}-01T00:00:00.000Z")
            .GetJsonAsync<AvailabilityResponse>();
    } 
    catch (Exception e)
    {
        if (retryCounter > 5)
            break;
        else
        {
            retryCounter++;
            continue;
        }
    }
    finally
    {
        retryCounter = 0;
    }


    // Find any available dats inside of our range
    foreach (var campsiteRecord in flurlResponse.campsites)
    {
        var openDays = campsiteRecord.Value.availabilities.Where(x => x.Value == "Open");
        var openAndInRangeDays = openDays.Where(x => DateTime.Compare(start, x.Key) <= 0 && DateTime.Compare(x.Key, end) <= 0).Select(x => x.Key);
        if (!openAndInRangeDays.Any())
            continue;

        // if we have sent an alert for this campsite already, don't send another
        if (alertsSentToday.ContainsKey(campsiteRecord.Value.campsite_id))
        {
            if (openAndInRangeDays.Except(alertsSentToday[campsiteRecord.Value.campsite_id]).Any())
            {
                alertsSentToday.Remove(campsiteRecord.Value.campsite_id);
            }
            else
            {
                continue;
            }
        }

        sendTheAlert(campsiteRecord.Value, openAndInRangeDays.ToList());
    }
}

await slack.SendSlackMessage("Retry limit exceeded. Site watcher shutting down.");


string getSiteLink(Campsite campsite)
{
    return $"https://www.recreation.gov/camping/campsites/{campsite.campsite_id}";
}

async Task sendTheAlert(Campsite campsite, List<DateTime> availabilities)
{
    var doublePrefix = campsite.IsDouble ? "Double" : "Single";
    var message = $"{doublePrefix} Campsite is available for the following days: {string.Join(",", availabilities.Select(x => x.ToString("d")))}";
    message += $"\nHere's the link: {getSiteLink(campsite)}";

    await slack.SendSlackMessage(message);

    alertsSentToday.Add(campsite.campsite_id, availabilities.ToList());
}
