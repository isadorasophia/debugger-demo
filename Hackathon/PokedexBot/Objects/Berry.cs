using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;

namespace PokeAPI
{
    public struct BerryFlavorMap
    {
        /// <summary>
        /// The power of the referenced flavor for this berry.
        /// </summary>
        public int Potency
        {
            get;
            internal set;
        }

        /// <summary>
        /// The referenced berry flavor.
        /// </summary>
        public NamedApiResource<BerryFlavor> Flavor
        {
            get;
            internal set;
        }
    }
    public struct FlavorBerryMap
    {
        /// <summary>
        /// The power of the flavor for the referenced flavor.
        /// </summary>
        public int Potency
        {
            get;
            internal set;
        }

        /// <summary>
        /// The referenced berry.
        /// </summary>
        public NamedApiResource<Berry> Berry
        {
            get;
            internal set;
        }
    }

    public class Berry : NamedApiObject
    {
        class BerryGrowthTimeConverter : IJsonConverter
        {
            public bool Deserialize(JsonData j, Type t /* TimeSpan */, out object value)
            {
                value = null;

                if (j.JsonType != JsonType.Int)
                    return false;

                value = new TimeSpan((int)j, 0, 0);
                return true;
            }
        }

        /// <summary>
        /// Time it takes the tree to grow one stage. Berry trees go through four of these growth stages before they can be picked.
        /// </summary>
        [JsonPropertyName("growth_time"), JsonConverter(typeof(BerryGrowthTimeConverter))]
        public TimeSpan GrowthTime
        {
            get;
            internal set;
        }

        /// <summary>
        /// The maximum number of these berries that can grow on one tree in Generation IV.
        /// </summary>
        [JsonPropertyName("max_harvest")]
        public int MaxHarvest
        {
            get;
            internal set;
        }
        /// <summary>
        /// The power of the move "Natural Gift" when used with this Berry.
        /// </summary>
        [JsonPropertyName("natural_gift_power")]
        public int NaturalGiftPower
        {
            get;
            internal set;
        }
        /// <summary>
        /// The size of this Berry, in millimeters.
        /// </summary>
        public int Size
        {
            get;
            internal set;
        }
        /// <summary>
        /// The smoothness of this Berry, used in making Pokéblocks or Poffins.
        /// </summary>
        public int Smoothness
        {
            get;
            internal set;
        }
        /// <summary>
        /// The speed at which this Berry dries out the soil as it grows. A higher rate means the soil dries more quickly.
        /// </summary>
        [JsonPropertyName("soil_dryness")]
        public int SoilDryness
        {
            get;
            internal set;
        }

        /// <summary>
        /// The firmness of this berry, used in making Pokéblocks or Poffins.
        /// </summary>
        public NamedApiResource<BerryFirmness> Firmness
        {
            get;
            internal set;
        }

        /// <summary>
        /// A list of references to each flavor a berry can have and the potency of each of those flavors in regard to this berry.
        /// </summary>
        public BerryFlavorMap[] Flavors
        {
            get;
            internal set;
        }

        /// <summary>
        ///  	Berries are actually items. This is a reference to the item specific data for this berry.
        /// </summary>
        public NamedApiResource<Item> Item
        {
            get;
            internal set;
        }

        /// <summary>
        /// The Type the move "Natural Gift" has when used with this Berry.
        /// </summary>
        [JsonPropertyName("natural_gift_type")]
        public NamedApiResource<PokemonType> NaturalGiftType
        {
            get;
            internal set;
        }
    }

    public class BerryFirmness : NamedApiObject
    {
        /// <summary>
        /// A list of the berries with this firmness.
        /// </summary>
        public NamedApiResource<Berry>[] Berries
        {
            get;
            internal set;
        }
    }

    public class BerryFlavor : NamedApiObject
    {
        /// <summary>
        /// A list of the berries with this flavor.
        /// </summary>
        public FlavorBerryMap[] Berries
        {
            get;
            internal set;
        }

        /// <summary>
        /// The contest type that correlates with this berry flavor.
        /// </summary>
        [JsonPropertyName("contest_type")]
        public NamedApiResource<ContestType> ContestType
        {
            get;
            internal set;
        }
    }
}
