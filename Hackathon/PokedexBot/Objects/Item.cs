using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;

namespace PokeAPI
{
    public class Item : NamedApiObject
    {
        public int Cost
        {
            get;
            internal set;
        }

        [JsonPropertyName("fling_power")]
        public int? FlingPower
        {
            get;
            internal set;
        }

        [JsonPropertyName("fling_effect")]
        public NamedApiResource<ItemFlingEffect> FlingEffect
        {
            get;
            internal set;
        }

        public NamedApiResource<ItemAttribute>[] Attributes
        {
            get;
            internal set;
        }

        public NamedApiResource<ItemCategory> Category
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

        [JsonPropertyName("flavor_text_entries")]
        public VersionGroupFlavorText[] FlavorTexts
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

        [JsonPropertyName("held_by_pokemon")]
        public ItemHeldBy[] HeldBy
        {
            get;
            internal set;
        }

        [JsonPropertyName("baby_trigger_for")]
        public ApiResource<EvolutionChain> BabyTriggerFor
        {
            get;
            internal set;
        }

        public ItemSprites Sprites
        {
            get;
            internal set;
        }

        public MachineVersionDetail[] Machines
        {
            get;
            internal set;
        }
    }

    public class ItemAttribute : NamedApiObject
    {
        public NamedApiResource<Item>[] Items
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

    public class ItemCategory : NamedApiObject
    {
        public NamedApiResource<Item>[] Items
        {
            get;
            internal set;
        }

        public NamedApiResource<ItemPocket> Pocket
        {
            get;
            internal set;
        }
    }

    public class ItemFlingEffect : NamedApiObject
    {
        [JsonPropertyName("effect_entries")]
        public Effect[] Effects
        {
            get;
            internal set;
        }

        public NamedApiResource<Item>[] Items
        {
            get;
            internal set;
        }
    }

    public class ItemPocket : NamedApiObject
    {
        public NamedApiResource<ItemCategory>[] Categories
        {
            get;
            internal set;
        }
    }

    public class ItemHeldBy
    {
        public NamedApiResource<Pokemon> Pokemon
        {
            get;
            internal set;
        }

        [JsonPropertyName("version_details")]
        public VersionDetails[] VersionDetails
        {
            get;
            internal set;
        }
    }

    public class VersionDetails
    {
        public int Rarity
        {
            get;
            internal set;
        }

        public NamedApiResource<GameVersion> Version
        {
            get;
            internal set;
        }
    }

    public class ItemSprites
    {
        public string Default { get; internal set; }
    }

    public class MachineVersionDetail
    {
        public ApiResource<Machine> Machine
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
