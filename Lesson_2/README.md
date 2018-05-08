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

- Fix any possible compilation errors by importing the required packages (press <kbd>Ctrl</kbd> <kbd>.</kbd> while the error line is selected to see suggestions).

- Test your bot again.