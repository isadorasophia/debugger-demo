using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;

namespace PokeAPI
{
    public class Pokemon : NamedApiObject
    {
        [JsonPropertyName("base_experience")]
        public int BaseExperience
        {
            get;
            internal set;
        }

        [JsonPropertyName("is_default")]
        public bool IsDefault
        {
            get;
            internal set;
        }

        public int Height
        {
            get;
            internal set;
        }
        [JsonPropertyName("weight")]
        public int Mass
        {
            get;
            internal set;
        }

        public int Order
        {
            get;
            internal set;
        }

        public PokemonAbility[] Abilities
        {
            get;
            internal set;
        }

        public NamedApiResource<PokemonForm>[] Forms
        {
            get;
            internal set;
        }

        [JsonPropertyName("game_indices")]
        public VersionGameIndex[] GameIndices
        {
            get;
            internal set;
        }

        [JsonPropertyName("held_items")]
        public PokemonHeldItem[] HeldItems
        {
            get;
            internal set;
        }

        [JsonPropertyName("location_area_encounters"), JsonConverter(typeof(StructResourceFromStringConverter<LocationAreaEncounter[]>))]
        public StructResource<LocationAreaEncounter[]> LocationAreaEncounters
        {
            get;
            internal set;
        }

        public PokemonMove[] Moves
        {
            get;
            internal set;
        }

        public NamedApiResource<PokemonSpecies> Species
        {
            get;
            internal set;
        }

        public PokemonStats[] Stats
        {
            get;
            internal set;
        }

        public PokemonTypeMap[] Types
        {
            get;
            internal set;
        }

        /// <summary>
        /// NOTE: some props can be null, fall back on male, non-shiny (if all shinies are null) values!
        /// </summary>
        public PokemonSprites Sprites
        {
            get;
            internal set;
        }
    }

    public class PokemonColour : NamedApiObject
    {
        [JsonPropertyName("pokemon_species")]
        public NamedApiResource<PokemonSpecies>[] Species
        {
            get;
            internal set;
        }
    }

    public class PokemonForm : NamedApiObject
    {
        public int Order
        {
            get;
            internal set;
        }

        [JsonPropertyName("form_order")]
        public int FormOrder
        {
            get;
            internal set;
        }

        /// <summary>
        /// NOTE: some props can be null, fall back on male, non-shiny (if all shinies are null) values!
        /// </summary>
        public PokemonSprites Sprites
        {
            get;
            internal set;
        }

        [JsonPropertyName("is_default")]
        public bool IsDefault
        {
            get;
            internal set;
        }
        [JsonPropertyName("is_battle_only")]
        public bool IsBattleOnly
        {
            get;
            internal set;
        }
        [JsonPropertyName("is_mega")]
        public bool IsMegaEvolution
        {
            get;
            internal set;
        }

        [JsonPropertyName("form_name")]
        public string FormName
        {
            get;
            internal set;
        }

        [JsonPropertyName("form_names")]
        public ResourceName[] FormNames
        {
            get;
            internal set;
        }

        public NamedApiResource<Pokemon> Pokemon
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

    public class PokemonHabitat : NamedApiObject
    {
        [JsonPropertyName("pokemon_species")]
        public NamedApiResource<PokemonSpecies>[] Species
        {
            get;
            internal set;
        }
    }

    public class PokemonShape : NamedApiObject
    {
        public PokemonShape()
        {
            AwesomeNames = null;
            Species = null;
        }

        [JsonPropertyName("awesome_names")]
        public AwesomeName[] AwesomeNames
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

    public class PokemonSpecies : NamedApiObject
    {
        public int Order
        {
            get;
            internal set;
        }

        [JsonPropertyName("gender_rate"), JsonConverter(typeof(PokemonSpeciesGender.GenderConverter))]
        public float? FemaleToMaleRate
        {
            get;
            internal set;
        }
        [JsonPropertyName("capture_rate")]
        public float CaptureRate
        {
            get;
            internal set;
        }

        [JsonPropertyName("base_happiness")]
        public int BaseHappiness
        {
            get;
            internal set;
        }

        [JsonPropertyName("is_baby")]
        public bool IsBaby
        {
            get;
            internal set;
        }

        [JsonPropertyName("hatch_counter")]
        public int HatchCounter
        {
            get;
            internal set;
        }

