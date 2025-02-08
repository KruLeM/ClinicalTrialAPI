namespace API.Validation.RequestModels
{
    public class GetTrialByStatusRequestModel
    {
        public string Status {  get; set; }
        public int? Page {  get; set; }
        public int? Size { get; set; }
    }
}
