using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using LusoBook.Models;
using LusoBook.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace LusoBook.Dialogs
{
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {
        private BookingService _service;

        public RootDialog() : base(new LuisService(new LuisModelAttribute(
            ConfigurationManager.AppSettings["LuisAppId"],
            ConfigurationManager.AppSettings["LuisAPIKey"],
            domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
            _service = new BookingService();
        }

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

        #region FindHotel

        [LuisIntent("FindHotel")]
        public async Task FindHotelIntent(IDialogContext context, LuisResult result)
        {
            var hotels = _service.GetHotels();

            if (result.TryFindEntity("Location", out EntityRecommendation location))
            {
                hotels = FilterByLocation(hotels, location.Entity);

                if (hotels.Count == 0)
                {
                    await context.PostAsync("Desculpe mas não tenho hoteis na localização " + location.Entity);
                    context.Wait(this.MessageReceived);
                    return;
                }
            }

            await ShowHotels(context, hotels);
            context.Wait(OnHotelSelected);
        }

        private List<Hotel> FilterByLocation(List<Hotel> hotels, string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return hotels;
            }

            location = location.ToLowerInvariant();
            return hotels
                    .Where(x => x.City.Name.ToLowerInvariant().Contains(location) ||
                                x.City.Country.Name.ToLowerInvariant().Contains(location))
                    .ToList();
        }

        private async Task OnHotelSelected(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var msg = await result;
            var hotelName = msg.Text;
            var hotel = _service.GetHotels().FirstOrDefault(x => x.Name == hotelName);
            if (hotel == null)
            {
                await context.PostAsync("Vejo que não quer reservar nenhum destes hoteis... tente novamente");
            }
            else
            {
                await context.PostAsync($"Reserva efectuada com sucesso!");
                await context.PostAsync($"Sabia que também temos optimos descontos em voos para {hotel.City.Name}?");
            }
            context.Wait(this.MessageReceived);
        }

        private static async Task ShowHotels(IDialogContext context, IEnumerable<Hotel> hotels)
        {
            var attatchments = hotels
                .OrderByDescending(x => x.Rate)
                .Select(h => BuildHotelAttatchment(h))
                .ToList();
            await ShowCarousel(context, attatchments, "Encontramos estes hoteis");
        }

        private static Attachment BuildHotelAttatchment(Hotel h)
        {
            return new HeroCard()
            {
                Title = h.Name,
                Subtitle = $"{h.City.Name} ({h.City.Country.Name}) | Rating: {h.Rate}",
                Images = new List<CardImage>() { new CardImage() { Url = h.Image } },
                Buttons = new List<CardAction>() { new CardAction() { Title = "Reservar", Type = "postBack", Value = h.Name } }
            }.ToAttachment();
        }

        #endregion

        #region Generic

        private static async Task ShowCarousel(IDialogContext context, List<Attachment> attachments, string msg = "")
        {
            var message = context.MakeMessage();
            message.Text = msg;
            message.AttachmentLayout = "carousel";
            message.Attachments = attachments;
            await context.PostAsync(message);
        }

        private async Task ShowLuisResult(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"You have reached {result.Intents[0].Intent}. You said: {result.Query}");

            foreach (var entity in result.Entities)
            {
                await context.PostAsync($"Got this entity: {entity.Type} => {entity.Entity}");
            }

            context.Wait(MessageReceived);
        }

        #endregion
    }
}