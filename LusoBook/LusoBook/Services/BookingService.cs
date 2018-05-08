using LusoBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LusoBook.Services
{
    [Serializable]
    public class BookingService
    {
        public static Random rand = new Random();

        private List<Country> _countries;
        private List<City> _cities;
        private List<Hotel> _hotels;

        public BookingService()
        {
            BuildStaticData();
        }

        public List<Hotel> GetHotels()
        {
            return _hotels.ToList();
        }

        public List<Country> GetCountries()
        {
            return _countries.ToList();
        }

        public List<City> GetCities()
        {
            return _cities.ToList();
        }

        private void BuildStaticData()
        {
            _countries = new List<Country>()
            {
                new Country { Name = "Portugal" },
                new Country { Name = "Espanha" },
                new Country { Name = "França" },
            };

            _cities = new List<City>()
            {
                new City() { Name = "Lisboa", Country = _countries[0] },
                new City() { Name = "Porto", Country = _countries[0] },

                new City() { Name = "Madrid", Country = _countries[1] },
                new City() { Name = "Barcelona", Country = _countries[1] },

                new City() { Name = "Paris", Country = _countries[2] },
                new City() { Name = "Marselha", Country = _countries[2] },
            };

            var images = BuildImages();
            var imageIndex = 0;
            _hotels = new List<Hotel>();
            foreach (var city in _cities)
            {
                var numHotels = rand.Next(4) + 1;
                for (int i = 0; i < numHotels; i++)
                {
                    _hotels.Add(new Hotel()
                    {
                        Name = $"{city.Name} Hotel {i + 1}",
                        HasPool = rand.Next(2) == 0,
                        Rate = Math.Round(rand.NextDouble() * 5, 2),
                        City = city,
                        Image = images[imageIndex],
                    });
                    imageIndex = (imageIndex + 1) % images.Length;
                }
            }
        }

        private string[] BuildImages()
        {
            return new string[]
            {
                @"https://www.omnihotels.com/-/media/images/hotels/ausctr/pool/ausctr-omni-austin-hotel-downtown-evening-pool.jpg?h=660&la=en&w=1170",
                @"http://rodrepel.com/blog/wp-content/uploads/2017/04/HRH01.jpg",
                @"https://cache.marriott.com/Images/Brands/Brand_Page_2016_redesign/Hero_Images_all_brands/AC_Hotel_294064_extracted.jpg",
                @"https://xohotels.com/wp-content/uploads/2018/03/xo-hotels-park-west-standard-double-twin-room-v17925543-753x500.jpg.pagespeed.ce.lEzQSd3cI8.jpg",
                @"https://d1i2hi5dlrpq5n.cloudfront.net/~/media/images/hotels/jdw_9897.jpg?h=660&la=en&w=555&vs=1&d=20170825T165033Z&crop=1&cropx=50&cropy=50&hash=0CAD617F4ACA04A2648AF16F0421BCC07539E024",
                @"https://www.qantas.com/images/qantas/ancillaries/hotels/crown-sale-nov16/jpg/article.mobile.jpg",
                @"https://firsthotelsiv.azurewebsites.net/publishedmedia/0yde9zpzv35v4vyr7lqe/double-first-hotel-petit-palace-museum-barcelona_3.jpg",
                @"https://ihg.scene7.com/is/image/ihg/Pattaya_Image_1200x375-1a",
                @"http://www3.hilton.com/resources/media/hi/BEYHGHI/en_US/img/shared/full_page_image_gallery/main/HL_exterdark_2_675x359_FitToBoxSmallDimension_Center.jpg",
                @"https://www.bulgarihotels.com/.imaging/bhr-wide-small-jpg/dam/pre-home/collection_2.png/jcr%3Acontent",
                @"https://media-cdn.tripadvisor.com/media/photo-s/0d/83/c7/dd/pool.jpg",
            };
        }
    }
}