namespace Template.Models.ViewModels
{
    public class BaseViewModel<TRequest> where TRequest : new()
    {
        public TRequest Request { get; set; }

        public BaseViewModel(TRequest request)
        {
            Request = request;
        }

        public BaseViewModel() : this(new TRequest()) { }
    }

    public class BaseViewModel
    {
        
    }
}