        [JsonPropertyName("has_gender_differences")]
        public bool HasGenderDifferences
        {
            get;
            internal set;
        }

        [JsonPropertyName("forms_switchable")]
        public bool FormsAreSwitchable
        {
            get;
            internal set;
        }

        [JsonPropertyName("growth_rate")]
        public NamedApiResource<GrowthRate> GrowthRate
        {
            get;
            internal set;
        }

        [JsonPropertyName("pokedex_numbers")]
        public PokemonSpeciesDexEntry[] PokedexNumbers
        {
            get;
            internal set;
        }

        [JsonPropertyName("egg_groups")]
        public NamedApiResource<EggGroup>[] EggGroups
        {
            get;
            internal set;
        }

        [JsonPropertyName("color")]
        public NamedApiResource<PokemonColour> Colours
        {
            get;
            internal set;
        }
        public NamedApiResource<PokemonShape> Shape
        {
            get;
            internal set;
        }
        [JsonPropertyName("evolves_from_species")]
        public NamedApiResource<PokemonSpecies> EvolvesFromSpecies
        {
            get;
            internal set;
        }
        [JsonPropertyName("evolution_chain")]
        public ApiResource<EvolutionChain> EvolutionChain
        {
            get;
            internal set;
        }
        public NamedApiResource<PokemonHabitat> Habitat
        {
            get;
            internal set;
        }
        public NamedApiResource<Generation> Generation
        {
            get;
            internal set;
        }

        [JsonPropertyName("pal_park_encounters")]
        public PalParkEncounterArea[] PalParkEncounters
        {
            get;
            internal set;
        }

        [JsonPropertyName("form_descriptions")]
        public Description[] Descriptions
        {
            get;
            internal set;
        }

        public Genus[] Genera
        {
            get;
            internal set;
        }

        public PokemonSpeciesVariety[] Varieties
        {
            get;
            internal set;
        }

        [JsonPropertyName("flavor_text_entries")]
        public PokemonSpeciesFlavorText[] FlavorTexts
        {
            get;
            internal set;
        }
    }

    public class PokemonType : NamedApiObject
    {
        [JsonPropertyName("damage_relations")]
        public TypeRelations DamageRelations
        {
            get;
            internal set;
        }

        [JsonPropertyName("game_indices")]
        public GenerationGameIndex[] GameIndices
        {
            get;
            internal set;
        }

        public NamedApiResource<Generation> Generation
        {
            get;
            internal set;
        }

        [JsonPropertyName("move_damage_class")]
        public NamedApiResource<MoveDamageClass> MoveDamageClass
        {
            get;
            internal set;
        }

        public TypePokemon[] Pokemon
        {
            get;
            internal set;
        }

        public NamedApiResource<Move>[] Moves
        {
            get;
            internal set;
        }

        public static double CalculateDamageMultiplier(PokemonType attackingType, PokemonType defendingType)
        {
            var ad = attackingType.DamageRelations;

            if (ad.NoDamageTo    .Any(t => t.Name == defendingType.Name))
                return 0d;
            if (ad.HalfDamageTo  .Any(t => t.Name == defendingType.Name))
                return 0.5d;
            if (ad.DoubleDamageTo.Any(t => t.Name == defendingType.Name))
                return 2.0d;

            return 1d;
        }
        public static double CalculateDamageMultiplier(PokemonType attackingType, PokemonType defendingA, PokemonType defendingB)
        {
            return CalculateDamageMultiplier(attackingType, defendingA)
                 * CalculateDamageMultiplier(attackingType, defendingB);
        }
    }

    public class Stat : NamedApiObject
    {
        [JsonPropertyName("game_index")]
        public int GameIndex
        {
            get;
            internal set;
        }

        [JsonPropertyName("is_battle_only")]
        public bool IsBattleOnly
        {
            get;
            internal set;
        }

        [JsonPropertyName("affecting_moves")]
        public StatAffectSets<Move> AffectingMoves
        {
            get;
            internal set;
        }
        [JsonPropertyName("affecting_natures")]
        public StatAffectNature AffectingNatures
        {
            get;
            internal set;
        }

        public ApiResource<Characteristic>[] Characteristics
        {
            get;
            internal set;
        }

        [JsonPropertyName("move_damage_class")]
        public NamedApiResource<MoveDamageClass> MoveDamageClass
        {
            get;
            internal set;
        }
    }
}
