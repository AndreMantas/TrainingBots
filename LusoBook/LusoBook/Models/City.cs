using System;

namespace LusoBook.Models
{
    [Serializable]
    public class City
    {
        public string Name { get; set; }
        public Country Country { get; set; }
    }
}