namespace JWTAuthServer.Common.Models
{
    public class ResponseViewModel<ViewModel>
    {
        public class Status
        {
            public int code { get; set; }
            public string message { get; set; }
            public string description { get; set; }
        }

        public Status status { get; set; }
        public ViewModel data { get; set; }

        public ResponseViewModel(ViewModel viewModel)
        {
            data = viewModel;
            status = new Status();

        }

        public ResponseViewModel()
        {

            status = new Status();

        }


    }
}
