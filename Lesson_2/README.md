# TrainingBots

Start by adding a reference to `System.Configuration` in your project. 

- In `Solution Explorer`, under your project, right click `References`, and choose `Add Reference`. Search `config` and select `System.Configuration`. Press OK.

Now add these settings to your `web.config`

    <add key="LuisAppId" value="ID" />
    <add key="LuisAPIKey" value="KEY" />
    <add key="LuisAPIHostName" value="westeurope.api.cognitive.microsoft.com" />

- Get your ID and KEY in your LUIS application page under the Publish menu (choose your region and check the endpoint).

We need these configs so we can instruct our dialog to connect to our LUIS app. Let's update our `RootDialog.cs`:

- Make it extend LuisDialog provided by Microsoft:  

        public class RootDialog : LuisDialog<object>

- Add a constructor

        public RootDialog() : base(new LuisService(new LuisModelAttribute(
            ConfigurationManager.AppSettings["LuisAppId"],
            ConfigurationManager.AppSettings["LuisAPIKey"],
            domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }

- Now let's add some methods to handle each of the intents we made in the LUIS app and one to debug information:

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        [LuisIntent("FindFlight")]
        public async Task FindFlightIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        [LuisIntent("FindHotel")]
        public async Task FindHotelIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        private async Task ShowLuisResult(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"You have reached {result.Intents[0].Intent}. You said: {result.Query}"); // reply to user
            context.Wait(MessageReceived); // wait for next user message
        }

- Note on how these methods receive a `LuisResult` parameter. This object has the same information we get from testing our LUIS application (the intent and the entities).

Fix any possible compilation errors by importing the required packages (press <kbd>Ctrl</kbd> <kbd>.</kbd> while the error line is selected to see suggestions).

Test your bot and make sure it works in this simple scenario. It should print the intents you declared in your LUIS app.

You can easily use the entities LUIS was able to extract from the user input. In the `ShowLuisResult` method, add this code before the context.Wait call

        foreach (var entity in result.Entities)
        {
            await context.PostAsync($"Got this entity: {entity.Type} => {entity.Entity}");
        }

Now let's start adding some functionality to our bot. In my case I'll create `models` and a `service` to book hotels.

- Create a new folder named `Models` and create classes that represent things that your bot knowns (e.g., countries, cities, hotels, ...).

- Create a new folder named `Services` and create one or more classes that our dialog will use to get data from (e.g., get a list of hotels).

- **Note: don't forget to mark these classes as `[Serializable]`.** For example:

`Hotel.cs`

    using System;

    namespace LusoBook.Models
    {
        [Serializable]
        public class Hotel
        {
            public string Name { get; set; }
            public City City { get; set; }
            public double Rate { get; set; }
            public string Image { get; set; }
        }
    }

`BookingService.cs`

        [Serializable]
        public class BookingService
        {
            ...
            public List<Hotel> GetHotels() { ... }
            public List<Country> GetCountries() { ... }
            public List<City> GetCities() { ... }
            ...
        }

Back to `RootDialog.cs`, update the constructor to initialize our service

    private BookingService _service;           // add

    public RootDialog() : base(new LuisService(new LuisModelAttribute(
        ConfigurationManager.AppSettings["LuisAppId"],
        ConfigurationManager.AppSettings["LuisAPIKey"],
        domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
    {
        _service = new BookingService();       // add
    }

Now, for example, in the `FindHotelIntent` let's show all available hotels:

    var hotels = _service.GetHotels();

    var attatchments = hotels
        .OrderByDescending(x => x.Rate)
        .Select(h => BuildHotelAttatchment(h))
        .ToList();

    var message = context.MakeMessage();
    message.Text = "We found these hotels:";
    message.AttachmentLayout = "carousel";
    message.Attachments = attachments;
    await context.PostAsync(message);

    context.Wait(MessageReceived);

You can implement the `BuildHotelAttatchment` as you prefer. Here is an implementation using the `HeroCard`:

    private static Attachment BuildHotelAttatchment(Hotel h)
    {
        return new HeroCard()
        {
            Title = h.Name,
            Subtitle = $"{h.City.Name} ({h.City.Country.Name}) | Rating: {h.Rate}",
            Images = new List<CardImage>() { new CardImage() { Url = h.Image } },
        }.ToAttachment();
    }

At this point you should be able to test your bot and see the HeroCards showing the hotels provided by the BookingService class.

We are still not using everything that LUIS provides. If the user says where he wants to search for hotels, we should only show hotels in that location. We can use LUIS entities to accomplish this. For example, we can check if the Location entity is present and filter all hotels before showing them:

    if (result.TryFindEntity("Location", out EntityRecommendation location))
    {
        hotels = FilterByLocation(hotels, location.Entity);

        if (hotels.Count == 0)
        {
            await context.PostAsync("Sorry but there are no hotels in " + location.Entity);
            context.Wait(this.MessageReceived);
            return;
        }
    }

The `FilterByLocation` method can simply check if the location that the user typed is contained in the hotel city or country name:

    private static List<Hotel> FilterByLocation(List<Hotel> hotels, string location)
    {
        location = location.ToLowerInvariant();
        return hotels
                .Where(x => x.City.Name.ToLowerInvariant().Contains(location) ||
                            x.City.Country.Name.ToLowerInvariant().Contains(location))
                .ToList();
    }

Now we just need to make sure that our LUIS app is well trained and will be able to extract the Location entity correctly from any sentence that the user types.

You can extend this logic to any property that your model can be filtered so you can show the results that the user wants to see.

---

## Deploy our bot to Azure

- Create a new web app bot in Azure. The bot template does not matter.

- Note down the `MicrosoftAppId` and `MicrosoftAppPassword` and use them in the respective web.config properties.

- Download the publish profile of the generated App Service resource.

- In Visual Studio right click in your project and choose Publish. Import the downloaded publish profile and publish.

- Now your bot is alive and can be reached by the available channels. 

- Review Lesson_1 to see how to connect a channel (e.g., directline) to a bot and deploya simple webchat connected to your bot.