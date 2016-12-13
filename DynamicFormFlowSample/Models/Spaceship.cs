using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;

namespace DynamicFormFlowSample.Models
{
    [Serializable]
    public class Spaceship
    {
        public enum Sizes
        {
            NotDefined,
            Small,
            Mid,
            Large
        }

        public enum EngineTypes
        {
            NotDefined,
            Inpulse,
            Warp,
            Sublight,
            Hyper,
            TurboThrust,
            FTL
        }

        public enum WeaponTypes
        {
            NotDefined,
            None,
            Phaser,
            Disruptor,
            PhotonTorpedos,
            Laser,
            Ion,
            ProtonTorpedos,
            KineticEnergy,
            Batteries,
            Missiles
        }

        public enum CrewTypes
        {
            NotDefined,
            GoodGuys,
            BadGuys
        }

        public List<EngineTypes> Engines
        {
            get;
            set;
        }

        public List<WeaponTypes> Weapons
        {
            get;
            set;
        }

        [Prompt("What size is the ship? {||}")]
        public Sizes Size
        {
            get;
            set;
        }

        /// <summary>
        /// Additional property added to allow the selection of just one weapon
        /// (instead of allowing multiple).
        /// </summary>
        public WeaponTypes Weapon
        {
            get
            {
                if (Weapons.Count > 0)
                {
                    return Weapons[0];
                }

                return WeaponTypes.NotDefined;
            }
            set
            {
                AddWeapon(value);
            }
        }

        [Optional] // Optional will add "No preference" as an option
        public CrewTypes Crew
        {
            get;
            set;
        }

        [Optional]
        public string Name
        {
            get;
            set;
        }

        public string ImageUri
        {
            get;
            set;
        }

        public Spaceship()
        {
            Reset();
        }

        public Spaceship(Spaceship other)
        {
            Reset();

            if (other != null)
            {
                Size = other.Size;
                Engines = other.Engines;
                Weapons = other.Weapons;
                Crew = other.Crew;
                Name = other.Name;
            }
        }

        public void AddEngine(EngineTypes engineType)
        {
            if (!Engines.Contains(engineType))
            {
                Engines.Add(engineType);
            }
        }

        public void AddWeapon(WeaponTypes weaponType)
        {
            if (!Weapons.Contains(weaponType))
            {
                Weapons.Add(weaponType);
            }
        }

        /// <summary>
        /// Tries to cast the given value to its proper type and verify its value based on
        /// the given property name.
        /// </summary>
        /// <param name="propertyValue">The value to cast.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value with a confirmed type and value (although return as object)
        /// or null in case of a failure.</returns>
        public static object VerifyPropertyValue(object propertyValue, string propertyName)
        {
            if (propertyValue is IList<object>)
            {
                // We do not handle lists here
                return propertyValue;
            }

            object verifiedValue = null;
            string propertyValueAsString = (propertyValue != null) ? propertyValue.ToString() : "NotDefined";

            try
            {
                switch (propertyName)
                {
                    case nameof(Size):
                        verifiedValue = (Sizes)Enum.Parse(typeof(Sizes), propertyValueAsString, true);
                        break;
                    case nameof(Engines):
                        verifiedValue = (EngineTypes)Enum.Parse(typeof(EngineTypes), propertyValueAsString, true);
                        break;
                    case nameof(Weapons):
                        verifiedValue = (WeaponTypes)Enum.Parse(typeof(WeaponTypes), propertyValueAsString, true);
                        break;
                    case nameof(Crew):
                        verifiedValue = (CrewTypes)Enum.Parse(typeof(CrewTypes), propertyValueAsString, true);
                        break;
                    case nameof(Name):
                        verifiedValue = propertyValueAsString;
                        break;
                }
            }
            catch (ArgumentException e)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to the property value: {e.Message}");
                verifiedValue = null;
            }
            catch (OverflowException e)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to the property value: {e.Message}");
                verifiedValue = null;
            }

