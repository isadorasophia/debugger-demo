using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;

namespace PokeAPI
{
    public struct ContestComboDetail
    {
        [JsonPropertyName("use_before")]
        public NamedApiResource<Move>[] UseBefore
        {
            get;
            internal set;
        }
        [JsonPropertyName("use_after")]
        public NamedApiResource<Move>[] UseAfter
        {
            get;
            internal set;
        }
    }
    public struct ContestComboSet
    {
        public ContestComboDetail Normal
        {
            get;
            internal set;
        }
        public ContestComboDetail Super
        {
            get;
            internal set;
        }
    }

    public struct MoveMetadata
    {
        public NamedApiResource<MoveAilment> Ailment
        {
            get;
            internal set;
        }
        public NamedApiResource<MoveCategory> Category
        {
            get;
            internal set;
        }

        [JsonPropertyName("min_hits")]
        public int? MinHits
        {
            get;
            internal set;
        }
        [JsonPropertyName("max_hits")]
        public int? MaxHits
        {
            get;
            internal set;
        }
        [JsonPropertyName("min_turns")]
        public int? MinTurns
        {
            get;
            internal set;
        }
        [JsonPropertyName("max_turns")]
        public int? MaxTurns
        {
            get;
            internal set;
        }

        [JsonPropertyName("drain")]
        public int DrainRecoil
        {
            get;
            internal set;
        }
        public int Healing
        {
            get;
            internal set;
        }

        [JsonPropertyName("crit_rate")]
        public int CritRate
        {
            get;
            internal set;
        }
        [JsonPropertyName("ailment_chance")]
        public float AilmentChance
        {
            get;
            internal set;
        }
        [JsonPropertyName("flinch_chance")]
        public float FlinchChance
        {
            get;
            internal set;
        }
        [JsonPropertyName("stat_chance")]
        public int StatChance
        {
            get;
            internal set;
        }
    }
    public struct MoveStatChange
    {
        public int Change
        {
            get;
            internal set;
        }

        public NamedApiResource<Stat> Stat
        {
            get;
            internal set;
        }
    }

    public struct PastMoveStatValue
    {
        public float? Accuracy
        {
            get;
            internal set;
        }
        [JsonPropertyName("effect_chance")]
        public float? EffectChance
        {
            get;
            internal set;
        }

        public int? Power
        {
            get;
            internal set;
        }
        public int? PP
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

        public NamedApiResource<PokemonType> Type
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

    public class Move : NamedApiObject
    {
        public float? Accuracy
        {
            get;
            internal set;
        }
        [JsonPropertyName("effect_chance")]
        public float? EffectChance
        {
            get;
            internal set;
        }

        public int? PP
        {
            get;
            internal set;
        }
        public int Priority
        {
            get;
            internal set;
        }
        public int? Power
        {
            get;
            internal set;
        }

        [JsonPropertyName("contest_combos")]
        public ContestComboSet? ComboSets
        {
            get;
            internal set;
        }

        [JsonPropertyName("contest_type")]
        public NamedApiResource<ContestType> ContestType
        {
            get;
            internal set;
        }

        [JsonPropertyName("contest_effect")]
        public ApiResource<ContestEffect> ContestEffect
        {
            get;
            internal set;
        }

        [JsonPropertyName("damage_class")]
        public NamedApiResource<MoveDamageClass> DamageClass
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

        public NamedApiResource<Generation> Generation
        {
            get;
            internal set;
        }

        public MoveMetadata? Meta
        {
            get;
            internal set;
        }

        [JsonPropertyName("past_values")]
        public PastMoveStatValue[] PastValues
        {
            get;
            internal set;
        }

        [JsonPropertyName("stat_changes")]
        public MoveStatChange[] StatChanges
        {
            get;
            internal set;
        }

        public NamedApiResource<MoveTarget> Target
        {
            get;
            internal set;
        }

        public NamedApiResource<PokemonType> Type
        {
            get;
            internal set;
        }

        [JsonPropertyName("super_contest_effect")]
        public ApiResource<SuperContestEffect> SuperContestEffect
        {
            get;
            internal set;
        }

        public MachineVersionDetail[] Machines
        {
            get;
            internal set;
        }
        [JsonPropertyName("flavor_text_entries")]
        public VersionGroupFlavorText[] FlavorTextEntries
        {
            get;
            internal set;
        }
    }

    public class MoveAilment : NamedApiObject
    {
        public NamedApiResource<Move>[] Moves
        {
            get;
            internal set;
        }
    }

    public class MoveBattleStyle : NamedApiObject
    {
    }

    public class MoveCategory : NamedApiObject
    {
        public NamedApiResource<Move>[] Moves
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

    public class MoveDamageClass : NamedApiObject
    {
        public Description[] Descriptions
        {
            get;
            internal set;
        }

        public NamedApiResource<Move>[] Moves
        {
            get;
            internal set;
        }
    }

    public class MoveLearnMethod : NamedApiObject
    {
        public Description[] Descriptions
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

    public class MoveTarget : NamedApiObject
    {
        public Description[] Descriptions
        {
            get;
            internal set;
        }

        public NamedApiResource<Move>[] Moves
        {
            get;
            internal set;
        }
    }
}
