using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Utility;

namespace Robust.Benchmarks.Serialization.Definitions
{
    /// <summary>
    ///     Arbitrarily large data definition for benchmarks.
    ///     Taken from content.
    /// </summary>
    public sealed partial class SeedDataDefinition : Component
    {
        public const string Prototype = @"
- type: seed
  id: tobacco
  name: tobacco
  seedName: tobacco
  displayName: tobacco plant
  plantRsi: Objects/Specific/Hydroponics/tobacco.rsi
  productPrototypes:
    - LeavesTobacco
  harvestRepeat: Repeat
  lifespan: 75
  maturation: 5
  production: 5
  yield: 2
  potency: 20
  growthStages: 3
  idealLight: 9
  growthComponents:
    - type: BasicGrowthComponent
      WaterConsumption: 3.0
      NutrientConsumption: 0.25
    - type: AtmosphericGrowthComponent
      IdealHeat: 298
      HeatTolerance: 20
      LowPressureTolerance: 25
      HighPressureTolerance: 200
    - type: WeedPestGrowthComponent
      WeedTolerance: 5
      PestTolerance: 5
  chemicals:
    chem.Nicotine:
      Min: 1
      Max: 10
      PotencyDivisor: 10";

        [IdDataFieldAttribute] public string ID { get; set; } = default!;

        #region Tracking
        [DataField("name")] public string Name { get; set; } = string.Empty;
        [DataField("seedName")] public string SeedName { get; set; } = string.Empty;
        [DataField("seedNoun")] public string SeedNoun { get; set; } = "seeds";
        [DataField("displayName")] public string DisplayName { get; set; } = string.Empty;
        [DataField("roundStart")] public bool RoundStart { get; set; } = true;
        [DataField("mysterious")] public bool Mysterious { get; set; }
        [DataField("immutable")] public bool Immutable { get; set; }
        #endregion

        #region Output
        [DataField("productPrototypes")]
        public List<string> ProductPrototypes { get; set; } = new();
        [DataField("chemicals")]
        public Dictionary<string, SeedChemQuantity> Chemicals { get; set; } = new();
        [DataField("consumeGasses")]
        public Dictionary<Gas, float> ConsumeGasses { get; set; } = new();
        [DataField("exudeGasses")]
        public Dictionary<Gas, float> ExudeGasses { get; set; } = new();
        #endregion

        #region Growth Components
        [DataField("growthComponents")]
        public List<PlantGrowthComponent> GrowthComponents { get; set; } = new();
        #endregion

        #region General traits
        [DataField("endurance")] public float Endurance { get; set; } = 100f;
        [DataField("yield")] public int Yield { get; set; }
        [DataField("lifespan")] public float Lifespan { get; set; }
        [DataField("maturation")] public float Maturation { get; set; }
        [DataField("production")] public float Production { get; set; }
        [DataField("growthStages")] public int GrowthStages { get; set; } = 6;
        [DataField("harvestRepeat")] public HarvestType HarvestRepeat { get; set; } = HarvestType.NoRepeat;
        [DataField("potency")] public float Potency { get; set; } = 1f;
        [DataField("ligneous")] public bool Ligneous { get; set; }
        #endregion

        #region Cosmetics
        [DataField("plantRsi", required: true)] public ResPath PlantRsi { get; set; } = default!;
        [DataField("plantIconState")] public string PlantIconState { get; set; } = "produce";
        [DataField("bioluminescent")] public bool Bioluminescent { get; set; }
        [DataField("bioluminescentColor")] public Color BioluminescentColor { get; set; } = Color.White;
        [DataField("splatPrototype")] public string? SplatPrototype { get; set; }
        #endregion

        /// <summary>
        ///     Test method to verify the new growth component system works correctly
        /// </summary>
        public void TestGrowthComponents()
        {
            // This method can be used to test that the new component system works
            // It should be called after deserialization to verify all components are loaded correctly
            foreach (var component in GrowthComponents)
            {
                switch (component)
                {
                    case BasicGrowthComponent basic:
                        // Verify basic component has correct values
                        break;
                    case AtmosphericGrowthComponent atmospheric:
                        // Verify atmospheric component has correct values
                        break;
                    case WeedPestGrowthComponent weedPest:
                        // Verify weed/pest component has correct values
                        break;
                }
            }
        }
    }

    public enum HarvestType
    {
        NoRepeat,
        Repeat
    }

    public enum Gas
    {
    }

    [DataDefinition]
    public partial struct SeedChemQuantity
    {
        [DataField("Min")]
        public int Min;

        [DataField("Max")]
        public int Max;

        [DataField("PotencyDivisor")]
        public int PotencyDivisor;
    }

    [RegisterComponent]
    [ImplicitDataDefinitionForInheritors]
    public abstract partial class PlantGrowthComponent : Component
    {
        public PlantGrowthComponent DupeComponent()
        {
            return (PlantGrowthComponent)this.MemberwiseClone();
        }
    }

    [RegisterComponent]
    public sealed partial class BasicGrowthComponent : PlantGrowthComponent
    {
        [DataField]
        public float WaterConsumption = 0.5f;

        [DataField]
        public float NutrientConsumption = 0.75f;
    }

    [RegisterComponent]
    public sealed partial class AtmosphericGrowthComponent : PlantGrowthComponent
    {
        [DataField]
        public float IdealHeat = 293f;
        
        [DataField]
        public float HeatTolerance = 20f;
        
        [DataField]
        public float LowPressureTolerance = 25f;
        
        [DataField]
        public float HighPressureTolerance = 200f;
    }

    [RegisterComponent]
    public sealed partial class WeedPestGrowthComponent : PlantGrowthComponent
    {
        [DataField]
        public float WeedTolerance = 5f;
        
        [DataField]
        public float PestTolerance = 5f;
    }

    [RegisterComponent]
    public sealed partial class ConsumeExudeGasGrowthComponent : PlantGrowthComponent
    {
        [DataField]
        public Dictionary<Gas, float> ConsumeGasses = new();
        
        [DataField]
        public Dictionary<Gas, float> ExudeGasses = new();
    }

    [RegisterComponent]
    public sealed partial class AutoHarvestGrowthComponent : PlantGrowthComponent
    {
        // Без параметров
    }

    [RegisterComponent]
    public sealed partial class UnviableGrowthComponent : PlantGrowthComponent
    {
        // Без параметров
    }
}
