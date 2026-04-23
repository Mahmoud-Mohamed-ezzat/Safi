using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Safi.Dto.AIModelsDto
{
    public class PrescriptionResponseDto
    {
        [JsonPropertyName("ok")]
        public bool Ok { get; set; }

        [JsonPropertyName("lines")]
        public List<PrescriptionLineDto> Lines { get; set; }

        [JsonPropertyName("device")]
        public string Device { get; set; }
    }

    public class PrescriptionLineDto
    {
        [JsonPropertyName("line_index")]
        public int LineIndex { get; set; }

        [JsonPropertyName("ocr_text")]
        public string OcrText { get; set; }

        [JsonPropertyName("medicine")]
        public MedicineResponseDto Medicine { get; set; }
    }
}
