using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;

namespace PokeAPI
{
    public class Ability : NamedApiObject
    {
        [JsonPropertyName("is_main_series")]
        public bool IsMainSeries
        {
            get;
            internal set;
        }

        public NamedApiResource<Generation> Generation
        {
            get;
            internal set;
        }

        [JsonPropertyName("effect_entries")]
        public VerboseEffect[] Effects
        {
            get;
            internal set;
        }

        [JsonPropertyName("effect_changes")]
        public AbilityEffectChange[] EffectChanges
        {
            get;
            internal set;
        }

        [JsonPropertyName("flavor_text_entries")]
        public VersionGroupFlavorText[] FlavorTexts
        {
            get;
            internal set;
        }

        public AbilityPokemon[] Pokemon
        {
            get;
            internal set;
        }
    }

    public class Characteristic : ApiObject
    {

        [JsonPropertyName("highest_stat")]
        public NamedApiResource<Stat> HighestStat
        {
            get;
            internal set;
        }

        [JsonPropertyName("gene_modulo")]
        public int GeneModulo
        {
            get;
            internal set;
        }

        [JsonPropertyName("possible_values")]
        public int[] PossibleValues
        {
            get;
            internal set;
        }

        public Description[] Descriptions
        {
            get;
            internal set;
        }
    }

    public class EggGroup : NamedApiObject
    {
        [JsonPropertyName("pokemon_species")]
        public NamedApiResource<PokemonSpecies>[] Species
        {
            get;
            internal set;
        }
    }

    public class Gender : NamedApiObject
    {
        [JsonPropertyName("pokemon_species_details")]
        public PokemonSpeciesGender[] SpeciesDetails
        {
            get;
            internal set;
        }

        [JsonPropertyName("required_for_evolution")]
        public NamedApiResource<PokemonSpecies>[] RequiredForEvolution
        {
            get;
            internal set;
        }
    }

    public class GrowthRate : NamedApiObject
    {
        /// <summary>
        /// LaTeX-style (maths mode)
        /// </summary>
        public string Formula
        {
            get;
            internal set;
        }

        public Description[] Descriptions
        {
            get;
            internal set;
        }

        public GrowthRateExperienceLevel[] Levels
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
    }

    public class Nature : NamedApiObject
    {
        [JsonPropertyName("decreased_stat")]
        public NamedApiResource<Stat> DecreasedStat
        {
            get;
            internal set;
        }
        [JsonPropertyName("increased_stat")]
        public NamedApiResource<Stat> IncreasedStat
        {
            get;
            internal set;
        }

        [JsonPropertyName("hates_flavor")]
        public NamedApiResource<BerryFlavor> HatesFlavor
        {
            get;
            internal set;
        }
        [JsonPropertyName("likes_flavor")]
        public NamedApiResource<BerryFlavor> LikesFlavor
        {
            get;
            internal set;
        }

        [JsonPropertyName("pokeathlon_stat_changes")]
        public NatureStatChange[] PokeathlonStatChanges
        {
            get;
            internal set;
        }

        [JsonPropertyName("move_battle_style_preferences")]
        public MoveBattleStylePreference[] BattleStylePreferences
        {
            get;
            internal set;
        }
    }

    public class PokeathlonStat : NamedApiObject
    {
        [JsonPropertyName("affecting_natures")]
        public NaturePokeathlonStatAffectSets AffectingNatures
        {
            get;
            internal set;
        }
    }

    public class Machine : ApiObject
    {
        public NamedApiResource<Item> Item
        {
            get;
            internal set;
        }
        public NamedApiResource<Move> Move
        {
            get;
            internal set;
        }
        [JsonPropertyName("version_group")]
        public NamedApiResource<VersionGroup> VersionGroup
        {
            get;
            internal set;
        }
    }
}
