namespace Safi.Dto.AIModelsDto
{
    public class HeartDiseaseRequestDto
    {
        public int Age { get; set; }
        public string Sex { get; set; }
        public int Cp { get; set; }
        public int Trestbps { get; set; }
        public int Chol { get; set; }
        public int Fbs { get; set; }
        public int Restecg { get; set; }
        public int Thalch { get; set; }
        public int Exang { get; set; }
        public float Oldpeak { get; set; }
        public int Slope { get; set; }
        public int Ca { get; set; }
        public int Thal { get; set; }
    }
}
