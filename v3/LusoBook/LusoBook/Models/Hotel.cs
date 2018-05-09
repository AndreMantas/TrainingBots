﻿using System;

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