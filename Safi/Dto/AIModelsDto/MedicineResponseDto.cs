using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Safi.Dto.AIModelsDto
{
    public class MedicineResponseDto
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("confidence")]
        public float Confidence { get; set; }

        [JsonPropertyName("Contains")]
        public string Contains { get; set; }

        [JsonPropertyName("ProductIntroduction")]
        public string ProductIntroduction { get; set; }

        [JsonPropertyName("ProductBenefits")]
        public string ProductBenefits { get; set; }

        [JsonPropertyName("SideEffect")]
        public string SideEffect { get; set; }

        [JsonPropertyName("HowToUse")]
        public string HowToUse { get; set; }

        [JsonPropertyName("HowWorks")]
        public string HowWorks { get; set; }

        [JsonPropertyName("QuickTips")]
        public string QuickTips { get; set; }

        [JsonPropertyName("SafetyAdvice")]
        public string SafetyAdvice { get; set; }

        [JsonPropertyName("alternatives")]
        public List<MedicineAlternativeDto> Alternatives { get; set; }
    }

    public class MedicineAlternativeDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("score")]
        public float Score { get; set; }
    }
}
