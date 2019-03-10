using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;

namespace PokeAPI
{
    public struct EvolutionDetail
    {
        public NamedApiResource<Item> Item
        {
            get;
            internal set;
        }

        public NamedApiResource<EvolutionTrigger> Trigger
        {
            get;
            internal set;
        }

        public int? Gender
        {
            get;
            internal set;
        }

        [JsonPropertyName("held_item")]
        public NamedApiResource<Item> HeldItem
        {
            get;
            internal set;
        }

        [JsonPropertyName("known_move")]
        public NamedApiResource<Move> KnownMove
        {
            get;
            internal set;
        }

        [JsonPropertyName("known_move_type")]
        public NamedApiResource<PokemonType> KnownMoveType
        {
            get;
            internal set;
        }

        public NamedApiResource<Location> Location
        {
            get;
            internal set;
        }

        [JsonPropertyName("min_level")]
        public int? MinLevel
        {
            get;
            internal set;
        }
        [JsonPropertyName("min_happiness")]
        public int? MinHappiness
        {
            get;
            internal set;
        }
        [JsonPropertyName("min_beauty")]
        public int? MinBeauty
        {
            get;
            internal set;
        }
        [JsonPropertyName("min_affection")]
        public int? MinAffection
        {
            get;
            internal set;
        }
        [JsonPropertyName("needs_overworld_rain")]
        public bool NeedsOverworldRain
        {
            get;
            internal set;
        }

        [JsonPropertyName("party_species")]
        public NamedApiResource<PokemonSpecies> PartySpecies
        {
            get;
            internal set;
        }

        [JsonPropertyName("party_type")]
        public NamedApiResource<PokemonType> PartyType
        {
            get;
            internal set;
        }

        [JsonPropertyName("relative_physical_stats")]
        public int? RelativePhysicalStats
        {
            get;
            internal set;
        }

        [JsonPropertyName("time_of_day")]
        public string TimeOfDay
        {
            get;
            internal set;
        }

        [JsonPropertyName("trade_species")]
        public NamedApiResource<PokemonSpecies> TradeSpecies
        {
            get;
            internal set;
        }

        [JsonPropertyName("turn_upside_down")]
        public bool TurnUpsideDown
        {
            get;
            internal set;
        }
    }

    public struct ChainLink
    {
        [JsonPropertyName("is_baby")]
        public bool IsBaby
        {
            get;
            internal set;
        }

        public NamedApiResource<PokemonSpecies> Species
        {
            get;
            internal set;
        }

        [JsonPropertyName("evolution_details")]
        public EvolutionDetail[] Details
        {
            get;
            internal set;
        }

        [JsonPropertyName("evolves_to")]
        public ChainLink[] EvolvesTo
        {
            get;
            internal set;
        }
    }

    public class EvolutionChain : ApiObject
    {
        [JsonPropertyName("baby_trigger_item")]
        public NamedApiResource<Item> BabyTriggerItem
        {
            get;
            internal set;
        }

        public ChainLink Chain
        {
            get;
            internal set;
        }
    }

    public class EvolutionTrigger : NamedApiObject
    {
        [JsonPropertyName("pokemon_species")]
        public NamedApiResource<PokemonSpecies>[] Species
        {
            get;
            internal set;
        }
    }
}
