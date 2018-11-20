using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InClassLab28.Models;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;

namespace InClassLab28.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult DeckOfCards()
        {
            return View();
        }

        public ActionResult CreateDeck()
        {
            string deck_id;
            HttpWebRequest request = WebRequest.CreateHttp("https://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1"); //Dan has count=1...

            //following line is optional, lets you use dummy information
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:64.0) Gecko/20100101 Firefox/64.0"; //Tells you what browser the user is using.
            
            //make your response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            //if the response comes back okay
            if (response.StatusCode == HttpStatusCode.OK) 
            {
                //get response stream
                StreamReader reader = new StreamReader(response.GetResponseStream());

                //read response stream as a string
                string output = reader.ReadToEnd();

                //convert response to JSON
                JObject Jparser = JObject.Parse(output);

                //If TempData is empty (checking for the deck)
                if(TempData["deck_id"] == null)
                {
                    //get the deck id from the JSON
                    TempData["deck_id"] = Jparser["deck_id"]; //Deck ID is the property
                    deck_id = Jparser["deck_id"].ToString();
                }
                //otherwise set the new deck id
                else
                {
                    //convert the deck ID to string
                    deck_id = TempData["deck_id"].ToString();
                }

                ViewBag.Deck = deck_id;
                reader.Close();
                response.Close();
            }
            return View("DeckOfCards");

        }
        public ActionResult DrawCards(string deck_id)
        {
            //Make a cookie to store the temporary data
            HttpCookie cookie;

            //if the cookie doesn't exist make it, and add it to the user's browser
            //If Request.Cookies = Null then make a new cookie
            if(Request.Cookies["deck_id"] == null)
            {
                cookie = new HttpCookie("deck_id");
                cookie.Value = deck_id;
                Response.Cookies.Add(cookie);
            }
            //anything else request cookie
            else
            {
                cookie = Request.Cookies["deck_id"];
                cookie.Value = deck_id;
            }

            //ensure that the deck_id is what the user has
            deck_id = cookie.Value.ToString();

            //Make the request 
            HttpWebRequest request = WebRequest.CreateHttp("https://deckofcardsapi.com/api/deck/" + deck_id + "/draw/?count=5");
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string output = reader.ReadToEnd();

                JObject Jparser = JObject.Parse(output);

                Cards[] cards = new Cards[Jparser["cards"].Count()];
                int i = 0;
                foreach(var x in Jparser["cards"])
                {
                    cards[i] = new Cards(x["image"].ToString(), x["value"].ToString(), x["suit"].ToString() );
                    i++;
                }

                ViewBag.CardsInHands = cards;
                reader.Close();
                response.Close();
                return View("DeckOfCards");
            }


            return View("DeckOfCards");
        }
    }
}