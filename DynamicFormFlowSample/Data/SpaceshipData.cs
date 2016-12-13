using DynamicFormFlowSample.Models;
using System.Collections.Generic;

namespace DynamicFormFlowSample.Data
{
    /// <summary>
    /// Represents the data in an imaginary database.
    /// 
    /// Note that normally this data would be retrieved online, but for the sake of simplicity of
    /// this sample, the dummy data here is local.
    /// </summary>
    public class SpaceshipData
    {
        private static SpaceshipData _instance;
        public static SpaceshipData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SpaceshipData();
                }

                return _instance;
            }
        }

        public IList<Spaceship> Spaceships
        {
            get;
            private set;
        }

        /// <summary>
        /// NOTE: This data should be session and user specific!
        /// For the sake of simplicity this data is local here, but in real-life scenario this
        /// would cause issues: Multiple user accessing the data simultaneously will lead to
        /// unwanted results.
        /// </summary>
        public IList<Spaceship> LastSearchResults
        {
            get;
            set;
        }

        private SpaceshipData()
        {
            Spaceships = new List<Spaceship>();
            CreateDummyData();
        }

        /// <summary>
        /// Looks for (partially) matching spaceships from the data based on the given criteria.
        /// </summary>
        /// <param name="spaceshipFilter">The filter for the search.</param>
        /// <returns>A list of partial matches or an empty list if none found.</returns>
        public IList<Spaceship> SearchForPartialMatches(Spaceship spaceshipFilter)
        {
            IList<Spaceship> partialMatches = new List<Spaceship>();

            if (spaceshipFilter != null)
            {
                foreach (Spaceship spaceship in Spaceships)
                {
                    if (spaceship.IsPartialMatch(spaceshipFilter))
                    {
                        partialMatches.Add(spaceship);
                    }
                }
            }

            return partialMatches;
        }

        private void CreateDummyData()
        {
            // Star Trek
            Spaceships.Add(new Spaceship()
            {
                Size = Spaceship.Sizes.Small,
                Engines = new List<Spaceship.EngineTypes>() { Spaceship.EngineTypes.Inpulse },
                Weapons = new List<Spaceship.WeaponTypes>() { Spaceship.WeaponTypes.Phaser, Spaceship.WeaponTypes.PhotonTorpedos },
                Crew = Spaceship.CrewTypes.GoodGuys,
                Name = "Federation attack fighter",
                ImageUri = "https://upload.wikimedia.org/wikipedia/en/thumb/9/9f/Star_Trek_movie_logo_2009.jpg/310px-Star_Trek_movie_logo_2009.jpg"
            });
            Spaceships.Add(new Spaceship()
            {
                Size = Spaceship.Sizes.Small,
                Engines = new List<Spaceship.EngineTypes>() { Spaceship.EngineTypes.Inpulse, Spaceship.EngineTypes.Warp },
                Weapons = new List<Spaceship.WeaponTypes>() { Spaceship.WeaponTypes.None },
                Crew = Spaceship.CrewTypes.GoodGuys,
                Name = "NX Alpha",
                ImageUri = "https://upload.wikimedia.org/wikipedia/en/thumb/9/9f/Star_Trek_movie_logo_2009.jpg/310px-Star_Trek_movie_logo_2009.jpg"
            });
            Spaceships.Add(new Spaceship()
            {
                Size = Spaceship.Sizes.Large,
                Engines = new List<Spaceship.EngineTypes>() { Spaceship.EngineTypes.Inpulse, Spaceship.EngineTypes.Warp },
                Weapons = new List<Spaceship.WeaponTypes>() { Spaceship.WeaponTypes.Phaser, Spaceship.WeaponTypes.PhotonTorpedos },
                Crew = Spaceship.CrewTypes.GoodGuys,
                Name = "Enterprise (NX-01)",
                ImageUri = "https://upload.wikimedia.org/wikipedia/en/thumb/9/9f/Star_Trek_movie_logo_2009.jpg/310px-Star_Trek_movie_logo_2009.jpg"
            });
            Spaceships.Add(new Spaceship()
            {
                Size = Spaceship.Sizes.Large,
                Engines = new List<Spaceship.EngineTypes>() { Spaceship.EngineTypes.Inpulse, Spaceship.EngineTypes.Warp },
                Weapons = new List<Spaceship.WeaponTypes>() { Spaceship.WeaponTypes.Phaser, Spaceship.WeaponTypes.PhotonTorpedos },
                Crew = Spaceship.CrewTypes.GoodGuys,
                Name = "USS Franklin (NX-326)",
                ImageUri = "https://upload.wikimedia.org/wikipedia/en/thumb/9/9f/Star_Trek_movie_logo_2009.jpg/310px-Star_Trek_movie_logo_2009.jpg"
            });
            Spaceships.Add(new Spaceship()
            {
                Size = Spaceship.Sizes.Large,
                Engines = new List<Spaceship.EngineTypes>() { Spaceship.EngineTypes.Inpulse, Spaceship.EngineTypes.Warp },
                Weapons = new List<Spaceship.WeaponTypes>() { Spaceship.WeaponTypes.Disruptor, Spaceship.WeaponTypes.PhotonTorpedos },
                Crew = Spaceship.CrewTypes.BadGuys,
                Name = "IKS Toh'Kaht (Klingon attack cruiser)",
                ImageUri = "https://upload.wikimedia.org/wikipedia/en/thumb/9/9f/Star_Trek_movie_logo_2009.jpg/310px-Star_Trek_movie_logo_2009.jpg"
            });
            Spaceships.Add(new Spaceship()
            {
                Size = Spaceship.Sizes.Large,
                Engines = new List<Spaceship.EngineTypes>() { Spaceship.EngineTypes.Inpulse, Spaceship.EngineTypes.Warp },
                Weapons = new List<Spaceship.WeaponTypes>() { Spaceship.WeaponTypes.Disruptor, Spaceship.WeaponTypes.PhotonTorpedos },
                Crew = Spaceship.CrewTypes.BadGuys,
                Name = "Scimitar (Reman warbird)",
                ImageUri = "https://upload.wikimedia.org/wikipedia/en/thumb/9/9f/Star_Trek_movie_logo_2009.jpg/310px-Star_Trek_movie_logo_2009.jpg"
            });

            // Star Wars
            Spaceships.Add(new Spaceship()
            {
                Size = Spaceship.Sizes.Small,
                Engines = new List<Spaceship.EngineTypes>() { Spaceship.EngineTypes.Sublight },
                Weapons = new List<Spaceship.WeaponTypes>() { Spaceship.WeaponTypes.Laser, Spaceship.WeaponTypes.ProtonTorpedos },
                Crew = Spaceship.CrewTypes.GoodGuys,
                Name = "X-Wing",
                ImageUri = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/6c/Star_Wars_Logo.svg/1280px-Star_Wars_Logo.svg.png"
            });
            Spaceships.Add(new Spaceship()
            {
                Size = Spaceship.Sizes.Mid,
                Engines = new List<Spaceship.EngineTypes>() { Spaceship.EngineTypes.Sublight, Spaceship.EngineTypes.Hyper },
                Weapons = new List<Spaceship.WeaponTypes>() { Spaceship.WeaponTypes.Laser, Spaceship.WeaponTypes.Missiles },
                Crew = Spaceship.CrewTypes.GoodGuys,
                Name = "Millenium Falcon (YT-1300)",
                ImageUri = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/6c/Star_Wars_Logo.svg/1280px-Star_Wars_Logo.svg.png"
            });
            Spaceships.Add(new Spaceship()
            {
                Size = Spaceship.Sizes.Large,
                Engines = new List<Spaceship.EngineTypes>() { Spaceship.EngineTypes.Sublight, Spaceship.EngineTypes.Hyper },
                Weapons = new List<Spaceship.WeaponTypes>() { Spaceship.WeaponTypes.Laser, Spaceship.WeaponTypes.Ion },
                Crew = Spaceship.CrewTypes.GoodGuys,
                Name = "Home One (MC80)",
                ImageUri = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/6c/Star_Wars_Logo.svg/1280px-Star_Wars_Logo.svg.png"
            });
            Spaceships.Add(new Spaceship()
            {
                Size = Spaceship.Sizes.Small,
                Engines = new List<Spaceship.EngineTypes>() { Spaceship.EngineTypes.Sublight },
                Weapons = new List<Spaceship.WeaponTypes>() { Spaceship.WeaponTypes.Laser, Spaceship.WeaponTypes.ProtonTorpedos },
                Crew = Spaceship.CrewTypes.BadGuys,
                Name = "Tie Fighter",
                ImageUri = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/6c/Star_Wars_Logo.svg/1280px-Star_Wars_Logo.svg.png"
            });
            Spaceships.Add(new Spaceship()
            {
                Size = Spaceship.Sizes.Small,
                Engines = new List<Spaceship.EngineTypes>() { Spaceship.EngineTypes.Sublight, Spaceship.EngineTypes.Hyper },
                Weapons = new List<Spaceship.WeaponTypes>()
                {
                    Spaceship.WeaponTypes.Laser,
                    Spaceship.WeaponTypes.Ion,
                    Spaceship.WeaponTypes.ProtonTorpedos,
                    Spaceship.WeaponTypes.Missiles
                },
                Crew = Spaceship.CrewTypes.BadGuys,
                Name = "Slave I (Firespray-31-class)",
                ImageUri = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/6c/Star_Wars_Logo.svg/1280px-Star_Wars_Logo.svg.png"
            });
            Spaceships.Add(new Spaceship()
            {
                Size = Spaceship.Sizes.Large,
                Engines = new List<Spaceship.EngineTypes>() { Spaceship.EngineTypes.Sublight, Spaceship.EngineTypes.Hyper },
                Weapons = new List<Spaceship.WeaponTypes>()
                {
                    Spaceship.WeaponTypes.Laser,
                    Spaceship.WeaponTypes.Ion,
                    Spaceship.WeaponTypes.Missiles
                },
                Crew = Spaceship.CrewTypes.BadGuys,
                Name = "Star Destroyer (Imperial-class)",
                ImageUri = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/6c/Star_Wars_Logo.svg/1280px-Star_Wars_Logo.svg.png"
            });

            // Battlestar Galactica
            Spaceships.Add(new Spaceship()
            {
                Size = Spaceship.Sizes.Small,
                Engines = new List<Spaceship.EngineTypes>() { Spaceship.EngineTypes.TurboThrust },
                Weapons = new List<Spaceship.WeaponTypes>() { Spaceship.WeaponTypes.KineticEnergy, Spaceship.WeaponTypes.Missiles },
                Crew = Spaceship.CrewTypes.GoodGuys,
                Name = "Viper Mk VII",
                ImageUri = "https://upload.wikimedia.org/wikipedia/en/4/40/Battlestar_Galactica_intro.jpg"
            });
            Spaceships.Add(new Spaceship()
            {
                Size = Spaceship.Sizes.Large,
                Engines = new List<Spaceship.EngineTypes>() { Spaceship.EngineTypes.Sublight, Spaceship.EngineTypes.FTL },
                Weapons = new List<Spaceship.WeaponTypes>() { Spaceship.WeaponTypes.Batteries, Spaceship.WeaponTypes.Missiles },
                Crew = Spaceship.CrewTypes.GoodGuys,
                Name = "Battlestar Galactica",
                ImageUri = "https://upload.wikimedia.org/wikipedia/en/4/40/Battlestar_Galactica_intro.jpg"
            });
            Spaceships.Add(new Spaceship()
            {
                Size = Spaceship.Sizes.Small,
                Engines = new List<Spaceship.EngineTypes>() { Spaceship.EngineTypes.Sublight },
                Weapons = new List<Spaceship.WeaponTypes>() { Spaceship.WeaponTypes.KineticEnergy, Spaceship.WeaponTypes.Missiles },
                Crew = Spaceship.CrewTypes.BadGuys,
                Name = "Cylon Raider",
                ImageUri = "https://upload.wikimedia.org/wikipedia/en/4/40/Battlestar_Galactica_intro.jpg"
            });
            Spaceships.Add(new Spaceship()
            {
                Size = Spaceship.Sizes.Large,
                Engines = new List<Spaceship.EngineTypes>() { Spaceship.EngineTypes.Sublight, Spaceship.EngineTypes.FTL },
                Weapons = new List<Spaceship.WeaponTypes>() { Spaceship.WeaponTypes.Missiles },
                Crew = Spaceship.CrewTypes.BadGuys,
                Name = "Cylon Basestar",
                ImageUri = "https://upload.wikimedia.org/wikipedia/en/4/40/Battlestar_Galactica_intro.jpg"
            });
        }
    }
}