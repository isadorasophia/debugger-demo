using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;

namespace PokeAPI
{
    public struct PokemonEntry
    {
        [JsonPropertyName("entry_number")]
        public int EntryNumber
        {
            get;
            internal set;
        }
        [JsonPropertyName("pokemon_species")]
        public NamedApiResource<PokemonSpecies> Species
        {
            get;
            internal set;
        }
    }

    public class Generation : NamedApiObject
    {
        [JsonPropertyName("main_region")]
        public NamedApiResource<Region> MainRegion
        {
            get;
            internal set;
        }

        public NamedApiResource<Ability>[] Abilities
        {
            get;
            internal set;
        }

        public NamedApiResource<Move>[] Moves
        {
            get;
            internal set;
        }

        [JsonPropertyName("pokemon_species")]
        public NamedApiResource<PokemonSpecies>[] Species
        {
            get;
            internal set;
        }


        public NamedApiResource<PokemonType>[] Types
        {
            get;
            internal set;
        }

        [JsonPropertyName("version_groups")]
        public NamedApiResource<VersionGroup>[] VersionGroups
        {
            get;
            internal set;
        }
    }

    public class Pokedex : NamedApiObject
    {
        [JsonPropertyName("is_main_series")]
        public bool IsMainSeries
        {
            get;
            internal set;
        }

        public Description[] Descriptions
        {
            get;
            internal set;
        }

        [JsonPropertyName("pokemon_entries")]
        public PokemonEntry[] Entries
        {
            get;
            internal set;
        }

        public NamedApiResource<Region> Region
        {
            get;
            internal set;
        }

        [JsonPropertyName("version_groups")]
        public NamedApiResource<VersionGroup>[] VersionGroups
        {
            get;
            internal set;
        }
    }

    public class GameVersion : NamedApiObject
    {
        [JsonPropertyName("version_group")]
        public NamedApiResource<VersionGroup> VersionGroup
        {
            get;
            internal set;
        }
    }

    public class VersionGroup : NamedApiObject
    {
        public int Order
        {
            get;
            internal set;
        }

        public NamedApiResource<Generation> Generation
        {
            get;
            internal set;
        }

        [JsonPropertyName("move_learn_methods")]
        public NamedApiResource<MoveLearnMethod>[] MoveLearnMethods
        {
            get;
            internal set;
        }

        [JsonPropertyName("pokedexes")]
        public NamedApiResource<Pokedex>[] Pokedices
        {
            get;
            internal set;
        }

        public NamedApiResource<Region>[] Regions
        {
            get;
            internal set;
        }

        public NamedApiResource<GameVersion>[] Versions
        {
            get;
            internal set;
        }
    }
}
