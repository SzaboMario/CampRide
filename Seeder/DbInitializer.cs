using CampRide.Data;
using CampRide.Data.Entities;

namespace CampRide.Seeder
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Caravans.Any())
                return;

            static CaravanImageEntity Img(string fn, bool primary, int sort) =>
                new() { FileName = fn, IsPrimary = primary, SortOrder = sort };

            context.Caravans.AddRange(

                // 1 — Fiat Ducato Camper
                // Képek: Fiat camper van 2019 + 1989 Fiat Ducato Maxi (Wikimedia Commons)
                new CaravanEntity
                {
                    Name        = "Fiat Ducato Camper",
                    Description = "Tágas, 7 személyes családi lakóautó konyhával, fürdőszobával és klímával. Ideális hosszabb utakra.",
                    Capacity    = 7,
                    PricePerDay = 35_000,
                    Images      = new List<CaravanImageEntity>
                    {
                        Img("main.jpg", true,  0),
                        Img("img2.jpg", false, 1),
                    }
                },

                // 2 — VW California
                // Kép: VW California Ocean T6.1 (Wikimedia Commons)
                new CaravanEntity
                {
                    Name        = "VW California",
                    Description = "Kompakt 4 személyes camper van, pop-up tetővel és beépített konyhával. Városi és vidéki túrákhoz egyaránt.",
                    Capacity    = 4,
                    PricePerDay = 28_000,
                    Images      = new List<CaravanImageEntity>
                    {
                        Img("main.jpg", true, 0),
                    }
                },

                // 3 — Mercedes Sprinter 4x4
                // Kép: 2006-14 4x4 Mercedes-Benz Sprinter (Wikimedia Commons)
                new CaravanEntity
                {
                    Name        = "Mercedes Sprinter 4x4",
                    Description = "Terepjáró képességű, 6 személyes luxuslakóautó. Napelem, téli csomag és prémium belső berendezés.",
                    Capacity    = 6,
                    PricePerDay = 52_000,
                    Images      = new List<CaravanImageEntity>
                    {
                        Img("main.jpg", true, 0),
                    }
                },

                // 4 — Ford Transit Nugget
                // Kép: Ford Transit Camper (Wikimedia Commons)
                new CaravanEntity
                {
                    Name        = "Ford Transit Nugget",
                    Description = "Stílusos 4 személyes camper Ford Transitból átalakítva. Dupla ágy, mini konyha, nagy csomagtér.",
                    Capacity    = 4,
                    PricePerDay = 30_000,
                    Images      = new List<CaravanImageEntity>
                    {
                        Img("main.jpg", true, 0),
                    }
                },

                // 5 — Hymer B-Klasse 580
                // Kép: Hymer B ML-I 790 front view (Wikimedia Commons)
                new CaravanEntity
                {
                    Name        = "Hymer B-Klasse 580",
                    Description = "Prémium 5 személyes integrált lakóautó. Panorámaablak, automata váltó, luxus konyha és nappali.",
                    Capacity    = 5,
                    PricePerDay = 48_000,
                    Images      = new List<CaravanImageEntity>
                    {
                        Img("main.jpg", true, 0),
                    }
                },

                // 6 — Knaus Van TI 650
                // Kép: Knaus Van TI (Wikimedia Commons)
                new CaravanEntity
                {
                    Name        = "Knaus Van TI 650",
                    Description = "Romantikus 2 személyes karavánja pároknak. Franciaágy, teli felszereltség, ideális hétvégi kirándulásokhoz.",
                    Capacity    = 2,
                    PricePerDay = 22_000,
                    Images      = new List<CaravanImageEntity>
                    {
                        Img("main.jpg", true, 0),
                    }
                },

                // 7 — Dethleffs Trend T 7057
                // Kép: Dethleffs Wohnauto (Wikimedia Commons)
                new CaravanEntity
                {
                    Name        = "Dethleffs Trend T 7057",
                    Description = "Nagy, 6 személyes félintegrált lakóautó két hálószobával. Klíma, TV, automata napellenző.",
                    Capacity    = 6,
                    PricePerDay = 38_000,
                    Images      = new List<CaravanImageEntity>
                    {
                        Img("main.jpg", true, 0),
                    }
                },

                // 8 — Weinsberg CaraBus 600
                // Kép: Weinsberg CaraCompact Edition Pepper (Wikimedia Commons)
                new CaravanEntity
                {
                    Name        = "Weinsberg CaraBus 600",
                    Description = "Kompakt, 2+1 személyes camper van kezdőknek és hétvégi kalandozóknak. Egyszerű kezelhetőség, kényelmes ágy.",
                    Capacity    = 3,
                    PricePerDay = 24_000,
                    Images      = new List<CaravanImageEntity>
                    {
                        Img("main.jpg", true, 0),
                    }
                },

                // 9 — Carthago C-Tourer I 150
                // Kép: Carthago RV Germany (Wikimedia Commons)
                new CaravanEntity
                {
                    Name        = "Carthago C-Tourer I 150",
                    Description = "Olasz design, 4 személyes félintegrált. Bőr ülőgarnitúra, induktív töltő, napelemes rendszer.",
                    Capacity    = 4,
                    PricePerDay = 45_000,
                    Images      = new List<CaravanImageEntity>
                    {
                        Img("main.jpg", true, 0),
                    }
                },

                // 10 — Adria Matrix Axess 600 SC
                // Kép: Adria Mobil motorhome 2021 Tour of Slovenia (Wikimedia Commons)
                new CaravanEntity
                {
                    Name        = "Adria Matrix Axess 600 SC",
                    Description = "Elegáns, 5 személyes félintegrált lakóautó. Tágas nappali, franciaágy a hátsó részben, gazdag felszereltség.",
                    Capacity    = 5,
                    PricePerDay = 36_000,
                    Images      = new List<CaravanImageEntity>
                    {
                        Img("main.jpg", true, 0),
                    }
                }
            );

            context.SaveChanges();
        }
    }
}
