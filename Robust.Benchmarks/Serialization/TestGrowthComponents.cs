using System.IO;
using Robust.Benchmarks.Serialization.Definitions;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using YamlDotNet.RepresentationModel;

namespace Robust.Benchmarks.Serialization
{
    /// <summary>
    ///     Simple test to verify the new growth component system works correctly
    /// </summary>
    public static class TestGrowthComponents
    {
        public static void TestNewFormat()
        {
            // Test that the new YAML format can be deserialized correctly
            var yamlStream = new YamlStream();
            yamlStream.Load(new StringReader(SeedDataDefinition.Prototype));

            var seedMapping = yamlStream.Documents[0].RootNode.ToDataNodeCast<SequenceDataNode>().Cast<MappingDataNode>(0);

            // This should not throw an exception with the new format
            var seed = SerializationManager.Read<SeedDataDefinition>(seedMapping, notNullableOverride: true);

            // Verify that growth components were loaded correctly
            if (seed.GrowthComponents.Count == 0)
            {
                throw new System.Exception("No growth components were loaded!");
            }

            // Verify each component type
            bool hasBasic = false;
            bool hasAtmospheric = false;
            bool hasWeedPest = false;

            foreach (var component in seed.GrowthComponents)
            {
                switch (component)
                {
                    case BasicGrowthComponent basic:
                        hasBasic = true;
                        if (basic.WaterConsumption != 3.0f || basic.NutrientConsumption != 0.25f)
                        {
                            throw new System.Exception($"BasicGrowthComponent has incorrect values: WaterConsumption={basic.WaterConsumption}, NutrientConsumption={basic.NutrientConsumption}");
                        }
                        break;
                    case AtmosphericGrowthComponent atmospheric:
                        hasAtmospheric = true;
                        if (atmospheric.IdealHeat != 298f || atmospheric.HeatTolerance != 20f)
                        {
                            throw new System.Exception($"AtmosphericGrowthComponent has incorrect values: IdealHeat={atmospheric.IdealHeat}, HeatTolerance={atmospheric.HeatTolerance}");
                        }
                        break;
                    case WeedPestGrowthComponent weedPest:
                        hasWeedPest = true;
                        if (weedPest.WeedTolerance != 5f || weedPest.PestTolerance != 5f)
                        {
                            throw new System.Exception($"WeedPestGrowthComponent has incorrect values: WeedTolerance={weedPest.WeedTolerance}, PestTolerance={weedPest.PestTolerance}");
                        }
                        break;
                }
            }

            if (!hasBasic || !hasAtmospheric || !hasWeedPest)
            {
                throw new System.Exception($"Not all expected component types were found: Basic={hasBasic}, Atmospheric={hasAtmospheric}, WeedPest={hasWeedPest}");
            }

            System.Console.WriteLine("✅ New growth component system test passed!");
        }
    }
}