using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InClassLab28.Models
{
    public class Cards
    {
        public Cards(string image, string value, string suit)
        {
            Image = image;
            Value = value;
            Suit = suit;
        }

        public string Image { get; set; }
        public string Value { get; set; }
        public string Suit { get; set; }

        public Cards()
        {

        }
    }
}