            return verifiedValue;
        }

        /// <summary>
        /// Sets the given value to the property (with the given name) of this instance.
        /// </summary>
        /// <param name="propertyValue">The value to set.</param>
        /// <param name="propertyName">The name of the property whose value to set.</param>
        /// <returns>True, if the value was set successfully. False otherwise.</returns>
        public bool SetPropertyValue(object propertyValue, string propertyName)
        {
            bool isList = (propertyValue is IList<object>);
            object verifiedValue = VerifyPropertyValue(propertyValue, propertyName);

            if (verifiedValue != null)
            {
                switch (propertyName)
                {
                    case nameof(Size):
                        Size = (Sizes)verifiedValue;
                        break;
                    case nameof(Engines):
                        if (isList)
                        {
                            foreach (object value in (propertyValue as IList<object>))
                            {
                                try
                                {
                                    AddEngine((EngineTypes)value);
                                }
                                catch (InvalidCastException e)
                                {
                                    System.Diagnostics.Debug.WriteLine("Failed to add engine: " + e.Message);
                                }
                            }
                        }
                        else
                        {
                            AddEngine((EngineTypes)verifiedValue);
                        }

                        break;
                    case nameof(Weapons):
                        if (isList)
                        {
                            foreach (object value in (propertyValue as IList<object>))
                            {
                                try
                                {
                                    AddWeapon((WeaponTypes)value);
                                }
                                catch (InvalidCastException e)
                                {
                                    System.Diagnostics.Debug.WriteLine("Failed to add weapon: " + e.Message);
                                }
                            }
                        }
                        else
                        {
                            AddWeapon((WeaponTypes)verifiedValue);
                        }

                        break;
                    case nameof(Crew):
                        Crew = (CrewTypes)verifiedValue;
                        break;
                    case nameof(Name):
                        Name = (string)verifiedValue;
                        break;
                }

            }

            return (verifiedValue != null);
        }

        /// <summary>
        /// Clears the value of the given property.
        /// </summary>
        /// <param name="propertyName">The name of the property whose value to clear.</param>
        public void ClearPropertyValue(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Size):
                    Size = Sizes.NotDefined;
                    break;
                case nameof(Engines):
                    Engines.Clear();
                    break;
                case nameof(Weapons):
                    Weapons.Clear();
                    break;
                case nameof(Crew):
                    Crew = CrewTypes.NotDefined;
                    break;
                case nameof(Name):
                    Name = string.Empty;
                    break;
            }
        }

        /// <summary>
        /// Checks if the given spaceship is partial match to this one.
        /// Any differences (other than properties not defined) will be considered mismatch.
        /// </summary>
        /// <param name="other">The spaceship to compare to this one.</param>
        /// <returns>True, if the spaceships are a partial match. False otherwise.</returns>
        public bool IsPartialMatch(Spaceship other)
        {
            if (other == null)
            {
                return false;
            }

            if (other.Size != Sizes.NotDefined && other.Size != Size)
            {
                return false;
            }

            foreach (EngineTypes engineType in other.Engines)
            {
                if (!Engines.Contains(engineType))
                {
                    return false;
                }
            }

            foreach (WeaponTypes weaponType in other.Weapons)
            {
                if (!Weapons.Contains(weaponType))
                {
                    return false;
                }
            }

            if (other.Crew != CrewTypes.NotDefined && other.Crew != Crew)
            {
                return false;
            }

            return true;
        }

        public string PropertiesAsFormattedString()
        {
            string retval = $"Size: {Size.ToString()}\r\nEngines: ";

            if (Engines.Count == 0)
            {
                retval += "N/A";
            }
            else
            {
                for (int i = 0; i < Engines.Count; ++i)
                {
                    retval += Engines[i].ToString();

                    if (i < Engines.Count - 1)
                    {
                        retval += ", ";
                    }
                }
            }

            retval += "\r\nWeapons: ";

            if (Weapons.Count == 0)
            {
                retval += "N/A";
            }
            else
            {
                for (int i = 0; i < Weapons.Count; ++i)
                {
                    retval += Weapons[i].ToString();

                    if (i < Weapons.Count - 1)
                    {
                        retval += ", ";
                    }
                }
            }

            retval += $"\r\nCrew: {Crew.ToString()}";
            return retval;
        }

        public override string ToString()
        {
            string retval = "[\"" + Name + "\"; " + Size + "; ";

            for (int i = 0; i < Engines.Count; ++i)
            {
                if (i == 0)
                {
                    retval += "[";
                }

                retval += Engines[i].ToString();

                if (i < Engines.Count - 1)
                {
                    retval += "; ";
                }
                else
                {
                    retval += "]; ";
                }
            }

            for (int i = 0; i < Weapons.Count; ++i)
            {
                if (i == 0)
                {
                    retval += "[";
                }

                retval += Weapons[i].ToString();

                if (i < Weapons.Count - 1)
                {
                    retval += "; ";
                }
                else
                {
                    retval += "]; ";
                }
            }

            retval += Crew + "]";
            return retval;
        }

        private void Reset()
        {
            Size = Sizes.NotDefined;
            Engines = new List<EngineTypes>();
            Weapons = new List<WeaponTypes>();
            Crew = CrewTypes.NotDefined;
            Name = string.Empty;
        }
    }
}